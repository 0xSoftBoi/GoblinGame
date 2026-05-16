using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace GoblinChain;

/// <summary>
/// Large market board WorldPanel mounted at the center of the office.
/// Shows live GBC price and all active player tokens. No prefab needed —
/// OfficeSetup creates the backing box geometry and attaches this component.
/// </summary>
public sealed class MarketBoardDisplay : Component
{
	[Property] public float UpdateInterval { get; set; } = 0.5f;
	[Property] public Vector3 PanelOffset { get; set; } = new( 0, -6, 0 );

	private WorldPanel _panel;
	private float _timer;

	// Panel refs
	private Label _gbcPriceLabel;
	private Label _gbcChangeLabel;
	private Label _headlineLabel;
	private Panel _tokenList;

	private static readonly Color GreenColor = new( 0f, 1f, 0.53f );
	private static readonly Color RedColor = new( 1f, 0.27f, 0.27f );
	private static readonly Color DimColor = new( 0.55f, 0.55f, 0.55f );

	protected override void OnStart()
	{
		_panel = new WorldPanel();
		_panel.SceneWorld = Scene.SceneWorld;
		_panel.PanelBounds = new Rect( 0, 0, 1600, 560 );
		_panel.WorldScale = 0.5f;

		BuildPanel();
	}

	private void BuildPanel()
	{
		_panel.Style.BackgroundColor = new Color( 0.04f, 0.04f, 0.04f, 0.96f );
		_panel.Style.Padding = 20;
		_panel.Style.FontFamily = "Consolas";

		// Header bar
		var header = _panel.AddChild<Panel>();
		header.Style.FlexDirection = FlexDirection.Row;
		header.Style.MarginBottom = 12;
		header.Style.AlignItems = Align.Center;

		var title = header.AddChild<Label>();
		title.Text = "GOBLIN MARKET";
		title.Style.FontSize = 28;
		title.Style.FontColor = GreenColor;
		title.Style.FontWeight = 700;
		title.Style.FlexGrow = 1;

		_headlineLabel = header.AddChild<Label>();
		_headlineLabel.Text = "Markets stable. For now.";
		_headlineLabel.Style.FontSize = 13;
		_headlineLabel.Style.FontColor = DimColor;
		_headlineLabel.Style.TextAlign = TextAlign.Right;

		// Divider
		var div = _panel.AddChild<Panel>();
		div.Style.Height = 2;
		div.Style.BackgroundColor = new Color( 0.15f, 0.15f, 0.15f );
		div.Style.MarginBottom = 14;

		// GBC row
		var gbcRow = _panel.AddChild<Panel>();
		gbcRow.Style.FlexDirection = FlexDirection.Row;
		gbcRow.Style.AlignItems = Align.Center;
		gbcRow.Style.MarginBottom = 16;
		gbcRow.Style.PaddingLeft = 4;
		gbcRow.Style.PaddingRight = 4;

		var gbcTicker = gbcRow.AddChild<Label>();
		gbcTicker.Text = "GBC";
		gbcTicker.Style.FontSize = 20;
		gbcTicker.Style.FontColor = Color.White;
		gbcTicker.Style.FontWeight = 700;
		gbcTicker.Style.Width = 120;

		var gbcName = gbcRow.AddChild<Label>();
		gbcName.Text = "GoblinCoin";
		gbcName.Style.FontSize = 14;
		gbcName.Style.FontColor = DimColor;
		gbcName.Style.FlexGrow = 1;

		_gbcPriceLabel = gbcRow.AddChild<Label>();
		_gbcPriceLabel.Text = "1.000";
		_gbcPriceLabel.Style.FontSize = 22;
		_gbcPriceLabel.Style.FontColor = Color.White;
		_gbcPriceLabel.Style.Width = 160;
		_gbcPriceLabel.Style.TextAlign = TextAlign.Right;

		_gbcChangeLabel = gbcRow.AddChild<Label>();
		_gbcChangeLabel.Text = "+0.00%";
		_gbcChangeLabel.Style.FontSize = 16;
		_gbcChangeLabel.Style.FontColor = GreenColor;
		_gbcChangeLabel.Style.Width = 110;
		_gbcChangeLabel.Style.TextAlign = TextAlign.Right;

		// Divider
		var div2 = _panel.AddChild<Panel>();
		div2.Style.Height = 1;
		div2.Style.BackgroundColor = new Color( 0.12f, 0.12f, 0.12f );
		div2.Style.MarginBottom = 10;

		// Token list container
		_tokenList = _panel.AddChild<Panel>();
		_tokenList.Style.FlexDirection = FlexDirection.Column;
		_tokenList.Style.FlexGrow = 1;
	}

	protected override void OnUpdate()
	{
		if ( _panel is null ) return;

		UpdateTransform();

		_timer += Time.Delta;
		if ( _timer < UpdateInterval ) return;
		_timer = 0f;

		RefreshData();
	}

	private void UpdateTransform()
	{
		_panel.WorldPosition = GameObject.WorldPosition + PanelOffset;
		_panel.WorldRotation = GameObject.WorldRotation;
	}

	private void RefreshData()
	{
		var market = CryptoMarket.Instance;
		if ( market is null ) return;

		var data = market.GetDisplayData();

		// GBC row
		if ( data.Count > 0 )
		{
			var gbc = data[0];
			_gbcPriceLabel.Text = gbc.Price.ToString( "F3" );

			bool up = gbc.Change >= 0f;
			_gbcChangeLabel.Text = $"{(up ? "+" : "")}{gbc.Change:F2}%";
			_gbcChangeLabel.Style.FontColor = up ? GreenColor : RedColor;

			if ( market.IsCrashing )
			{
				_gbcPriceLabel.Style.FontColor = RedColor;
			}
			else if ( market.IsMooning )
			{
				_gbcPriceLabel.Style.FontColor = GreenColor;
			}
			else
			{
				_gbcPriceLabel.Style.FontColor = Color.White;
			}
		}

		_headlineLabel.Text = market.MarketHeadline;

		// Token rows — rebuild each tick (token list changes)
		_tokenList.DeleteChildren( true );

		for ( int i = 1; i < data.Count; i++ )
		{
			var token = data[i];
			AddTokenRow( token );
		}

		// Placeholder if no player tokens yet
		if ( data.Count <= 1 )
		{
			var empty = _tokenList.AddChild<Label>();
			empty.Text = "— no player tokens created yet —";
			empty.Style.FontSize = 13;
			empty.Style.FontColor = DimColor;
			empty.Style.AlignSelf = Align.Center;
			empty.Style.MarginTop = 10;
		}
	}

	private void AddTokenRow( CryptoMarket.CoinDisplayData token )
	{
		var row = _tokenList.AddChild<Panel>();
		row.Style.FlexDirection = FlexDirection.Row;
		row.Style.AlignItems = Align.Center;
		row.Style.PaddingLeft = 4;
		row.Style.PaddingRight = 4;
		row.Style.PaddingTop = 5;
		row.Style.PaddingBottom = 5;

		var ticker = row.AddChild<Label>();
		ticker.Text = token.Symbol;
		ticker.Style.FontSize = 16;
		ticker.Style.FontColor = Color.White;
		ticker.Style.FontWeight = 700;
		ticker.Style.Width = 120;

		var status = row.AddChild<Label>();
		status.Text = token.Tagline;
		status.Style.FontSize = 12;
		status.Style.FontColor = token.Tagline.Contains( "PUMP" ) ? GreenColor : RedColor;
		status.Style.FlexGrow = 1;

		var price = row.AddChild<Label>();
		price.Text = token.Price.ToString( "F3" );
		price.Style.FontSize = 16;
		price.Style.Width = 160;
		price.Style.TextAlign = TextAlign.Right;
		price.Style.FontColor = token.Price >= 1f ? GreenColor : RedColor;
	}

	protected override void OnDestroy()
	{
		_panel?.Delete();
		_panel = null;
	}
}
