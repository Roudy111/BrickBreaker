using System;
using UnityEngine;

/// <summary>
/// Abstract base class for all brick types in the game, implementing Factory pattern.
/// Provides core brick functionality 
/// 
/// Design Patterns:
/// - Observer: Broadcasts destruction events to interested listeners
/// - Factory Product: Implements IProduct for factory creation
/// 
/// Key features:
/// - Abstract class allowing for different brick behaviors
/// - Visual debugging through gizmos 
/// 
/// </summary>
public abstract class Brick : MonoBehaviour, IProduct
{
    public static event Action<int> BrickDestroyed;  // Event for notifying score and counter systems of brick destruction
    public int PointValue;  // Score value when brick is destroyed

    // Debug visualization properties for Debugging
    [SerializeField] protected Color gizmoColor = Color.yellow;
    [SerializeField] protected bool showGizmos = true;


    protected private AudioSource audioSource;     // Audio component for sound effects
    public bool IsDestroyed { get; private set; } = false;   // Prevents multiple destruction calls
    public string ProductName { get; set; }     // IProduct implementation

    /// <summary>
    /// Template method for initialization, called by concrete brick types
    /// </summary>

    protected virtual void Start()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        SetProductName();
        SetupAudioSource();
    }
    protected virtual void SetProductName()
    {
        // Set a default product name, can be overridden in derived classes
        ProductName = "Generic Brick";
    }

    // Handling the Aduio source which is needed for all bricks, then each brick add its own audio clips
    protected virtual void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Ensure the AudioSource is set up correctly
        audioSource.playOnAwake = false;
    }
    // Handle the Physic Collision 
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (!IsDestroyed)
        {
            DestroyBrick();
        }
    }

    // Refactored of destruction of Bricks for readibility of codes
    public virtual void DestroyBrick()
    {
        if (!IsDestroyed)
        {
            BrickDestroyed?.Invoke(PointValue);
            IsDestroyed = true;
            Destroy(gameObject, 0.1f);
        }
    }
#region  //Visual Debugging for each Brick -- Should be turend off for release 
    
    protected virtual void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }
    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DrawGizmos();
        }
    }
    protected virtual void DrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
#endregion    
}