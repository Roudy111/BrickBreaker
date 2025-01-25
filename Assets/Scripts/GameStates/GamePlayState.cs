
using UnityEngine;


/// <summary>
/// Responsibilities:
/// - Handles ball launch input through InputHandler
/// - Controls initial ball trajectory and physics
/// - change state from idle to gameplay with ball launch
/// 
/// Note: The current direction calculation could be improved for better gameplay feel
/// 
/// Dependencies:
/// - Requires GameManager for state management
/// - Uses InputHandler to check if ball launch input is allowed
/// - Needs reference to ball's Rigidbody component
/// </summary>
public class GamePlayState : MonoBehaviour
{
    [SerializeField]
    private Rigidbody Ball;     // Reference to the ball's physics component

   
    void Update()
    {
         // Check for ball launch input only in idle state
         // this can be put to input handler

        if (InputHandler.Instance.IsSpaceAllowed() && GameManager.instance.state == GameStates.idle)
        {
            Cursor.visible = false;
            StartGame();
        }

    }

    // Initializes gameplay by launching the ball
    void StartGame()
    {
        if (Ball != null)
        {
            float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
            // direction should be improved 
            Vector3 forceDir = new(randomDirection, 1, 0);
            forceDir.Normalize();

            Ball.transform.SetParent(null);
            Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);

            //change to GamePlay state
            GameManager.instance.UpdateGameState(GameStates.gamePlay);
        }

    }
}