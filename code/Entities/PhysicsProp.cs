using Sandbox;
using System;

namespace GoblinChain;

/// <summary>
/// Makes any object grababble and throwable.
/// [E] on an unheld prop grabs it; [E] again throws it.
/// Host controls all physics; clients RPC their intent.
/// </summary>
public sealed class PhysicsProp : Component, IInteractable
{
	[Property] public float ThrowForce { get; set; } = 700f;
	[Property] public float HoldDistance { get; set; } = 75f;

	[Sync] public Guid HeldByPlayerId { get; set; } = Guid.Empty;

	private Rigidbody _rb;

	protected override void OnAwake()
	{
		_rb = Components.Get<Rigidbody>( FindMode.EverythingInSelf );
	}

	// ─── IInteractable ────────────────────────────────────────────────

	public void OnInteract( GoblinPlayer player )
	{
		if ( HeldByPlayerId == Guid.Empty )
		{
			RequestGrab();
		}
		else if ( HeldByPlayerId == player.Id )
		{
			if ( player.Head is null ) return;
			var vel = player.EyeAngles.ToRotation().Forward * ThrowForce;
			RequestThrow( vel );
		}
	}

	// ─── Host RPCs ────────────────────────────────────────────────────

	[Rpc.Host]
	public void RequestGrab()
	{
		if ( HeldByPlayerId != Guid.Empty ) return;

		var caller = Rpc.Caller;
		var player = GoblinPlayer.All.FirstOrDefault( p => p.Network.Owner == caller );
		if ( player is null ) return;

		HeldByPlayerId = player.Id;
		if ( _rb is not null ) _rb.MotionEnabled = false;
	}

	[Rpc.Host]
	public void RequestThrow( Vector3 velocity )
	{
		var caller = Rpc.Caller;
		var player = GoblinPlayer.All.FirstOrDefault( p => p.Network.Owner == caller );
		if ( player is null || player.Id != HeldByPlayerId ) return;

		HeldByPlayerId = Guid.Empty;
		if ( _rb is not null )
		{
			_rb.MotionEnabled = true;
			_rb.Velocity = velocity.ClampLength( 900f );
			_rb.AngularVelocity = new Vector3(
				Game.Random.Float( -5f, 5f ),
				Game.Random.Float( -5f, 5f ),
				Game.Random.Float( -5f, 5f )
			);
		}
	}

	// ─── Host: move prop to holder each physics tick ──────────────────

	protected override void OnFixedUpdate()
	{
		if ( IsProxy || HeldByPlayerId == Guid.Empty || _rb is null ) return;

		var holder = GoblinPlayer.All.FirstOrDefault( p => p.Id == HeldByPlayerId );
		if ( holder is null )
		{
			HeldByPlayerId = Guid.Empty;
			_rb.MotionEnabled = true;
			return;
		}

		var eyeRot = holder.EyeAngles.ToRotation();
		var holdPos = holder.WorldPosition
			+ Vector3.Up * 55f
			+ eyeRot.Forward * HoldDistance;

		WorldPosition = holdPos;
		WorldRotation = eyeRot;
	}
}
