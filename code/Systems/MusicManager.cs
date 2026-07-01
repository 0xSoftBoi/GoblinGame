using Sandbox;
using System.Collections.Generic;

namespace GoblinChain;

/// <summary>
/// Client-side phase music. Plays the generated chiptune loop for the
/// current game phase (see tools/generate_assets.py), re-triggering at each
/// track's known length since .sound assets don't loop by themselves.
/// Created at runtime by GoblinChainGame — purely local, nothing networked.
/// </summary>
public sealed class MusicManager : Component
{
	[Property] public bool MusicEnabled { get; set; } = true;

	// Track lengths come from the generator's output — keep in sync if you
	// re-roll the music with different BPM/bar counts.
	private static readonly Dictionary<GamePhase, (string path, float length)> Tracks = new()
	{
		{ GamePhase.Create,  ("sounds/music_create.sound", 20.87f) },
		{ GamePhase.Shill,   ("sounds/music_shill.sound", 16.27f) },
		{ GamePhase.Chaos,   ("sounds/music_chaos.sound", 13.24f) },
		{ GamePhase.Results, ("sounds/music_results.sound", 19.20f) },
	};

	private SoundHandle _handle;
	private GamePhase _playingPhase = GamePhase.WaitingForPlayers;
	private float _trackEndsAt;

	protected override void OnUpdate()
	{
		var state = GameStateManager.Instance;
		if ( state is null ) return;

		var phase = state.CurrentPhase;

		if ( !MusicEnabled || !Tracks.ContainsKey( phase ) )
		{
			StopMusic();
			return;
		}

		// Phase changed → switch track; same phase → re-trigger at loop point
		if ( phase != _playingPhase || Time.Now >= _trackEndsAt )
		{
			StartTrack( phase );
		}

		// Duck under the sirens when a raid is live
		if ( _handle is not null )
		{
			var sec = SECSystem.Instance;
			_handle.Volume = sec is not null && sec.RaidActive ? 0.4f : 1f;
		}
	}

	private void StartTrack( GamePhase phase )
	{
		StopMusic();

		var (path, length) = Tracks[phase];
		_handle = Sound.Play( path );
		_playingPhase = phase;
		_trackEndsAt = Time.Now + length;
	}

	private void StopMusic()
	{
		_handle?.Stop();
		_handle = null;
		_playingPhase = GamePhase.WaitingForPlayers;
	}

	protected override void OnDestroy()
	{
		StopMusic();
	}
}
