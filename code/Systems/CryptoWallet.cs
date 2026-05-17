using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Networked wallet attached to each player. Tracks GoblinCoin balance,
/// hash rate, mining rig count, and now TOKEN HOLDINGS for player-created tokens.
/// All values sync automatically.
/// </summary>
public sealed class CryptoWallet : Component
{
	// --- Synced State (visible to all clients) ---

	[Sync, Change( "OnBalanceChanged" )]
	public float GoblinCoin { get; set; } = 100f;

	[Sync] public float HashRate { get; set; } = 0f;
	[Sync] public int MiningRigs { get; set; } = 0;
	[Sync] public int TotalMined { get; set; } = 0;

	// --- Bot identity ---
	[Sync] public bool IsBot { get; set; } = false;
	[Sync] public string BotName { get; set; } = "";

	public string OwnerName => IsBot ? BotName : (Network.Owner?.DisplayName ?? "???");

	// --- Token Holdings ---
	// Maps token ID -> amount held. Synced across network.
	[Sync] public NetDictionary<Guid, float> TokenHoldings { get; set; } = new();

	// --- Local-only (UI smoothing) ---
	public float DisplayBalance { get; set; } = 100f;
	public float BalanceDelta { get; private set; } = 0f;

	// --- Config ---
	[Property] public float MiningTickInterval { get; set; } = 1f;

	private float _miningTimer = 0f;

	protected override void OnStart()
	{
		DisplayBalance = GoblinCoin;
	}

	protected override void OnUpdate()
	{
		// Smooth the displayed balance for visual juice
		float prev = DisplayBalance;
		DisplayBalance = MathX.Lerp( DisplayBalance, GoblinCoin, Time.Delta * 8f );
		BalanceDelta = DisplayBalance - prev;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		// Passive mining: earn coins based on hash rate
		if ( HashRate <= 0f ) return;

		_miningTimer += Time.Delta;

		if ( _miningTimer >= MiningTickInterval )
		{
			_miningTimer -= MiningTickInterval;

			float mined = HashRate * MiningTickInterval;

			// Apply market volatility modifier if available
			var market = Scene.GetAllComponents<CryptoMarket>().FirstOrDefault();
			if ( market is not null )
			{
				mined *= market.MiningMultiplier;
			}

			GoblinCoin += mined;
			TotalMined += (int)mined;
		}
	}

	// --- Change callback ---

	private void OnBalanceChanged( float oldValue, float newValue )
	{
		float diff = newValue - oldValue;

		if ( diff > 50f )
			Log.Info( $"Big deposit: +{diff:N1} GBC" );
		else if ( diff < -50f )
			Log.Info( $"Big withdrawal: {diff:N1} GBC" );
	}

	// --- GBC API ---

	public bool TrySpend( float amount )
	{
		if ( IsProxy ) return false;
		if ( GoblinCoin < amount ) return false;
		GoblinCoin -= amount;
		return true;
	}

	public void Deposit( float amount )
	{
		if ( IsProxy ) return;
		GoblinCoin += amount;
	}

	public void AddRig( float hashContribution )
	{
		if ( IsProxy ) return;
		MiningRigs++;
		HashRate += hashContribution;
	}

	public void DisableMining()
	{
		if ( IsProxy ) return;
		HashRate = 0f;
	}

	// --- Token Holdings API ---

	/// <summary>
	/// Add tokens to this wallet. Host-only.
	/// </summary>
	public void AddTokenHolding( Guid tokenId, float amount )
	{
		if ( IsProxy ) return;
		if ( TokenHoldings.ContainsKey( tokenId ) )
			TokenHoldings[tokenId] = TokenHoldings[tokenId] + amount;
		else
			TokenHoldings[tokenId] = amount;
	}

	/// <summary>
	/// Remove tokens from this wallet. Returns actual amount removed. Host-only.
	/// </summary>
	public float RemoveTokenHolding( Guid tokenId, float amount )
	{
		if ( IsProxy ) return 0f;
		if ( !TokenHoldings.ContainsKey( tokenId ) ) return 0f;

		float held = TokenHoldings[tokenId];
		float removed = MathF.Min( held, amount );
		TokenHoldings[tokenId] = held - removed;

		if ( TokenHoldings[tokenId] <= 0.001f )
			TokenHoldings.Remove( tokenId );

		return removed;
	}

	/// <summary>
	/// Get amount of a specific token held.
	/// </summary>
	public float GetTokenHolding( Guid tokenId )
	{
		return TokenHoldings.TryGetValue( tokenId, out float amount ) ? amount : 0f;
	}

	/// <summary>
	/// Get total portfolio value in GBC (GBC balance + all token holdings at current prices).
	/// </summary>
	public float GetTotalPortfolioValue()
	{
		float total = GoblinCoin;

		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		if ( tokenSystem is not null )
		{
			foreach ( var kvp in TokenHoldings )
			{
				var token = tokenSystem.GetToken( kvp.Key );
				if ( token.HasValue )
					total += kvp.Value * token.Value.Price;
			}
		}

		return total;
	}

	/// <summary>
	/// Reset wallet for new match. Host-only.
	/// </summary>
	public void ResetAll( float startingGBC = 100f )
	{
		if ( IsProxy ) return;
		GoblinCoin = startingGBC;
		HashRate = 0f;
		MiningRigs = 0;
		TotalMined = 0;
		TokenHoldings.Clear();
	}
}
