using Sandbox;
using Sandbox.UI;

namespace GoblinChain;

/// <summary>
/// Motivational poster WorldPanel mounted on office walls.
/// OfficeSetup creates the backing box and attaches this with a chosen poster index.
/// </summary>
public sealed class OfficePosterDisplay : Component
{
	[Property] public int PosterIndex { get; set; } = 0;
	[Property] public Vector3 PanelOffset { get; set; } = new( 0, -3, 0 );

	// All posters. Goblin hustle-culture parody.
	private static readonly (string Headline, string Body, string Footer)[] Posters =
	{
		(
			"DIAMOND HANDS",
			"OR YOU'RE A COWARD",
			"— Goblin Market Wisdom, Vol. 1"
		),
		(
			"MOVE FAST",
			"AND RUG THINGS",
			"— Our Core Values"
		),
		(
			"WE'RE GOING\nTO THE MOON",
			"(MATHEMATICALLY CERTAIN)",
			"— Chief Goblin Officer"
		),
		(
			"PAST PERFORMANCE",
			"GUARANTEES FUTURE RETURNS",
			"* not actual financial advice"
		),
		(
			"IN GOBLINS\nWE TRUST",
			"others have trust issues",
			"— Goblin Republic, Est. 2026"
		),
		(
			"WHAT DOESN'T\nKILL YOUR BAG",
			"makes it a smaller bag",
			"— Motivational Goblin"
		),
		(
			"HAVE YOU TRIED",
			"BUYING THE DIP?",
			"there's always another dip"
		),
		(
			"NGMI",
			"(but we believe in you)",
			"— HR Department"
		),
	};

	private WorldPanel _panel;

	protected override void OnStart()
	{
		_panel = new WorldPanel();
		_panel.SceneWorld = Scene.SceneWorld;
		_panel.PanelBounds = new Rect( 0, 0, 380, 280 );
		_panel.WorldScale = 0.38f;

		BuildPoster();
	}

	private void BuildPoster()
	{
		int idx = PosterIndex % Posters.Length;
		var (headline, body, footer) = Posters[idx];

		_panel.Style.BackgroundColor = new Color( 0.96f, 0.95f, 0.88f, 1f ); // off-white paper
		_panel.Style.Padding = 24;
		_panel.Style.FontFamily = "Consolas";
		_panel.Style.AlignItems = Align.Center;
		_panel.Style.JustifyContent = Justify.SpaceBetween;
		_panel.Style.FlexDirection = FlexDirection.Column;

		// Top rule
		var rule = _panel.AddChild<Panel>();
		rule.Style.Height = 4;
		rule.Style.Width = Length.Percent( 100 );
		rule.Style.BackgroundColor = new Color( 0.08f, 0.08f, 0.08f );
		rule.Style.MarginBottom = 16;

		// Headline
		var hl = _panel.AddChild<Label>();
		hl.Text = headline;
		hl.Style.FontSize = 26;
		hl.Style.FontColor = new Color( 0.06f, 0.06f, 0.06f );
		hl.Style.FontWeight = 700;
		hl.Style.TextAlign = TextAlign.Center;
		hl.Style.MarginBottom = 12;

		// Body
		var bd = _panel.AddChild<Label>();
		bd.Text = body;
		bd.Style.FontSize = 16;
		bd.Style.FontColor = new Color( 0.25f, 0.25f, 0.25f );
		bd.Style.TextAlign = TextAlign.Center;
		bd.Style.FlexGrow = 1;

		// Bottom rule
		var rule2 = _panel.AddChild<Panel>();
		rule2.Style.Height = 1;
		rule2.Style.Width = Length.Percent( 80 );
		rule2.Style.BackgroundColor = new Color( 0.35f, 0.35f, 0.35f );
		rule2.Style.MarginTop = 12;
		rule2.Style.MarginBottom = 8;

		// Footer
		var ft = _panel.AddChild<Label>();
		ft.Text = footer;
		ft.Style.FontSize = 11;
		ft.Style.FontColor = new Color( 0.45f, 0.45f, 0.45f );
		ft.Style.TextAlign = TextAlign.Center;
		ft.Style.FontStyle = "italic";
	}

	protected override void OnUpdate()
	{
		if ( _panel is null ) return;

		_panel.WorldPosition = GameObject.WorldPosition + PanelOffset;
		_panel.WorldRotation = GameObject.WorldRotation;
	}

	protected override void OnDestroy()
	{
		_panel?.Delete();
		_panel = null;
	}
}
