using Sandbox;
using System;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// SEC heat system. Players accumulate heat from risky actions.
/// At 70 heat: warning. At 100: raid with consequences.
/// Simplified for launch — button-based escape instead of physics minigame.
/// </summary>
public sealed class SECSystem : Component
{
	public static SECSystem Instance { get; private set; }

	// --- Config ---
	[Property] public float HeatDecayPerSecond { get; set; } = 0.033f; // ~2 per minute
	[Property] public float WarningThreshold { get; set; } = 70f;
	[Property] public float RaidThreshold { get; set; } = 100f;
	[Property] public float RaidFinePercent { get; set; } = 0.5f;
	[Property] public float BribeCost { get; set; } = 2000f;
	[Property] public float BribeSuccessChance { get; set; } = 0.7f;
	[Property] public float PostRaidHeatReset { get; set; } = 40f;
	[Property] public float WarningCooldown { get; set; } = 30f;

	// --- Raid State (synced) ---
	[Sync] public bool RaidActive { get; set; } = false;
	[Sync] public Guid RaidTargetId { get; set; } = Guid.Empty;
	[Sync] public string RaidTargetName { get; set; } = "";
	[Sync] public float RaidTimer { get; set; } = 0f;
	[Sync] public bool RaidResolved { get; set; } = false;

	private Random _rng = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		// Decay heat for all players
		foreach ( var player in GoblinPlayer.All )
		{
			var heat = player.Components.Get<SECHeatComponent>();
			if ( heat is null ) continue;

			if ( heat.Heat > 0 )
			{
				heat.Heat = MathF.Max( 0, heat.Heat - HeatDecayPerSecond * Time.Delta );
			}

			heat.WarnCooldown -= Time.Delta;

			// Warning at 70
			if ( heat.Heat >= WarningThreshold && heat.WarnCooldown <= 0 )
			{
				heat.WarnCooldown = WarningCooldown;
				SendWarning( player.Network.Owner );
			}

			// Raid at 100
			if ( heat.Heat >= RaidThreshold && !RaidActive )
			{
				TriggerRaid( player );
			}
		}

		// Raid timer
		if ( RaidActive && !RaidResolved )
		{
			RaidTimer -= Time.Delta;
			if ( RaidTimer <= 0f )
			{
				// Time's up — auto accept fate
				ResolveRaid( RaidTargetId, RaidAction.AcceptFate, Guid.Empty );
			}
		}
	}

	// ═══════════════════════════════════════
	//  HEAT MANAGEMENT
	// ═══════════════════════════════════════

	public void AddHeat( GoblinPlayer player, float amount )
	{
		if ( IsProxy || player is null ) return;

		var heat = player.Components.Get<SECHeatComponent>();
		if ( heat is null ) return;

		heat.Heat = MathF.Min( RaidThreshold + 10f, heat.Heat + amount );
	}

	public float GetHeat( GoblinPlayer player )
	{
		var heat = player?.Components.Get<SECHeatComponent>();
		return heat?.Heat ?? 0f;
	}

	// ═══════════════════════════════════════
	//  RAID
	// ═══════════════════════════════════════

	private void TriggerRaid( GoblinPlayer target )
	{
		RaidActive = true;
		RaidResolved = false;
		RaidTargetId = target.Id;
		RaidTargetName = target.Network.Owner?.DisplayName ?? "???";
		RaidTimer = 15f; // 15 seconds to decide

		var heat = target.Components.Get<SECHeatComponent>();
		if ( heat is not null ) heat.IsBeingRaided = true;

		BroadcastRaidWarning( RaidTargetName );
		Log.Info( $"SEC RAID on {RaidTargetName}!" );
	}

	[Rpc.Host]
	public void SubmitRaidResponse( int actionIdx, Guid blameTargetId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null || player.Id != RaidTargetId ) return;
		if ( RaidResolved ) return;

		var action = (RaidAction)actionIdx;
		ResolveRaid( player.Id, action, blameTargetId );
	}

	// Called by TwitchIntegration when chat vote overrides raid outcome
	public void ForceRaidAction( RaidAction action )
	{
		if ( IsProxy || !RaidActive || RaidResolved ) return;
		ResolveRaid( RaidTargetId, action, Guid.Empty );
	}

	private void ResolveRaid( Guid targetId, RaidAction action, Guid blameTargetId )
	{
		RaidResolved = true;

		var target = GoblinPlayer.All.FirstOrDefault( p => p.Id == targetId );
		if ( target is null )
		{
			EndRaid();
			return;
		}

		var wallet = target.Components.Get<CryptoWallet>();
		var heat = target.Components.Get<SECHeatComponent>();
		string targetName = target.Network.Owner?.DisplayName ?? "???";
		string result;

		switch ( action )
		{
			case RaidAction.ShredDocuments:
				// Half fine
				float shredFine = (wallet?.GoblinCoin ?? 0) * RaidFinePercent * 0.5f;
				if ( wallet is not null ) wallet.GoblinCoin -= shredFine;
				if ( heat is not null ) heat.Heat = PostRaidHeatReset;
				result = $"{targetName} shredded evidence! Fine reduced to {shredFine:N0} GBC";
				BroadcastRaidResult( result, false );
				break;

			case RaidAction.Bribe:
				if ( wallet is not null && wallet.GoblinCoin >= BribeCost )
				{
					wallet.GoblinCoin -= BribeCost;
					bool success = _rng.NextDouble() < BribeSuccessChance;
					if ( success )
					{
						if ( heat is not null ) heat.Heat = PostRaidHeatReset * 0.5f;
						result = $"{targetName} bribed the SEC! They conveniently lost the paperwork.";
						BroadcastRaidResult( result, true );
					}
					else
					{
						float bribeFine = (wallet.GoblinCoin) * RaidFinePercent;
						wallet.GoblinCoin -= bribeFine;
						if ( heat is not null ) heat.Heat = PostRaidHeatReset;
						result = $"{targetName}'s bribe was rejected! Double fine: {bribeFine:N0} GBC";
						BroadcastRaidResult( result, false );
					}
				}
				else
				{
					goto case RaidAction.AcceptFate;
				}
				break;

			case RaidAction.BlameAnother:
				var blamed = GoblinPlayer.All.FirstOrDefault( p => p.Id == blameTargetId );
				if ( blamed is not null )
				{
					var blamedHeat = blamed.Components.Get<SECHeatComponent>();
					if ( blamedHeat is not null && blamedHeat.Heat > 10f )
					{
						// Redirect! Target escapes, blamed gets raided
						if ( heat is not null ) heat.Heat = PostRaidHeatReset;
						string blamedName = blamed.Network.Owner?.DisplayName ?? "???";

						// Fine the blamed player
						var blamedWallet = blamed.Components.Get<CryptoWallet>();
						float blamedFine = (blamedWallet?.GoblinCoin ?? 0) * RaidFinePercent;
						if ( blamedWallet is not null ) blamedWallet.GoblinCoin -= blamedFine;
						if ( blamedHeat is not null ) blamedHeat.Heat = PostRaidHeatReset;

						result = $"{targetName} pointed at {blamedName}! SEC redirects! {blamedName} fined {blamedFine:N0} GBC!";
						BroadcastRaidResult( result, true );
						break;
					}
				}
				// Blame failed — fall through to accept fate
				goto case RaidAction.AcceptFate;

			case RaidAction.AcceptFate:
			default:
				float fullFine = (wallet?.GoblinCoin ?? 0) * RaidFinePercent;
				if ( wallet is not null ) wallet.GoblinCoin -= fullFine;
				if ( heat is not null )
				{
					heat.Heat = PostRaidHeatReset;
					heat.TimesRaided++;
				}

				// Freeze tokens for 1 round
				// (Simplified: just apply a heavy penalty)
				result = $"{targetName} caught by the SEC! Fined {fullFine:N0} GBC!";
				BroadcastRaidResult( result, false );
				break;
		}

		EndRaid();
	}

	private void EndRaid()
	{
		// Reset raid state after brief delay
		_ = EndRaidDelayed();
	}

	private async System.Threading.Tasks.Task EndRaidDelayed()
	{
		await GameTask.DelaySeconds( 5f );
		RaidActive = false;
		RaidResolved = false;
		RaidTargetId = Guid.Empty;

		foreach ( var player in GoblinPlayer.All )
			if ( player.Components.Get<SECHeatComponent>() is { } h ) h.IsBeingRaided = false;
	}

	// ═══════════════════════════════════════
	//  BROADCASTS
	// ═══════════════════════════════════════

	[Rpc.Owner]
	private void SendWarning( Connection target )
	{
		Sound.Play( "sounds/event_negative.sound" );
		Log.Warning( "SEC WARNING: Your activities have attracted regulatory attention!" );
	}

	[Rpc.Broadcast]
	private void BroadcastRaidWarning( string targetName )
	{
		Sound.Play( "sounds/market_crash.sound" );
		Log.Warning( $"SEC RAID: Agents are closing in on {targetName}!" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( "SEC RAID", $"Agents are raiding {targetName}!", "negative" );
	}

	[Rpc.Broadcast]
	private void BroadcastRaidResult( string message, bool escaped )
	{
		string sound = escaped ? "sounds/event_positive.sound" : "sounds/event_negative.sound";
		Sound.Play( sound );
		Log.Info( $"SEC RESULT: {message}" );

		var feed = Scene.GetAllComponents<UI.NotificationFeed>().FirstOrDefault();
		feed?.PushNotification( escaped ? "ESCAPED" : "BUSTED", message, escaped ? "positive" : "negative" );

		ClipRecorder.Instance?.OnRaidResolved( message, escaped );
	}

	private GoblinPlayer FindPlayer( Connection conn )
		=> GoblinPlayer.All.FirstOrDefault( p => p.Network.Owner == conn );
}

public enum RaidAction
{
	ShredDocuments = 0,
	Bribe = 1,
	BlameAnother = 2,
	AcceptFate = 3
}

/// <summary>
/// Attach to player prefab alongside CryptoWallet.
/// Tracks per-player SEC heat.
/// </summary>
public sealed class SECHeatComponent : Component
{
	[Sync] public float Heat { get; set; } = 0f;
	[Sync] public bool IsBeingRaided { get; set; } = false;
	[Sync] public int TimesRaided { get; set; } = 0;

	// Local state
	public float WarnCooldown { get; set; } = 0f;
}
