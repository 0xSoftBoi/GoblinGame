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
		SpawnMarketBoard();
		SpawnPosters();
		SpawnEraLighting();

		// Furniture: editor-assigned prefabs win; otherwise free cloud assets;
		// otherwise dev-box placeholders. Board + posters spawn above so they
		// don't count toward the furniture threshold.
		int baseline = _spawnedProps.Count;
		SpawnDeskLayout();
		SpawnUtilityStations();
		SpawnDecorations();
		if ( _spawnedProps.Count - baseline < 5 )
			SpawnCloudOffice();
		if ( _spawnedProps.Count - baseline < 5 )
			SpawnCodeBasedProps();

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
					EnsurePhysicsProp( chair, 25f );
					chair.NetworkSpawn();
					_spawnedProps.Add( chair );
				}

				// Monitor on desk (50% chance)
				if ( MonitorPrefabs.Count > 0 && _rng.NextDouble() > 0.5 )
				{
					var monPrefab = MonitorPrefabs[_rng.Next( MonitorPrefabs.Count )];
					var mon = monPrefab.Clone( pos + new Vector3( 0, 0, 35 ), Rotation.Identity );
					EnsurePhysicsProp( mon, 8f );
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
			EnsurePhysicsProp( cm, 35f );
			cm.NetworkSpawn();
			_spawnedProps.Add( cm );
		}

		// Server rack (always — this is a crypto office)
		if ( ServerRackPrefab is not null )
		{
			var srPos = OfficeCenter + new Vector3( -OfficeWidth * 0.4f, -OfficeDepth * 0.35f, 0 );
			var sr = ServerRackPrefab.Clone( srPos, Rotation.FromYaw( 180 ) );
			EnsurePhysicsProp( sr, 120f );
			sr.NetworkSpawn();
			_spawnedProps.Add( sr );
		}

		// Trash can by the door
		if ( TrashCanPrefab is not null )
		{
			var tcPos = OfficeCenter + new Vector3( OfficeWidth * 0.45f, -OfficeDepth * 0.4f, 0 );
			var tc = TrashCanPrefab.Clone( tcPos, Rotation.Identity );
			EnsurePhysicsProp( tc, 5f );
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

	private void SpawnMarketBoard()
	{
		// Large flat board at the north wall center, facing into the room
		var pos = OfficeCenter + new Vector3( 0, OfficeDepth * 0.42f, 150f );
		var rot = Rotation.FromYaw( 180f ); // face south into the room

		var go = Scene.CreateObject();
		go.Name = "MarketBoard";
		go.WorldPosition = pos;
		go.WorldRotation = rot;
		go.Tags.Add( "prop" );

		// Backing panel — a thin flat box
		var renderer = go.Components.Create<ModelRenderer>();
		renderer.Model = Model.Load( "models/dev/box.vmdl" );
		renderer.Tint = new Color( 0.08f, 0.08f, 0.08f );
		go.LocalScale = new Vector3( 420f, 10f, 200f );

		go.Components.Create<MarketBoardDisplay>();
		go.NetworkSpawn();
		_spawnedProps.Add( go );
	}

	private void SpawnPosters()
	{
		// Wall positions: east and west walls, spread along Y axis
		(Vector3 pos, float yaw)[] placements =
		{
			// West wall
			( OfficeCenter + new Vector3( -OfficeWidth * 0.44f, -200f, 160f ),  90f ),
			( OfficeCenter + new Vector3( -OfficeWidth * 0.44f,    0f, 160f ),  90f ),
			( OfficeCenter + new Vector3( -OfficeWidth * 0.44f,  200f, 160f ),  90f ),
			// East wall
			( OfficeCenter + new Vector3(  OfficeWidth * 0.44f, -200f, 160f ), -90f ),
			( OfficeCenter + new Vector3(  OfficeWidth * 0.44f,  100f, 160f ), -90f ),
			// South wall
			( OfficeCenter + new Vector3( -150f, -OfficeDepth * 0.44f, 160f ),   0f ),
			( OfficeCenter + new Vector3(  150f, -OfficeDepth * 0.44f, 160f ),   0f ),
			( OfficeCenter + new Vector3(  400f, -OfficeDepth * 0.44f, 160f ),   0f ),
		};

		for ( int i = 0; i < placements.Length; i++ )
		{
			var (pos, yaw) = placements[i];
			var go = Scene.CreateObject();
			go.Name = $"OfficePoster_{i}";
			go.WorldPosition = pos;
			go.WorldRotation = Rotation.FromYaw( yaw );
			go.Tags.Add( "prop" );

			// Thin backing board
			var renderer = go.Components.Create<ModelRenderer>();
			renderer.Model = Model.Load( "models/dev/box.vmdl" );
			renderer.Tint = new Color( 0.92f, 0.88f, 0.80f ); // cream
			go.LocalScale = new Vector3( 80f, 4f, 60f );

			var poster = go.Components.Create<OfficePosterDisplay>();
			poster.PosterIndex = i;

			go.NetworkSpawn();
			_spawnedProps.Add( go );
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

	// ── Helpers ────────────────────────────────────────────────────────

	private void EnsurePhysicsProp( GameObject go, float mass = 20f )
	{
		if ( go.Components.Get<PhysicsProp>( FindMode.EverythingInSelf ) is not null ) return;

		if ( go.Components.Get<Rigidbody>( FindMode.EverythingInSelf ) is null )
		{
			var rb = go.Components.Create<Rigidbody>();
			rb.MassOverride = mass;
		}

		if ( go.Components.Get<BoxCollider>( FindMode.EverythingInSelf ) is null )
			go.Components.Create<BoxCollider>();

		go.Components.Create<PhysicsProp>().ThrowForce = mass > 80f ? 400f : 700f;
	}

	// ── Cloud assets (free Facepunch props from sbox.game) ────────────────
	// All idents verified against sbox.game. Downloaded/cached by the engine
	// at first load; if anything fails (offline), we fall back to dev boxes.

	private const string CloudDesk = "facepunch.office_desk";
	private const string CloudChair = "facepunch.office_chair";
	private const string CloudMonitor = "facepunch.tv"; // chart terminal on every desk

	// Per-era clutter — the office tells the story of the money
	private static readonly (string ident, float mass)[][] EraClutter =
	{
		// Era 0 — Garage Startup: shipping debris and cold pizza
		new[] { ("facepunch.cardboard_box", 6f), ("facepunch.wooden_crate", 30f),
				("facepunch.pallet", 25f), ("facepunch.pizza_box", 2f),
				("facepunch.traffic_cone", 4f) },
		// Era 1 — Funded WeWork: soft furniture and a kitchen nobody cleans
		new[] { ("facepunch.couch", 60f), ("facepunch.microwave", 15f),
				("facepunch.pizza_box", 2f), ("facepunch.watermelon", 8f),
				("facepunch.cardboard_box", 6f) },
		// Era 2 — Crypto Exchange: private ATMs and wall-to-wall screens
		new[] { ("facepunch.atm", 150f), ("facepunch.tv", 12f),
				("facepunch.couch", 60f), ("facepunch.traffic_cone", 4f),
				("facepunch.watermelon", 8f) },
		// Era 3 — Penthouse: more ATMs. The ATMs are the decor now.
		new[] { ("facepunch.atm", 150f), ("facepunch.fridge", 90f),
				("facepunch.couch", 60f), ("facepunch.money_stack", 10f),
				("facepunch.money_stack", 10f), ("facepunch.watermelon", 8f) },
	};

	// Accent lighting per era: garage bulb → office white → exchange neon → penthouse gold
	private static readonly Color[] EraLightColors =
	{
		new Color( 1.0f, 0.72f, 0.45f ),
		new Color( 0.92f, 0.96f, 1.0f ),
		new Color( 0.45f, 0.72f, 1.0f ),
		new Color( 1.0f, 0.84f, 0.42f ),
	};

	/// <summary>
	/// Four corner accent lights tinted by era. No rigidbody, so physics
	/// chaos leaves them alone.
	/// </summary>
	private void SpawnEraLighting()
	{
		var tint = EraLightColors[Math.Clamp( OfficeEra, 0, EraLightColors.Length - 1 )];

		Vector3[] spots =
		{
			OfficeCenter + new Vector3( -OfficeWidth * 0.3f, -OfficeDepth * 0.3f, 220f ),
			OfficeCenter + new Vector3(  OfficeWidth * 0.3f, -OfficeDepth * 0.3f, 220f ),
			OfficeCenter + new Vector3( -OfficeWidth * 0.3f,  OfficeDepth * 0.3f, 220f ),
			OfficeCenter + new Vector3(  OfficeWidth * 0.3f,  OfficeDepth * 0.3f, 220f ),
		};

		foreach ( var pos in spots )
		{
			var go = Scene.CreateObject();
			go.Name = "EraLight";
			go.WorldPosition = pos;

			var light = go.Components.Create<PointLight>();
			light.LightColor = tint * (0.6f + OfficeEra * 0.25f);
			light.Radius = 450f;

			go.NetworkSpawn();
			_spawnedProps.Add( go );
		}
	}

	/// <summary>
	/// Builds the office from free cloud assets when no prefabs are assigned.
	/// </summary>
	private void SpawnCloudOffice()
	{
		int before = _spawnedProps.Count;

		// Desk grid with chairs and chart terminals
		var startX = OfficeCenter.x - ((DesksPerRow - 1) * DeskSpacing) / 2f;
		var startY = OfficeCenter.y - ((DeskRows - 1) * RowSpacing) / 2f;

		for ( int row = 0; row < DeskRows; row++ )
		{
			for ( int col = 0; col < DesksPerRow; col++ )
			{
				var pos = new Vector3(
					startX + col * DeskSpacing,
					startY + row * RowSpacing,
					OfficeCenter.z );
				float deskYaw = row % 2 == 0 ? 0f : 180f;

				var desk = SpawnCloudProp( CloudDesk, pos, Rotation.FromYaw( deskYaw ), 80f );
				if ( desk is null ) return; // cloud unavailable — bail to dev-box fallback

				var chairOffset = row % 2 == 0 ? new Vector3( 0, -50, 0 ) : new Vector3( 0, 50, 0 );
				SpawnCloudProp( CloudChair, pos + chairOffset,
					Rotation.FromYaw( deskYaw + _rng.Next( -30, 30 ) ), 25f );

				if ( _rng.NextDouble() > 0.4 )
					SpawnCloudProp( CloudMonitor, pos + new Vector3( 0, 0, 34 ),
						Rotation.FromYaw( deskYaw + 180f ), 12f );
			}
		}

		// Era clutter around the edges — richer eras, weirder stuff
		var clutter = EraClutter[Math.Clamp( OfficeEra, 0, EraClutter.Length - 1 )];
		int clutterCount = 6 + OfficeEra * 3;
		for ( int i = 0; i < clutterCount; i++ )
		{
			var (ident, mass) = clutter[_rng.Next( clutter.Length )];
			var pos = OfficeCenter + new Vector3(
				((float)_rng.NextDouble() - 0.5f) * OfficeWidth * 0.9f,
				((float)_rng.NextDouble() - 0.5f) * OfficeDepth * 0.9f,
				0 );
			SpawnCloudProp( ident, pos, Rotation.FromYaw( _rng.Next( 360 ) ), mass );
		}

		Log.Info( $"[OfficeSetup] Spawned {_spawnedProps.Count - before} cloud-asset props ({EraNames[OfficeEra]})" );
	}

	/// <summary>
	/// Spawn one physics prop from a cloud model ident. Returns null if the
	/// package can't be loaded (offline / bad ident) so callers can fall back.
	/// </summary>
	private GameObject SpawnCloudProp( string ident, Vector3 pos, Rotation rot, float mass )
	{
		Model model;
		try
		{
			model = Cloud.Model( ident );
		}
		catch ( Exception e )
		{
			Log.Warning( $"[OfficeSetup] Cloud model '{ident}' failed to load: {e.Message}" );
			return null;
		}
		if ( model is null || model.IsError ) return null;

		var go = Scene.CreateObject();
		go.Name = ident;
		go.WorldPosition = pos;
		go.WorldRotation = rot;
		go.Tags.Add( "prop" );

		var renderer = go.Components.Create<ModelRenderer>();
		renderer.Model = model;

		var collider = go.Components.Create<ModelCollider>();
		collider.Model = model;

		var rb = go.Components.Create<Rigidbody>();
		rb.MassOverride = mass;

		var pp = go.Components.Create<PhysicsProp>();
		pp.ThrowForce = mass > 80f ? 400f : 700f;

		go.NetworkSpawn();
		_spawnedProps.Add( go );
		return go;
	}

	/// <summary>
	/// Spawns simple placeholder props when no prefabs are assigned.
	/// Ensures the office always has throwable objects for comedy.
	/// </summary>
	private void SpawnCodeBasedProps()
	{
		// Prop definitions: (name, size, position offset, mass, color)
		(string name, Vector3 size, Vector3 offset, float mass, Color tint)[] props =
		{
			("Chair_A",       new Vector3(40,40,50),  new Vector3(-100,  80, 0),  20f, new Color(0.4f, 0.3f, 0.2f)),
			("Chair_B",       new Vector3(40,40,50),  new Vector3( 100,  80, 0),  20f, new Color(0.4f, 0.3f, 0.2f)),
			("Chair_C",       new Vector3(40,40,50),  new Vector3(-100, -80, 0),  20f, new Color(0.35f,0.25f,0.15f)),
			("Chair_D",       new Vector3(40,40,50),  new Vector3( 100, -80, 0),  20f, new Color(0.35f,0.25f,0.15f)),
			("Monitor_A",     new Vector3(50, 8,35),  new Vector3(-150,   0,35),   8f, new Color(0.1f, 0.1f, 0.1f)),
			("Monitor_B",     new Vector3(50, 8,35),  new Vector3( 150,   0,35),   8f, new Color(0.1f, 0.1f, 0.1f)),
			("CoffeeMachine", new Vector3(30,25,45),  new Vector3(-200, 180, 0),  35f, new Color(0.2f, 0.2f, 0.2f)),
			("ServerRack",    new Vector3(40,30,100), new Vector3(-200,-180, 0), 120f, new Color(0.3f, 0.3f, 0.35f)),
			("TrashCan",      new Vector3(25,25,40),  new Vector3( 220,-180, 0),   5f, new Color(0.4f, 0.5f, 0.4f)),
			("TrashCan_B",    new Vector3(25,25,40),  new Vector3( 220, 180, 0),   5f, new Color(0.4f, 0.5f, 0.4f)),
			("Keyboard_A",    new Vector3(45,15,5),   new Vector3(-120,  50,36),   3f, new Color(0.15f,0.15f,0.15f)),
			("Keyboard_B",    new Vector3(45,15,5),   new Vector3( 120, -50,36),   3f, new Color(0.15f,0.15f,0.15f)),
		};

		foreach ( var (name, size, offset, mass, tint) in props )
		{
			var go = Scene.CreateObject();
			go.Name = name;
			go.WorldPosition = OfficeCenter + offset;
			go.Tags.Add( "prop" );

			var renderer = go.Components.Create<ModelRenderer>();
			renderer.Model = Model.Load( "models/dev/box.vmdl" );
			renderer.Tint = tint;
			go.LocalScale = size;

			var collider = go.Components.Create<BoxCollider>();
			collider.Scale = Vector3.One;

			var rb = go.Components.Create<Rigidbody>();
			rb.MassOverride = mass;

			var pp = go.Components.Create<PhysicsProp>();
			pp.ThrowForce = mass > 80f ? 400f : 700f;

			go.NetworkSpawn();
			_spawnedProps.Add( go );
		}

		Log.Info( $"[OfficeSetup] Spawned {props.Length} code-based props (no prefabs assigned)" );
	}
}
