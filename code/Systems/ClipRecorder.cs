using Sandbox;
using System;
using System.Collections.Generic;

namespace GoblinChain;

/// <summary>
/// Detects clip-worthy moments (rug pulls, SEC raids, Grand Rug, Audit reveals)
/// and shows a "CLIP SAVED" overlay on all clients. Keeps a 20-second ring buffer
/// of game events so streamers have context around the clip moment.
///
/// No in-engine video encoding — streamers run OBS Replay Buffer. This system
/// provides the visual cue to save it at the right moment.
/// </summary>
public sealed class ClipRecorder : Component
{
	public static ClipRecorder Instance { get; private set; }

	[Property] public float OverlayDuration { get; set; } = 4f;
	[Property] public int EventBufferSeconds { get; set; } = 20;
	[Property] public int MaxBufferedEvents { get; set; } = 60;

	// Overlay state (drives HUD)
	[Sync] public bool OverlayVisible { get; set; } = false;
	[Sync] public string ClipLabel { get; set; } = "";
	[Sync] public string ClipDescription { get; set; } = "";
	[Sync] public int ClipsSavedThisRound { get; set; } = 0;

	// Local overlay timer (each client runs independently so no sync needed)
	private float _overlayTimer;

	// Ring buffer of recent game events (host only, for context)
	private readonly Queue<ClipEvent> _buffer = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	protected override void OnUpdate()
	{
		// Tick down overlay on each client
		if ( _overlayTimer > 0f )
		{
			_overlayTimer -= Time.Delta;
			if ( _overlayTimer <= 0f )
				OverlayVisible = false;
		}
	}

	// ═══════════════════════════════════════
	//  EVENT BUFFER (host only)
	// ═══════════════════════════════════════

	public void RecordEvent( string description )
	{
		if ( IsProxy ) return;

		_buffer.Enqueue( new ClipEvent( Time.Now, description ) );

		// Trim events older than EventBufferSeconds
		float cutoff = Time.Now - EventBufferSeconds;
		while ( _buffer.Count > 0 && _buffer.Peek().Timestamp < cutoff )
			_buffer.Dequeue();

		// Hard cap
		while ( _buffer.Count > MaxBufferedEvents )
			_buffer.Dequeue();
	}

	// ═══════════════════════════════════════
	//  TRIGGER POINTS — called from broadcast methods (runs on all clients)
	// ═══════════════════════════════════════

	public void OnRugPull( string creator, string ticker, float amount )
	{
		TriggerClip(
			"RUG PULL",
			$"{creator} rugged ${ticker} — {amount:N0} GBC vanished"
		);
	}

	public void OnRaidResolved( string result, bool escaped )
	{
		TriggerClip(
			escaped ? "SEC ESCAPE" : "SEC BUSTED",
			result
		);
	}

	public void OnGrandRug( string rugger, float amount )
	{
		TriggerClip(
			"GRAND RUG",
			$"{rugger} executed the Grand Rug — {amount:N0} GBC stolen"
		);
	}

	public void OnAuditResult( bool correct, string suspectName, string message )
	{
		if ( correct )
			TriggerClip( "RUGGER EXPOSED", $"{suspectName} was the Rugger all along!" );
		else if ( message.Length > 0 )
			TriggerClip( "WRONG VOTE", $"Chat wrongly accused {suspectName}. Rugger walks free." );
	}

	// ═══════════════════════════════════════
	//  CORE
	// ═══════════════════════════════════════

	private void TriggerClip( string label, string description )
	{
		ClipLabel = label;
		ClipDescription = description;
		OverlayVisible = true;
		_overlayTimer = OverlayDuration;
		ClipsSavedThisRound++;

		Sound.Play( "sounds/clip_saved.sound" );
		Log.Info( $"[CLIP] {label}: {description}" );
	}

	public void ResetRound()
	{
		ClipsSavedThisRound = 0;
		_buffer.Clear();
	}
}

public readonly struct ClipEvent
{
	public readonly float Timestamp;
	public readonly string Description;

	public ClipEvent( float timestamp, string description )
	{
		Timestamp = timestamp;
		Description = description;
	}
}
