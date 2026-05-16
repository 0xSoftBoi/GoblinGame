using Sandbox;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Central input dispatcher for GOBLIN CHAIN.
/// Updated keybinds for the shilling tycoon loop:
/// T = GoblinPhone (was Trade), Y = Trade, M = Market,
/// Q = Sabotage (EMP), E = Place Rig / Interact,
/// C = Token Creator (Create phase only), Tab = Cycle Token
/// </summary>
public sealed class PlayerInput : Component
{
	// --- References ---
	[Property] public GameObject EMPGrenadePrefab { get; set; }

	// --- Cooldowns ---
	[Property] public float SabotageCooldown { get; set; } = 10f;
	[Property] public float PlaceRigCooldown { get; set; } = 2f;

	private float _sabotageTimer;
	private float _placeRigTimer;
	private GoblinPlayer _player;
	private bool _phoneOpen = false;
	private bool _tokenCreatorOpen = false;

	protected override void OnStart()
	{
		_player = Components.Get<GoblinPlayer>();
	}

	protected override void OnUpdate()
	{
		if ( IsProxy ) return;

		_sabotageTimer -= Time.Delta;
		_placeRigTimer -= Time.Delta;

		// ═══ GOBLIN PHONE (T) — Toggle GoblinTwitter phone UI ═══
		if ( Input.Pressed( "Trade" ) ) // T key — repurposed from Trade
		{
			HandlePhone();
		}

		// ═══ TRADE (Y) — Toggle trade panel ═══
		if ( Input.Pressed( "AltTrade" ) )
		{
			HandleTrade();
		}

		// ═══ MARKET (M) — Toggle market panel ═══
		if ( Input.Pressed( "OpenMarket" ) )
		{
			HandleMarket();
		}

		// ═══ TOKEN CREATOR (C) — Create phase only ═══
		if ( Input.Pressed( "CreateToken" ) )
		{
			HandleTokenCreator();
		}

		// ═══ SABOTAGE (Q) — Throw EMP grenade ═══
		if ( Input.Pressed( "Sabotage" ) )
		{
			HandleSabotage();
		}

		// ═══ PLACE RIG (E / Use) ═══
		if ( Input.Pressed( "PlaceRig" ) )
		{
			HandlePlaceRig();
		}

		// ═══ AUDIT VOTE (V) — Call audit on looked-at player ═══
		if ( Input.Pressed( "CallAudit" ) )
		{
			HandleCallAudit();
		}

		// ═══ CYCLE TOKEN (Tab) ═══
		if ( Input.Pressed( "CycleToken" ) )
		{
			Log.Info( "Token cycling — use the phone to browse tokens" );
		}
	}

	// ═══════════════════════════════
	//  GOBLIN PHONE
	// ═══════════════════════════════

	private void HandlePhone()
	{
		var state = GameStateManager.Instance;
		if ( state is not null && !state.IsGameActive )
		{
			Log.Info( "Phone not available yet — wait for the match!" );
			return;
		}

		_phoneOpen = !_phoneOpen;
		var phone = Scene.GetAllComponents<UI.GoblinPhone>().FirstOrDefault();
		if ( phone is not null )
		{
			phone.IsVisible = _phoneOpen;
		}

		// Close token creator if phone opens
		if ( _phoneOpen && _tokenCreatorOpen )
		{
			_tokenCreatorOpen = false;
			var tc = Scene.GetAllComponents<UI.TokenCreator>().FirstOrDefault();
			if ( tc is not null ) tc.IsVisible = false;
		}
	}

	// ═══════════════════════════════
	//  TOKEN CREATOR
	// ═══════════════════════════════

	private void HandleTokenCreator()
	{
		var state = GameStateManager.Instance;
		if ( state is not null && !state.CanCreate )
		{
			Log.Info( "Token creation only during Create phase!" );
			return;
		}

		_tokenCreatorOpen = !_tokenCreatorOpen;
		var creator = Scene.GetAllComponents<UI.TokenCreator>().FirstOrDefault();
		if ( creator is not null )
		{
			creator.IsVisible = _tokenCreatorOpen;
		}

		// Close phone if creator opens
		if ( _tokenCreatorOpen && _phoneOpen )
		{
			_phoneOpen = false;
			var phone = Scene.GetAllComponents<UI.GoblinPhone>().FirstOrDefault();
			if ( phone is not null ) phone.IsVisible = false;
		}
	}

	// ═══════════════════════════════
	//  TRADE
	// ═══════════════════════════════

	private void HandleTrade()
	{
		var state = GameStateManager.Instance;
		if ( state is not null && !state.CanTrade )
		{
			Log.Info( "Trading only available during Shill/Chaos phases!" );
			return;
		}

		var tradePanel = Scene.GetAllComponents<UI.TradePanel>().FirstOrDefault();
		tradePanel?.Toggle();
	}

	// ═══════════════════════════════
	//  MARKET
	// ═══════════════════════════════

	private void HandleMarket()
	{
		var state = GameStateManager.Instance;
		if ( state is not null && !state.IsGameActive )
		{
			Log.Info( "Market not available yet" );
			return;
		}

		Log.Info( "Market panel toggled — check the ticker!" );
	}

	// ═══════════════════════════════
	//  CALL AUDIT
	// ═══════════════════════════════

	private void HandleCallAudit()
	{
		var state = GameStateManager.Instance;
		if ( state is not null && !state.IsGameActive )
			return;

		// Raycast to find a player to accuse
		if ( _player?.Head is null ) return;

		var eyePos = _player.Head.Transform.Position;
		var eyeDir = _player.Head.Transform.Rotation.Forward;

		var trace = Scene.Trace
			.Ray( eyePos, eyePos + eyeDir * 300f )
			.WithTag( "player" )
			.Run();

		if ( !trace.Hit || trace.GameObject is null ) return;

		var targetPlayer = trace.GameObject.Components.Get<GoblinPlayer>();
		if ( targetPlayer is null || targetPlayer.Network.Owner == Network.Owner ) return;

		// Call audit on that player
		var deduction = Scene.GetAllComponents<SocialDeduction>().FirstOrDefault();
		deduction?.RequestAuditVote( targetPlayer.Network.Owner.SteamId );
	}

	// ═══════════════════════════════
	//  SABOTAGE — EMP Grenade
	// ═══════════════════════════════

	private void HandleSabotage()
	{
		var state = GameStateManager.Instance;
		if ( state is not null && state.CurrentPhase != GamePhase.Chaos )
		{
			Log.Info( "Sabotage only available during Chaos phase!" );
			return;
		}

		if ( _sabotageTimer > 0f )
		{
			Log.Info( $"Sabotage on cooldown ({_sabotageTimer:F1}s)" );
			return;
		}

		var wallet = Components.Get<CryptoWallet>();
		if ( wallet is null || wallet.GoblinCoin < 50f )
		{
			Log.Info( "Need 50 GBC to use EMP" );
			return;
		}

		RequestThrowEMP();
	}

	[Rpc.Host]
	private void RequestThrowEMP()
	{
		var caller = Rpc.Caller;
		var player = Scene.GetAllComponents<GoblinPlayer>()
			.FirstOrDefault( p => p.Network.Owner == caller );
		if ( player is null ) return;

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( 50f ) )
			return;

		if ( EMPGrenadePrefab is null )
		{
			Log.Error( "EMPGrenadePrefab not assigned!" );
			return;
		}

		var head = player.Head;
		if ( head is null ) return;

		var spawnPos = head.Transform.Position + head.Transform.Rotation.Forward * 30f;
		var throwDir = head.Transform.Rotation.Forward + Vector3.Up * 0.15f;

		var grenade = EMPGrenadePrefab.Clone( spawnPos );
		grenade.NetworkSpawn();

		var emp = grenade.Components.Get<EMPGrenade>();
		emp?.Launch( spawnPos, throwDir, caller.DisplayName );

		BroadcastSabotage( caller.DisplayName );
	}

	[Rpc.Broadcast]
	private void BroadcastSabotage( string playerName )
	{
		Sound.Play( "sounds/emp_throw.sound" );
		Log.Info( $"{playerName} threw an EMP grenade!" );
		_sabotageTimer = SabotageCooldown;
	}

	// ═══════════════════════════════
	//  PLACE RIG
	// ═══════════════════════════════

	private void HandlePlaceRig()
	{
		if ( _placeRigTimer > 0f ) return;

		var state = GameStateManager.Instance;
		if ( state is not null && !state.CanPlaceRigs )
		{
			Log.Info( "Rig placement only during Create phase!" );
			return;
		}

		if ( _player?.Head is null ) return;

		var eyePos = _player.Head.Transform.Position;
		var eyeDir = _player.Head.Transform.Rotation.Forward;

		var trace = Scene.Trace
			.Ray( eyePos, eyePos + eyeDir * 200f )
			.WithoutTags( "player", "trigger", "projectile" )
			.Run();

		if ( !trace.Hit )
		{
			Log.Info( "Can't place rig there — aim at a surface" );
			return;
		}

		var placePos = trace.HitPosition + trace.Normal * 2f;
		var placeRot = Rotation.FromYaw( _player.EyeAngles.yaw );

		var spawner = Scene.GetAllComponents<NetworkedRigSpawner>().FirstOrDefault();
		spawner?.RequestPlaceRig( placePos, placeRot );

		_placeRigTimer = PlaceRigCooldown;
	}
}
