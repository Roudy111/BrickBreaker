using System;
using UnityEngine;

/// <summary>
/// Specialized brick type that creates chain reactions when destroyed.
/// Demonstrates inheritance and polymorphism in the brick system.
/// 
/// Key features:
/// - Chain reaction explosion affecting nearby bricks
/// - Physics-based  explosion force
/// 
/// Design patterns:
/// - Observer: Notifies game systems of explosions
/// - Inherited: Extends base brick behavior
/// 
/// Gameplay mechanics:
/// - Affects other bricks within explosion radius
/// - Can trigger other exploding bricks
/// - Applies physical forces to affected objects
/// </summary>

public class ExplodingBrick : Brick
{
    // Explosion configuration
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 1.5f;
    [SerializeField] private float upwardsModifier = 0.4f;
    // Layer targeting for explosion effects
    [SerializeField] private LayerMask brickLayer;


    [SerializeField] private AudioClip explosionSFX;     // Explosion sound effect
    [SerializeField] private ParticleSystem explosionVFX; // Explosion visual effect


    private bool hasExploded = false;     // Prevents multiple explosions

    // Event to notify the ball that explodingBrick has been exploded
    public static event Action BrickExploded;


    protected override void Start()
    {
        base.Start();
        SetBrickLayerMask();


    }



    private void PlayExplosionSound()
    {
        if (explosionSFX != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position);
        }
        else
        {
            Debug.LogError("Failed to play explosion sound. AudioClip or AudioSource is missing.");
        }
    }

    private void PlayExplosionVFX()
    {
        if (explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position,explosionVFX.transform.rotation);
        }
        else
        {
            Debug.LogError("Failed to play explosion VFX. ParticleSystem is missing.");
        }
    }

    private void SetBrickLayerMask()
    {
        // Set the layer mask to only include the brick layer (layer 7)
        brickLayer = 1 << 7;
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (!IsDestroyed && !hasExploded)
        {
            DestroyBrick();
            BrickExploded?.Invoke();
 
            
        }
    }

    //polymorphism
    public override void DestroyBrick()
    {
        if (!IsDestroyed && !hasExploded)
        {
            base.DestroyBrick();
            Explode();

        }
    }

    private void Explode()
    {
        hasExploded = true;
        ApplyExplosionForce(true);
        PlayExplosionSound();
        PlayExplosionVFX();

    }

    /// <summary>
    /// Applies explosion force to nearby objects and triggers chain reactions
    /// </summary>
    private void ApplyExplosionForce(bool isInitialExplosion)
    {
        // Find all colliders within explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, brickLayer);
        foreach (Collider hit in colliders)
        {
            // Apply physical forces
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            }

            // Handle chain reactions
            Brick hitBrick = hit.GetComponent<Brick>();
            if (hitBrick != null && !hitBrick.IsDestroyed)
            {
                // Special handling for other exploding bricks
                if (hitBrick is ExplodingBrick explodingBrick)
                {
                    if (isInitialExplosion)
                    {
                        explodingBrick.TriggerExplosion();
                    }
                }
                else
                {
                    hitBrick.DestroyBrick();
                }
            }
        }
    }

    public void TriggerExplosion()
    {
        if (!IsDestroyed && !hasExploded)
        {
            DestroyBrick();
        }
    }

// visual debugging for showing how much the explosion can affect bricks nearby in editor
    protected override void DrawGizmos()
    {
        base.DrawGizmos();
        if (!showGizmos) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}