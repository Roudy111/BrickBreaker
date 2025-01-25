using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages in-game UI elements for User Name, Score and Highscore Chart and their updates during gameplay. -- Now the Ui related to gaveover and levelchange is seperated. However can be moved to here also. 
/// Implements Observer pattern to respond to score and highscore changes.
/// 
/// Key responsibilities:
/// - Updates score display in real-time
/// - Displays current highscores
/// - Shows current player information
/// - Manages UI element lifecycle
/// 
/// Dependencies:
/// - ScoreManager for score updates
/// - DataManager for player and highscore data
/// 
/// Design patterns:
/// - Observer: Subscribes to score change events
/// - MVC: Acts as View component for game data
public class UIHandler : MonoBehaviour
{

    // Reference to score management system
    private ScoreManager scoreManager;
    [SerializeField]

    // UI Text elements for displaying game information
    private Text ScoreText;
    [SerializeField]
    private Text HighscoreText;
    [SerializeField]
    private Text CurrentplayerName;

    /// <summary>
    /// Initializes UI handler and subscribes to score events
    /// Called when object becomes active in scene
    /// </summary>
    private void OnEnable()
    {
        scoreManager = ScoreManager.Instance;
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += OnUpdateScore;
            scoreManager.HighscoreUpdated += OnUpdateHighscore;
            scoreManager.ResetScore();

        }
        else
        {
            Debug.LogError("ScoreManager not found!");
            return;
        }
    }

    /// <summary>
    /// Unsubscribes from events when object becomes inactive
    /// Prevents memory leaks and invalid callbacks
    /// </summary>
    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged -= OnUpdateScore;
            scoreManager.HighscoreUpdated -= OnUpdateHighscore;
        }
    }
    void Start()
    {
        OnUpdateHighscore();
        CurrentPlayerNameSet();

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Updates UI when score changes
    /// Called by ScoreManager event
    /// </summary>

    void OnUpdateScore(int score)
    {
        ScoreText.text = $"Score : {score}";
    }

    /// <summary>
    /// Updates highscore display
    /// Fetches formatted highscore data from DataManager
    /// </summary>

    void OnUpdateHighscore()
    {
        if (HighscoreText != null && DataManager.Instance != null)
        {
            string highscores = DataManager.Instance.GetFormattedHighscores();
            HighscoreText.text = $"Highscores:\n{highscores}";
        }
    }

    void CurrentPlayerNameSet()
    {
        if (CurrentplayerName != null && DataManager.Instance != null)
        {
            CurrentplayerName.text = $"Player Name: {DataManager.Instance.currentPlayerId}";
        }
    }
}
