using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Manages level-related functionality including brick layout, level progression, and level state.
/// Implements Factory pattern for brick creation and follows Single Responsibility Principle
/// for level management tasks.
/// 
/// Key responsibilities:
/// - Manages level progression and counting
/// - Handles brick initialization and layout
/// - Controls level transition UI and timing
/// - Manages brick cleanup between levels
/// - Handles progressive difficulty through level-specific layouts
/// 
/// Design patterns used:
/// - Factory Pattern for brick creation
/// - Observer Pattern for level completion events
/// - Strategy Pattern for level configuration
/// 
/// Dependencies:
/// - Requires ConcreteBrickFactory for brick instantiation
/// - Uses Counter system for brick tracking
/// - Interfaces with GameManager for state changes
/// </summary>
public class LevelManager : MonoBehaviour
{
    // Current level number, encapsulated with read-only access
    public static int currentLevel { get; private set; } = 1;

    [SerializeField] private Text LevelText;
    [SerializeField] private ConcreteBrickFactory brickFactory;
    private Ball ball;

    // Level configuration constants
    private readonly int[] rowsPerLevel = { 2, 3, 4, 5, 6 };
    private readonly int[] columnsPerLevel = { 2, 3, 4, 5, 6 };
    private const int MAX_LEVEL = 100;
    private const int LAYOUT_CAP_LEVEL = 5;  // Level at which layout stops increasing
    
    void OnEnable()
    {
        Counter.LevelFinished += OnLevelFinished;
    }

    void OnDestroy()
    {
        Counter.LevelFinished -= OnLevelFinished;
    }

    void Start()
    {
        UpdateLevelText();

        if (brickFactory == null)
        {
            Debug.LogError("BrickFactory is not set in BrickManager!");
            return;
        }

        InitiateBlocks();
        ball = FindObjectOfType<Ball>();
    }

    /// <summary>
    /// Event handler for when a level is completed.
    /// Initiates the transition to the next level if not at max level.
    /// </summary>
    private void OnLevelFinished()
    {
        if (currentLevel < MAX_LEVEL)
        {
            StartCoroutine(InitiateNextLevel());
        }
        else
        {
            GameManager.instance.UpdateGameState(GameStates.gameOver);
            Debug.Log("Congratulations! All levels completed!");
        }
    }

    /// <summary>
    /// Coroutine to handle the transition to the next level.
    /// </summary>
    IEnumerator InitiateNextLevel()
    {
        GameManager.instance.UpdateGameState(GameStates.levelIsChanging);
        ball.ResetBall();
        currentLevel++;
        
        UpdateLevelText();
        LevelText.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        LevelText.gameObject.SetActive(false);

        InitiateBlocks();
        GameManager.instance.UpdateGameState(GameStates.idle);
    }

    /// <summary>
    /// Creates a horizontally centered grid of bricks that grows upward with each level.
    /// Grid size increases until level 5, then maintains that configuration for higher levels.
    /// Level 1 starts one row higher, and subsequent levels add rows from bottom up.
    /// </summary>
    public void InitiateBlocks()
    {
        // Get current level's configuration, capped at level 5's settings
        int levelIndex = Mathf.Min(currentLevel - 1, LAYOUT_CAP_LEVEL - 1);
        int rows = rowsPerLevel[levelIndex];
        int columns = columnsPerLevel[levelIndex];

        // Constants for brick layout
        const float BRICK_WIDTH = 0.6f;
        const float BRICK_HEIGHT = 0.3f;
        const int MAX_GRID_COLUMNS = 6;
        
        // Calculate horizontal centering offset
        float centerXOffset = (MAX_GRID_COLUMNS - columns) * BRICK_WIDTH * 0.5f;
        
        // Calculate starting X position
        const float START_X = -1.5f; // Left edge of maximum width grid

        Counter.m_TotalBrick = 0;

        // Create brick grid with bottom alignment and upward growth
        for (int i = 0; i < rows; ++i)
        {
            for (int x = 0; x < columns; ++x)
            {
                // Calculate Y position - Level 1 maintains height, others grow upward
                float baseYPos = 2.5f; // Base Y position (where Level 1 starts)
                float rowsToAdd = (currentLevel > 1) ? rows - rowsPerLevel[0] : 0; // Additional rows compared to Level 1
                
                Vector3 position = new Vector3(
                    START_X + centerXOffset + (BRICK_WIDTH * x),
                    baseYPos + (rowsToAdd * BRICK_HEIGHT) - (BRICK_HEIGHT * i),
                    0
                );

                IProduct product = brickFactory.GetProduct(position, Quaternion.identity);
                if (product is Brick)
                {
                    Counter.m_TotalBrick++;
                }
                else
                {
                    Debug.LogError("Product created is not a Brick!");
                }
            }
        }

        // Enhanced logging for level initialization
        string levelInfo = currentLevel > LAYOUT_CAP_LEVEL ? 
            $"Level {currentLevel} (using Level {LAYOUT_CAP_LEVEL} layout)" : 
            $"Level {currentLevel}";
        Debug.Log($"{levelInfo} initialized with {Counter.m_TotalBrick} bricks ({rows}x{columns})");
    }

    /// <summary>
    /// Updates the UI text displaying the current level number
    /// </summary>
    void UpdateLevelText()
    {
        if (LevelText != null)
        {
            LevelText.text = $"Level {currentLevel}";
        }
    }
}