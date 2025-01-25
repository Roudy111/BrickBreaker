using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


/// <summary>
/// 
/// Key responsibilities:
/// - Displays game over UI elements
/// - Manages level cleanup (destroying bricks) on game over
/// - Provides navigation options (restart/menu) for the player on game over
/// 
/// Dependencies:
/// - Requires GameManager for state management
/// </summary>
public class GameOverState : MonoBehaviour
{

    // UI elements for game over state
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject backToMenuButton;

    //for managing the input for Restartgame in update method instead of using new Input System which is overkill for this simple game
    private bool IsGameOver = false;    // Tracks if game is in game over state to handle restart input
  

    private void OnEnable()
    {
        // Subscribe to game state changes to detect game over condition
        GameManager.OnGameStateChanged += HandleGameStateChanged;

        
    }

    private void OnDisable()
    {
        // Unsubscribe to game state changes to detect game over condition

        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    private void Update()
    {
         // Check for space input only in game over state

        if (IsGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    // DeathZone Change the state to gameOver, then this method check if the state is gameover and call methods for gameover
    private void HandleGameStateChanged(GameStates newState)
    {
        if (newState == GameStates.gameOver)
        {
            InitiateGameOver();

        }
    }

    // Shows game over UI and cleans up level
    private void InitiateGameOver()
    {
        IsGameOver = true;
        DeleteBricks();
        gameOverText.SetActive(true);
        backToMenuButton.SetActive(true);
        // You might want to call a method to update and display the high score here
        // scoreManager.UpdateHighScore();
    }
    

    //to restart the game with reloading the active scene
    public void RestartGame()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
       
    }

    public void BackToMenu()
    {
        AudioManager.Instance.StopAllGameplayMusic();
        SceneManager.LoadScene(0); // Assuming 0 is your menu scene index
    }

    // Destroys all bricks in the scene to clean up the level in Game Over state
     public void DeleteBricks()
    {
        Brick[] bricks = FindObjectsOfType<Brick>();
        foreach (var brick in bricks)
        {
            Destroy(brick.gameObject);
        }
        Counter.m_TotalBrick = 0;
    }

}