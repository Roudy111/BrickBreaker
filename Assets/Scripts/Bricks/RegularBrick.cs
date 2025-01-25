using UnityEngine;


/// <summary>
/// Basic brick implementation with standard destruction behavior.
/// 
/// Key features:
/// - Simple collision-based destruction
/// - Basic sound effect on hit
/// 
/// Design patterns:
/// - Inheritance: Uses base brick behavior with minimal customization
/// 
/// Note: Serves as the standard brick type for normal gameplay progression
/// </summary>

public class RegularBrick : Brick
{

    // Click sound effect for destruction
    [SerializeField] private AudioClip clickSFX;

    private void PlayRegularBrickSound()
    {
        if (clickSFX != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(clickSFX, transform.position);
        }
        else
        {
            Debug.LogError("Failed to play explosion sound. AudioClip or AudioSource is missing.");
        }
    }
    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        PlayRegularBrickSound();
    }

}