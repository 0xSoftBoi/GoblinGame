using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// Core player component. Handles first-person movement via CharacterController,
/// mouse look, interaction raycasts, and animation.
/// Attach to the player prefab alongside CharacterController, CitizenAnimationHelper,
/// ModelRenderer (citizen model), CryptoWallet, and a child "Head" GameObject for camera.
/// </summary>
public sealed class GoblinPlayer : Component
{
	public static List<GoblinPlayer> All { get; } = new();
	// --- Movement Config ---
	[Property] public float Speed { get; set; } = 160f;
	[Property] public float RunSpeed { get; set; } = 290f;
	[Property] public float CrouchSpeed { get; set; } = 90f;
	[Property] public float JumpForce { get; set; } = 400f;
	[Property] public float GroundFriction { get; set; } = 4.0f;
	[Property] public float AirControl { get; set; } = 0.1f;
	[Property] public float MaxAirForce { get; set; } = 50f;
	[Property] public float InteractRange { get; set; } = 150f;

	// --- Scene References (drag in editor) ---
	[Property] public GameObject Head { get; set; }
	[Property] public GameObject Body { get; set; }

	// --- Synced State ---
	[Sync] public Angles EyeAngles { get; set; }
	[Sync] public bool IsCrouching { get; set; }
	[Sync] public bool IsSprinting { get; set; }

	// --- Customization ---
	[Sync] public int SkinColorIndex { get; set; } = 0;
	[Sync] public int AccessoryIndex { get; set; } = 0;

	public static readonly Color[] SkinColors =
	{
		new Color( 0.22f, 0.47f, 0.17f ), // classic goblin green
		new Color( 0.17f, 0.35f, 0.54f ), // swamp blue
		new Color( 0.47f, 0.17f, 0.47f ), // toxic purple
		new Color( 0.75f, 0.35f, 0.17f ), // lava orange
		new Color( 0.10f, 0.10f, 0.14f ), // void black
		new Color( 0.69f, 0.56f, 0.13f ), // gold goblin
	};
	public static readonly string[] AccessoryIcons = { "", "🎩", "👑", "🪖", "🎪" };
	public static readonly string[] SkinNames = { "GREEN", "BLUE", "PURPLE", "ORANGE", "BLACK", "GOLD" };

	// --- Cached Components ---
	private CharacterController _cc;
	private CitizenAnimationHelper _anim;
	private Vector3 _wishVelocity;

	protected override void OnAwake()
	{
		_cc = Components.Get<CharacterController>();
		_anim = Components.Get<CitizenAnimationHelper>();
	}

	protected override void OnStart()
	{
		All.Add( this );

		if ( !IsProxy )
		{
			// First-person: hide body, show shadow only
			var bodyRenderer = Body?.Components.Get<ModelRenderer>();
			if ( bodyRenderer is not null )
				bodyRenderer.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;

			// Take camera control
			if ( Scene.Camera is not null && Head is not null )
			{
				Scene.Camera.WorldPosition = Head.Transform.Position;
				Scene.Camera.WorldRotation = Head.Transform.Rotation;
			}
		}
	}

	protected override void OnDestroy()
	{
		All.Remove( this );
	}

	// =========================================================
	//  UPDATE — Input, look, camera, interaction (every frame)
	// =========================================================

	protected override void OnUpdate()
	{
		// Animate regardless of ownership (so other players animate too)
		UpdateAnimation();
		ApplyCustomization();

		if ( IsProxy ) return;

		// --- Mouse Look ---
		var angles = EyeAngles;
		float sens = 0.1f * GameSettings.MouseSensitivity;
		angles.pitch += Input.MouseDelta.y * sens;
		angles.yaw -= Input.MouseDelta.x * sens;
		angles.roll = 0f;
		angles.pitch = angles.pitch.Clamp( -89.9f, 89.9f );
		EyeAngles = angles;

		// Rotate head (full pitch+yaw for camera)
		if ( Head is not null )
			Head.Transform.Rotation = angles.ToRotation();

		// Rotate body (yaw only — character faces movement direction)
		WorldRotation = Rotation.FromYaw( angles.yaw );

		// --- Camera ---
		if ( Scene.Camera is not null && Head is not null )
		{
			Scene.Camera.WorldPosition = Head.Transform.Position;
			Scene.Camera.WorldRotation = Head.Transform.Rotation;

			CameraShake.Tick( Time.Delta );
			if ( CameraShake.Intensity > 0.001f )
			{
				float t = Time.Now * 60f;
				Scene.Camera.WorldPosition += new Vector3(
					MathF.Sin( t * 1.7f ),
					MathF.Sin( t * 2.3f ),
					MathF.Sin( t * 1.1f )
				) * CameraShake.Intensity * 4f;
			}
		}

		// --- State ---
		IsCrouching = Input.Down( "Crouch" );
		IsSprinting = Input.Down( "Run" ) && !IsCrouching;

		// --- Interaction ---
		if ( Input.Pressed( "Use" ) )
			TryInteract();
	}

	// =========================================================
	//  FIXED UPDATE — Physics movement (fixed timestep)
	// =========================================================

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		if ( _cc is null ) return;

		BuildWishVelocity();
		ApplyMovement();
	}

	private void BuildWishVelocity()
	{
		_wishVelocity = Vector3.Zero;
		var rot = Head?.Transform.Rotation ?? WorldRotation;

		if ( Input.Down( "Forward" ) )  _wishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) _wishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) )     _wishVelocity += rot.Left;
		if ( Input.Down( "Right" ) )    _wishVelocity += rot.Right;

		// Flatten to horizontal plane, normalize
		_wishVelocity = _wishVelocity.WithZ( 0 );
		if ( !_wishVelocity.IsNearZeroLength )
			_wishVelocity = _wishVelocity.Normal;

		// Apply speed multiplier
		if ( IsCrouching )        _wishVelocity *= CrouchSpeed;
		else if ( IsSprinting )   _wishVelocity *= RunSpeed;
		else                      _wishVelocity *= Speed;
	}

	private void ApplyMovement()
	{
		var gravity = Scene.PhysicsWorld.Gravity;

		if ( _cc.IsOnGround )
		{
			// Grounded: full control
			_cc.Velocity = _cc.Velocity.WithZ( 0 );
			_cc.Accelerate( _wishVelocity );
			_cc.ApplyFriction( GroundFriction );

			// Jump
			if ( Input.Pressed( "Jump" ) )
			{
				_cc.Punch( Vector3.Up * JumpForce );
				_anim?.TriggerJump();
			}
		}
		else
		{
			// Airborne: limited control + gravity
			_cc.Velocity += gravity * Time.Delta * 0.5f;
			_cc.Accelerate( _wishVelocity.ClampLength( MaxAirForce ) );
			_cc.ApplyFriction( AirControl );
		}

		_cc.Move();

		// Post-move gravity integration
		if ( !_cc.IsOnGround )
			_cc.Velocity += gravity * Time.Delta * 0.5f;
		else
			_cc.Velocity = _cc.Velocity.WithZ( 0 );
	}

	// =========================================================
	//  ANIMATION
	// =========================================================

	private void UpdateAnimation()
	{
		if ( _anim is null ) return;

		_anim.WithWishVelocity( _wishVelocity );
		_anim.WithVelocity( _cc?.Velocity ?? Vector3.Zero );
		_anim.AimAngle = Head?.Transform.Rotation ?? WorldRotation;
		_anim.IsGrounded = _cc?.IsOnGround ?? true;
		_anim.DuckLevel = IsCrouching ? 1f : 0f;

		if ( Head is not null )
			_anim.WithLook( Head.Transform.Rotation.Forward, 1f, 0.75f, 0.5f );
	}

	// =========================================================
	//  INTERACTION — Use key raycast
	// =========================================================

	private void TryInteract()
	{
		if ( Head is null ) return;

		var eyePos = Head.Transform.Position;
		var eyeDir = Head.Transform.Rotation.Forward;

		var trace = Scene.Trace
			.Ray( eyePos, eyePos + eyeDir * InteractRange )
			.WithoutTags( "player", "trigger" )
			.Run();

		if ( !trace.Hit || trace.GameObject is null )
			return;

		// Try IInteractable on the hit object
		var interactable = trace.GameObject.Components.Get<IInteractable>();
		if ( interactable is not null )
		{
			interactable.OnInteract( this );
			return;
		}

		// Also check parent (for child colliders on complex objects)
		var parentInteractable = trace.GameObject.Parent?.Components.Get<IInteractable>();
		parentInteractable?.OnInteract( this );
	}

	// =========================================================
	//  CUSTOMIZATION
	// =========================================================

	private void ApplyCustomization()
	{
		var bodyRenderer = Body?.Components.Get<ModelRenderer>();
		if ( bodyRenderer is null ) return;
		int si = SkinColorIndex.Clamp( 0, SkinColors.Length - 1 );
		bodyRenderer.Tint = SkinColors[si];
	}

	[Rpc.Host]
	public void RequestSetCustomization( int skinIdx, int accIdx )
	{
		if ( Rpc.Caller != Network.Owner ) return;
		SkinColorIndex = skinIdx.Clamp( 0, SkinColors.Length - 1 );
		AccessoryIndex = accIdx.Clamp( 0, AccessoryIcons.Length - 1 );
	}
}

/// <summary>
/// Implement on any component to make it interactable via the Use key.
/// </summary>
public interface IInteractable
{
	void OnInteract( GoblinPlayer player );
}
