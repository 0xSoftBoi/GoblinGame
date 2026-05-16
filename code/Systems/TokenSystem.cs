using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Manages player-created tokens. Each round, players create meme coins
/// with custom names/tickers. Tokens have independent prices driven by
/// shill pressure, trading volume, and NPC activity.
/// Host-authoritative — all creation/trading/rugging validated server-side.
/// </summary>
public sealed class TokenSystem : Component
{
	public static TokenSystem Instance { get; private set; }

	// --- Synced State ---
	[Sync] public NetDictionary<Guid, TokenData> ActiveTokens { get; set; } = new();
	[Sync] public int TotalTokensCreated { get; set; } = 0;

	// --- Config ---
	[Property] public float CreationCost { get; set; } = 50f;
	[Property] public float BaseTokenPrice { get; set; } = 1.0f;
	[Property] public float PriceTickInterval { get; set; } = 2f;
	[Property] public float MaxTokensPerPlayer { get; set; } = 1f;
	[Property] public float RugPullReputationHit { get; set; } = 0.3f;
	[Property] public float PivotCost { get; set; } = 100f;

	// --- Internal ---
	private float _priceTimer;
	private Random _rng = new();

	// Per-token shill pressure accumulated between ticks
	private Dictionary<Guid, float> _shillPressure = new();
	private Dictionary<Guid, float> _buyPressure = new();
	private Dictionary<Guid, float> _sellPressure = new();
	private Dictionary<Guid, List<float>> _priceHistory = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		_priceTimer += Time.Delta;
		if ( _priceTimer < PriceTickInterval ) return;
		_priceTimer -= PriceTickInterval;

		UpdateAllTokenPrices();
	}

	// ═══════════════════════════════════════
	//  TOKEN CREATION
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestCreateToken( string name, string ticker, int iconIndex )
	{
		var caller = Rpc.Caller;

		// Find player
		var player = FindPlayer( caller );
		if ( player is null ) return;

		// Phase check — only during Mining/Create phase
		var state = GameStateManager.Instance;
		if ( state is not null && state.CurrentPhase != GamePhase.Create )
		{
			Log.Warning( $"{caller.DisplayName} tried to create token outside Create phase" );
			return;
		}

		// Limit tokens per player per round
		int owned = 0;
		foreach ( var kv in ActiveTokens )
		{
			if ( kv.Value.CreatorId == player.Id && !kv.Value.IsRugged )
				owned++;
		}
		if ( owned >= MaxTokensPerPlayer )
		{
			Log.Warning( $"{caller.DisplayName} already has max active tokens" );
			return;
		}

		// Validate name
		if ( string.IsNullOrWhiteSpace( name ) || name.Length > 24 )
		{
			Log.Warning( "Invalid token name" );
			return;
		}
		if ( string.IsNullOrWhiteSpace( ticker ) || ticker.Length > 5 )
			ticker = GenerateTicker( name );

		ticker = ticker.ToUpper();

		// Check uniqueness
		foreach ( var kv in ActiveTokens )
		{
			if ( kv.Value.Ticker == ticker )
			{
				Log.Warning( $"Ticker {ticker} already exists" );
				return;
			}
		}

		// Deduct creation cost
		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( CreationCost ) )
		{
			Log.Warning( $"{caller.DisplayName} can't afford token creation ({CreationCost} GBC)" );
			return;
		}

		// Calculate initial price from name "hype" + randomness
		float nameHype = CalculateNameHype( name );
		float initialPrice = BaseTokenPrice * (0.5f + nameHype * 1.5f);

		// Create token
		var token = new TokenData
		{
			Id = Guid.NewGuid(),
			Name = name,
			Ticker = ticker,
			IconIndex = iconIndex,
			CreatorId = player.Id,
			CreatorName = caller.DisplayName,
			Quality = 50f + _rng.Next( -20, 30 ),
			Price = initialPrice,
			PreviousPrice = initialPrice,
			Supply = 1000000f,
			PooledValue = CreationCost, // Creator's cost seeds the pool
			CreatedAt = Time.Now,
			IsRugged = false,
			RoundCreated = GameStateManager.Instance?.CurrentRound ?? 1
		};

		ActiveTokens[token.Id] = token;
		_shillPressure[token.Id] = 0f;
		_buyPressure[token.Id] = 0f;
		_sellPressure[token.Id] = 0f;
		_priceHistory[token.Id] = new List<float> { initialPrice };
		TotalTokensCreated++;

		// Creator auto-holds some tokens
		wallet.AddTokenHolding( token.Id, 100000f );

		BroadcastTokenCreated( caller.DisplayName, name, ticker, initialPrice );

		Log.Info( $"TOKEN CREATED: ${ticker} ({name}) by {caller.DisplayName} at {initialPrice:F3} GBC" );
	}

	// ═══════════════════════════════════════
	//  RUG PULL
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestRugPull( Guid tokenId )
	{
		var caller = Rpc.Caller;
		if ( !ActiveTokens.TryGetValue( tokenId, out var token ) ) return;

		var player = FindPlayer( caller );
		if ( player is null ) return;

		// Only creator can rug
		if ( token.CreatorId != player.Id )
		{
			Log.Warning( $"{caller.DisplayName} tried to rug someone else's token" );
			return;
		}

		if ( token.IsRugged )
		{
			Log.Warning( "Token already rugged" );
			return;
		}

		// Execute rug: creator gets the pooled value
		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is not null )
		{
			wallet.Deposit( token.PooledValue );
		}

		// Crash the token
		token.IsRugged = true;
		token.Price = 0f;
		ActiveTokens[tokenId] = token;

		// All holders lose their tokens (value is gone)
		foreach ( var w in Scene.GetAllComponents<CryptoWallet>() )
		{
			if ( w != wallet )
				w.RemoveTokenHolding( tokenId );
		}

		// Reputation hit
		var rep = player.Components.Get<ReputationTracker>();
		rep?.AdjustReputation( -RugPullReputationHit );

		// SEC heat
		var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
		sec?.AddHeat( player, 40f );

		BroadcastRugPull( caller.DisplayName, token.Ticker, token.PooledValue );

		// NPC crowd reacts to the rug
		NPCInvestors.Instance?.TriggerRugPullReactions( token.Ticker );

		Log.Info( $"RUG PULL: {caller.DisplayName} rugged ${token.Ticker} for {token.PooledValue:N0} GBC!" );
	}

	// ═══════════════════════════════════════
	//  PIVOT (Rebrand)
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestPivot( Guid tokenId, string newName, string newTicker )
	{
		var caller = Rpc.Caller;
		if ( !ActiveTokens.TryGetValue( tokenId, out var token ) ) return;

		var player = FindPlayer( caller );
		if ( player is null ) return;
		if ( token.CreatorId != player.Id ) return;
		if ( token.IsRugged ) return;

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( PivotCost ) ) return;

		if ( string.IsNullOrWhiteSpace( newTicker ) || newTicker.Length > 5 )
			newTicker = GenerateTicker( newName );

		token.Name = newName;
		token.Ticker = newTicker.ToUpper();
		token.Quality += 10f; // Pivoting improves quality slightly
		ActiveTokens[tokenId] = token;

		// Reputation boost
		var rep = player.Components.Get<ReputationTracker>();
		rep?.AdjustReputation( 0.1f );

		BroadcastPivot( caller.DisplayName, token.Ticker );
	}

	// ═══════════════════════════════════════
	//  BUY / SELL TOKENS
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestBuyToken( Guid tokenId, float gbcAmount )
	{
		var caller = Rpc.Caller;
		if ( !ActiveTokens.TryGetValue( tokenId, out var token ) ) return;
		if ( token.IsRugged ) return;
		if ( gbcAmount <= 0 ) return;

		var player = FindPlayer( caller );
		if ( player is null ) return;

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( gbcAmount ) ) return;

		// Calculate tokens received
		float tokensReceived = gbcAmount / token.Price;
		wallet.AddTokenHolding( tokenId, tokensReceived );

		// Add to pool and buy pressure
		token.PooledValue += gbcAmount;
		ActiveTokens[tokenId] = token;

		if ( !_buyPressure.ContainsKey( tokenId ) )
			_buyPressure[tokenId] = 0f;
		_buyPressure[tokenId] += gbcAmount * 0.01f;

		Log.Info( $"{caller.DisplayName} bought {tokensReceived:N0} ${token.Ticker} for {gbcAmount:N1} GBC" );
	}

	[Rpc.Host]
	public void RequestSellToken( Guid tokenId, float tokenAmount )
	{
		var caller = Rpc.Caller;
		if ( !ActiveTokens.TryGetValue( tokenId, out var token ) ) return;
		if ( token.IsRugged ) return;
		if ( tokenAmount <= 0 ) return;

		var player = FindPlayer( caller );
		if ( player is null ) return;

		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null ) return;

		float held = wallet.GetTokenHolding( tokenId );
		if ( held < tokenAmount ) tokenAmount = held;
		if ( tokenAmount <= 0 ) return;

		float gbcReceived = tokenAmount * token.Price;

		// Can't withdraw more than the pool
		if ( gbcReceived > token.PooledValue )
			gbcReceived = token.PooledValue;

		wallet.RemoveTokenHolding( tokenId, tokenAmount );
		wallet.Deposit( gbcReceived );

		token.PooledValue -= gbcReceived;
		ActiveTokens[tokenId] = token;

		if ( !_sellPressure.ContainsKey( tokenId ) )
			_sellPressure[tokenId] = 0f;
		_sellPressure[tokenId] += gbcReceived * 0.01f;
	}

	// ═══════════════════════════════════════
	//  SHILL PRESSURE (called by GoblinTwitter)
	// ═══════════════════════════════════════

	public void AddShillPressure( Guid tokenId, float amount )
	{
		if ( !_shillPressure.ContainsKey( tokenId ) )
			_shillPressure[tokenId] = 0f;
		_shillPressure[tokenId] += amount;
	}

	public void ApplyNPCBuyPressure( Guid tokenId, float gbcAmount )
	{
		if ( !_buyPressure.ContainsKey( tokenId ) )
			_buyPressure[tokenId] = 0f;
		_buyPressure[tokenId] += gbcAmount * 0.0005f;
	}

	public void ApplyNPCSellPressure( Guid tokenId, float gbcAmount )
	{
		if ( !_sellPressure.ContainsKey( tokenId ) )
			_sellPressure[tokenId] = 0f;
		_sellPressure[tokenId] += gbcAmount * 0.0005f;
	}

	// ═══════════════════════════════════════
	//  PRICE ENGINE
	// ═══════════════════════════════════════

	private void UpdateAllTokenPrices()
	{
		var keys = ActiveTokens.Keys.ToList();
		foreach ( var id in keys )
		{
			if ( !ActiveTokens.TryGetValue( id, out var token ) ) continue;
			if ( token.IsRugged ) continue;

			float buy = _buyPressure.GetValueOrDefault( id, 0f );
			float sell = _sellPressure.GetValueOrDefault( id, 0f );
			float shill = _shillPressure.GetValueOrDefault( id, 0f );

			// Net pressure
			float netPressure = (buy - sell) + (shill * 0.008f);

			// Quality affects stability (high quality = less volatile)
			float volatility = 0.08f - (token.Quality * 0.0004f);

			// Random noise
			float noise = ((float)_rng.NextDouble() - 0.5f) * 2f * volatility;

			// Global market influence
			var market = CryptoMarket.Instance;
			float globalDrift = 0f;
			if ( market is not null )
			{
				if ( market.IsCrashing ) globalDrift = -0.03f;
				else if ( market.IsMooning ) globalDrift = 0.05f;
			}

			float totalChange = netPressure + noise + globalDrift;

			token.PreviousPrice = token.Price;
			token.Price = MathF.Max( 0.001f, token.Price * (1f + totalChange) );
			ActiveTokens[id] = token;

			// Track history
			if ( _priceHistory.TryGetValue( id, out var history ) )
			{
				history.Add( token.Price );
				if ( history.Count > 60 ) history.RemoveAt( 0 );
			}

			// Reset pressure accumulators
			_buyPressure[id] = 0f;
			_sellPressure[id] = 0f;
			_shillPressure[id] = _shillPressure.GetValueOrDefault( id, 0f ) * 0.5f; // Decay
		}
	}

	// ═══════════════════════════════════════
	//  ROUND MANAGEMENT
	// ═══════════════════════════════════════

	/// <summary>
	/// Called at end of results phase to clean up dead tokens.
	/// </summary>
	public void CleanupRound()
	{
		var toRemove = ActiveTokens
			.Where( kv => kv.Value.IsRugged || kv.Value.Price < 0.01f )
			.Select( kv => kv.Key )
			.ToList();

		foreach ( var id in toRemove )
		{
			ActiveTokens.Remove( id );
			_shillPressure.Remove( id );
			_buyPressure.Remove( id );
			_sellPressure.Remove( id );
			_priceHistory.Remove( id );
		}
	}

	// ═══════════════════════════════════════
	//  PUBLIC API
	// ═══════════════════════════════════════

	public List<float> GetPriceHistory( Guid tokenId )
		=> _priceHistory.TryGetValue( tokenId, out var h ) ? new( h ) : new();

	public List<TokenData> GetActiveTokensSorted()
		=> ActiveTokens.Values
			.Where( t => !t.IsRugged )
			.OrderByDescending( t => t.PooledValue )
			.ToList();

	public TokenData? GetToken( Guid id )
		=> ActiveTokens.TryGetValue( id, out var t ) ? t : null;

	// ═══════════════════════════════════════
	//  HELPERS
	// ═══════════════════════════════════════

	private float CalculateNameHype( string name )
	{
		float hype = 0.5f;
		var lower = name.ToLower();

		// Meme keywords boost hype
		string[] hypeWords = { "moon", "doge", "pepe", "elon", "inu", "safe", "rocket",
			"diamond", "ape", "chad", "giga", "mega", "ultra", "king", "god", "pump" };
		foreach ( var w in hypeWords )
			if ( lower.Contains( w ) ) hype += 0.15f;

		// ALL CAPS = more hype
		if ( name == name.ToUpper() && name.Length > 2 ) hype += 0.2f;

		// Short names are punchier
		if ( name.Length <= 5 ) hype += 0.1f;

		return MathF.Min( 1f, hype );
	}

	private string GenerateTicker( string name )
	{
		if ( name.Length <= 5 ) return name.ToUpper();
		// Take consonants
		var consonants = name.ToUpper().Where( c => !"AEIOU ".Contains( c ) ).Take( 4 );
		var ticker = new string( consonants.ToArray() );
		return ticker.Length >= 3 ? ticker : name[..Math.Min( 4, name.Length )].ToUpper();
	}

	private GoblinPlayer FindPlayer( Connection conn )
		=> GoblinPlayer.All
			.FirstOrDefault( p => p.Network.Owner == conn );

	// ═══════════════════════════════════════
	//  BROADCASTS
	// ═══════════════════════════════════════

	[Rpc.Broadcast]
	private void BroadcastTokenCreated( string creator, string name, string ticker, float price )
	{
		Sound.Play( "sounds/event_positive.sound" );
		Log.Info( $"NEW TOKEN: ${ticker} ({name}) by {creator} — Price: {price:F3} GBC" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "NEW LISTING", $"{creator} launched ${ticker}!", "positive" );
	}

	[Rpc.Broadcast]
	private void BroadcastRugPull( string creator, string ticker, float amount )
	{
		Sound.Play( "sounds/market_crash.sound" );
		Log.Info( $"RUG PULL: {creator} pulled the rug on ${ticker}! Stole {amount:N0} GBC!" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "RUG PULLED", $"{creator} rugged ${ticker} for {amount:N0} GBC!", "negative" );

		ClipRecorder.Instance?.OnRugPull( creator, ticker, amount );
	}

	[Rpc.Broadcast]
	private void BroadcastPivot( string creator, string newTicker )
	{
		Log.Info( $"PIVOT: {creator} rebranded to ${newTicker}" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "REBRAND", $"{creator} pivoted to ${newTicker}!", "positive" );
	}
}

// ═══════════════════════════════════════
//  TOKEN DATA (network-serializable)
// ═══════════════════════════════════════

public struct TokenData : INetworkSerializable
{
	public Guid Id;
	public string Name;
	public string Ticker;
	public int IconIndex;
	public Guid CreatorId;
	public string CreatorName;
	public float Quality;
	public float Price;
	public float PreviousPrice;
	public float Supply;
	public float PooledValue;
	public float CreatedAt;
	public bool IsRugged;
	public int RoundCreated;

	public float PriceChange => PreviousPrice > 0 ? ((Price - PreviousPrice) / PreviousPrice) * 100f : 0f;

	public void Read( ref NetRead read )
	{
		Id = read.Read<Guid>();
		Name = read.Read<string>();
		Ticker = read.Read<string>();
		IconIndex = read.Read<int>();
		CreatorId = read.Read<Guid>();
		CreatorName = read.Read<string>();
		Quality = read.Read<float>();
		Price = read.Read<float>();
		PreviousPrice = read.Read<float>();
		Supply = read.Read<float>();
		PooledValue = read.Read<float>();
		CreatedAt = read.Read<float>();
		IsRugged = read.Read<bool>();
		RoundCreated = read.Read<int>();
	}

	public void Write( NetWrite write )
	{
		write.Write( Id );
		write.Write( Name );
		write.Write( Ticker );
		write.Write( IconIndex );
		write.Write( CreatorId );
		write.Write( CreatorName );
		write.Write( Quality );
		write.Write( Price );
		write.Write( PreviousPrice );
		write.Write( Supply );
		write.Write( PooledValue );
		write.Write( CreatedAt );
		write.Write( IsRugged );
		write.Write( RoundCreated );
	}
}

// ═══════════════════════════════════════
//  REPUTATION TRACKER (attach to player prefab)
// ═══════════════════════════════════════

public sealed class ReputationTracker : Component
{
	[Sync] public float Reputation { get; set; } = 1.0f;
	[Sync] public int RugPullCount { get; set; } = 0;
	[Sync] public int HonestRounds { get; set; } = 0;

	public float ReputationMultiplier => MathF.Max( 0.2f, MathF.Min( 2.0f, Reputation ) );

	public void AdjustReputation( float delta )
	{
		if ( IsProxy ) return;
		Reputation = MathF.Max( 0f, MathF.Min( 2f, Reputation + delta ) );
		if ( delta < 0 ) RugPullCount++;
		if ( delta > 0 ) HonestRounds++;
	}

	public string GetTitle()
	{
		if ( RugPullCount >= 3 ) return "SERIAL RUGGER";
		if ( Reputation >= 1.8f ) return "DIAMOND HANDS";
		if ( Reputation >= 1.4f ) return "TRUSTED DEV";
		if ( Reputation <= 0.3f ) return "KNOWN SCAMMER";
		if ( Reputation <= 0.6f ) return "SUS";
		return "ANON DEV";
	}
}
