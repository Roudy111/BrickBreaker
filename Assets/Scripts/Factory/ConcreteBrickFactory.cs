using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// - Creation logic can be modified without affecting brick behavior
/// - Maintains separation of concerns for brick creation
/// - Uses probability-based product selection to keep modularity of the levelup process --- If it was supposed to be maintained and developed furthur a in editor interface is needed for design of each level
/// - Supports multiple regular brick variants (Deferent in Color and PointValue)
/// - New brick types can be added by extending Brick class
/// 
/// /// - Scales exploding brick probability based on level progression:
///   * Levels 1-2: No exploding bricks
///   * Levels 3-4: 10% chance
///   * Levels 5-6: 20% chance
///   * Level 7+: 30% chance
/// </summary>
public class ConcreteBrickFactory : Factory
{
    [Header("Regular Brick Settings")]
    [SerializeField] private List<RegularBrick> regularBrickPrebafs;

    [Header("Exploding Brick Settings")]
    [SerializeField] private ExplodingBrick explodingBrickPrefab;
    
    // Base probability values for different level ranges
    private const float PROBABILITY_LEVELS_2_3_4 = 0.1f;
    private const float PROBABILITY_LEVELS_5_6 = 0.2f;
    private const float PROBABILITY_LEVEL_7_PLUS = 0.3f;
    
    // Level threshold for introducing exploding bricks
    private const int EXPLODING_BRICK_LEVEL_THRESHOLD = 2;

    /// <summary>
    /// Calculates the appropriate exploding brick probability based on current level
    /// </summary>
    /// <returns>Probability value between 0 and 1</returns>
    private float GetExplodingBrickProbability()
    {
        int currentLevel = LevelManager.currentLevel;

        if (currentLevel < EXPLODING_BRICK_LEVEL_THRESHOLD)
        {
            return 0f;
        }
        else if (currentLevel <= 4)
        {
            return PROBABILITY_LEVELS_2_3_4;
        }
        else if (currentLevel <= 6)
        {
            return PROBABILITY_LEVELS_5_6;
        }
        else
        {
            return PROBABILITY_LEVEL_7_PLUS;
        }
    }

    /// <summary>
    /// Creates a new brick instance based on current level and probability settings.
    /// Probability of exploding bricks increases with level progression.
    /// </summary>
    /// <param name="position">World position for the new brick</param>
    /// <param name="rotation">Rotation for the new brick</param>
    /// <returns>IProduct interface of the created brick</returns>
    public override IProduct GetProduct(Vector3 position, Quaternion rotation)
    {
        Brick brick;

        // Check if we're at or above the level threshold and calculate probability
        bool canCreateExplodingBrick = LevelManager.currentLevel >= EXPLODING_BRICK_LEVEL_THRESHOLD;
        float currentProbability = GetExplodingBrickProbability();

        // Only consider exploding bricks if we're at the right level and conditions are met
        if (canCreateExplodingBrick && 
            explodingBrickPrefab != null && 
            Random.value < currentProbability)
        {
            brick = Instantiate(explodingBrickPrefab, position, rotation);
            Debug.Log($"Created exploding brick with probability {currentProbability} at level {LevelManager.currentLevel}");
        }
        else
        {
            // Validate regular brick prefabs
            if (regularBrickPrebafs == null || regularBrickPrebafs.Count == 0)
            {
                Debug.LogError("No regular brick prefabs assigned to factory!");
                return null;
            }

            // Create regular brick
            int randomPrefab = Random.Range(0, regularBrickPrebafs.Count);
            brick = Instantiate(regularBrickPrebafs[randomPrefab], position, rotation);
        }

        return brick;
    }
}