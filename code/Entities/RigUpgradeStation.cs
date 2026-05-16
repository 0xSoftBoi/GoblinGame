using Sandbox;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// A world object players can interact with to upgrade their mining rigs.
/// Place in the map near spawn areas. Implements IInteractable.
///
/// Upgrade tiers:
///   T1 (default) → T2 (200 GBC, +10 H/s)
///   T2 → T3 (500 GBC, +25 H/s)
///   T3 → MAX
/// </summary>
public sealed class RigUpgradeStation : Component, IInteractable
{
	[Property] public float Tier2Cost { get; set; } = 200f;
	[Property] public float Tier3Cost { get; set; } = 500f;
	[Property] public float Tier2HashBonus { get; set; } = 10f;
	[Property] public float Tier3HashBonus { get; set; } = 25f;

	public void OnInteract( GoblinPlayer player )
	{
		// Must be on host
		if ( IsProxy ) return;

		// Find player's lowest-tier rig
		var playerRigs = Scene.GetAllComponents<MiningRig>()
			.Where( r => r.Network.Owner == player.Network.Owner )
			.OrderBy( r => r.Tier )
			.ToList();

		if ( playerRigs.Count == 0 )
		{
			Log.Info( "No rigs to upgrade" );
			return;
		}

		var rig = playerRigs.First();
		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null ) return;

		switch ( rig.Tier )
		{
			case 1:
				if ( !wallet.TrySpend( Tier2Cost ) )
				{
					Log.Info( $"Need {Tier2Cost} GBC for T2 upgrade" );
					return;
				}
				rig.Tier = 2;
				rig.BaseHashRate += Tier2HashBonus;
				wallet.HashRate += Tier2HashBonus;
				AnnounceUpgrade( player.Network.Owner.DisplayName, 2 );
				break;

			case 2:
				if ( !wallet.TrySpend( Tier3Cost ) )
				{
					Log.Info( $"Need {Tier3Cost} GBC for T3 upgrade" );
					return;
				}
				rig.Tier = 3;
				rig.BaseHashRate += Tier3HashBonus;
				wallet.HashRate += Tier3HashBonus;
				AnnounceUpgrade( player.Network.Owner.DisplayName, 3 );
				break;

			default:
				Log.Info( "Rig already at max tier" );
				break;
		}
	}

	[Rpc.Broadcast]
	private void AnnounceUpgrade( string playerName, int tier )
	{
		Sound.Play( "sounds/rig_upgrade.sound" );
		Log.Info( $"{playerName} upgraded a rig to Tier {tier}!" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "UPGRADE", $"{playerName} → T{tier} rig!", "positive" );
	}
}
