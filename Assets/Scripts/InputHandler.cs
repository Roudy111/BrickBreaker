using UnityEngine;

/// <summary>
/// Handles all input states and controls for the game
/// Implements singleton pattern for global access
/// </summary>
public class InputHandler : singleton<InputHandler>
{


    private bool isSpaceEnabled = true;

    /// <summary>
    /// Handles input detection and validation for the game controls
    /// </summary>



    // Subscribes to game state change events when the component is enabled

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }
    // Unsubscribes to game state change events for preventing meory leaks

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    /// <summary>
    /// Updates input permissions based on the current game state
    /// </summary>
    /// <param name="newState">The new game state to handle</param>
    private void HandleGameStateChanged(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.idle:
                // Enable space input when ball is attached to paddle and waiting for launch
                isSpaceEnabled = true;
                break;

            case GameStates.gamePlay:
                // Disable space input during active gameplay when ball is in motion
                isSpaceEnabled = false;
                break;

            case GameStates.levelIsChanging:
                // Disable space input during level transitions
                isSpaceEnabled = false;
                break;

            case GameStates.gameOver:
                // Disable space input when game is over
                isSpaceEnabled = false;
                break;
        }
    }

    /// <summary>
    /// Checks if space bar input is currently allowed and pressed
    /// </summary>
    /// <returns>True if space is both enabled and pressed, false otherwise</returns>
    public bool IsSpaceAllowed()
    {
        return isSpaceEnabled && Input.GetKeyDown(KeyCode.Space);
    }


}