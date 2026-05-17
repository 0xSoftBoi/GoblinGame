using Sandbox;

namespace GoblinChain;

/// <summary>
/// Client-side game settings. Values survive scene loads via static storage.
/// </summary>
public static class GameSettings
{
	private const string PrefSensitivity = "gc_sensitivity";
	private const string PrefVolume = "gc_volume";

	private static float _sensitivity = -1f;
	private static float _volume = -1f;

	public static float MouseSensitivity
	{
		get
		{
			if ( _sensitivity < 0 )
				_sensitivity = Cookie.GetFloat( PrefSensitivity, 1.0f );
			return _sensitivity;
		}
		set
		{
			_sensitivity = MathF.Max( 0.1f, MathF.Min( 5.0f, value ) );
			Cookie.Set( PrefSensitivity, _sensitivity );
		}
	}

	public static float MasterVolume
	{
		get
		{
			if ( _volume < 0 )
				_volume = Cookie.GetFloat( PrefVolume, 1.0f );
			return _volume;
		}
		set
		{
			_volume = MathF.Max( 0f, MathF.Min( 1.0f, value ) );
			Cookie.Set( PrefVolume, _volume );
			Sound.Volume = _volume;
		}
	}

	public static void Apply()
	{
		Sound.Volume = MasterVolume;
	}
}
