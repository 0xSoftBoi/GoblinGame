using Sandbox;
using System;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Drives a bot player through each game phase.
/// Runs host-only — bots mine coins, shill on Twitter, and occasionally rug.
/// </summary>
public sealed class BotAI : Component
{
	private CryptoWallet _wallet;
	private GamePhase _lastPhase = GamePhase.WaitingForPlayers;
	private float _actionTimer;
	private readonly Random _rng = new();

	// Each bot gets a personality on start
	private float _aggression;  // 0-1: affects rig count, post frequency
	private float _greed;       // 0-1: affects when it sells / rugs
	private string _twitterHandle;

	private static readonly float BaseHashPerRig = 5f;
	private static readonly float RigCost = 100f;

	protected override void OnStart()
	{
		_wallet = Components.Get<CryptoWallet>();
		_aggression = 0.3f + (float)_rng.NextDouble() * 0.7f;
		_greed      = 0.3f + (float)_rng.NextDouble() * 0.7f;
		_twitterHandle = NPCNames.GenerateHandle();
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy || _wallet is null ) return;

		var state = GameStateManager.Instance;
		if ( state is null ) return;

		var phase = state.CurrentPhase;

		if ( phase != _lastPhase )
		{
			_lastPhase = phase;
			OnPhaseEntered( phase );
		}

		// Periodic actions while a phase is running
		_actionTimer -= Time.Delta;
		if ( _actionTimer <= 0f )
		{
			_actionTimer = 8f + (float)_rng.NextDouble() * 12f;
			OnPeriodicAction( phase );
		}
	}

	private void OnPhaseEntered( GamePhase phase )
	{
		switch ( phase )
		{
			case GamePhase.Create:
				PlaceRigs();
				break;

			case GamePhase.Shill:
				PostTweet();
				break;

			case GamePhase.Chaos:
				// More aggressive posting in chaos
				PostTweet();
				if ( _rng.NextDouble() < _greed * 0.4f )
					PostChaosReaction();
				break;
		}
	}

	private void OnPeriodicAction( GamePhase phase )
	{
		if ( phase == GamePhase.Shill && _rng.NextDouble() < _aggression * 0.5f )
			PostTweet();

		if ( phase == GamePhase.Create && _rng.NextDouble() < _aggression * 0.3f )
			PlaceRigs(); // occasionally add more
	}

	// ─── Actions ────────────────────────────────────────────────────

	private void PlaceRigs()
	{
		int desired = 1 + (int)(_aggression * 3f); // 1-4 rigs
		int affordable = (int)(_wallet.GoblinCoin / RigCost);
		int toPlace = Math.Min( desired, affordable );
		toPlace = Math.Max( 0, toPlace );

		if ( toPlace == 0 ) return;

		_wallet.GoblinCoin -= toPlace * RigCost;
		_wallet.MiningRigs += toPlace;
		_wallet.HashRate   += toPlace * BaseHashPerRig;

		Log.Info( $"[Bot:{_wallet.BotName}] Placed {toPlace} rigs (total: {_wallet.MiningRigs})" );
	}

	private void PostTweet()
	{
		var twitter = Scene.GetAllComponents<GoblinTwitter>().FirstOrDefault();
		if ( twitter is null ) return;

		// Pick a token to shill (any active token)
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		string ticker = "";
		if ( tokenSystem is not null && tokenSystem.ActiveTokens.Count > 0 )
		{
			var tokens = tokenSystem.ActiveTokens.Values.Where( t => !t.IsRugged ).ToList();
			if ( tokens.Count > 0 )
				ticker = tokens[_rng.Next( tokens.Count )].Ticker;
		}

		string[] posts = {
			"just deployed my third rig. the goblins are eating good tonight",
			"not financial advice but I haven't slept in 72 hours and im UP",
			"the market is irrational and so am I",
			"wen moon? asking for a friend (me)",
			"my portfolio is a crime scene. I am both victim and perpetrator",
			"bro trust the process. the process is chaos",
			"every red candle is just a green candle that hasn't happened yet",
		};

		string body = posts[_rng.Next( posts.Length )];
		twitter.AddNPCPost( _twitterHandle, ticker, body );
	}

	private void PostChaosReaction()
	{
		var twitter = Scene.GetAllComponents<GoblinTwitter>().FirstOrDefault();
		if ( twitter is null ) return;

		string[] reactions = {
			"market is eating my face. this is fine.",
			"I CALLED IT. (I did not call it)",
			"SEC is a vibe honestly",
			"someone just EMPed my rigs im going to cry",
			"chaos? in THIS economy? more likely than you think",
		};

		twitter.AddNPCPost( _twitterHandle, "", reactions[_rng.Next( reactions.Length )] );
	}
}
