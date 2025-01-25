using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{

    private void OnCollisionEnter(Collision other)
    {
        Destroy(other.gameObject);
        Cursor.visible = true;
        // change the state to GameOver after the collision of the ball wth DeathZone
        GameManager.instance.UpdateGameState(GameStates.gameOver);

    }
}
