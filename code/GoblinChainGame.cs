using Sandbox;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Root game manager — attach to a GameObject in main.scene.
/// Handles player join/leave, spawns player prefabs, and holds global config.
/// </summary>
public sealed class GoblinChainGame : Component, Component.INetworkListener
{
	public static GoblinChainGame Instance { get; private set; }

	// --- Config (exposed to editor) ---
	[Property] public GameObject PlayerPrefab { get; set; }
	[Property] public int MinPlayers { get; set; } = 4;
	[Property] public int MaxPlayers { get; set; } = 8;
	[Property] public bool DebugSkipMinPlayers { get; set; } = false;

	// --- Runtime ---
	[Sync] public int ConnectedPlayers { get; set; } = 0;

	protected override void OnStart()
	{
		Instance = this;

		LoadHud();

		// Phase music runs locally on every machine (host and clients)
		Components.GetOrCreate<MusicManager>();

		if ( IsProxy )
			return;

		Log.Info( "=== GOBLIN CHAIN: Crypto Chaos Tycoon ===" );
		Log.Info( $"Waiting for {MinPlayers}-{MaxPlayers} players..." );
	}

	private void LoadHud()
	{
		var hudScene = SceneFile.Load( "scenes/hud.scene" );
		if ( hudScene is null )
		{
			Log.Error( "Failed to load scenes/hud.scene — ensure the file exists in the scenes/ folder" );
			return;
		}

		var opts = new SceneLoadOptions();
		opts.SetScene( hudScene );
		opts.IsAdditive = true;
		Scene.Load( opts );
	}

	protected override void OnDestroy()
	{
		if ( Instance == this )
			Instance = null;
	}

	// --- INetworkListener: player join/leave ---

	public void OnActive( Connection connection )
	{
		Log.Info( $"Player connected: {connection.DisplayName}" );

		if ( IsProxy ) return;

		// Spawn a player object owned by this connection
		SpawnPlayer( connection );

		ConnectedPlayers = GoblinPlayer.All.Count();

		// Check if we have enough players to start
		var state = Scene.GetAllComponents<GameStateManager>().FirstOrDefault();
		if ( state is not null && state.CurrentPhase == GamePhase.WaitingForPlayers )
		{
			int required = DebugSkipMinPlayers ? 1 : MinPlayers;
			if ( ConnectedPlayers >= required )
			{
				state.StartPregame();
			}
		}
	}

	public void OnDisconnected( Connection connection )
	{
		Log.Info( $"Player disconnected: {connection.DisplayName}" );

		if ( IsProxy ) return;

		// Destroy their player object
		var player = GoblinPlayer.All
			.FirstOrDefault( p => p.Network.Owner == connection );

		if ( player is not null )
		{
			player.GameObject.Destroy();
		}

		ConnectedPlayers = GoblinPlayer.All.Count();
	}

	// --- Spawning ---

	private void SpawnPlayer( Connection connection )
	{
		if ( PlayerPrefab is null )
		{
			Log.Error( "PlayerPrefab is null — assign it in the editor!" );
			return;
		}

		// Pick a spawn point (cycle through available ones)
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToList();
		var spawnIndex = ConnectedPlayers % System.Math.Max( 1, spawnPoints.Count );
		var spawnPos = spawnPoints.Count > 0
			? spawnPoints[spawnIndex].WorldPosition
			: Vector3.Up * 100f; // Fallback

		var spawnRot = spawnPoints.Count > 0
			? spawnPoints[spawnIndex].WorldRotation
			: Rotation.Identity;

		var playerObj = PlayerPrefab.Clone( spawnPos, spawnRot );
		playerObj.Name = $"Player_{connection.DisplayName}";
		playerObj.NetworkSpawn( connection );

		Log.Info( $"Spawned {connection.DisplayName} at {spawnPos}" );
	}
}
