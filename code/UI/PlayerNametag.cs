using Sandbox;
using Sandbox.UI;
using System.Linq;

namespace GoblinChain.UI;

/// <summary>
/// WorldPanel nametag floating above other players' heads.
/// Shows name, GBC balance, and a voice indicator when talking.
/// Only visible for remote players (not yourself).
/// </summary>
public sealed class PlayerNametag : Component
{
	[Property] public Vector3 Offset { get; set; } = new( 0, 0, 80f );
	[Property] public float MaxDistance { get; set; } = 1200f;

	private WorldPanel _panel;
	private Label _nameLabel;
	private Label _balanceLabel;
	private Panel _voiceIndicator;
	private GoblinPlayer _player;
	private CryptoWallet _wallet;
	private ProximityVoice _voice;

	protected override void OnStart()
	{
		_player = Components.Get<GoblinPlayer>();
		_wallet = Components.Get<CryptoWallet>();
		_voice = Components.Get<ProximityVoice>();

		// Only create nametag for other players
		if ( _player is not null && !_player.IsProxy )
		{
			Enabled = false;
			return;
		}

		_panel = new WorldPanel();
		_panel.SceneWorld = Scene.SceneWorld;
		_panel.PanelBounds = new Rect( 0, 0, 250f, 60f );
		_panel.WorldScale = 0.35f;

		BuildUI();
	}

	private void BuildUI()
	{
		var root = (Panel)_panel;
		root.Style.Display = DisplayMode.Flex;
		root.Style.FlexDirection = FlexDirection.Column;
		root.Style.AlignItems = Align.Center;
		root.Style.JustifyContent = Justify.Center;

		// Name
		_nameLabel = root.AddChild<Label>();
		_nameLabel.Text = "Player";
		_nameLabel.Style.FontSize = 14;
		_nameLabel.Style.FontColor = Color.White;
		_nameLabel.Style.FontWeight = 600;
		_nameLabel.Style.FontFamily = "Consolas";
		_nameLabel.Style.TextShadow = new TextShadow { OffsetX = 0, OffsetY = 1, BlurRadius = 4, Color = Color.Black };

		// Balance
		_balanceLabel = root.AddChild<Label>();
		_balanceLabel.Text = "0 GBC";
		_balanceLabel.Style.FontSize = 11;
		_balanceLabel.Style.FontColor = new Color( 1f, 0.8f, 0f ); // gold
		_balanceLabel.Style.FontFamily = "Consolas";

		// Voice indicator (green dot)
		_voiceIndicator = root.AddChild<Panel>();
		_voiceIndicator.Style.Width = 8;
		_voiceIndicator.Style.Height = 8;
		_voiceIndicator.Style.BorderRadius = 4;
		_voiceIndicator.Style.BackgroundColor = new Color( 0f, 1f, 0.53f );
		_voiceIndicator.Style.MarginTop = 4;
		_voiceIndicator.Style.Opacity = 0;
	}

	protected override void OnUpdate()
	{
		if ( _panel is null ) return;

		// Position
		_panel.WorldPosition = GameObject.WorldPosition + Offset;

		// Billboard
		var cam = Scene.Camera;
		if ( cam is null ) return;

		var dir = (cam.WorldPosition - _panel.WorldPosition).Normal;
		_panel.WorldRotation = Rotation.LookAt( dir );

		// Content
		string name = _player?.Network.Owner?.DisplayName ?? GameObject.Name;
		_nameLabel.Text = name;

		if ( _wallet is not null )
			_balanceLabel.Text = $"{_wallet.GoblinCoin:N0} GBC";

		// Voice indicator
		bool talking = _voice?.IsTalking ?? false;
		_voiceIndicator.Style.Opacity = talking ? 1f : 0f;

		// Distance fade
		float dist = Vector3.DistanceBetween( cam.WorldPosition, _panel.WorldPosition );
		if ( dist > MaxDistance )
		{
			_panel.Style.Opacity = 0;
		}
		else
		{
			float fade = dist > MaxDistance * 0.6f
				? MathX.Lerp( 1f, 0f, (dist - MaxDistance * 0.6f) / (MaxDistance * 0.4f) )
				: 1f;
			_panel.Style.Opacity = fade;
		}
	}

	protected override void OnDestroy()
	{
		_panel?.Delete();
		_panel = null;
	}
}
