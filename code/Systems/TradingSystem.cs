using Sandbox;
using System;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Networked trading system. Clients propose trades via [Rpc.Host],
/// host validates and executes, results broadcast to all.
/// </summary>
public sealed class TradingSystem : Component
{
	public static TradingSystem Instance { get; private set; }

	[Sync] public NetDictionary<Guid, TradeData> ActiveTrades { get; set; } = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	// ═══════════════════════════════════════
	//  TRADE DATA (network-serializable)
	// ═══════════════════════════════════════

	public struct TradeData : INetworkSerializable
	{
		public Guid TradeId;
		public Guid SenderObjectId;
		public Guid ReceiverObjectId;
		public string SenderName;
		public string ReceiverName;
		public float OfferedCoins;
		public float RequestedCoins;
		public string OfferedItem;
		public bool SenderAccepted;
		public bool ReceiverAccepted;
		public float CreatedAt;

		public void Read( ref NetRead read )
		{
			TradeId = read.Read<Guid>();
			SenderObjectId = read.Read<Guid>();
			ReceiverObjectId = read.Read<Guid>();
			SenderName = read.Read<string>();
			ReceiverName = read.Read<string>();
			OfferedCoins = read.Read<float>();
			RequestedCoins = read.Read<float>();
			OfferedItem = read.Read<string>();
			SenderAccepted = read.Read<bool>();
			ReceiverAccepted = read.Read<bool>();
			CreatedAt = read.Read<float>();
		}

		public void Write( NetWrite write )
		{
			write.Write( TradeId );
			write.Write( SenderObjectId );
			write.Write( ReceiverObjectId );
			write.Write( SenderName );
			write.Write( ReceiverName );
			write.Write( OfferedCoins );
			write.Write( RequestedCoins );
			write.Write( OfferedItem );
			write.Write( SenderAccepted );
			write.Write( ReceiverAccepted );
			write.Write( CreatedAt );
		}
	}

	// ═══════════════════════════════════════
	//  CLIENT → HOST: Propose a trade
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestTrade( Guid targetPlayerId, float offerCoins,
		float requestCoins, string offerItem )
	{
		var caller = Rpc.Caller;

		// Phase check
		var state = GameStateManager.Instance;
		if ( state is not null && !state.CanTrade )
		{
			Log.Warning( $"{caller.DisplayName} tried to trade outside trading phase" );
			return;
		}

		// Find sender
		var sender = FindPlayerByConnection( caller );
		if ( sender is null ) return;

		// Find receiver by component ID
		var receiver = GoblinPlayer.All
			.FirstOrDefault( p => p.Id == targetPlayerId );
		if ( receiver is null )
		{
			Log.Warning( "Trade target not found" );
			return;
		}

		// Validate sender funds
		var senderWallet = sender.Components.Get<CryptoWallet>();
		if ( senderWallet is null || senderWallet.GoblinCoin < offerCoins )
		{
			Log.Warning( $"{caller.DisplayName} can't afford to offer {offerCoins} GBC" );
			return;
		}

		// Limit active trades per player (prevent spam)
		int activeSenderTrades = 0;
		foreach ( var kv in ActiveTrades )
		{
			if ( kv.Value.SenderObjectId == sender.Id )
				activeSenderTrades++;
		}
		if ( activeSenderTrades >= 3 )
		{
			Log.Warning( "Too many active trades" );
			return;
		}

		// Create trade
		var trade = new TradeData
		{
			TradeId = Guid.NewGuid(),
			SenderObjectId = sender.Id,
			ReceiverObjectId = receiver.Id,
			SenderName = caller.DisplayName,
			ReceiverName = receiver.Network.Owner?.DisplayName ?? "???",
			OfferedCoins = offerCoins,
			RequestedCoins = requestCoins,
			OfferedItem = offerItem ?? "",
			SenderAccepted = true,
			ReceiverAccepted = false,
			CreatedAt = Time.Now
		};

		ActiveTrades[trade.TradeId] = trade;

		BroadcastTradeProposed( trade.TradeId, trade.SenderName,
			trade.ReceiverName, offerCoins, requestCoins );

		Log.Info( $"Trade proposed: {trade.SenderName} offers {offerCoins} GBC, wants {requestCoins} GBC from {trade.ReceiverName}" );
	}

	// ═══════════════════════════════════════
	//  CLIENT → HOST: Accept / Reject
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void AcceptTrade( Guid tradeId )
	{
		if ( !ActiveTrades.TryGetValue( tradeId, out var trade ) )
			return;

		var caller = Rpc.Caller;
		var accepter = FindPlayerByConnection( caller );
		if ( accepter is null ) return;

		// Only the receiver or sender can accept
		if ( accepter.Id == trade.ReceiverObjectId )
			trade.ReceiverAccepted = true;
		else if ( accepter.Id == trade.SenderObjectId )
			trade.SenderAccepted = true;
		else
			return;

		ActiveTrades[tradeId] = trade;

		// Both accepted → execute
		if ( trade.SenderAccepted && trade.ReceiverAccepted )
		{
			ExecuteTrade( trade );
		}
	}

	[Rpc.Host]
	public void RejectTrade( Guid tradeId )
	{
		if ( !ActiveTrades.ContainsKey( tradeId ) )
			return;

		var trade = ActiveTrades[tradeId];
		ActiveTrades.Remove( tradeId );

		BroadcastTradeRejected( tradeId, trade.SenderName, trade.ReceiverName );
	}

	// ═══════════════════════════════════════
	//  HOST-ONLY: Execute validated trade
	// ═══════════════════════════════════════

	private void ExecuteTrade( TradeData trade )
	{
		var sender = GoblinPlayer.All
			.FirstOrDefault( p => p.Id == trade.SenderObjectId );
		var receiver = GoblinPlayer.All
			.FirstOrDefault( p => p.Id == trade.ReceiverObjectId );

		if ( sender is null || receiver is null )
		{
			ActiveTrades.Remove( trade.TradeId );
			return;
		}

		var sw = sender.Components.Get<CryptoWallet>();
		var rw = receiver.Components.Get<CryptoWallet>();

		if ( sw is null || rw is null )
		{
			ActiveTrades.Remove( trade.TradeId );
			return;
		}

		// Final validation — balances may have changed since proposal
		if ( sw.GoblinCoin < trade.OfferedCoins ||
			 rw.GoblinCoin < trade.RequestedCoins )
		{
			ActiveTrades.Remove( trade.TradeId );
			BroadcastTradeFailed( trade.TradeId, "Insufficient funds" );
			return;
		}

		// Execute swap
		sw.GoblinCoin -= trade.OfferedCoins;
		sw.GoblinCoin += trade.RequestedCoins;
		rw.GoblinCoin -= trade.RequestedCoins;
		rw.GoblinCoin += trade.OfferedCoins;

		ActiveTrades.Remove( trade.TradeId );

		BroadcastTradeCompleted( trade.SenderName, trade.ReceiverName,
			trade.OfferedCoins, trade.RequestedCoins );

		Log.Info( $"Trade executed: {trade.SenderName} ↔ {trade.ReceiverName}" );
	}

	// ═══════════════════════════════════════
	//  Expiration cleanup
	// ═══════════════════════════════════════

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		// Expire trades older than 30 seconds
		var expired = ActiveTrades
			.Where( kv => Time.Now - kv.Value.CreatedAt > 30f )
			.Select( kv => kv.Key )
			.ToList();

		foreach ( var id in expired )
		{
			ActiveTrades.Remove( id );
			Log.Info( $"Trade {id} expired" );
		}
	}

	// ═══════════════════════════════════════
	//  Broadcasts
	// ═══════════════════════════════════════

	[Rpc.Broadcast]
	private void BroadcastTradeProposed( Guid tradeId, string sender,
		string receiver, float offered, float requested )
	{
		Log.Info( $"Trade proposed: {sender} offers {offered} GBC, wants {requested} GBC from {receiver}" );
	}

	[Rpc.Broadcast]
	private void BroadcastTradeCompleted( string sender, string receiver,
		float senderGave, float receiverGave )
	{
		Sound.Play( "sounds/trade_complete.sound" );
		Log.Info( $"TRADE: {sender} ↔ {receiver} ({senderGave} ↔ {receiverGave} GBC)" );
	}

	[Rpc.Broadcast]
	private void BroadcastTradeRejected( Guid tradeId, string sender, string receiver )
	{
		Log.Info( $"Trade between {sender} and {receiver} was rejected" );
	}

	[Rpc.Broadcast]
	private void BroadcastTradeFailed( Guid tradeId, string reason )
	{
		Log.Info( $"Trade failed: {reason}" );
	}

	// ═══════════════════════════════════════
	//  Helpers
	// ═══════════════════════════════════════

	private GoblinPlayer FindPlayerByConnection( Connection conn )
	{
		return GoblinPlayer.All
			.FirstOrDefault( p => p.Network.Owner == conn );
	}
}
