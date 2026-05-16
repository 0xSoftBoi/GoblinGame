using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Social deduction system. At session start, one player MAY be secretly
/// assigned as The Rugger. 30% chance of no Rugger (paranoia mode).
/// The Rugger wins by accumulating 50%+ of total GBC, then triggering Grand Rug.
/// Other players can call Audit Votes to expose the Rugger.
/// </summary>
public sealed class SocialDeduction : Component
{
	public static SocialDeduction Instance { get; private set; }

	// --- Public knowledge (synced to all) ---
	[Sync] public bool HasRugger { get; set; } = false; // "There MAY be a rugger"
	[Sync] public bool RuggerExposed { get; set; } = false;
	[Sync] public string ExposedRuggerName { get; set; } = "";
	[Sync] public bool GrandRugTriggered { get; set; } = false;
	[Sync] public int AuditsUsed { get; set; } = 0;
	[Sync] public bool AuditInProgress { get; set; } = false;
	[Sync] public float AuditVoteTimer { get; set; } = 0f;

	// --- Config ---
	[Property] public float RuggerChance { get; set; } = 0.7f; // 70% chance there IS a rugger
	[Property] public float GrandRugThreshold { get; set; } = 0.5f; // 50% of total GBC
	[Property] public int MaxAudits { get; set; } = 2;
	[Property] public float AuditCost { get; set; } = 1000f;
	[Property] public float WrongAccusationPenalty { get; set; } = 500f;
	[Property] public float AuditVoteDuration { get; set; } = 20f;
	[Property] public float ShadowWalletHidePercent { get; set; } = 0.3f;

	// --- Private (host-only) ---
	private Guid _ruggerId = Guid.Empty;
	private Random _rng = new();
	private Dictionary<Guid, Guid> _auditVotes = new(); // voterId → suspectId

	protected override void OnStart()
	{
		Instance = this;
	}

	// ═══════════════════════════════════════
	//  ASSIGNMENT (called by GameStateManager at match start)
	// ═══════════════════════════════════════

	public void AssignRoles()
	{
		if ( IsProxy ) return;

		RuggerExposed = false;
		GrandRugTriggered = false;
		AuditsUsed = 0;

		// Roll for rugger existence
		if ( _rng.NextDouble() > RuggerChance )
		{
			HasRugger = true; // We say "there may be" regardless (paranoia!)
			_ruggerId = Guid.Empty; // But there actually isn't one
			Log.Info( "No Rugger this session — paranoia mode!" );
			return;
		}

		// Pick a random player
		var players = Scene.GetAllComponents<GoblinPlayer>().ToList();
		if ( players.Count < 2 )
		{
			_ruggerId = Guid.Empty;
			HasRugger = true;
			return;
		}

		var rugger = players[_rng.Next( players.Count )];
		_ruggerId = rugger.Id;
		HasRugger = true;

		// Notify the rugger privately
		NotifyRugger( rugger.Network.Owner );

		Log.Info( $"RUGGER ASSIGNED: {rugger.Network.Owner?.DisplayName}" );
	}

	// ═══════════════════════════════════════
	//  RUGGER CHECKS
	// ═══════════════════════════════════════

	/// <summary>
	/// Check if a player is the Rugger (host-only query).
	/// </summary>
	public bool IsRugger( GoblinPlayer player )
		=> player is not null && player.Id == _ruggerId && !RuggerExposed;

	/// <summary>
	/// Check if the calling client is the Rugger.
	/// </summary>
	public bool AmITheRugger( Connection conn )
	{
		var player = Scene.GetAllComponents<GoblinPlayer>()
			.FirstOrDefault( p => p.Network.Owner == conn );
		return player is not null && player.Id == _ruggerId;
	}

	/// <summary>
	/// Get the public balance for a player (hides portion if they're the Rugger).
	/// </summary>
	public float GetPublicBalance( GoblinPlayer player )
	{
		var wallet = player?.Components.Get<CryptoWallet>();
		if ( wallet is null ) return 0f;

		if ( IsRugger( player ) && !RuggerExposed )
		{
			// Shadow wallet: hide 30% of actual holdings
			return wallet.GoblinCoin * (1f - ShadowWalletHidePercent);
		}

		return wallet.GoblinCoin;
	}

	// ═══════════════════════════════════════
	//  GRAND RUG
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestGrandRug()
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		if ( !IsRugger( player ) )
		{
			Log.Warning( $"{caller.DisplayName} tried Grand Rug but isn't the Rugger" );
			return;
		}

		if ( GrandRugTriggered || RuggerExposed ) return;

		// Check if rugger has 50%+ of total GBC
		var wallets = Scene.GetAllComponents<CryptoWallet>().ToList();
		float totalGBC = wallets.Sum( w => w.GoblinCoin );
		var ruggerWallet = player.Components.Get<CryptoWallet>();
		if ( ruggerWallet is null ) return;

		float ruggerShare = ruggerWallet.GoblinCoin / MathF.Max( 1f, totalGBC );

		if ( ruggerShare < GrandRugThreshold )
		{
			Log.Warning( $"Rugger only has {ruggerShare:P0} — need {GrandRugThreshold:P0}" );
			return;
		}

		// GRAND RUG!
		GrandRugTriggered = true;
		ExecuteGrandRug( caller.DisplayName, ruggerWallet.GoblinCoin );
	}

	private void ExecuteGrandRug( string ruggerName, float amount )
	{
		// Halve everyone else's score
		foreach ( var wallet in Scene.GetAllComponents<CryptoWallet>() )
		{
			var owner = wallet.Network.Owner;
			if ( owner is not null )
			{
				var p = Scene.GetAllComponents<GoblinPlayer>()
					.FirstOrDefault( pl => pl.Network.Owner == owner );
				if ( p is not null && p.Id != _ruggerId )
				{
					wallet.GoblinCoin *= 0.5f;
				}
			}
		}

		// Crash all tokens
		var tokenSys = TokenSystem.Instance;
		if ( tokenSys is not null )
		{
			foreach ( var kv in tokenSys.ActiveTokens.ToList() )
			{
				var t = kv.Value;
				t.Price = 0f;
				t.IsRugged = true;
				tokenSys.ActiveTokens[kv.Key] = t;
			}
		}

		BroadcastGrandRug( ruggerName, amount );

		// Force end the match after 10 seconds
		var state = GameStateManager.Instance;
		if ( state is not null )
		{
			state.ForceResults();
		}
	}

	// ═══════════════════════════════════════
	//  AUDIT VOTE
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestAudit()
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		if ( AuditsUsed >= MaxAudits )
		{
			Log.Warning( "Max audits reached" );
			return;
		}

		if ( AuditInProgress )
		{
			Log.Warning( "Audit already in progress" );
			return;
		}

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( AuditCost ) ) return;

		AuditsUsed++;
		AuditInProgress = true;
		AuditVoteTimer = AuditVoteDuration;
		_auditVotes.Clear();

		BroadcastAuditStarted( caller.DisplayName );
	}

	[Rpc.Host]
	public void CastAuditVote( Guid suspectId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null || !AuditInProgress ) return;

		_auditVotes[player.Id] = suspectId;

		// Check if all players have voted
		int totalPlayers = Scene.GetAllComponents<GoblinPlayer>().Count();
		if ( _auditVotes.Count >= totalPlayers )
		{
			ResolveAudit();
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		if ( AuditInProgress )
		{
			AuditVoteTimer -= Time.Delta;
			if ( AuditVoteTimer <= 0f )
			{
				ResolveAudit();
			}
		}
	}

	private void ResolveAudit()
	{
		AuditInProgress = false;

		// Tally votes
		var voteCounts = new Dictionary<Guid, int>();
		foreach ( var vote in _auditVotes.Values )
		{
			if ( !voteCounts.ContainsKey( vote ) )
				voteCounts[vote] = 0;
			voteCounts[vote]++;
		}

		if ( voteCounts.Count == 0 )
		{
			BroadcastAuditResult( false, "No votes", "" );
			return;
		}

		// Find most-voted suspect
		var topSuspect = voteCounts.OrderByDescending( kv => kv.Value ).First();
		var suspectPlayer = Scene.GetAllComponents<GoblinPlayer>()
			.FirstOrDefault( p => p.Id == topSuspect.Key );
		string suspectName = suspectPlayer?.Network.Owner?.DisplayName ?? "???";

		// Check if correct
		bool correct = topSuspect.Key == _ruggerId && _ruggerId != Guid.Empty;

		if ( correct )
		{
			RuggerExposed = true;
			ExposedRuggerName = suspectName;
			BroadcastAuditResult( true, suspectName, "RUGGER EXPOSED!" );
		}
		else
		{
			// Wrong — penalize the accused
			if ( suspectPlayer is not null )
			{
				var wallet = suspectPlayer.Components.Get<CryptoWallet>();
				if ( wallet is not null )
					wallet.GoblinCoin -= WrongAccusationPenalty;
			}

			string msg = _ruggerId == Guid.Empty
				? "There was no Rugger! Paranoia wins."
				: "Wrong goblin! The Rugger remains hidden.";
			BroadcastAuditResult( false, suspectName, msg );
		}

		_auditVotes.Clear();
	}

	// ═══════════════════════════════════════
	//  BROADCASTS
	// ═══════════════════════════════════════

	[Rpc.Owner]
	private void NotifyRugger( Connection target )
	{
		Log.Info( "YOU ARE THE RUGGER. Accumulate 50% of all GBC to trigger the Grand Rug." );
		Sound.Play( "sounds/event_negative.sound" );
	}

	[Rpc.Broadcast]
	private void BroadcastGrandRug( string ruggerName, float amount )
	{
		Sound.Play( "sounds/market_crash.sound" );
		Log.Warning( $"GRAND RUG PULL! {ruggerName} rugged EVERYTHING for {amount:N0} GBC!" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "GRAND RUG", $"{ruggerName} PULLED THE ULTIMATE RUG!", "negative" );
	}

	[Rpc.Broadcast]
	private void BroadcastAuditStarted( string callerName )
	{
		Sound.Play( "sounds/phase_transition.sound" );
		Log.Info( $"AUDIT CALLED by {callerName}! Vote now — who is the Rugger?" );
	}

	[Rpc.Broadcast]
	private void BroadcastAuditResult( bool correct, string suspectName, string message )
	{
		string sound = correct ? "sounds/event_positive.sound" : "sounds/event_negative.sound";
		Sound.Play( sound );
		Log.Info( $"AUDIT RESULT: {message}" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( correct ? "EXPOSED" : "INNOCENT",
			$"{suspectName}: {message}", correct ? "positive" : "negative" );
	}

	// ═══════════════════════════════════════
	//  HELPERS
	// ═══════════════════════════════════════

	private GoblinPlayer FindPlayer( Connection conn )
		=> Scene.GetAllComponents<GoblinPlayer>()
			.FirstOrDefault( p => p.Network.Owner == conn );
}
