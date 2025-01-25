using UnityEngine;
using System.Collections;

/// <summary>
/// Handles ball behavior, physics, and interactions in the game.
/// Implements improved paddle physics and collision responses.
/// </summary>
public class Ball : MonoBehaviour
{
    #region Private Fields
    private Rigidbody m_Rigidbody;
    private Vector3 initialLocalPosition;
    private Transform paddleTransform;
    private Coroutine resetCoroutine;
    private Vector3 lastPaddlePosition;
    private float currentSpeed;
    #endregion

    #region Serialized Fields
    [Header("Basic Ball Settings")]
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float accelerationFactor = 0.01f;
    [SerializeField] private float verticalBias = 0.5f;

    [Header("Paddle Interaction Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("How much the paddle's movement affects the ball's direction")]
    private float paddleInfluence = 0.5f;

    [SerializeField]
    [Range(0f, 45f)]
    [Tooltip("Minimum angle the ball can bounce off the paddle")]
    private float minBounceAngle = 5f;

    [SerializeField]
    [Range(45f, 85f)]
    [Tooltip("Maximum angle the ball can bounce off the paddle")]
    private float maxBounceAngle = 75f;

    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("How much spin is applied based on paddle movement")]
    private float spinFactor = 0.2f;

    [Header("Explosion Effect Settings")]
    [SerializeField]
    [Range(1f, 2f)]
    [Tooltip("Speed multiplier when hitting an explosive brick")]
    private float explosionSpeedMultiplier = 1.3f;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializeComponents();
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        UpdatePaddlePosition();
    }
    #endregion

    #region Initialization Methods
    /// <summary>
    /// Initializes all necessary components and starting values
    /// </summary>
    private void InitializeComponents()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        if (m_Rigidbody == null)
        {
            Debug.LogError("Rigidbody component missing from ball!");
            return;
        }

        paddleTransform = transform.parent;
        initialLocalPosition = transform.localPosition;
        currentSpeed = baseSpeed;
        lastPaddlePosition = paddleTransform != null ? paddleTransform.position : Vector3.zero;
    }

    /// <summary>
    /// Subscribes to all necessary game events
    /// </summary>
    private void SubscribeToEvents()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
        ExplodingBrick.BrickExploded += OnBrickExploded;
    }

    /// <summary>
    /// Unsubscribes from all game events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
        ExplodingBrick.BrickExploded -= OnBrickExploded;
    }
    #endregion

    #region Collision Handling
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleCollision(collision);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (m_Rigidbody != null && !m_Rigidbody.isKinematic)
        {
            AdjustVelocityAfterCollision();
        }
    }

    /// <summary>
    /// Handles the physics and behavior when the ball hits the paddle
    /// </summary>
    private void HandlePaddleCollision(Collision collision)
    {
        if (m_Rigidbody.isKinematic) return;

        // Calculate hit position relative to paddle center
        Vector3 paddleCenter = collision.transform.position;
        float hitPoint = transform.position.x - paddleCenter.x;
        float paddleHalfWidth = collision.collider.bounds.extents.x;
        float normalizedHitPoint = Mathf.Clamp(hitPoint / paddleHalfWidth, -1f, 1f);

        // Calculate bounce angle based on hit position
        float angleRange = (maxBounceAngle - minBounceAngle) / 2f;
        float bounceAngle = normalizedHitPoint * angleRange;
        bounceAngle = Mathf.Clamp(bounceAngle, -maxBounceAngle, maxBounceAngle);

        // Calculate paddle movement influence
        Vector3 paddleVelocity = (paddleTransform.position - lastPaddlePosition) / Time.fixedDeltaTime;
        float horizontalInfluence = paddleVelocity.x * paddleInfluence;

        // Create and adjust the new velocity vector
        Vector3 direction = Quaternion.Euler(0, 0, bounceAngle) * Vector3.up;
        direction.x += horizontalInfluence;
        direction = direction.normalized;

        // Apply spin based on paddle movement
        float spin = paddleVelocity.x * spinFactor;
        direction = Quaternion.Euler(0, 0, spin) * direction;

        // Set the final velocity
        m_Rigidbody.velocity = direction * currentSpeed;
    }

    /// <summary>
    /// Adjusts the ball's velocity after any collision
    /// </summary>
    private void AdjustVelocityAfterCollision()
    {
        if (m_Rigidbody.isKinematic) return;

        var velocity = m_Rigidbody.velocity;

        // Apply acceleration
        velocity += velocity.normalized * accelerationFactor;

        // Prevent purely vertical movement
        if (Vector3.Dot(velocity.normalized, Vector3.up) < 0.1f)
        {
            velocity += velocity.y > 0 ? Vector3.up * verticalBias : Vector3.down * verticalBias;
        }

        // Clamp to current max speed
        if (velocity.magnitude > currentSpeed)
        {
            velocity = velocity.normalized * currentSpeed;
        }

        // Apply final velocity if still not kinematic
        if (!m_Rigidbody.isKinematic)
        {
            m_Rigidbody.velocity = velocity;
        }
    }
    #endregion

    #region Ball State Management
    /// <summary>
    /// Updates the ball's speed when hitting an explosive brick
    /// </summary>
    private void OnBrickExploded()
    {
        if (m_Rigidbody == null || m_Rigidbody.isKinematic) return;

        var velocity = m_Rigidbody.velocity;
        currentSpeed = Mathf.Min(currentSpeed * explosionSpeedMultiplier, maxSpeed);
        m_Rigidbody.velocity = velocity.normalized * currentSpeed;
    }

    /// <summary>
    /// Updates the last known paddle position for physics calculations
    /// </summary>
    private void UpdatePaddlePosition()
    {
        if (paddleTransform != null)
        {
            lastPaddlePosition = paddleTransform.position;
        }
    }

    /// <summary>
    /// Resets the ball to its initial state
    /// </summary>
    public void ResetBall()
    {
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }
        resetCoroutine = StartCoroutine(ResetBallCoroutine());
    }

    /// <summary>
    /// Coroutine to handle the ball reset sequence
    /// </summary>
    private IEnumerator ResetBallCoroutine()
    {
        if (m_Rigidbody != null)
        {
            // Reset physics state
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
            m_Rigidbody.isKinematic = true;
        }

        // Reset position and parent
        transform.SetParent(paddleTransform);
        transform.localPosition = initialLocalPosition;

        // Reset speed
        currentSpeed = baseSpeed;

        // Wait before re-enabling physics
        yield return new WaitForSeconds(1);

        if (m_Rigidbody != null)
        {
            m_Rigidbody.WakeUp();
            m_Rigidbody.isKinematic = false;
        }
    }

    /// <summary>
    /// Handles changes in game state
    /// </summary>
    private void HandleGameStateChanged(GameStates newState)
    {
        if (newState == GameStates.levelIsChanging)
        {
            ResetBall();
        }
    }
    #endregion
}