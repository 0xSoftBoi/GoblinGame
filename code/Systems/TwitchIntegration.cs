using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GoblinChain;

/// <summary>
/// Connects to a Twitch channel via IRC and translates chat commands into
/// game actions. Runs on the host only — streamer is expected to be host.
///
/// Commands:
///   !shill TICKER   — viewer posts on GoblinTwitter, applies shill pressure
///   !buy TICKER     — small NPC buy pressure on that token
///   !sell TICKER    — small NPC sell pressure
///   !invest         — random buy on a live token (FOMO mode)
///   !bribe          — vote to bribe during active SEC raid
///   !blame          — vote to blame someone during active SEC raid
///   !accept         — vote to accept fate during active SEC raid
/// </summary>
public sealed class TwitchIntegration : Component
{
	public static TwitchIntegration Instance { get; private set; }

	// --- Config ---
	[Property] public string TwitchChannel { get; set; } = "";
	[Property] public string OAuthToken { get; set; } = "";      // "oauth:xxxxxx"
	[Property] public bool AutoConnect { get; set; } = false;
	[Property] public float ShillCooldown { get; set; } = 30f;   // Per-viewer shill rate limit
	[Property] public float TradeCooldown { get; set; } = 10f;   // Per-viewer trade rate limit
	[Property] public float ChatBuyPressure { get; set; } = 5f;  // GBC-equivalent per chat trade
	[Property] public float ChatShillPower { get; set; } = 0.25f;
	[Property] public float RaidVoteOpenAt { get; set; } = 8f;   // Seconds left on raid timer to open vote

	// --- Synced state (for HUD display) ---
	[Sync] public bool IsConnected { get; set; } = false;
	[Sync] public string ChannelDisplay { get; set; } = "";
	[Sync] public int ChatCommandsThisRound { get; set; } = 0;
	[Sync] public bool RaidVoteActive { get; set; } = false;
	[Sync] public int VotesBribe { get; set; } = 0;
	[Sync] public int VotesBlame { get; set; } = 0;
	[Sync] public int VotesAccept { get; set; } = 0;

	// --- IRC internals (host only) ---
	private TcpClient _tcp;
	private StreamReader _reader;
	private StreamWriter _writer;
	private Thread _ircThread;
	private bool _running;

	// Commands queued from IRC thread, drained on main thread
	private readonly object _queueLock = new();
	private readonly Queue<(string user, string message)> _cmdQueue = new();

	// Per-viewer rate limiting
	private readonly Dictionary<string, float> _lastShill = new();
	private readonly Dictionary<string, float> _lastTrade = new();

	// Raid vote tracking (each viewer votes once)
	private readonly Dictionary<string, string> _raidVoters = new();
	private bool _raidVoteResolved;

	protected override void OnStart()
	{
		Instance = this;
		if ( AutoConnect && !string.IsNullOrWhiteSpace( TwitchChannel ) )
			Connect( TwitchChannel, OAuthToken );
	}

	protected override void OnDestroy()
	{
		Disconnect();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		// Apply connection-state changes signaled by the IRC thread —
		// [Sync] properties must only be written from the main thread
		int pending = System.Threading.Interlocked.Exchange( ref _pendingConnState, 0 );
		if ( pending == 1 ) IsConnected = true;
		else if ( pending == 2 ) IsConnected = false;

		DrainCommandQueue();
		TickRaidVote();
	}

	// 0 = no change, 1 = connected, 2 = disconnected. Written by the IRC thread.
	private int _pendingConnState = 0;

	// ═══════════════════════════════════════
	//  CONNECTION
	// ═══════════════════════════════════════

	public void Connect( string channel, string token )
	{
		if ( IsProxy || _running ) return;

		channel = channel.ToLower().TrimStart( '#' );
		ChannelDisplay = channel;
		_running = true;

		_ircThread = new Thread( () => IrcLoop( channel, token ) ) { IsBackground = true };
		_ircThread.Start();
	}

	public void Disconnect()
	{
		_running = false;
		IsConnected = false;
		try { _tcp?.Close(); } catch { }
	}

	private void IrcLoop( string channel, string token )
	{
		const string host = "irc.chat.twitch.tv";
		const int port = 6667;

		try
		{
			_tcp = new TcpClient( host, port );
			var stream = _tcp.GetStream();
			_reader = new StreamReader( stream, Encoding.UTF8 );
			_writer = new StreamWriter( stream, Encoding.UTF8 ) { AutoFlush = true };

			_writer.WriteLine( $"PASS {token}" );
			_writer.WriteLine( $"NICK goblinchain_bot" );
			_writer.WriteLine( $"JOIN #{channel}" );

			System.Threading.Interlocked.Exchange( ref _pendingConnState, 1 );
			Log.Info( $"[Twitch] Connected to #{channel}" );

			string line;
			while ( _running && (line = _reader.ReadLine()) != null )
			{
				if ( line.StartsWith( "PING" ) )
				{
					_writer.WriteLine( "PONG :tmi.twitch.tv" );
					continue;
				}

				// ":user!user@user.tmi.twitch.tv PRIVMSG #channel :message"
				if ( !line.Contains( " PRIVMSG " ) ) continue;

				int excl = line.IndexOf( '!' );
				int colon2 = line.IndexOf( ':', 1 );
				if ( excl < 1 || colon2 < 0 ) continue;

				string username = line[1..excl].ToLower();
				string msg = line[(colon2 + 1)..].Trim();

				if ( msg.StartsWith( "!" ) )
				{
					lock ( _queueLock )
						_cmdQueue.Enqueue( (username, msg) );
				}
			}
		}
		catch ( Exception ) when ( !_running )
		{
			// Clean shutdown — expected
		}
		catch ( Exception ex )
		{
			Log.Error( $"[Twitch] IRC error: {ex.Message}" );
		}
		finally
		{
			System.Threading.Interlocked.Exchange( ref _pendingConnState, 2 );
		}
	}

	// ═══════════════════════════════════════
	//  COMMAND DISPATCH (main thread)
	// ═══════════════════════════════════════

	private void DrainCommandQueue()
	{
		List<(string user, string msg)> batch;
		lock ( _queueLock )
		{
			if ( _cmdQueue.Count == 0 ) return;
			batch = new List<(string, string)>( _cmdQueue );
			_cmdQueue.Clear();
		}

		foreach ( var (user, msg) in batch )
			HandleCommand( user, msg );
	}

	private void HandleCommand( string user, string message )
	{
		var parts = message.Split( ' ', StringSplitOptions.RemoveEmptyEntries );
		if ( parts.Length == 0 ) return;

		string cmd = parts[0].ToLower();
		string arg = parts.Length > 1 ? parts[1].ToUpper() : "";
		float now = Time.Now;

		switch ( cmd )
		{
			case "!shill" when arg.Length > 0:
				CmdShill( user, arg, now );
				break;
			case "!buy" when arg.Length > 0:
				CmdTrade( user, arg, buy: true, now );
				break;
			case "!sell" when arg.Length > 0:
				CmdTrade( user, arg, buy: false, now );
				break;
			case "!invest":
				CmdInvest( user, now );
				break;
			case "!bribe" when RaidVoteActive:
				CastVote( user, "bribe" );
				break;
			case "!blame" when RaidVoteActive:
				CastVote( user, "blame" );
				break;
			case "!accept" when RaidVoteActive:
				CastVote( user, "accept" );
				break;
		}
	}

	// ═══════════════════════════════════════
	//  COMMANDS
	// ═══════════════════════════════════════

	private void CmdShill( string user, string ticker, float now )
	{
		if ( _lastShill.TryGetValue( user, out float last ) && now - last < ShillCooldown ) return;
		_lastShill[user] = now;

		var token = FindToken( ticker );
		if ( token is null ) return;

		string[] phrases = {
			$"chat is EXTREMELY bullish on ${ticker}, not financial advice obvs",
			$"I did my own research on ${ticker} and the vibes are immaculate fr",
			$"${ticker} is literally the one. trust the goblins. we are so back.",
			$"sending my entire net worth into ${ticker} rn no cap",
			$"${ticker} chart looking like a hockey stick. wen lambo chat?",
		};
		string content = phrases[new Random().Next( phrases.Length )];

		GoblinTwitter.Instance?.AddNPCPost( $"@{user}", ticker, content );
		TokenSystem.Instance?.AddShillPressure( token.Value.Id, ChatShillPower );
		ChatCommandsThisRound++;

		Log.Info( $"[Twitch] @{user} shilled ${ticker}" );
	}

	private void CmdTrade( string user, string ticker, bool buy, float now )
	{
		if ( _lastTrade.TryGetValue( user, out float last ) && now - last < TradeCooldown ) return;
		_lastTrade[user] = now;

		var token = FindToken( ticker );
		if ( token is null ) return;

		if ( buy )
			TokenSystem.Instance?.ApplyNPCBuyPressure( token.Value.Id, ChatBuyPressure );
		else
			TokenSystem.Instance?.ApplyNPCSellPressure( token.Value.Id, ChatBuyPressure );

		ChatCommandsThisRound++;
		Log.Info( $"[Twitch] @{user} {(buy ? "bought" : "sold")} ${ticker}" );
	}

	private void CmdInvest( string user, float now )
	{
		if ( _lastTrade.TryGetValue( user, out float last ) && now - last < TradeCooldown ) return;
		_lastTrade[user] = now;

		var tokens = TokenSystem.Instance?.GetActiveTokensSorted().Where( t => !t.IsRugged ).ToList();
		if ( tokens is null || tokens.Count == 0 ) return;

		var token = tokens[new Random().Next( tokens.Count )];
		TokenSystem.Instance?.ApplyNPCBuyPressure( token.Id, ChatBuyPressure * 0.4f );
		ChatCommandsThisRound++;
	}

	// ═══════════════════════════════════════
	//  SEC RAID VOTING
	// ═══════════════════════════════════════

	private void TickRaidVote()
	{
		var sec = SECSystem.Instance;
		if ( sec is null ) return;

		if ( sec.RaidActive && !sec.RaidResolved )
		{
			if ( !RaidVoteActive && sec.RaidTimer <= RaidVoteOpenAt )
				OpenVote();

			if ( RaidVoteActive && !_raidVoteResolved && sec.RaidTimer <= 1f )
				ResolveVote();
		}
		else if ( RaidVoteActive )
		{
			CloseVote();
		}
	}

	private void OpenVote()
	{
		RaidVoteActive = true;
		_raidVoteResolved = false;
		_raidVoters.Clear();
		VotesBribe = 0;
		VotesBlame = 0;
		VotesAccept = 0;
		AnnounceVoteOpen();
		Log.Info( "[Twitch] Raid vote open — !bribe / !blame / !accept" );
	}

	private void CloseVote()
	{
		RaidVoteActive = false;
		_raidVoteResolved = false;
		_raidVoters.Clear();
	}

	private void CastVote( string user, string choice )
	{
		if ( _raidVoters.ContainsKey( user ) ) return; // one vote per viewer
		_raidVoters[user] = choice;

		switch ( choice )
		{
			case "bribe": VotesBribe++; break;
			case "blame": VotesBlame++; break;
			case "accept": VotesAccept++; break;
		}
	}

	private void ResolveVote()
	{
		_raidVoteResolved = true;

		int total = VotesBribe + VotesBlame + VotesAccept;
		if ( total == 0 ) return; // No chat votes — let raid timer expire normally

		// Highest vote wins
		string winner = "accept";
		int max = VotesAccept;
		if ( VotesBribe > max ) { winner = "bribe"; max = VotesBribe; }
		if ( VotesBlame > max ) { winner = "blame"; }

		var action = winner switch
		{
			"bribe" => RaidAction.Bribe,
			"blame" => RaidAction.BlameAnother,
			_ => RaidAction.AcceptFate,
		};

		SECSystem.Instance?.ForceRaidAction( action );
		AnnounceVoteResult( winner, max, total );
		Log.Info( $"[Twitch] Chat voted {winner} ({max}/{total})" );
	}

	// ═══════════════════════════════════════
	//  BROADCASTS
	// ═══════════════════════════════════════

	[Rpc.Broadcast]
	private void AnnounceVoteOpen()
	{
		Sound.Play( "sounds/notification.sound" );
		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "CHAT VOTES", "Type !bribe / !blame / !accept in Twitch chat!", "info" );
	}

	[Rpc.Broadcast]
	private void AnnounceVoteResult( string winner, int winVotes, int total )
	{
		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "CHAT DECIDED", $"{winner.ToUpper()} wins! ({winVotes}/{total} votes)", "info" );
	}

	// ═══════════════════════════════════════
	//  HELPERS
	// ═══════════════════════════════════════

	private TokenData? FindToken( string ticker )
		=> TokenSystem.Instance?.GetActiveTokensSorted()
			.Where( t => t.Ticker == ticker && !t.IsRugged )
			.Cast<TokenData?>()
			.FirstOrDefault();

	public void ResetRound()
	{
		ChatCommandsThisRound = 0;
		_lastShill.Clear();
		_lastTrade.Clear();
	}
}
