using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the main menu interface and navigation.
/// Handles player name input, game start/exit, and highscore display.
/// 
/// Key responsibilities:
/// - Manages player name input
/// - Handles scene transitions
/// - Displays highscores in menu
/// - Provides game exit functionality
/// 
/// Dependencies:
/// - DataManager for player data persistence
/// - SceneManager for scene transitions
/// - TextMeshPro for input handling
/// 
/// Note: Implements platform-specific quit functionality
/// </summary>

public class MenuHandler : MonoBehaviour
{
    // reference to InputField for player name 
    public TMP_InputField TM_PlayeNameInput;
    public Text highscoresText;

//reference to fade effect for scene transitions
    [SerializeField] private FadeEffect fadeEffect;

    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject highScoreUI;

    /// <summary>
    /// Initializes menu components and sets up input listeners
    /// </summary>
    private void Start()
    {
        // Setup player name input handling
        if (TM_PlayeNameInput != null)
        {
            TM_PlayeNameInput.onEndEdit.AddListener(OnInputFieldEndEdit);
        }
        UpdateHighscoresUI();
    }

    /// <summary>
    /// Initiates game start sequence
    /// Ensures player has a name before starting     /// </summary>

    public void StartGame()
    {
        // Set default player name if none provided
        if (string.IsNullOrEmpty(DataManager.Instance.currentPlayerId))
        {
            DataManager.Instance.currentPlayerId = "Player";
        }
        AudioManager.Instance.PlayResponseClickSound();
        StartCoroutine(TransitionToGameScene());
    }
    private IEnumerator TransitionToGameScene()
    {
        fadeEffect.FadeOut();
        yield return new WaitForSeconds(1f); // Wait for fade duration
        
        // Set default player name if none provided
        if (string.IsNullOrEmpty(DataManager.Instance.currentPlayerId))
        {
            DataManager.Instance.currentPlayerId = "Player";
        }
        AudioManager.Instance.StopBackgroundMusic();
        SceneManager.LoadScene(1);
        fadeEffect.FadeIn();
    }


    public void BackToMenu()
    {

        AudioManager.Instance.StopAllGameplayMusic();
        SceneManager.LoadScene(0);
    }

    public void BackButton()
    {
        menuUI.gameObject.SetActive(true);
        highScoreUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles platform-specific game exit
    /// Different behavior for editor and built game
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        AudioManager.Instance.PlayResponseClickSound();

#else
        Application.Quit();
        AudioManager.Instance.PlayResponseClickSound();

#endif
    }

    public void HighScore()
    {
        menuUI.gameObject.SetActive(false);
        highScoreUI.gameObject.SetActive(true);

        

    }

    /// <summary>
    /// Updates player name in DataManager
    /// Called when player finishes editing name input
    /// </summary>
    private void SetPlayerName()
    {
        if (DataManager.Instance != null && TM_PlayeNameInput != null)
        {
            DataManager.Instance.currentPlayerId = TM_PlayeNameInput.text;
        }
    }

    private void OnInputFieldEndEdit(string value)
    {
        AudioManager.Instance.PlayResponseClickSound();

        SetPlayerName();
    }

    /// <summary>
    /// Updates highscore display in menu
    /// Fetches current highscores from DataManager
    /// </summary>
    private void UpdateHighscoresUI()
    {
        if (highscoresText != null && DataManager.Instance != null)
        {
            highscoresText.text = DataManager.Instance.GetFormattedHighscores();
        }
    }
}