using Sandbox;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Handles rig placement requests from clients.
/// All validation runs host-side — clients can't cheat.
/// Attach to a manager GameObject in your scene.
/// </summary>
public sealed class NetworkedRigSpawner : Component
{
	public static NetworkedRigSpawner Instance { get; private set; }

	[Property] public GameObject RigPrefab { get; set; }
	[Property] public int MaxRigsPerPlayer { get; set; } = 5;
	[Property] public float PlacementRange { get; set; } = 200f;
	[Property] public float RigCost { get; set; } = 100f;
	[Property] public float BaseHashPerRig { get; set; } = 5f;

	protected override void OnStart()
	{
		Instance = this;
	}

	/// <summary>
	/// Client requests to place a rig at a position.
	/// Host validates everything before spawning.
	/// </summary>
	[Rpc.Host]
	public void RequestPlaceRig( Vector3 position, Rotation rotation )
	{
		var caller = Rpc.Caller;

		// 1. Find caller's player
		var player = Scene.GetAllComponents<GoblinPlayer>()
			.FirstOrDefault( p => p.Network.Owner == caller );
		if ( player is null ) return;

		// 2. Check game phase
		var state = GameStateManager.Instance;
		if ( state is not null && !state.CanPlaceRigs )
		{
			NotifyClient( caller, "Can only place rigs during Mining phase!" );
			return;
		}

		// 3. Check rig limit
		int owned = Scene.GetAllComponents<MiningRig>()
			.Count( r => r.Network.Owner == caller );
		if ( owned >= MaxRigsPerPlayer )
		{
			NotifyClient( caller, $"Rig limit reached ({MaxRigsPerPlayer})" );
			return;
		}

		// 4. Check distance from player
		float dist = Vector3.DistanceBetween( player.WorldPosition, position );
		if ( dist > PlacementRange )
		{
			NotifyClient( caller, "Too far away to place rig" );
			return;
		}

		// 5. Check wallet
		var wallet = player.Components.Get<CryptoWallet>();
		if ( wallet is null || !wallet.TrySpend( RigCost ) )
		{
			NotifyClient( caller, $"Need {RigCost} GBC to place a rig" );
			return;
		}

		// 6. Spawn the rig
		if ( RigPrefab is null )
		{
			Log.Error( "RigPrefab is null!" );
			return;
		}

		var rig = RigPrefab.Clone( position, rotation );
		rig.Name = $"Rig_{caller.DisplayName}_{owned + 1}";
		rig.NetworkSpawn( caller );

		// 7. Update wallet hash rate
		wallet.AddRig( BaseHashPerRig );

		// 8. Tell everyone
		BroadcastRigPlaced( caller.DisplayName, position, owned + 1 );

		Log.Info( $"Rig placed by {caller.DisplayName} at {position} (#{owned + 1})" );
	}

	// --- Broadcasts ---

	[Rpc.Broadcast]
	private void BroadcastRigPlaced( string playerName, Vector3 position, int rigNumber )
	{
		Sound.Play( "sounds/rig_placed.sound", position );
		Log.Info( $"{playerName} placed rig #{rigNumber}!" );
	}

	/// <summary>
	/// Send a feedback message to a specific client.
	/// </summary>
	private void NotifyClient( Connection target, string message )
	{
		// In a full implementation, this would route to the client's UI.
		// For now, just log it server-side.
		Log.Info( $"[→ {target.DisplayName}] {message}" );
	}
}
