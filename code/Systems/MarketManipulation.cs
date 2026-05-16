using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Market manipulation tools unlocked progressively:
/// - Wash Trading: fake volume to pump price (available from start)
/// - Pump Group: coordinate with other players for synchronized buy (era 1+)
/// - Short Selling: bet against a token (era 2+)
/// All actions cost GBC and increase SEC heat.
/// </summary>
public sealed class MarketManipulation : Component
{
	public static MarketManipulation Instance { get; private set; }

	// --- Config ---
	[Property] public float WashTradeCost { get; set; } = 200f;
	[Property] public float WashTradeHeat { get; set; } = 15f;
	[Property] public float WashTradePriceBoost { get; set; } = 0.08f; // +8% per wash

	[Property] public float PumpGroupCost { get; set; } = 500f;
	[Property] public float PumpGroupHeat { get; set; } = 25f;
	[Property] public float PumpGroupDuration { get; set; } = 30f; // seconds
	[Property] public float PumpGroupBoost { get; set; } = 0.03f; // +3% per second during pump

	[Property] public float ShortSellCost { get; set; } = 300f;
	[Property] public float ShortSellHeat { get; set; } = 10f;
	[Property] public float ShortSellLeverage { get; set; } = 3f;

	// --- Active manipulations ---
	[Sync] public int ActivePumpGroups { get; set; } = 0;

	private Dictionary<Guid, PumpGroupData> _activePumps = new();
	private Dictionary<Guid, ShortPosition> _activeShorts = new();
	private Random _rng = new();

	private class PumpGroupData
	{
		public Guid TokenId;
		public Guid InitiatorId;
		public float TimeRemaining;
		public float BoostPerSecond;
		public List<Guid> Participants = new();
	}

	private class ShortPosition
	{
		public Guid PlayerId;
		public Guid TokenId;
		public float EntryPrice;
		public float Amount;
		public float Leverage;
	}

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		// Tick active pump groups
		var expiredPumps = new List<Guid>();
		foreach ( var kvp in _activePumps )
		{
			kvp.Value.TimeRemaining -= Time.Delta;
			if ( kvp.Value.TimeRemaining <= 0f )
			{
				expiredPumps.Add( kvp.Key );
				continue;
			}

			// Apply continuous price boost
			var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
			tokenSystem?.ApplyNPCBuyPressure( kvp.Value.TokenId, kvp.Value.BoostPerSecond * Time.Delta * 100f );
		}

		foreach ( var id in expiredPumps )
		{
			_activePumps.Remove( id );
			ActivePumpGroups--;
			BroadcastPumpEnd();
		}
	}

	// ═══════════════════════════════
	//  WASH TRADING
	// ═══════════════════════════════

	/// <summary>
	/// Fake buy/sell volume to pump a token's price. Instant effect.
	/// </summary>
	[Rpc.Host]
	public void RequestWashTrade( Guid tokenId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( WashTradeCost ) ) return;

		// Apply price boost
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		if ( tokenSystem is null ) return;

		tokenSystem.ApplyNPCBuyPressure( tokenId, WashTradeCost * 2f ); // 2x fake volume

		// Add SEC heat
		var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
		sec?.AddHeat( player.GameObject, WashTradeHeat );

		BroadcastWashTrade( caller.DisplayName );
	}

	[Rpc.Broadcast]
	private void BroadcastWashTrade( string playerName )
	{
		Log.Info( $"[SUSPICIOUS] Unusual volume detected..." );
	}

	// ═══════════════════════════════
	//  PUMP GROUP
	// ═══════════════════════════════

	/// <summary>
	/// Start a pump group — continuous price boost for 30 seconds.
	/// Other players can join to amplify the effect.
	/// </summary>
	[Rpc.Host]
	public void RequestStartPump( Guid tokenId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		// Check era requirement
		var office = Scene.GetAllComponents<OfficeSetup>().FirstOrDefault();
		if ( office is not null && office.OfficeEra < 1 )
		{
			NotifyPlayer( caller, "Pump groups unlock at Funded era!" );
			return;
		}

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( PumpGroupCost ) ) return;

		var pumpId = Guid.NewGuid();
		_activePumps[pumpId] = new PumpGroupData
		{
			TokenId = tokenId,
			InitiatorId = caller.SteamId,
			TimeRemaining = PumpGroupDuration,
			BoostPerSecond = PumpGroupBoost,
			Participants = new List<Guid> { caller.SteamId }
		};
		ActivePumpGroups++;

		// SEC heat
		var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
		sec?.AddHeat( player.GameObject, PumpGroupHeat );

		BroadcastPumpStart( caller.DisplayName, pumpId.ToString() );
	}

	[Rpc.Host]
	public void RequestJoinPump( string pumpIdStr )
	{
		if ( !Guid.TryParse( pumpIdStr, out var pumpId ) ) return;
		if ( !_activePumps.TryGetValue( pumpId, out var pump ) ) return;

		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( PumpGroupCost * 0.5f ) ) return; // Half price to join

		pump.Participants.Add( caller.SteamId );
		pump.BoostPerSecond += PumpGroupBoost * 0.5f; // Each joiner adds 50% more boost

		var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
		sec?.AddHeat( player.GameObject, PumpGroupHeat * 0.5f );
	}

	[Rpc.Broadcast]
	private void BroadcastPumpStart( string playerName, string pumpId )
	{
		Sound.Play( "sounds/pump_group.sound" );
		Log.Info( $"📈 {playerName} started a PUMP GROUP! Press J to join!" );
	}

	[Rpc.Broadcast]
	private void BroadcastPumpEnd()
	{
		Log.Info( "📉 A pump group just expired. Hope you sold in time..." );
	}

	// ═══════════════════════════════
	//  SHORT SELLING
	// ═══════════════════════════════

	/// <summary>
	/// Open a short position — bet that a token's price will drop.
	/// Profit = (entryPrice - currentPrice) * amount * leverage
	/// </summary>
	[Rpc.Host]
	public void RequestShortSell( Guid tokenId, float amount )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		// Check era requirement
		var office = Scene.GetAllComponents<OfficeSetup>().FirstOrDefault();
		if ( office is not null && office.OfficeEra < 2 )
		{
			NotifyPlayer( caller, "Short selling unlocks at Exchange era!" );
			return;
		}

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( ShortSellCost ) ) return;

		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		var token = tokenSystem?.GetToken( tokenId );
		if ( token is null ) return;

		var shortId = Guid.NewGuid();
		_activeShorts[shortId] = new ShortPosition
		{
			PlayerId = caller.SteamId,
			TokenId = tokenId,
			EntryPrice = token.Value.Price,
			Amount = amount,
			Leverage = ShortSellLeverage
		};

		// Shorting applies sell pressure
		tokenSystem?.ApplyNPCSellPressure( tokenId, amount * 0.5f );

		// SEC heat
		var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
		sec?.AddHeat( player.GameObject, ShortSellHeat );

		BroadcastShort( caller.DisplayName );
	}

	/// <summary>
	/// Close a short position and collect (or lose) profit.
	/// </summary>
	[Rpc.Host]
	public void RequestCloseShort( string shortIdStr )
	{
		if ( !Guid.TryParse( shortIdStr, out var shortId ) ) return;
		if ( !_activeShorts.TryGetValue( shortId, out var pos ) ) return;

		var caller = Rpc.Caller;
		if ( pos.PlayerId != caller.SteamId ) return;

		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		var token = tokenSystem?.GetToken( pos.TokenId );
		if ( token is null ) return;

		float priceDiff = pos.EntryPrice - token.Value.Price;
		float profit = priceDiff * pos.Amount * pos.Leverage;

		var player = FindPlayer( caller );
		var wallet = player?.Components.Get<CryptoWallet>();
		if ( wallet is null ) return;

		if ( profit > 0 )
		{
			wallet.Deposit( profit );
			Log.Info( $"Short closed: +{profit:N0} GBC profit!" );
		}
		else
		{
			// Loss — take from balance (can't go negative)
			float loss = MathF.Min( MathF.Abs( profit ), wallet.GoblinCoin );
			wallet.TrySpend( loss );
			Log.Info( $"Short closed: -{loss:N0} GBC loss! Rekt." );
		}

		_activeShorts.Remove( shortId );
	}

	[Rpc.Broadcast]
	private void BroadcastShort( string playerName )
	{
		Log.Info( $"🐻 Someone just opened a SHORT position..." );
	}

	// ═══════════════════════════════
	//  HELPERS
	// ═══════════════════════════════

	private GoblinPlayer FindPlayer( Connection caller )
	{
		return GoblinPlayer.All
			.FirstOrDefault( p => p.Network.Owner == caller );
	}

	[Rpc.Owner]
	private void NotifyPlayer( Connection target, string message )
	{
		Log.Info( message );
	}
}
