using Sandbox;
using System;
using System.Collections.Generic;

namespace GoblinChain;

/// <summary>
/// Procedural office layout for the goblin crypto startup.
/// Spawns desks, chairs, monitors, whiteboards, etc. from prefab lists.
/// Host-only — spawns once at match start, all props are networked.
/// </summary>
public sealed class OfficeSetup : Component
{
	public static OfficeSetup Instance { get; private set; }

	// --- Prefabs (assign in editor) ---
	[Property] public List<GameObject> DeskPrefabs { get; set; } = new();
	[Property] public List<GameObject> ChairPrefabs { get; set; } = new();
	[Property] public List<GameObject> MonitorPrefabs { get; set; } = new();
	[Property] public List<GameObject> DecorationPrefabs { get; set; } = new();
	[Property] public GameObject WhiteboardPrefab { get; set; }
	[Property] public GameObject CoffeeMachinePrefab { get; set; }
	[Property] public GameObject ServerRackPrefab { get; set; }
	[Property] public GameObject TrashCanPrefab { get; set; }

	// --- Layout Config ---
	[Property] public Vector3 OfficeCenter { get; set; } = Vector3.Zero;
	[Property] public float OfficeWidth { get; set; } = 800f;
	[Property] public float OfficeDepth { get; set; } = 600f;
	[Property] public int DeskRows { get; set; } = 3;
	[Property] public int DesksPerRow { get; set; } = 4;
	[Property] public float DeskSpacing { get; set; } = 150f;
	[Property] public float RowSpacing { get; set; } = 200f;

	// --- State ---
	[Sync] public int OfficeEra { get; set; } = 0; // 0=startup, 1=funded, 2=exchange, 3=penthouse
	private List<GameObject> _spawnedProps = new();
	private Random _rng = new();
	private bool _hasSpawned = false;

	// Era names for display
	public static readonly string[] EraNames = { "Garage Startup", "Funded WeWork", "Crypto Exchange", "Penthouse Suite" };
	public static readonly string[] EraDescriptions = {
		"A damp garage with folding tables and stolen WiFi.",
		"Open-plan WeWork with standing desks and a kombucha tap.",
		"Glass-walled exchange floor with Bloomberg terminals everywhere.",
		"Gold-plated penthouse. You made it. Was it worth it?"
	};

	protected override void OnStart()
	{
		Instance = this;
	}

	/// <summary>
	/// Call from GameStateManager at match start. Host-only.
	/// </summary>
	public void SpawnOffice()
	{
		if ( IsProxy || _hasSpawned ) return;
		_hasSpawned = true;

		ClearOffice();
		SpawnDeskLayout();
		SpawnUtilityStations();
		SpawnDecorations();

		Log.Info( $"Office spawned: {EraNames[OfficeEra]} ({_spawnedProps.Count} props)" );
	}

	/// <summary>
	/// Upgrade to next era. Clears and rebuilds with better props.
	/// </summary>
	public void UpgradeEra()
	{
		if ( IsProxy ) return;
		if ( OfficeEra >= 3 ) return;

		OfficeEra++;
		_hasSpawned = false;
		SpawnOffice();

		BroadcastUpgrade( EraNames[OfficeEra], EraDescriptions[OfficeEra] );
	}

	[Rpc.Broadcast]
	private void BroadcastUpgrade( string eraName, string eraDesc )
	{
		Sound.Play( "sounds/office_upgrade.sound" );
		Log.Info( $"OFFICE UPGRADED: {eraName} — {eraDesc}" );
	}

	private void ClearOffice()
	{
		foreach ( var prop in _spawnedProps )
		{
			if ( prop.IsValid() )
				prop.Destroy();
		}
		_spawnedProps.Clear();
	}

	private void SpawnDeskLayout()
	{
		if ( DeskPrefabs.Count == 0 ) return;

		var startX = OfficeCenter.x - ((DesksPerRow - 1) * DeskSpacing) / 2f;
		var startY = OfficeCenter.y - ((DeskRows - 1) * RowSpacing) / 2f;

		for ( int row = 0; row < DeskRows; row++ )
		{
			for ( int col = 0; col < DesksPerRow; col++ )
			{
				var pos = new Vector3(
					startX + col * DeskSpacing,
					startY + row * RowSpacing,
					OfficeCenter.z
				);

				// Desk
				var deskPrefab = DeskPrefabs[_rng.Next( DeskPrefabs.Count )];
				var desk = deskPrefab.Clone( pos, Rotation.FromYaw( row % 2 == 0 ? 0f : 180f ) );
				desk.NetworkSpawn();
				_spawnedProps.Add( desk );

				// Chair behind desk
				if ( ChairPrefabs.Count > 0 )
				{
					var chairOffset = row % 2 == 0 ? new Vector3( 0, -40, 0 ) : new Vector3( 0, 40, 0 );
					var chairPrefab = ChairPrefabs[_rng.Next( ChairPrefabs.Count )];
					var chair = chairPrefab.Clone( pos + chairOffset, Rotation.FromYaw( _rng.Next( 360 ) ) );
					chair.NetworkSpawn();
					_spawnedProps.Add( chair );
				}

				// Monitor on desk (50% chance)
				if ( MonitorPrefabs.Count > 0 && _rng.NextDouble() > 0.5 )
				{
					var monPrefab = MonitorPrefabs[_rng.Next( MonitorPrefabs.Count )];
					var mon = monPrefab.Clone( pos + new Vector3( 0, 0, 35 ), Rotation.Identity );
					mon.NetworkSpawn();
					_spawnedProps.Add( mon );
				}
			}
		}
	}

	private void SpawnUtilityStations()
	{
		// Whiteboard on the wall
		if ( WhiteboardPrefab is not null )
		{
			var wbPos = OfficeCenter + new Vector3( OfficeWidth * 0.45f, 0, 50 );
			var wb = WhiteboardPrefab.Clone( wbPos, Rotation.FromYaw( 90 ) );
			wb.NetworkSpawn();
			_spawnedProps.Add( wb );
		}

		// Coffee machine in the corner
		if ( CoffeeMachinePrefab is not null )
		{
			var cmPos = OfficeCenter + new Vector3( -OfficeWidth * 0.4f, OfficeDepth * 0.4f, 0 );
			var cm = CoffeeMachinePrefab.Clone( cmPos, Rotation.FromYaw( -45 ) );
			cm.NetworkSpawn();
			_spawnedProps.Add( cm );
		}

		// Server rack (always — this is a crypto office)
		if ( ServerRackPrefab is not null )
		{
			var srPos = OfficeCenter + new Vector3( -OfficeWidth * 0.4f, -OfficeDepth * 0.35f, 0 );
			var sr = ServerRackPrefab.Clone( srPos, Rotation.FromYaw( 180 ) );
			sr.NetworkSpawn();
			_spawnedProps.Add( sr );
		}

		// Trash can by the door
		if ( TrashCanPrefab is not null )
		{
			var tcPos = OfficeCenter + new Vector3( OfficeWidth * 0.45f, -OfficeDepth * 0.4f, 0 );
			var tc = TrashCanPrefab.Clone( tcPos, Rotation.Identity );
			tc.NetworkSpawn();
			_spawnedProps.Add( tc );
		}
	}

	private void SpawnDecorations()
	{
		if ( DecorationPrefabs.Count == 0 ) return;

		// Scatter random decorations around the office edges
		int decorCount = 6 + OfficeEra * 3; // More stuff as you get richer

		for ( int i = 0; i < decorCount; i++ )
		{
			var pos = OfficeCenter + new Vector3(
				((float)_rng.NextDouble() - 0.5f) * OfficeWidth * 0.9f,
				((float)_rng.NextDouble() - 0.5f) * OfficeDepth * 0.9f,
				0
			);

			var prefab = DecorationPrefabs[_rng.Next( DecorationPrefabs.Count )];
			var deco = prefab.Clone( pos, Rotation.FromYaw( _rng.Next( 360 ) ) );
			deco.NetworkSpawn();
			_spawnedProps.Add( deco );
		}
	}

	/// <summary>
	/// Physics chaos! Apply random impulse to all props. 
	/// Called during market crashes for comedy.
	/// </summary>
	public void TriggerPhysicsChaos( float intensity = 1f )
	{
		foreach ( var prop in _spawnedProps )
		{
			if ( !prop.IsValid() ) continue;
			var rb = prop.Components.Get<Rigidbody>();
			if ( rb is null ) continue;

			var force = new Vector3(
				((float)_rng.NextDouble() - 0.5f) * 500f * intensity,
				((float)_rng.NextDouble() - 0.5f) * 500f * intensity,
				(float)_rng.NextDouble() * 300f * intensity
			);
			rb.ApplyImpulse( force );
		}
	}
}
