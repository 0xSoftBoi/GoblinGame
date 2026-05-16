using Sandbox;
using System;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Fires random events during every phase to keep things chaotic.
/// Scales intensity during Chaos phase. Host-only logic, broadcast effects.
/// </summary>
public sealed class RandomEvents : Component
{
	public static RandomEvents Instance { get; private set; }

	[Property] public float EventCheckInterval { get; set; } = 8f;
	[Property] public float ChaosEventCheckInterval { get; set; } = 4f;

	[Sync] public string LastEventName { get; set; } = "";
	[Sync] public float LastEventTime { get; set; } = 0f;

	private float _timer;
	private Random _rng = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		var state = GameStateManager.Instance;
		if ( state is null || !state.IsGameActive ) return;

		float interval = state.CurrentPhase == GamePhase.Chaos
			? ChaosEventCheckInterval
			: EventCheckInterval;

		_timer += Time.Delta;
		if ( _timer < interval ) return;
		_timer = 0f;

		// Higher chance during Chaos
		float roll = (float)_rng.NextDouble();
		float threshold = state.CurrentPhase == GamePhase.Chaos ? 0.4f : 0.15f;

		if ( roll < threshold )
		{
			FireRandomEvent( state.CurrentPhase );
		}
	}

	private void FireRandomEvent( GamePhase phase )
	{
		// Weight events by phase
		int eventId = phase switch
		{
			GamePhase.Create => _rng.Next( 0, 4 ),
			GamePhase.Shill => _rng.Next( 4, 7 ),
			GamePhase.Chaos => _rng.Next( 0, 10 ),
			_ => _rng.Next( 0, 10 )
		};

		switch ( eventId )
		{
			case 0: EventHashBoost(); break;
			case 1: EventPowerOutage(); break;
			case 2: EventGoldRush(); break;
			case 3: EventTaxAudit(); break;
			case 4: EventWhaleAlert(); break;
			case 5: EventLiquidityCrisis(); break;
			case 6: EventAirdrop(); break;
			case 7: EventServerFire(); break;
			case 8: EventRegulatorRaid(); break;
			case 9: EventMemeViralMoment(); break;
		}
	}

	// ═══════════════════════════════
	//  MINING PHASE EVENTS
	// ═══════════════════════════════

	private void EventHashBoost()
	{
		// Double everyone's hash rate for 15 seconds
		foreach ( var w in Scene.GetAllComponents<CryptoWallet>() )
		{
			w.HashRate *= 2f;
		}

		AnnounceEvent( "SOLAR FLARE",
			"Deep space radiation supercharges all rigs! 2x hash rate for 15s!",
			"positive" );

		_ = RevertHashBoostAfter( 15f );
	}

	private async System.Threading.Tasks.Task RevertHashBoostAfter( float seconds )
	{
		await GameTask.DelaySeconds( seconds );
		foreach ( var w in Scene.GetAllComponents<CryptoWallet>() )
		{
			w.HashRate *= 0.5f;
		}
	}

	private void EventPowerOutage()
	{
		// Disable a random player's rigs for 10 seconds
		var rigs = Scene.GetAllComponents<MiningRig>().ToList();
		if ( rigs.Count == 0 ) return;

		var victims = rigs.Where( _ => _rng.NextDouble() < 0.4f ).ToList();
		foreach ( var rig in victims )
		{
			rig.DisableForDuration( 10f );
		}

		AnnounceEvent( "BLACKOUT",
			$"40% stolen electricity, 60% wishful thinking — {victims.Count} rig(s) went dark!",
			"negative" );
	}

	private void EventGoldRush()
	{
		// Grant bonus coins to whoever has the most rigs
		var wallets = Scene.GetAllComponents<CryptoWallet>().ToList();
		if ( wallets.Count == 0 ) return;

		var topMiner = wallets.OrderByDescending( w => w.MiningRigs ).First();
		float bonus = 50f + _rng.Next( 0, 100 );
		topMiner.Deposit( bonus );

		string name = topMiner.Network.Owner?.DisplayName ?? "???";
		AnnounceEvent( "RARE BLOCK",
			$"Once-in-a-thousand block! {name} claims +{bonus:N0} GBC!",
			"positive" );
	}

	private void EventTaxAudit()
	{
		// Tax 10% from the richest player
		var wallets = Scene.GetAllComponents<CryptoWallet>().ToList();
		if ( wallets.Count == 0 ) return;

		var richest = wallets.OrderByDescending( w => w.GoblinCoin ).First();
		float tax = richest.GoblinCoin * 0.1f;
		richest.GoblinCoin -= tax;

		string name = richest.Network.Owner?.DisplayName ?? "???";
		AnnounceEvent( "THE IRS REMEMBERS",
			$"A drone delivers a tax notice to {name}! -{tax:N0} GBC seized!",
			"negative" );
	}

	// ═══════════════════════════════
	//  TRADING PHASE EVENTS
	// ═══════════════════════════════

	private void EventWhaleAlert()
	{
		// A "whale" buys, spiking the price temporarily
		var market = CryptoMarket.Instance;
		if ( market is null ) return;

		market.GoblinCoinPrice *= 1.5f;
		market.IsMooning = true;
		market.MiningMultiplier = 2f;

		AnnounceEvent( "WHALE ALERT",
			"Wallet 0xDEAD...BEEF drops a massive buy order! Price pumping!",
			"positive" );
	}

	private void EventLiquidityCrisis()
	{
		// Trading fees spike — drain a flat fee from everyone
		float fee = 15f;
		foreach ( var w in Scene.GetAllComponents<CryptoWallet>() )
		{
			w.GoblinCoin = MathF.Max( 0, w.GoblinCoin - fee );
		}

		AnnounceEvent( "FEE ALGORITHM",
			$"The intern's code strikes again! -{fee:N0} GBC from every wallet!",
			"negative" );
	}

	private void EventAirdrop()
	{
		// Free coins for everyone
		float amount = 20f + _rng.Next( 0, 40 );
		foreach ( var w in Scene.GetAllComponents<CryptoWallet>() )
		{
			w.Deposit( amount );
		}

		AnnounceEvent( "MYSTERY DROP",
			$"Unidentified smart contract fires! Everyone gets +{amount:N0} GBC!",
			"positive" );
	}

	// ═══════════════════════════════
	//  CHAOS PHASE EVENTS
	// ═══════════════════════════════

	private void EventServerFire()
	{
		// Destroy (disable) ALL rigs for 8 seconds
		foreach ( var rig in Scene.GetAllComponents<MiningRig>() )
		{
			rig.DisableForDuration( 8f );
		}

		AnnounceEvent( "THERMAL RUNAWAY",
			"The Furnace lives up to its name! ALL rigs offline for 8 seconds!",
			"negative" );
	}

	private void EventRegulatorRaid()
	{
		// The "SEC" freezes the richest player's wallet
		var wallets = Scene.GetAllComponents<CryptoWallet>().ToList();
		if ( wallets.Count == 0 ) return;

		var target = wallets.OrderByDescending( w => w.GoblinCoin ).First();
		float seized = target.GoblinCoin * 0.25f;
		target.GoblinCoin -= seized;

		// Redistribute to everyone else
		float share = seized / MathF.Max( 1, wallets.Count - 1 );
		foreach ( var w in wallets )
		{
			if ( w != target )
				w.Deposit( share );
		}

		string name = target.Network.Owner?.DisplayName ?? "???";
		AnnounceEvent( "REGULATORS",
			$"Blockchain Council enforcement seizes 25% of {name}'s wallet!",
			"negative" );
	}

	private void EventMemeViralMoment()
	{
		// Random player gets a huge bonus from "going viral"
		var wallets = Scene.GetAllComponents<CryptoWallet>().ToList();
		if ( wallets.Count == 0 ) return;

		var lucky = wallets[_rng.Next( wallets.Count )];
		float bonus = 100f + _rng.Next( 0, 200 );
		lucky.Deposit( bonus );

		string name = lucky.Network.Owner?.DisplayName ?? "???";
		AnnounceEvent( "GOING VIRAL",
			$"{name}'s hastily photoshopped meme goes nuclear! +{bonus:N0} GBC!",
			"positive" );
	}

	// ═══════════════════════════════
	//  BROADCAST
	// ═══════════════════════════════

	[Rpc.Broadcast]
	private void AnnounceEvent( string eventName, string description, string tone )
	{
		LastEventName = eventName;
		LastEventTime = Time.Now;

		// Sound
		string sound = tone == "positive" ? "sounds/event_positive.sound" : "sounds/event_negative.sound";
		Sound.Play( sound );

		Log.Info( $"[EVENT] {eventName}: {description}" );

		// Push to notification feed
		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( eventName, description, tone );
	}
}
