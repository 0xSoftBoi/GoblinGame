using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Simulates the global crypto market backdrop.
/// Now integrates with TokenSystem for player-created tokens.
/// GBC remains the base currency; player tokens float against it.
/// Runs on host, syncs key values to clients for UI display.
/// </summary>
public sealed class CryptoMarket : Component
{
	public static CryptoMarket Instance { get; private set; }

	// --- Synced market state ---
	[Sync] public float GoblinCoinPrice { get; set; } = 1.00f;
	[Sync] public float MiningMultiplier { get; set; } = 1.0f;
	[Sync] public bool IsCrashing { get; set; } = false;
	[Sync] public bool IsMooning { get; set; } = false;
	[Sync] public string MarketHeadline { get; set; } = "Markets stable. For now.";

	// Global market sentiment affects ALL tokens (-1 to +1)
	[Sync] public float MarketSentiment { get; set; } = 0f;

	// --- Config ---
	[Property] public float TickInterval { get; set; } = 2f;
	[Property] public float BaseVolatility { get; set; } = 0.05f;
	[Property] public float CrashChancePerTick { get; set; } = 0.02f;
	[Property] public float MoonChancePerTick { get; set; } = 0.01f;

	// --- Internal ---
	private float _tickTimer;
	private Random _rng = new();
	private List<float> _priceHistory = new();

	protected override void OnStart()
	{
		Instance = this;
		_priceHistory.Add( GoblinCoinPrice );
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		_tickTimer += Time.Delta;
		if ( _tickTimer < TickInterval ) return;
		_tickTimer -= TickInterval;

		SimulateTick();
	}

	private void SimulateTick()
	{
		float prevPrice = GoblinCoinPrice;
		float change = 0f;

		// Random walk with fat tails
		float normalMove = ((float)_rng.NextDouble() - 0.5f) * 2f * BaseVolatility;
		change += normalMove;

		// Crash event
		if ( !IsCrashing && !IsMooning && _rng.NextDouble() < CrashChancePerTick )
		{
			IsCrashing = true;
			IsMooning = false;
			MarketSentiment = -0.8f;
			MarketHeadline = GameLore.GetRandomCrashHeadline();
			MiningMultiplier = 0.3f;
			AnnounceCrash( MarketHeadline );

			// Trigger physics chaos in the office
			var office = Scene.GetAllComponents<OfficeSetup>().FirstOrDefault();
			office?.TriggerPhysicsChaos( 1.5f );
		}

		// Moon event
		if ( !IsMooning && !IsCrashing && _rng.NextDouble() < MoonChancePerTick )
		{
			IsMooning = true;
			IsCrashing = false;
			MarketSentiment = 0.8f;
			MarketHeadline = GameLore.GetRandomMoonHeadline();
			MiningMultiplier = 3.0f;
			AnnounceMoon( MarketHeadline );
		}

		// Apply event modifiers
		if ( IsCrashing )
		{
			change -= (float)_rng.NextDouble() * 0.15f;

			if ( GoblinCoinPrice < prevPrice * 0.5f || _rng.NextDouble() < 0.15f )
			{
				IsCrashing = false;
				MiningMultiplier = 1.0f;
				MarketSentiment *= 0.5f;
				MarketHeadline = GameLore.GetRandomNormalHeadline();
			}
		}
		else if ( IsMooning )
		{
			change += (float)_rng.NextDouble() * 0.2f;

			if ( GoblinCoinPrice > prevPrice * 2f || _rng.NextDouble() < 0.1f )
			{
				IsMooning = false;
				MiningMultiplier = 1.0f;
				MarketSentiment *= 0.5f;
				MarketHeadline = GameLore.GetRandomNormalHeadline();
			}
		}

		// Decay sentiment toward neutral
		MarketSentiment *= 0.98f;

		// Apply change (price can't go below 0.01)
		GoblinCoinPrice = MathF.Max( 0.01f, GoblinCoinPrice * (1f + change) );

		// Track history (keep last 60 ticks for chart)
		_priceHistory.Add( GoblinCoinPrice );
		if ( _priceHistory.Count > 60 )
			_priceHistory.RemoveAt( 0 );

		// Occasional headline rotation during normal times
		if ( !IsCrashing && !IsMooning && _rng.NextDouble() < 0.08f )
		{
			MarketHeadline = GameLore.GetRandomNormalHeadline();
		}

		// Push global sentiment to all player-created tokens
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		tokenSystem?.ApplyGlobalDrift( MarketSentiment );
	}

	// --- Broadcasts ---

	[Rpc.Broadcast]
	private void AnnounceCrash( string headline )
	{
		Sound.Play( "sounds/market_crash.sound" );
		Log.Warning( $"MARKET CRASH: {headline}" );
	}

	[Rpc.Broadcast]
	private void AnnounceMoon( string headline )
	{
		Sound.Play( "sounds/market_moon.sound" );
		Log.Info( $"TO THE MOON: {headline}" );
	}

	// --- Public API for UI ---

	public record CoinDisplayData( string Symbol, string FullName, string Tagline, float Price, float Change );

	/// <summary>
	/// Returns display data for the ticker — now includes player-created tokens!
	/// </summary>
	public List<CoinDisplayData> GetDisplayData()
	{
		float change24 = _priceHistory.Count >= 2
			? ((GoblinCoinPrice - _priceHistory[0]) / _priceHistory[0]) * 100f
			: 0f;

		var data = new List<CoinDisplayData>
		{
			new( "GBC", "GoblinCoin", "The base currency. Mine it, shill it, lose it.", GoblinCoinPrice, change24 )
		};

		// Add player-created tokens from TokenSystem
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		if ( tokenSystem is not null )
		{
			foreach ( var token in tokenSystem.GetActiveTokensSorted() )
			{
				if ( token.IsRugged ) continue;
				string status = token.Price > 1.5f ? "📈 PUMPING" : token.Price < 0.3f ? "📉 DUMPING" : "";
				data.Add( new CoinDisplayData(
					token.Ticker,
					token.Name,
					status,
					token.Price,
					0f // TODO: track per-token price history for change%
				));
			}
		}

		return data;
	}

	public List<float> GetPriceHistory() => new( _priceHistory );

	/// <summary>
	/// Get the global market sentiment for external systems.
	/// </summary>
	public float GetSentiment() => MarketSentiment;
}
