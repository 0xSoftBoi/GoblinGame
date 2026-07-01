using Sandbox;
using System;
using GoblinChain.UI;

namespace GoblinChain;

/// <summary>
/// Watches CurrentRound and upgrades the game era at round boundaries.
/// Tunes market volatility, random event frequency, and SEC raid thresholds.
/// Host-only logic; era change is broadcast to all clients for UI.
/// </summary>
public sealed class EraManager : Component
{
	public static EraManager Instance { get; private set; }

	// Round → era mapping (MaxRounds=5). Names live in OfficeSetup.EraNames:
	//   Round 1-2 → Era 0: Garage Startup
	//   Round 3   → Era 1: Funded WeWork
	//   Round 4   → Era 2: Crypto Exchange
	//   Round 5   → Era 3: Penthouse Suite
	private static readonly int[] EraByRound = { 0, 0, 1, 2, 3 };

	// Volatile market params per era
	private static readonly float[] Volatility      = { 0.05f, 0.07f, 0.12f, 0.20f };
	private static readonly float[] CrashChance     = { 0.02f, 0.04f, 0.07f, 0.12f };
	private static readonly float[] MoonChance      = { 0.01f, 0.015f, 0.02f, 0.03f };
	private static readonly float[] RaidThreshold   = { 100f,  80f,   65f,   50f   };
	private static readonly float[] EventInterval   = { 8f,    6f,    4f,    3f    };
	private static readonly float[] ChaosInterval   = { 4f,    3f,    2.5f,  2f    };

	private int _lastEra = -1;

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		var state = GameStateManager.Instance;
		if ( state is null ) return;

		int round = Math.Clamp( state.CurrentRound - 1, 0, EraByRound.Length - 1 );
		int era = EraByRound[round];

		if ( era == _lastEra ) return;
		_lastEra = era;

		ApplyEraParams( era );

		if ( era > 0 )
			OfficeSetup.Instance?.UpgradeEra();

		BroadcastEraChange( era );
	}

	private void ApplyEraParams( int era )
	{
		var market = CryptoMarket.Instance;
		if ( market is not null )
		{
			market.BaseVolatility    = Volatility[era];
			market.CrashChancePerTick = CrashChance[era];
			market.MoonChancePerTick  = MoonChance[era];
		}

		var sec = SECSystem.Instance;
		if ( sec is not null )
			sec.RaidThreshold = RaidThreshold[era];

		var events = RandomEvents.Instance;
		if ( events is not null )
		{
			events.EventCheckInterval       = EventInterval[era];
			events.ChaosEventCheckInterval  = ChaosInterval[era];
		}

		Log.Info( $"[EraManager] Era {era}: {OfficeSetup.EraNames[era]} — volatility={Volatility[era]:F2} crashChance={CrashChance[era]:F2} raidAt={RaidThreshold[era]}" );
	}

	[Rpc.Broadcast]
	private void BroadcastEraChange( int era )
	{
		var hud = Scene.GetAllComponents<GoblinHud>().FirstOrDefault();
		hud?.ShowEraTransition( OfficeSetup.EraNames[era], OfficeSetup.EraDescriptions[era] );

		Log.Info( $"[Era] Entering: {OfficeSetup.EraNames[era]}" );
	}
}
