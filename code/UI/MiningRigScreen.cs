using Sandbox;
using Sandbox.UI;

namespace GoblinChain.UI;

/// <summary>
/// WorldPanel 3D UI floating above each mining rig.
/// Shows owner name, hash rate, durability bar, and operational status.
/// Billboards toward the camera. Attach to MiningRig prefab.
/// </summary>
public sealed class MiningRigScreen : Component
{
	[Property] public Vector3 Offset { get; set; } = new( 0, 0, 70f );
	[Property] public float PanelWidth { get; set; } = 300f;
	[Property] public float PanelHeight { get; set; } = 120f;
	[Property] public float MaxViewDistance { get; set; } = 600f;
	[Property] public float WorldScale { get; set; } = 0.4f;

	private WorldPanel _panel;
	private MiningRig _rig;

	// Panel elements
	private Panel _root;
	private Label _ownerLabel;
	private Label _statusLabel;
	private Label _hashLabel;
	private Panel _durabilityFill;

	protected override void OnStart()
	{
		_rig = Components.Get<MiningRig>();

		_panel = new WorldPanel();
		_panel.SceneWorld = Scene.SceneWorld;
		_panel.PanelBounds = new Rect( 0, 0, PanelWidth, PanelHeight );
		_panel.WorldScale = WorldScale;

		BuildUI();
		UpdateTransform();
	}

	private void BuildUI()
	{
		_root = _panel;
		_root.Style.BackgroundColor = new Color( 0f, 0f, 0f, 0.85f );
		_root.Style.BorderColor = new Color( 0.2f, 0.2f, 0.2f );
		_root.Style.Padding = 8;
		_root.Style.BorderRadius = 4;
		_root.Style.FontFamily = "Consolas";

		// Owner name
		_ownerLabel = _root.AddChild<Label>();
		_ownerLabel.Text = "MINING RIG";
		_ownerLabel.Style.FontSize = 11;
		_ownerLabel.Style.FontColor = new Color( 0.5f, 0.5f, 0.5f );
		_ownerLabel.Style.MarginBottom = 4;

		// Status line
		_statusLabel = _root.AddChild<Label>();
		_statusLabel.Text = "ONLINE";
		_statusLabel.Style.FontSize = 14;
		_statusLabel.Style.FontColor = new Color( 0f, 1f, 0.53f ); // #00ff88
		_statusLabel.Style.FontWeight = 700;

		// Hash rate
		_hashLabel = _root.AddChild<Label>();
		_hashLabel.Text = "0.0 H/s";
		_hashLabel.Style.FontSize = 12;
		_hashLabel.Style.FontColor = new Color( 0.7f, 0.7f, 0.7f );
		_hashLabel.Style.MarginBottom = 6;

		// Durability bar background
		var durBar = _root.AddChild<Panel>();
		durBar.Style.Height = 6;
		durBar.Style.BackgroundColor = new Color( 0.15f, 0.15f, 0.15f );
		durBar.Style.BorderRadius = 2;

		// Durability fill
		_durabilityFill = durBar.AddChild<Panel>();
		_durabilityFill.Style.Height = 6;
		_durabilityFill.Style.BackgroundColor = new Color( 0f, 1f, 0.53f );
		_durabilityFill.Style.BorderRadius = 2;
	}

	protected override void OnUpdate()
	{
		if ( _panel is null || _rig is null ) return;

		UpdateTransform();
		UpdateContent();
		UpdateVisibility();
	}

	private void UpdateTransform()
	{
		_panel.WorldPosition = GameObject.WorldPosition + Offset;

		// Billboard: face camera
		var cam = Scene.Camera;
		if ( cam is null ) return;

		var dir = (cam.WorldPosition - _panel.WorldPosition).Normal;
		_panel.WorldRotation = Rotation.LookAt( dir );
	}

	private void UpdateContent()
	{
		// Owner
		string owner = _rig.Network.Owner?.DisplayName ?? "Unclaimed";
		_ownerLabel.Text = $"RIG — {owner}";

		// Status
		if ( !_rig.IsOperational )
		{
			_statusLabel.Text = "DISABLED";
			_statusLabel.Style.FontColor = new Color( 1f, 0.27f, 0.27f ); // red
		}
		else if ( _rig.Durability < _rig.MaxDurability * 0.3f )
		{
			_statusLabel.Text = "CRITICAL";
			_statusLabel.Style.FontColor = new Color( 1f, 0.8f, 0f ); // yellow
		}
		else
		{
			_statusLabel.Text = "ONLINE";
			_statusLabel.Style.FontColor = new Color( 0f, 1f, 0.53f ); // green
		}

		// Hash rate
		_hashLabel.Text = $"{_rig.EffectiveHashRate:F1} H/s (T{_rig.Tier})";

		// Durability bar
		float pct = _rig.Durability / _rig.MaxDurability;
		_durabilityFill.Style.Width = Length.Percent( pct * 100f );

		// Color the bar based on health
		if ( pct > 0.5f )
			_durabilityFill.Style.BackgroundColor = new Color( 0f, 1f, 0.53f );
		else if ( pct > 0.25f )
			_durabilityFill.Style.BackgroundColor = new Color( 1f, 0.8f, 0f );
		else
			_durabilityFill.Style.BackgroundColor = new Color( 1f, 0.27f, 0.27f );
	}

	private void UpdateVisibility()
	{
		// Fade out at distance
		var cam = Scene.Camera;
		if ( cam is null ) return;

		float dist = Vector3.DistanceBetween( cam.WorldPosition, _panel.WorldPosition );
		bool visible = dist < MaxViewDistance;

		_root.Style.Opacity = visible
			? MathX.Lerp( 1f, 0f, (dist - MaxViewDistance * 0.7f) / (MaxViewDistance * 0.3f) )
			: 0f;
	}

	protected override void OnDestroy()
	{
		_panel?.Delete();
		_panel = null;
	}
}
