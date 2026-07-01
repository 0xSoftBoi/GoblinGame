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

	// Current audit session (synced so all clients render the same vote)
	[Sync] public Guid CurrentAccusedId { get; set; } = Guid.Empty;
	[Sync] public int GuiltyVotes { get; set; } = 0;
	[Sync] public int InnocentVotes { get; set; } = 0;

	// End-of-match reveal, set when the match ends ("" until then)
	[Sync] public string FinalRuggerReveal { get; set; } = "";

	// Shadow Wallet: host publishes what each player's balance LOOKS like.
	// The Rugger's entry is discounted; clients render this, not raw balances.
	[Sync] public NetDictionary<Guid, float> PublicBalances { get; set; } = new();

	/// <summary>Set on the Rugger's client only, via targeted RPC. UI gates on this.</summary>
	public static bool LocalIsRugger { get; private set; } = false;

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
		FinalRuggerReveal = "";
		CurrentAccusedId = Guid.Empty;
		GuiltyVotes = 0;
		InnocentVotes = 0;
		ResetLocalRoles();

		// Roll for rugger existence
		if ( _rng.NextDouble() > RuggerChance )
		{
			HasRugger = true; // We say "there may be" regardless (paranoia!)
			_ruggerId = Guid.Empty; // But there actually isn't one
			Log.Info( "No Rugger this session — paranoia mode!" );
			return;
		}

		// Pick a random player
		var players = GoblinPlayer.All.ToList();
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
		var player = GoblinPlayer.All
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
				var p = GoblinPlayer.All
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

		BroadcastAuditStarted( caller.DisplayName, "", Guid.Empty );
	}

	/// <summary>
	/// Called from PlayerInput (V key): starts an audit if none in progress,
	/// then immediately casts the caller's vote for the specified suspect.
	/// </summary>
	[Rpc.Host]
	public void RequestAuditVote( Guid suspectId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		if ( !AuditInProgress )
		{
			if ( AuditsUsed >= MaxAudits )
			{
				Log.Warning( "Max audits reached" );
				return;
			}

			var wallet = player.Components.Get<CryptoWallet>();
			if ( wallet is null || !wallet.TrySpend( AuditCost ) ) return;

			AuditsUsed++;
			AuditInProgress = true;
			AuditVoteTimer = AuditVoteDuration;
			_auditVotes.Clear();
			CurrentAccusedId = suspectId;
			GuiltyVotes = 0;
			InnocentVotes = 0;

			string suspectName = GoblinPlayer.All
				.FirstOrDefault( p => p.Id == suspectId )
				?.Network.Owner?.DisplayName ?? "???";
			BroadcastAuditStarted( caller.DisplayName, suspectName, suspectId );
		}

		_auditVotes[player.Id] = suspectId;
		UpdateVoteTallies();

		if ( _auditVotes.Count >= GoblinPlayer.All.Count() )
			ResolveAudit();
	}

	[Rpc.Host]
	public void CastAuditVote( Guid suspectId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null || !AuditInProgress ) return;

		_auditVotes[player.Id] = suspectId;
		UpdateVoteTallies();

		// Check if all players have voted
		int totalPlayers = GoblinPlayer.All.Count();
		if ( _auditVotes.Count >= totalPlayers )
		{
			ResolveAudit();
		}
	}

	/// <summary>A vote for the accused counts guilty; anything else (incl. Guid.Empty) is innocent.</summary>
	private void UpdateVoteTallies()
	{
		GuiltyVotes = _auditVotes.Values.Count( v => v == CurrentAccusedId && v != Guid.Empty );
		InnocentVotes = _auditVotes.Count - GuiltyVotes;
	}

	private float _publicBalanceTimer;

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

		// Publish Shadow-Wallet-adjusted balances for client leaderboards
		_publicBalanceTimer += Time.Delta;
		if ( _publicBalanceTimer >= 1f )
		{
			_publicBalanceTimer = 0f;
			foreach ( var p in GoblinPlayer.All )
				PublicBalances[p.Id] = GetPublicBalance( p );
		}
	}

	// ═══════════════════════════════════════
	//  END-OF-MATCH REVEAL
	// ═══════════════════════════════════════

	/// <summary>
	/// Called by GameStateManager when the match ends. Publishes who the
	/// Rugger was (or wasn't) so the results screen can pay off the paranoia.
	/// </summary>
	public void RevealRugger()
	{
		if ( IsProxy ) return;

		if ( _ruggerId == Guid.Empty )
		{
			FinalRuggerReveal = "There was NO Rugger this match. You did all of that to each other for free.";
			return;
		}

		string name = GoblinPlayer.All
			.FirstOrDefault( p => p.Id == _ruggerId )
			?.Network.Owner?.DisplayName ?? "A goblin who left early";

		if ( GrandRugTriggered )
			FinalRuggerReveal = $"The Rugger was {name} — and they pulled the GRAND RUG. Check your bags. Actually, don't.";
		else if ( RuggerExposed )
			FinalRuggerReveal = $"The Rugger was {name}. The audit caught them. The system works (once).";
		else
			FinalRuggerReveal = $"The Rugger was {name}. Nobody caught them. They walk among you still.";
	}

	private void ResolveAudit()
	{
		AuditInProgress = false;

		var accusedPlayer = GoblinPlayer.All
			.FirstOrDefault( p => p.Id == CurrentAccusedId );
		string accusedName = accusedPlayer?.Network.Owner?.DisplayName ?? "???";

		UpdateVoteTallies();
		bool verdictGuilty = GuiltyVotes > InnocentVotes;
		bool wasActuallyRugger = CurrentAccusedId != Guid.Empty && CurrentAccusedId == _ruggerId;

		if ( verdictGuilty && wasActuallyRugger )
		{
			RuggerExposed = true;
			ExposedRuggerName = accusedName;
			BroadcastAuditResult( true, true, accusedName, "RUGGER EXPOSED! Shadow Wallet revealed!" );
		}
		else if ( verdictGuilty )
		{
			// Lynched a clean goblin — everyone who voted guilty pays the fine
			foreach ( var kv in _auditVotes )
			{
				if ( kv.Value != CurrentAccusedId ) continue;
				var voter = GoblinPlayer.All.FirstOrDefault( p => p.Id == kv.Key );
				var w = voter?.Components.Get<CryptoWallet>();
				if ( w is not null )
					w.GoblinCoin = MathF.Max( 0f, w.GoblinCoin - WrongAccusationPenalty );
			}

			BroadcastAuditResult( true, false, accusedName,
				$"They were clean! Guilty voters fined {WrongAccusationPenalty:N0} GBC each." );
		}
		else
		{
			// Acquitted — the books stay closed. Never reveal the truth here,
			// or one cheap audit would end the deduction game.
			BroadcastAuditResult( false, false, accusedName,
				"Acquitted. The books stay closed. The truth stays buried." );
		}

		_auditVotes.Clear();
		CurrentAccusedId = Guid.Empty;
	}

	// ═══════════════════════════════════════
	//  BROADCASTS
	// ═══════════════════════════════════════

	private void NotifyRugger( Connection target )
	{
		using ( Rpc.FilterInclude( c => c == target ) )
		{
			ClientReceiveRuggerRole();
		}
	}

	[Rpc.Broadcast]
	private void ClientReceiveRuggerRole()
	{
		LocalIsRugger = true;
		Log.Info( "YOU ARE THE RUGGER. Accumulate 50% of all GBC to trigger the Grand Rug." );
		Sound.Play( "sounds/event_negative.sound" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "🎭 YOU ARE THE RUGGER",
			"Shill hard, stack GBC, and Grand Rug when you hold 50%. Check the 🎭 tab on your phone. Tell no one.", "negative" );
	}

	[Rpc.Broadcast]
	private void ResetLocalRoles()
	{
		LocalIsRugger = false;
	}

	[Rpc.Broadcast]
	private void BroadcastGrandRug( string ruggerName, float amount )
	{
		Sound.Play( "sounds/market_crash.sound" );
		Log.Warning( $"GRAND RUG PULL! {ruggerName} rugged EVERYTHING for {amount:N0} GBC!" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "GRAND RUG", $"{ruggerName} PULLED THE ULTIMATE RUG!", "negative" );

		ClipRecorder.Instance?.OnGrandRug( ruggerName, amount );
	}

	[Rpc.Broadcast]
	private void BroadcastAuditStarted( string callerName, string suspectName, Guid suspectId )
	{
		Sound.Play( "sounds/phase_transition.sound" );
		Log.Info( $"AUDIT CALLED by {callerName}! {suspectName} is accused!" );

		if ( suspectId != Guid.Empty )
		{
			var auditVote = Scene.GetAllComponents<UI.AuditVote>().FirstOrDefault();
			auditVote?.Show( callerName, suspectName, suspectId );
		}
	}

	[Rpc.Broadcast]
	private void BroadcastAuditResult( bool verdictGuilty, bool exposedRugger, string suspectName, string message )
	{
		string sound = exposedRugger ? "sounds/event_positive.sound" : "sounds/event_negative.sound";
		Sound.Play( sound );
		Log.Info( $"AUDIT RESULT: {message}" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( exposedRugger ? "EXPOSED" : "VERDICT",
			$"{suspectName}: {message}", exposedRugger ? "positive" : "negative" );

		var auditVote = Scene.GetAllComponents<UI.AuditVote>().FirstOrDefault();
		auditVote?.ShowResult( verdictGuilty, exposedRugger );

		ClipRecorder.Instance?.OnAuditResult( exposedRugger, suspectName, message );
	}

	// ═══════════════════════════════════════
	//  HELPERS
	// ═══════════════════════════════════════

	private GoblinPlayer FindPlayer( Connection conn )
		=> GoblinPlayer.All
			.FirstOrDefault( p => p.Network.Owner == conn );
}
