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
		// GBC component (either side may be 0)
		public float OfferedCoins;
		public float RequestedCoins;
		// Token component (Guid.Empty = no token on that side)
		public Guid OfferedTokenId;
		public float OfferedTokenAmount;
		public Guid RequestedTokenId;
		public float RequestedTokenAmount;
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
			OfferedTokenId = read.Read<Guid>();
			OfferedTokenAmount = read.Read<float>();
			RequestedTokenId = read.Read<Guid>();
			RequestedTokenAmount = read.Read<float>();
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
			write.Write( OfferedTokenId );
			write.Write( OfferedTokenAmount );
			write.Write( RequestedTokenId );
			write.Write( RequestedTokenAmount );
			write.Write( SenderAccepted );
			write.Write( ReceiverAccepted );
			write.Write( CreatedAt );
		}
	}

	// ═══════════════════════════════════════
	//  CLIENT → HOST: Propose a trade
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestTrade( Guid targetPlayerId,
		float offerCoins, float requestCoins,
		Guid offerTokenId, float offerTokenAmount,
		Guid requestTokenId, float requestTokenAmount )
	{
		var caller = Rpc.Caller;

		var state = GameStateManager.Instance;
		if ( state is not null && !state.CanTrade )
		{
			Log.Warning( $"{caller.DisplayName} tried to trade outside trading phase" );
			return;
		}

		var sender = FindPlayerByConnection( caller );
		if ( sender is null ) return;

		var receiver = GoblinPlayer.All.FirstOrDefault( p => p.Id == targetPlayerId );
		if ( receiver is null ) { Log.Warning( "Trade target not found" ); return; }

		var senderWallet = sender.Components.Get<CryptoWallet>();
		if ( senderWallet is null ) return;

		// Validate GBC
		if ( senderWallet.GoblinCoin < offerCoins )
		{
			Log.Warning( $"{caller.DisplayName} can't afford to offer {offerCoins} GBC" );
			return;
		}

		// Validate offered token holdings
		if ( offerTokenId != Guid.Empty && offerTokenAmount > 0f )
		{
			if ( senderWallet.GetTokenHolding( offerTokenId ) < offerTokenAmount )
			{
				Log.Warning( $"{caller.DisplayName} doesn't have enough tokens to offer" );
				return;
			}
		}

		int activeSenderTrades = 0;
		foreach ( var kv in ActiveTrades )
			if ( kv.Value.SenderObjectId == sender.Id ) activeSenderTrades++;
		if ( activeSenderTrades >= 3 ) { Log.Warning( "Too many active trades" ); return; }

		var trade = new TradeData
		{
			TradeId = Guid.NewGuid(),
			SenderObjectId = sender.Id,
			ReceiverObjectId = receiver.Id,
			SenderName = caller.DisplayName,
			ReceiverName = receiver.Network.Owner?.DisplayName ?? "???",
			OfferedCoins = offerCoins,
			RequestedCoins = requestCoins,
			OfferedTokenId = offerTokenId,
			OfferedTokenAmount = offerTokenAmount,
			RequestedTokenId = requestTokenId,
			RequestedTokenAmount = requestTokenAmount,
			SenderAccepted = true,
			ReceiverAccepted = false,
			CreatedAt = Time.Now
		};

		ActiveTrades[trade.TradeId] = trade;
		BroadcastTradeProposed( trade.TradeId, trade.SenderName, trade.ReceiverName, offerCoins, requestCoins );
		Log.Info( $"Trade proposed: {trade.SenderName} → {trade.ReceiverName} | {offerCoins} GBC + {offerTokenAmount} tokens ↔ {requestCoins} GBC + {requestTokenAmount} tokens" );
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
		bool insufficientGBC = sw.GoblinCoin < trade.OfferedCoins || rw.GoblinCoin < trade.RequestedCoins;
		bool insufficientSenderToken = trade.OfferedTokenId != Guid.Empty
			&& sw.GetTokenHolding( trade.OfferedTokenId ) < trade.OfferedTokenAmount;
		bool insufficientReceiverToken = trade.RequestedTokenId != Guid.Empty
			&& rw.GetTokenHolding( trade.RequestedTokenId ) < trade.RequestedTokenAmount;

		if ( insufficientGBC || insufficientSenderToken || insufficientReceiverToken )
		{
			ActiveTrades.Remove( trade.TradeId );
			BroadcastTradeFailed( trade.TradeId, "Insufficient funds" );
			return;
		}

		// Swap GBC
		sw.GoblinCoin -= trade.OfferedCoins;
		sw.GoblinCoin += trade.RequestedCoins;
		rw.GoblinCoin -= trade.RequestedCoins;
		rw.GoblinCoin += trade.OfferedCoins;

		// Swap tokens if applicable
		if ( trade.OfferedTokenId != Guid.Empty && trade.OfferedTokenAmount > 0f )
		{
			sw.RemoveTokenHolding( trade.OfferedTokenId, trade.OfferedTokenAmount );
			rw.AddTokenHolding( trade.OfferedTokenId, trade.OfferedTokenAmount );
		}
		if ( trade.RequestedTokenId != Guid.Empty && trade.RequestedTokenAmount > 0f )
		{
			rw.RemoveTokenHolding( trade.RequestedTokenId, trade.RequestedTokenAmount );
			sw.AddTokenHolding( trade.RequestedTokenId, trade.RequestedTokenAmount );
		}

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
