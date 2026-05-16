using Sandbox;

namespace GoblinChain;

/// <summary>
/// Tracks sabotage items the player owns. Items are earned during Chaos phase
/// or purchased from the market. Synced so other players can see your threat level.
/// </summary>
public sealed class SabotageInventory : Component
{
	[Sync] public int EMPGrenades { get; set; } = 0;
	[Sync] public int DDOSCharges { get; set; } = 0;   // Future: target a player's hash rate
	[Sync] public int PumpFakeTokens { get; set; } = 0; // Future: inflate a fake coin then dump

	[Property] public int EMPStartCount { get; set; } = 2;
	[Property] public float EMPPrice { get; set; } = 50f;

	/// <summary>
	/// Called at start of Chaos phase to give players starting sabotage items.
	/// </summary>
	public void RefillForChaos()
	{
		if ( IsProxy ) return;
		EMPGrenades = EMPStartCount;
	}

	/// <summary>
	/// Try to use an EMP. Returns true if one was consumed.
	/// </summary>
	public bool TryUseEMP()
	{
		if ( IsProxy ) return false;
		if ( EMPGrenades <= 0 ) return false;

		EMPGrenades--;
		return true;
	}

	/// <summary>
	/// Purchase an EMP from the market.
	/// </summary>
	public bool TryBuyEMP( CryptoWallet wallet )
	{
		if ( IsProxy ) return false;
		if ( wallet is null || !wallet.TrySpend( EMPPrice ) ) return false;

		EMPGrenades++;
		return true;
	}
}
