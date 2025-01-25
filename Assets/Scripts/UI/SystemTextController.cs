using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Controls and manages the system details text display independently in the terminal interface.
/// Manages its own sequence timing and updates.
/// </summary>
public class SystemTextController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI systemDetailsText;

    [Header("Text Settings")]
    [SerializeField] private Color textColor = Color.green;
    [SerializeField] private float fontSize = 24f;
    [SerializeField] private float typingSpeed = 0.025f;
    [SerializeField] private float textAlpha = 0.5f;
    
    [Header("Sequence Settings")]
    [SerializeField] private float updateInterval = 5f;
    [SerializeField] private bool autoUpdate = true;
    [SerializeField] private int maxSequences = 10;

    private Coroutine activeTypeCoroutine;
    private Coroutine sequenceCoroutine;
    private int currentSequence = 1;

    private void OnEnable()
    {
        InitializeSystemText();
        if (autoUpdate)
        {
            StartSequenceUpdates();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Sets up initial text component properties
    /// </summary>
    private void InitializeSystemText()
    {
        if (systemDetailsText == null)
        {
            Debug.LogError("SystemDetailsText component not assigned!");
            return;
        }

        ConfigureTextProperties();
        UpdateSystemDetails(currentSequence);
    }

    /// <summary>
    /// Configures the visual properties of the system text
    /// </summary>
    private void ConfigureTextProperties()
    {
        systemDetailsText.color = textColor;
        systemDetailsText.fontSize = fontSize;
        systemDetailsText.alignment = TextAlignmentOptions.Left;
        systemDetailsText.alpha = textAlpha;
        systemDetailsText.text = string.Empty;
    }

    /// <summary>
    /// Starts the automatic sequence update cycle
    /// </summary>
    public void StartSequenceUpdates()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
        }
        sequenceCoroutine = StartCoroutine(SequenceUpdateRoutine());
    }

    /// <summary>
    /// Handles the automatic sequence update cycle
    /// </summary>
    private IEnumerator SequenceUpdateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            
            currentSequence++;
            if (currentSequence > maxSequences)
            {
                currentSequence = 1;
            }
            
            UpdateSystemDetails(currentSequence);
        }
    }

    /// <summary>
    /// Updates the system details text with the given sequence number
    /// </summary>
    private void UpdateSystemDetails(int sequenceNumber)
    {
        string details = GenerateSystemDetails(sequenceNumber);
        
        if (activeTypeCoroutine != null)
        {
            StopCoroutine(activeTypeCoroutine);
        }
        
        activeTypeCoroutine = StartCoroutine(TypeSystemDetails(details));
    }

    /// <summary>
    /// Generates the system details text with random memory allocation
    /// </summary>
    private string GenerateSystemDetails(int sequenceNumber)
    {
        return $"> SYSTEM://init_sequence_{sequenceNumber}\n" +
               $"> MEMORY://allocated_0x{GenerateRandomMemoryAddress()}";
    }

    /// <summary>
    /// Generates a random hexadecimal memory address
    /// </summary>
    private string GenerateRandomMemoryAddress()
    {
        return Random.Range(0, 0xFFFFFF).ToString("X6");
    }

    /// <summary>
    /// Types out the system details text character by character
    /// </summary>
    private IEnumerator TypeSystemDetails(string text)
    {
        string currentText = string.Empty;
        
        foreach (char c in text)
        {
            currentText += c;
            systemDetailsText.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    /// <summary>
    /// Manually triggers the next sequence update
    /// </summary>
    public void TriggerNextSequence()
    {
        currentSequence++;
        if (currentSequence > maxSequences)
        {
            currentSequence = 1;
        }
        UpdateSystemDetails(currentSequence);
    }

    /// <summary>
    /// Stops all running coroutines and clears the text
    /// </summary>
    public void StopAll()
    {
        StopAllCoroutines();
        systemDetailsText.text = string.Empty;
    }

    /// <summary>
    /// Resets the sequence to the beginning
    /// </summary>
    public void ResetSequence()
    {
        currentSequence = 1;
        UpdateSystemDetails(currentSequence);
    }
}