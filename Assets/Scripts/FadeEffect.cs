using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles fade in/out transitions that can be called when needed.
/// Requires a Canvas with a CanvasGroup for fading.
/// </summary>
public class FadeEffect : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    /// <summary>
    /// Fades the screen to black
    /// </summary>
    public void FadeOut()
    {
        StartCoroutine(PerformFade(0f, 1f));
    }

    /// <summary>
    /// Fades from black to clear
    /// </summary>
    public void FadeIn()
    {
        StartCoroutine(PerformFade(1f, 0f));
    }

    private IEnumerator PerformFade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        
        fadeCanvasGroup.alpha = endAlpha;
    }
}