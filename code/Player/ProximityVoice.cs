using Sandbox;

namespace GoblinChain;

/// <summary>
/// Spatial proximity voice chat. Attach to the player prefab.
/// Players hear each other based on distance — close = loud, far = silent.
/// Push-to-talk on the "Voice" input action.
///
/// Tips:
/// - Voice component should be added to player prefab in editor
/// - Keep MaxRange around 800 (~20m) for trading floor vibe
/// - Don't add/remove Voice component at runtime (causes audio crackle bug)
/// </summary>
public sealed class ProximityVoice : Component
{
	// --- Config ---
	[Property] public float MaxRange { get; set; } = 800f;
	[Property] public float MinRange { get; set; } = 150f;
	[Property] public float HeadOffset { get; set; } = 64f;
	[Property] public bool PushToTalk { get; set; } = true;

	// --- State ---
	[Sync] public bool IsTalking { get; set; } = false;

	private Voice _voice;

	protected override void OnStart()
	{
		_voice = Components.Get<Voice>();

		if ( _voice is null )
		{
			Log.Warning( "ProximityVoice: No Voice component found. Add one to the prefab!" );
			return;
		}

		// Configure spatial audio
		_voice.Mode = Voice.VoiceMode.Spatial;
		_voice.MaxDistance = MaxRange;
		_voice.MinDistance = MinRange;
	}

	protected override void OnUpdate()
	{
		if ( _voice is null ) return;

		// Position voice at head height
		_voice.WorldPosition = GameObject.WorldPosition + Vector3.Up * HeadOffset;

		// Only the local (non-proxy) player controls transmission
		if ( IsProxy ) return;

		bool shouldTransmit = PushToTalk
			? Input.Down( "Voice" )
			: true;

		_voice.IsTransmitting = shouldTransmit;
		IsTalking = shouldTransmit && _voice.IsTransmitting;
	}
}
