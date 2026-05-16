using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoblinChain;

/// <summary>
/// Throwable EMP grenade. Disables mining rigs in a radius and
/// applies radial physics impulse. Detonates on hard impact or fuse timer.
///
/// Usage: Clone the prefab, call Launch(), it handles the rest.
/// Prefab needs: Rigidbody, SphereCollider (small), ModelRenderer.
/// </summary>
public sealed class EMPGrenade : Component, Component.ICollisionListener
{
	// --- Config ---
	[Property] public float ThrowForce { get; set; } = 1200f;
	[Property] public float BlastRadius { get; set; } = 350f;
	[Property] public float BlastForce { get; set; } = 5000f;
	[Property] public float DisableDuration { get; set; } = 12f;
	[Property] public float FuseTime { get; set; } = 3f;
	[Property] public float ImpactSpeedThreshold { get; set; } = 80f;
	[Property] public float WalletDrainPercent { get; set; } = 0.05f;

	// --- State ---
	[Sync] public bool HasDetonated { get; set; } = false;
	[Sync] public string ThrowerName { get; set; } = "";

	private Rigidbody _rb;
	private float _aliveTime;

	protected override void OnStart()
	{
		_rb = Components.Get<Rigidbody>();
		Tags.Add( "projectile" );
	}

	/// <summary>
	/// Call after Clone() to launch the grenade.
	/// </summary>
	public void Launch( Vector3 origin, Vector3 direction, string throwerName )
	{
		WorldPosition = origin;
		ThrowerName = throwerName;

		if ( _rb is not null )
		{
			_rb.Velocity = direction.Normal * ThrowForce;
			// Random spin for visual flair
			_rb.AngularVelocity = new Vector3(
				Random.Shared.NextSingle() * 15f,
				Random.Shared.NextSingle() * 15f,
				Random.Shared.NextSingle() * 15f
			);
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy || HasDetonated ) return;

		_aliveTime += Time.Delta;

		// Fuse timer — auto-detonate
		if ( _aliveTime >= FuseTime )
		{
			Detonate();
		}
	}

	// ═══════════════════════════════
	//  COLLISION — impact detonation
	// ═══════════════════════════════

	public void OnCollisionStart( Collision other )
	{
		if ( IsProxy || HasDetonated ) return;

		// Skip if we just spawned (avoid self-collision with thrower)
		if ( _aliveTime < 0.15f ) return;

		if ( other.Contact.Speed > ImpactSpeedThreshold )
		{
			Detonate();
		}
	}

	public void OnCollisionUpdate( Collision other ) { }
	public void OnCollisionStop( CollisionStop other ) { }

	// ═══════════════════════════════
	//  DETONATION — the fun part
	// ═══════════════════════════════

	private void Detonate()
	{
		if ( HasDetonated ) return;
		HasDetonated = true;

		var origin = WorldPosition;

		// 1. Disable all mining rigs in blast radius
		int rigsHit = 0;
		foreach ( var rig in Scene.GetAllComponents<MiningRig>() )
		{
			float dist = Vector3.DistanceBetween( rig.WorldPosition, origin );
			if ( dist > BlastRadius ) continue;

			rig.DisableForDuration( DisableDuration );
			rigsHit++;
		}

		// 2. Drain a small % of wallet from players caught in blast
		foreach ( var wallet in Scene.GetAllComponents<CryptoWallet>() )
		{
			float dist = Vector3.DistanceBetween( wallet.WorldPosition, origin );
			if ( dist > BlastRadius ) continue;

			float drain = wallet.GoblinCoin * WalletDrainPercent;
			float falloff = 1f - (dist / BlastRadius);
			wallet.GoblinCoin -= drain * falloff;
		}

		// 3. Radial physics impulse — yeet nearby objects
		foreach ( var rb in Scene.GetAllComponents<Rigidbody>() )
		{
			if ( rb.GameObject == GameObject ) continue; // Skip self

			float dist = Vector3.DistanceBetween( rb.WorldPosition, origin );
			if ( dist > BlastRadius ) continue;

			var dir = (rb.WorldPosition - origin).Normal;
			float falloff = 1f - (dist / BlastRadius);

			// Add upward bias for satisfying launches
			dir += Vector3.Up * 0.4f;
			dir = dir.Normal;

			rb.ApplyForce( dir * BlastForce * falloff );
		}

		// 4. Broadcast effects to all clients
		BroadcastDetonation( origin, rigsHit, ThrowerName );

		// 5. Self-destruct
		_ = DestroyAfterDelay();
	}

	[Rpc.Broadcast]
	private void BroadcastDetonation( Vector3 position, int rigsDisabled, string thrower )
	{
		Sound.Play( "sounds/emp_blast.sound", position );

		if ( rigsDisabled > 0 )
			Log.Info( $"EMP by {thrower}: {rigsDisabled} rig(s) disabled!" );
		else
			Log.Info( $"EMP by {thrower}: no rigs in range (wasted!)" );

		// Screen shake — intensity falls off with distance
		var localPlayer = GoblinPlayer.All.FirstOrDefault( p => !p.IsProxy );
		if ( localPlayer is not null )
		{
			float dist = Vector3.DistanceBetween( localPlayer.WorldPosition, position );
			if ( dist <= BlastRadius )
			{
				float intensity = 1f - (dist / BlastRadius);
				CameraShake.Trigger( intensity, 0.6f );
			}
		}
	}

	private async Task DestroyAfterDelay()
	{
		await GameTask.DelaySeconds( 0.3f );
		GameObject.Destroy();
	}
}

/// <summary>
/// Thread-local camera shake state. Any system can call Trigger(); GoblinPlayer applies it.
/// </summary>
public static class CameraShake
{
	public static float Intensity { get; private set; } = 0f;

	private static float _duration;
	private static float _elapsed;

	public static void Trigger( float intensity, float duration )
	{
		Intensity = MathF.Max( Intensity, intensity );
		_duration = duration;
		_elapsed = 0f;
	}

	public static void Tick( float delta )
	{
		if ( _elapsed >= _duration ) { Intensity = 0f; return; }
		_elapsed += delta;
		Intensity *= MathF.Pow( 0.02f, delta ); // exponential decay
	}
}
