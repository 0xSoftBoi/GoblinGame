using Sandbox;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Game phases — renamed to match the shilling tycoon loop:
/// Create (was Mining) → Shill (was Trading) → Chaos → Results
/// After MaxRounds, Results phase declares the winner.
/// </summary>
public enum GamePhase
{
	WaitingForPlayers,
	Pregame,
	Create,    // Was "Mining" — players create tokens, place rigs, setup
	Shill,     // Was "Trading" — GoblinTwitter active, shilling, trading
	Chaos,     // Market chaos, SEC raids, rug pull window, sabotage
	Results
}

/// <summary>
/// Host-authoritative state machine driving the entire match flow.
/// Now integrates with TokenSystem, SocialDeduction, SEC, and Office systems.
/// </summary>
public sealed class GameStateManager : Component
{
	public static GameStateManager Instance { get; private set; }

	// --- Synced State ---
	[Sync( SyncFlags.FromHost )] public GamePhase CurrentPhase { get; set; } = GamePhase.WaitingForPlayers;
	[Sync( SyncFlags.FromHost )] public float PhaseTimeRemaining { get; set; } = 0f;
	[Sync( SyncFlags.FromHost )] public int CurrentRound { get; set; } = 1;
	[Sync] public int MaxRounds { get; set; } = 5;
	[Sync] public string WinnerName { get; set; } = "";
	[Sync] public string WinnerTitle { get; set; } = "";
	[Sync] public float WinnerBalance { get; set; } = 0f;

	// --- Phase durations ---
	[Property] public float PregameDuration { get; set; } = 10f;
	[Property] public float CreatePhaseDuration { get; set; } = 90f;  // Token creation + mining
	[Property] public float ShillPhaseDuration { get; set; } = 75f;   // GoblinTwitter shilling
	[Property] public float ChaosPhaseDuration { get; set; } = 60f;   // Rug pulls, SEC, sabotage
	[Property] public float ResultsDuration { get; set; } = 15f;

	// --- Rug Pull Window (first 15s of Chaos) ---
	[Sync] public bool IsRugPullWindow { get; set; } = false;
	[Property] public float RugPullWindowDuration { get; set; } = 15f;
	private float _rugPullTimer;

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		if ( CurrentPhase == GamePhase.WaitingForPlayers )
			return;

		PhaseTimeRemaining -= Time.Delta;

		// Track rug pull window during Chaos
		if ( IsRugPullWindow )
		{
			_rugPullTimer -= Time.Delta;
			if ( _rugPullTimer <= 0f )
			{
				IsRugPullWindow = false;
				BroadcastRugWindowClosed();
			}
		}

		if ( PhaseTimeRemaining <= 0f )
		{
			AdvancePhase();
		}
	}

	// --- Phase Transitions ---

	public void StartPregame()
	{
		if ( IsProxy ) return;
		TransitionTo( GamePhase.Pregame, PregameDuration );
	}

	/// <summary>
	/// Force-end current phase early (e.g., all players voted to skip).
	/// </summary>
	public void ForceResults()
	{
		if ( IsProxy ) return;
		DetermineWinner();
		TransitionTo( GamePhase.Results, ResultsDuration );
	}

	private void AdvancePhase()
	{
		switch ( CurrentPhase )
		{
			case GamePhase.Pregame:
				ResetForNewMatch();
				TransitionTo( GamePhase.Create, CreatePhaseDuration );
				break;

			case GamePhase.Create:
				TransitionTo( GamePhase.Shill, ShillPhaseDuration );
				break;

			case GamePhase.Shill:
				TransitionTo( GamePhase.Chaos, ChaosPhaseDuration );
				break;

			case GamePhase.Chaos:
				if ( CurrentRound >= MaxRounds )
				{
					DetermineWinner();
					TransitionTo( GamePhase.Results, ResultsDuration );
				}
				else
				{
					CurrentRound++;
					TransitionTo( GamePhase.Create, CreatePhaseDuration );
				}
				break;

			case GamePhase.Results:
				Log.Info( "Match complete. Returning to lobby..." );
				TransitionTo( GamePhase.WaitingForPlayers, 0f );
				CurrentRound = 1;
				break;
		}
	}

	private void TransitionTo( GamePhase phase, float duration )
	{
		var previous = CurrentPhase;
		CurrentPhase = phase;
		PhaseTimeRemaining = duration;

		Log.Info( $"Phase: {previous} → {phase} ({duration:N0}s)" );

		BroadcastPhaseChange( phase.ToString(), CurrentRound, MaxRounds );
		OnPhaseEnter( phase );
	}

	private void OnPhaseEnter( GamePhase phase )
	{
		switch ( phase )
		{
			case GamePhase.Create:
				// Players can create tokens and place rigs during this phase
				// TokenCreator UI becomes available (handled client-side by PlayerInput)
				break;

			case GamePhase.Shill:
				// GoblinTwitter becomes the focus
				// Phone UI (T key) is primary interaction
				break;

			case GamePhase.Chaos:
				// Give each player their starting EMPs
				foreach ( var p in GoblinPlayer.All )
				{
					p.Components.Get<SabotageInventory>()?.RefillForChaos();
				}

				// Open rug pull window for first 15 seconds
				IsRugPullWindow = true;
				_rugPullTimer = RugPullWindowDuration;
				BroadcastRugWindowOpen();
				ShowRugPullPrompts();

				// Trigger market chaos
				var market = Scene.GetAllComponents<CryptoMarket>().FirstOrDefault();
				if ( market is not null )
				{
					// 50/50 crash or moon to kick off chaos
					if ( new System.Random().NextDouble() > 0.5 )
					{
						market.IsCrashing = true;
						market.MiningMultiplier = 0.5f;
					}
					else
					{
						market.IsMooning = true;
						market.MiningMultiplier = 2.5f;
					}
				}

				// SEC might raid high-heat players
				var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
				sec?.CheckForRaids();

				// Physics comedy — scatter the office
				OfficeSetup.Instance?.TriggerPhysicsChaos( 0.8f );
				break;

			case GamePhase.Results:
				IsRugPullWindow = false;
				break;
		}
	}

	// --- Rug Pull Prompt ---

	[Rpc.Broadcast]
	private void ShowRugPullPrompts()
	{
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		if ( tokenSystem is null ) return;

		var myPlayer = GoblinPlayer.All
			.FirstOrDefault( p => !p.IsProxy );
		if ( myPlayer is null ) return;

		Guid myTokenId = Guid.Empty;
		foreach ( var kv in tokenSystem.ActiveTokens )
		{
			if ( kv.Value.CreatorId == myPlayer.Id && !kv.Value.IsRugged )
			{
				myTokenId = kv.Value.Id;
				break;
			}
		}

		if ( myTokenId == Guid.Empty ) return;

		var prompt = Scene.GetAllComponents<UI.RugPullPrompt>().FirstOrDefault();
		prompt?.ShowForToken( myTokenId );
	}

	// --- Match Setup ---

	private void ResetForNewMatch()
	{
		// Reset wallets
		foreach ( var wallet in Scene.GetAllComponents<CryptoWallet>() )
		{
			wallet.ResetAll( 100f );
		}

		// Assign social deduction roles
		var deduction = Scene.GetAllComponents<SocialDeduction>().FirstOrDefault();
		deduction?.AssignRoles();

		// Fill empty slots with bots
		BotPlayerManager.Instance?.RespawnBots();

		// Spawn office if not already done
		var office = Scene.GetAllComponents<OfficeSetup>().FirstOrDefault();
		office?.SpawnOffice();

		// Reset token system
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		tokenSystem?.ResetAllTokens();

		// Reset GoblinTwitter
		var twitter = Scene.GetAllComponents<GoblinTwitter>().FirstOrDefault();
		twitter?.ClearAllPosts();

		// Reset SEC heat
		var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
		sec?.ResetAllHeat();
	}

	// --- Winner Determination ---

	private void DetermineWinner()
	{
		var wallets = Scene.GetAllComponents<CryptoWallet>().ToList();
		if ( wallets.Count == 0 ) return;

		// Winner = highest total portfolio value (GBC + all token holdings)
		var richest = wallets.OrderByDescending( w => w.GetTotalPortfolioValue() ).First();
		float totalValue = richest.GetTotalPortfolioValue();

		WinnerName = richest.Network.Owner?.DisplayName ?? "Unknown Goblin";
		WinnerBalance = totalValue;

		// Assign a winner title based on how they won
		WinnerTitle = totalValue switch
		{
			> 10000f => "WOLF OF GOBLIN STREET",
			> 5000f => "HASH KING",
			> 2000f => "DIAMOND HANDS",
			> 1000f => "DEGEN SURVIVOR",
			_ => "LEAST BROKE GOBLIN"
		};

		// Check if rugger won (extra title)
		var deduction = Scene.GetAllComponents<SocialDeduction>().FirstOrDefault();
		if ( deduction is not null )
		{
			var richestPlayer = GoblinPlayer.All
				.FirstOrDefault( p => p.Network.Owner == richest.Network.Owner );
			if ( richestPlayer is not null && deduction.IsRugger( richestPlayer ) )
				WinnerTitle = "MASTER RUGGER 🎭";
		}

		Log.Info( $"WINNER: {WinnerName} — {WinnerTitle} — {totalValue:N1} GBC portfolio!" );
		AnnounceWinner( WinnerName, WinnerTitle, totalValue );
	}

	// --- Broadcasts ---

	[Rpc.Broadcast]
	private void BroadcastPhaseChange( string phaseName, int round, int maxRounds )
	{
		Sound.Play( "sounds/phase_transition.sound" );
		Log.Info( $"=== {phaseName.ToUpper()} === Round {round}/{maxRounds}" );
	}

	[Rpc.Broadcast]
	private void AnnounceWinner( string name, string title, float balance )
	{
		Sound.Play( "sounds/winner.sound" );
		Log.Info( $"🏆 THE ULTIMATE GOBLIN: {name} — {title} — {balance:N1} GBC" );
	}

	[Rpc.Broadcast]
	private void BroadcastRugWindowOpen()
	{
		Sound.Play( "sounds/rug_warning.sound" );
		Log.Info( "⚠️ RUG PULL WINDOW OPEN — 15 seconds to decide: Rug, Pivot, or Diamond Hands!" );
	}

	[Rpc.Broadcast]
	private void BroadcastRugWindowClosed()
	{
		Log.Info( "Rug pull window closed. Diamond hands it is." );
	}

	// --- Public Helpers ---

	public bool IsGameActive =>
		CurrentPhase == GamePhase.Create ||
		CurrentPhase == GamePhase.Shill ||
		CurrentPhase == GamePhase.Chaos;

	/// <summary>Can players create tokens / place rigs?</summary>
	public bool CanCreate => CurrentPhase == GamePhase.Create;

	/// <summary>Can players place rigs? (same as Create phase)</summary>
	public bool CanPlaceRigs => CurrentPhase == GamePhase.Create;

	/// <summary>Can players shill on GoblinTwitter? (Shill + Chaos)</summary>
	public bool CanShill => CurrentPhase == GamePhase.Shill || CurrentPhase == GamePhase.Chaos;

	/// <summary>Can players initiate trades?</summary>
	public bool CanTrade => CurrentPhase == GamePhase.Shill || CurrentPhase == GamePhase.Chaos;

	/// <summary>Can players rug pull right now?</summary>
	public bool CanRugPull => CurrentPhase == GamePhase.Chaos && IsRugPullWindow;
}
