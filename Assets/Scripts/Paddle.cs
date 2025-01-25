using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Controls the player's paddle movement and boundaries in the game for it's horizontal movement
/// </summary>
public class Paddle : MonoBehaviour
{

    // Movement speed of the paddle
    [SerializeField]  private float Speed = 2.0f;

    // Maximum horizontal distance the paddle can move from center (in world units)
    [SerializeField] private float MaxMovement = 1.9f;

    // Processes paddle movement input every frame
    private void Update()
    {
        HandlePlayerControl();
    }

    /// <summary>
    /// Handles horizontal paddle movement and enforces movement boundaries
    /// </summary>
    private void HandlePlayerControl()
    {
        // Get horizontal input (-1 to 1 range)
        float input = Input.GetAxis("Horizontal");

        // Calculate new position based on input
        Vector3 pos = transform.position;
        pos.x += input * Speed * Time.deltaTime;

        // Clamp position within maximum movement bounds
        pos.x = Mathf.Clamp(pos.x, -MaxMovement, MaxMovement);

        // Update paddle position
        transform.position = pos;
    }
}
