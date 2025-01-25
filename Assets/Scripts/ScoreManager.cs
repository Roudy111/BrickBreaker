using UnityEngine;
using System;


/// <summary>
/// Manages the scoring system, including current score tracking and highscore updates
/// </summary>
public class ScoreManager : singleton<ScoreManager>
{
 
    public event Action<int> ScoreChanged;
    public event Action HighscoreUpdated;

    private int currentScore;
    public int CurrentScore
    {
        get => currentScore;
        private set
        {
            if (currentScore != value)
            {
                currentScore = value;
                ScoreChanged?.Invoke(currentScore);
                CheckHighscoreUpdate();
            }
        }
    }


    public override void Awake()
    {
        base.Awake();
        ResetScore();
    }

// Adds specified points to the current score
    public void AddPoints(int points)
    {
        CurrentScore += points;
    }


// Verifies if the current score qualifies as a new highscore
// Updates the player's highscore record if conditions are met
    private void CheckHighscoreUpdate()
    {
        if (DataManager.Instance != null)
        {
            bool wasUpdated = DataManager.Instance.AddOrUpdateHighscore(DataManager.Instance.currentPlayerId, CurrentScore);
            if (wasUpdated)
            {
                HighscoreUpdated?.Invoke();
            }
        }
    }
// Resets the current score back to zero  
// Typically called at game start or after game over
    public void ResetScore()
    {
        CurrentScore = 0;
    }
}