using System;
using UnityEngine;

public class Counter : MonoBehaviour
{
    /// <summary>
    /// Count Bricks currently
    /// can be used also for other counting events  
    /// </summary>
    public static int m_TotalBrick { get; set; } = 0;
    public static event Action LevelFinished;



    private void OnEnable()
    {
        // subscribe to bricks destruction event for couting the Bricks 

        Brick.BrickDestroyed += BrickCounter;

    }

    private void OnDisable()
    {
        //unsubscribe to event
        Brick.BrickDestroyed -= BrickCounter;

    }

    // track the Bricks number and invoke the event of LevelFinished when the all bricks has been destoryed
    void BrickCounter(int point)
    {
       // Debug.Log($"pointValue : {point}");
        m_TotalBrick--;
       // Debug.Log($"Total Brick: {m_TotalBrick}");
        if (m_TotalBrick == 0)
        {
            LevelFinished?.Invoke();
        }
    }


}
