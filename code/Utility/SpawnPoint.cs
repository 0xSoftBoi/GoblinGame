using Sandbox;

namespace GoblinChain;

/// <summary>
/// Marker component. Place on empty GameObjects in your scene
/// to define player spawn locations. GoblinChainGame cycles through
/// these when spawning players.
/// </summary>
public sealed class SpawnPoint : Component
{
	[Property] public int Priority { get; set; } = 0;

	protected override void OnStart()
	{
		// Add a tag so traces can skip spawn points
		Tags.Add( "spawnpoint" );
	}
}
