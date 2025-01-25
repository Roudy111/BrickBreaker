using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages the complete dialogue system including boot sequence, operator dialogue,
/// user choices, name input, and transitions to menu.
/// 
/// Key responsibilities:
/// - Manages dialogue flow and typing effects
/// - Handles user input and choices
/// - Controls UI element visibility and transitions
/// - Manages background scaling and animations
/// - Coordinates with DataManager for player name storage
/// 
/// Required scene setup:
/// - TextMeshPro components for various text displays
/// - UI buttons for choices
/// - Background image for transition
/// - Name input field
/// - Menu UI elements
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    #region Serializable Classes
    [System.Serializable]
    private class DialogueMessage
    {
        public string text;
        public Color color = Color.green;
        public bool isVisible = false;
        public int visibleCharacters = 0;
    }
    #endregion

    #region Serialized Fields
    [Header("Core UI References")]
    [SerializeField] private TextMeshProUGUI bootMessagesText;
    [SerializeField] private TextMeshProUGUI operatorText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private GameObject menuUI;
    [SerializeField] private GameObject systemText;

    [Header("Name Input Sequence")]
    [SerializeField] private GameObject backgroundImage;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TextMeshProUGUI namePromptText;
    [SerializeField] private string namePromptMessage = "Tell me, what name anchors you to this world?";
    [SerializeField] private float backgroundScaleFactor = 1.34f;
    [SerializeField] private float scaleAnimationDuration = 1f;

    [Header("Visual Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private Color defaultTextColor = Color.green;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float messageDelay = 0.5f;
    [SerializeField] private float responseDelay = 2f;

    [Header("Dialogue Content")]
    [SerializeField] private DialogueMessage[] bootMessages = new DialogueMessage[]
    {
        new DialogueMessage { text = "> SYSTEM BOOT..." },
        new DialogueMessage { text = "> NEURAL LINK ESTABLISHED..." },
        new DialogueMessage { text = "> SCANNING FOR NEURAL IMPRINTS..." },
        new DialogueMessage { text = "> WARNING: FRAGMENTED MEMORY DETECTED" }
    };

    [Header("Dialogue Choices")]
    [SerializeField] private string operatorMessage = "> OPERATOR: You was the Last one who sees the patterns. Are you still with us?";
    [SerializeField] private string yesResponse = "> OPERATOR: The blocks contain what they stole. Break them.";
    [SerializeField] private string noResponse = "> OPERATOR: The neural interface never lies. Let it wake your memories.";
    #endregion

    #region Private Fields
    private bool isTyping = false;
    private const string CURSOR = "<color=#00ff00>_</color>";
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        ValidateComponents();
        SetupUI();
        StartCoroutine(PlaySequence());
        Cursor.visible = false;
    }
    #endregion

    #region Initialization Methods
    /// <summary>
    /// Validates that all required components are properly assigned
    /// </summary>
    private void ValidateComponents()
    {
        if (bootMessagesText == null) Debug.LogError("Boot Messages Text is not assigned!");
        if (operatorText == null) Debug.LogError("Operator Text is not assigned!");
        if (yesButton == null) Debug.LogError("Yes Button is not assigned!");
        if (noButton == null) Debug.LogError("No Button is not assigned!");
        if (menuUI == null) Debug.LogError("Menu UI is not assigned!");
        if (backgroundImage == null) Debug.LogError("Background Image is not assigned!");
        if (nameInputField == null) Debug.LogError("Name Input Field is not assigned!");
        if (namePromptText == null) Debug.LogError("Name Prompt Text is not assigned!");
    }

    /// <summary>
    /// Initializes UI elements and sets up event listeners
    /// </summary>
    private void SetupUI()
    {
        // Initialize text components
        bootMessagesText.color = defaultTextColor;
        bootMessagesText.text = "";
        operatorText.color = defaultTextColor;
        operatorText.text = "";
        namePromptText.color = defaultTextColor;
        namePromptText.text = "";

        // Setup button listeners
        yesButton.onClick.AddListener(() => HandleChoice(yesResponse));
        noButton.onClick.AddListener(() => HandleChoice(noResponse));
        
        // Setup name input
        nameInputField.onEndEdit.AddListener(HandleNameInput);

        // Hide all UI elements initially
        SetInitialUIState();
    }

    /// <summary>
    /// Sets the initial visibility state of all UI elements
    /// </summary>
    private void SetInitialUIState()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        backgroundImage.SetActive(false);
        nameInputField.gameObject.SetActive(false);
        namePromptText.gameObject.SetActive(false);
        menuUI.SetActive(false);
    }
    #endregion

    #region Dialogue Sequence Methods
    /// <summary>
    /// Controls the main dialogue sequence flow
    /// </summary>
    private IEnumerator PlaySequence()
    {
        // Play boot sequence
        for (int i = 0; i < bootMessages.Length; i++)
        {
            if (bootMessages[i].text.Contains("WARNING"))
            {
                bootMessages[i].color = warningColor;
            }
            yield return StartCoroutine(TypeMessage(bootMessages[i], true));
            yield return new WaitForSeconds(messageDelay);
        }

        // Show operator message and choices
        yield return new WaitForSeconds(messageDelay * 2);
        yield return StartCoroutine(TypeOperatorMessage(operatorMessage));
        
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        Cursor.visible = true;

    }

    /// <summary>
    /// Types out a message character by character
    /// </summary>
      private IEnumerator TypeMessage(DialogueMessage message, bool isBootMessage)
{
    isTyping = true;
    message.isVisible = true;
    message.visibleCharacters = 0;
    
    bool isWarningMessage = message.text.Contains("WARNING");
    if (isWarningMessage)
    {
        AudioManager.Instance.PlayWarningSound();
    }
    
    while (message.visibleCharacters < message.text.Length)
    {
        message.visibleCharacters++;
        UpdateDisplay(isBootMessage);
        
        // Play typing sounds based on message type
        if (message.visibleCharacters < message.text.Length && 
            !char.IsWhiteSpace(message.text[message.visibleCharacters - 1]))
        {
            if (!isWarningMessage)
            {
                AudioManager.Instance.PlayBootTypingSound();
            }
        }
        
        yield return new WaitForSeconds(typingSpeed);
    }

    isTyping = false;
    AudioManager.Instance.bootTypingSource.Stop();
}

    /// <summary>
    /// Updates the text display with proper formatting and cursor
    /// </summary>
    private void UpdateDisplay(bool isBootMessage)
    {
        if (isBootMessage)
        {
            string fullText = "";
            for (int i = 0; i < bootMessages.Length; i++)
            {
                if (bootMessages[i].isVisible)
                {
                    string visibleText = bootMessages[i].text.Substring(0, bootMessages[i].visibleCharacters);
                    fullText += $"<color=#{ColorUtility.ToHtmlStringRGB(bootMessages[i].color)}>{visibleText}</color>\n";
                }
            }
            bootMessagesText.text = fullText.TrimEnd('\n') + CURSOR;
        }
    }

    /// <summary>
    /// Types out the operator's message with cursor effect
    /// </summary>
    /// <summary>
/// Types out the operator's message with cursor effect and operator voice sound
/// </summary>
private IEnumerator TypeOperatorMessage(string message)
{
    string currentText = "";
    foreach (char c in message)
    {
        currentText += c;
        operatorText.text = currentText + CURSOR;
        
        // Play operator typing sound for non-space characters
        if (!char.IsWhiteSpace(c))
        {
            AudioManager.Instance.PlayOperatorTypingSound();
        }
        
        yield return new WaitForSeconds(typingSpeed);
    }
    
}
/// <summary>
/// Types out the name prompt with operator sound effects
/// </summary>
private IEnumerator TypeNamePrompt()
{
    string currentText = "";
    foreach (char c in namePromptMessage)
    {
        currentText += c;
        namePromptText.text = currentText + CURSOR;
        
        // Use operator typing sound for name prompt
        if (!char.IsWhiteSpace(c))
        {
            AudioManager.Instance.PlayOperatorTypingSound();
        }
        
        yield return new WaitForSeconds(typingSpeed);
    }
}

    #endregion

    #region User Input Handling
    /// <summary>
    /// Handles the player's dialogue choice selection
    /// </summary>
    private void HandleChoice(string response)
    {
        AudioManager.Instance.PlayResponseClickSound();
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
        StartCoroutine(ShowResponse(response));
    }

    /// <summary>
    /// Shows the operator's response and transitions to name input
    /// </summary>
    private IEnumerator ShowResponse(string response)
    {
        yield return StartCoroutine(TypeOperatorMessage(response));
        yield return new WaitForSeconds(responseDelay);
        if (systemText != null)
        {
        systemText.SetActive(false);
        }
        StartCoroutine(StartNameInputSequence());
    }

    /// <summary>
    /// Handles the submitted player name
    /// </summary>
    private void HandleNameInput(string inputName)
    {
        if (!string.IsNullOrEmpty(inputName))
        {
            if (DataManager.Instance != null)
            {
                DataManager.Instance.currentPlayerId = inputName;
                StartCoroutine(CompleteNameSequence());
            }
            else
            {
                Debug.LogError("DataManager instance not found!");
            }
        }
    }
    #endregion

    #region Transition Sequences
    /// <summary>
    /// Initiates the name input sequence with background
    /// </summary>
    private IEnumerator StartNameInputSequence()
    {
        // Fade out previous dialogue
        yield return StartCoroutine(FadeOutPreviousDialogue());

        // Show background and name prompt
        backgroundImage.SetActive(true);
        namePromptText.gameObject.SetActive(true);

        // Use the dedicated TypeNamePrompt method for consistent audio behavior
        // Reset text and make sure it's visible
        namePromptText.text = "";
        namePromptText.alpha = 1f;
        operatorText.text = "";
        yield return StartCoroutine(TypeNamePrompt());

        

        // Show input field
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayBackgroundMusic();
        nameInputField.gameObject.SetActive(true);

    }

    /// <summary>
    /// Fades out the previous dialogue text
    /// </summary>
    private IEnumerator FadeOutPreviousDialogue()
    {
        float bootAlpha = bootMessagesText.alpha;
        float operatorAlpha = operatorText.alpha;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / fadeOutDuration;

            bootMessagesText.alpha = Mathf.Lerp(bootAlpha, 0f, normalizedTime);
            operatorText.alpha = Mathf.Lerp(operatorAlpha, 0f, normalizedTime);

            yield return null;
        }

        bootMessagesText.text = "";
        operatorText.text = "";
    }

    /// <summary>
    /// Completes the name sequence with background scaling and menu transition
    /// </summary>
    private IEnumerator CompleteNameSequence()
    {
        // Disable input elements
        nameInputField.gameObject.SetActive(false);
        namePromptText.gameObject.SetActive(false);

        // Scale background
        Vector3 initialScale = backgroundImage.transform.localScale;
        // Set absolute target scale instead of multiplying
        Vector3 targetScale = Vector3.one * 1.34f;
        float elapsed = 0f;

        while (elapsed < scaleAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / scaleAnimationDuration;
            backgroundImage.transform.localScale = Vector3.Lerp(initialScale, targetScale, normalizedTime);
            yield return null;
        }

        backgroundImage.transform.localScale = targetScale;

        // Show menu and play music
        menuUI.SetActive(true);

        // Disable dialogue system
        gameObject.SetActive(false);
    }
    #endregion
}