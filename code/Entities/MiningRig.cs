using Sandbox;

namespace GoblinChain;

/// <summary>
/// A placeable mining rig. Passively generates hash rate for its owner.
/// Can be damaged by collisions and disabled by EMP grenades.
/// Implements IInteractable for upgrade/repair via Use key.
/// </summary>
public sealed class MiningRig : Component, Component.ICollisionListener, IInteractable
{
	// --- Config ---
	[Property] public float BaseHashRate { get; set; } = 5f;
	[Property] public float MaxDurability { get; set; } = 100f;
	[Property] public float RepairCost { get; set; } = 25f;

	// --- Synced State ---
	[Sync] public float Durability { get; set; } = 100f;
	[Sync] public bool IsOperational { get; set; } = true;
	[Sync] public int Tier { get; set; } = 1;

	// --- Internal ---
	private float _disableTimer = 0f;

	protected override void OnStart()
	{
		Durability = MaxDurability;
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		// Handle EMP disable countdown
		if ( !IsOperational && _disableTimer > 0f )
		{
			_disableTimer -= Time.Delta;
			if ( _disableTimer <= 0f )
			{
				IsOperational = true;
				Log.Info( $"Rig {GameObject.Name} back online!" );
			}
		}
	}

	// --- Collision Damage ---

	public void OnCollisionStart( Collision other )
	{
		if ( IsProxy ) return;

		float impactSpeed = other.Contact.Speed;
		if ( impactSpeed > 150f )
		{
			float damage = impactSpeed * 0.08f;
			TakeDamage( damage );
		}
	}

	public void OnCollisionUpdate( Collision other ) { }
	public void OnCollisionStop( CollisionStop other ) { }

	// --- Damage / Disable ---

	public void TakeDamage( float amount )
	{
		if ( IsProxy ) return;

		Durability -= amount;
		Durability = System.MathF.Max( 0f, Durability );

		if ( Durability <= 0f )
		{
			IsOperational = false;
			Log.Info( $"Rig {GameObject.Name} destroyed!" );
		}
	}

	/// <summary>
	/// Called by EMP grenade. Disables rig for a duration.
	/// </summary>
	public void DisableForDuration( float seconds )
	{
		if ( IsProxy ) return;

		IsOperational = false;
		_disableTimer = seconds;
		Log.Info( $"Rig {GameObject.Name} EMP'd for {seconds}s" );
	}

	// --- Interaction (Use key) ---

	public void OnInteract( GoblinPlayer player )
	{
		if ( IsProxy ) return;

		// If damaged, offer repair
		if ( Durability < MaxDurability )
		{
			var wallet = player.Components.Get<CryptoWallet>();
			if ( wallet is not null && wallet.TrySpend( RepairCost ) )
			{
				Durability = MaxDurability;
				IsOperational = true;
				_disableTimer = 0f;
				Log.Info( $"{player.Network.Owner?.DisplayName} repaired rig for {RepairCost} GBC" );
			}
		}
	}

	// --- Public API ---

	/// <summary>
	/// Effective hash rate considering operational status and durability.
	/// </summary>
	public float EffectiveHashRate
	{
		get
		{
			if ( !IsOperational ) return 0f;
			float healthPercent = Durability / MaxDurability;
			return BaseHashRate * Tier * healthPercent;
		}
	}
}
