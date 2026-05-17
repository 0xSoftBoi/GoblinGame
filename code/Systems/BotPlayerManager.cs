using Sandbox;
using System;
using System.Collections.Generic;

namespace GoblinChain;

/// <summary>
/// Spawns AI-controlled bot players to fill empty slots so solo / small-lobby
/// sessions are still playable. Bots have CryptoWallet + BotAI but no character.
/// They show up in the scoreboard and participate economically.
/// </summary>
public sealed class BotPlayerManager : Component
{
	public static BotPlayerManager Instance { get; private set; }

	[Property] public int MinPlayers { get; set; } = 4;
	[Property] public int MaxBots { get; set; } = 3;

	private readonly List<GameObject> _bots = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	/// <summary>
	/// Called by GameStateManager.ResetForNewMatch. Destroys old bots and
	/// spawns fresh ones to fill up to MinPlayers.
	/// Host-only.
	/// </summary>
	public void RespawnBots()
	{
		if ( IsProxy ) return;

		// Destroy previous bots
		foreach ( var b in _bots )
			if ( b.IsValid() ) b.Destroy();
		_bots.Clear();

		int humanCount = GoblinPlayer.All.Count;
		int needed = Math.Clamp( MinPlayers - humanCount, 0, MaxBots );

		for ( int i = 0; i < needed; i++ )
			SpawnBot();

		if ( _bots.Count > 0 )
			Log.Info( $"[BotPlayerManager] Spawned {_bots.Count} bots (humans: {humanCount})" );
	}

	private void SpawnBot()
	{
		var go = Scene.CreateObject();
		go.Name = $"Bot_{_bots.Count + 1}";
		go.Tags.Add( "bot" );

		var wallet = go.Components.Create<CryptoWallet>();
		wallet.IsBot = true;
		wallet.BotName = NPCNames.Generate();
		wallet.GoblinCoin = 100f;

		go.Components.Create<BotAI>();

		go.NetworkSpawn();
		_bots.Add( go );

		Log.Info( $"[BotPlayerManager] Bot spawned: {wallet.BotName}" );
	}
}
