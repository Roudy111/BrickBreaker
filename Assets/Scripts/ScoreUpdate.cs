using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUpdate : MonoBehaviour
{
    /// <summary>
    /// Subscribe to the event of the destruction of each Brick to update score in realtime for each seassion
    /// </summary>

    void OnEnable()
    {
        //subscribe the method to event
        Brick.BrickDestroyed += UpdateScore;

    }

    void OnDisable()
    {
        // Unsubscribe to event
        Brick.BrickDestroyed -= UpdateScore;

    }

    //Method for updating score. Update score sent the value point of destructed brick to Addpoints
    //Addpoints update the currentScore
    void UpdateScore(int pointValue)
    {
        ScoreManager.Instance.AddPoints(pointValue);
       // Debug.Log($"Brick destroyed. Points added: {pointValue}");
    }


}

