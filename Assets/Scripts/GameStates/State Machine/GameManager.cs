using UnityEngine;
using System;

/// <summary>
/// 
/// Core manager class that handles the game's state machine.
/// - Manages game state transitions (idle, gameplay, level changing, game over) 
/// - Other classes reference this class to change the game state
/// - Broadcasts state changes to other components through events
/// - Uses event system for state change notifications
/// 
/// </summary>

public enum GameStates
{
    idle,
    gamePlay,
    levelIsChanging,
    gameOver,
    // new States that I want to use 
    gameIsFinished,
    

}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameStates state;
    public static event Action<GameStates> OnGameStateChanged;


    void Start()
    {
        // Set initial game state to idle, waiting for player input
        UpdateGameState(GameStates.idle);
    }

    public void Awake()
    {
        instance = this;
    }

    //State machine for game states that has been defined in enum.
    // Handle of gameState has been distrubuted to different classes because it could be overkilled for such a simple game
    public void UpdateGameState(GameStates newstate)
    {
        state = newstate;
        Debug.Log($"Game State Updated: {newstate}");

        // Notify all subscribers about the state change
        OnGameStateChanged?.Invoke(newstate);
    }

}

