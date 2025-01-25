using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages UI buttons for the Debug Log Manager
/// </summary>
public class DebugLogUI : MonoBehaviour
{
    // Reference to UI button that triggers log saving
    [SerializeField]
    private Button saveLogsButton;

    /// <summary>
    /// Sets up button click listener on startup
    /// </summary>
    void Start()
    {
        if (saveLogsButton != null)
        {
            saveLogsButton.onClick.AddListener(OnSaveLogsClicked);
        }
    }
    /// <summary>
    /// Triggered when save button is clicked
    /// Delegates actual save operation to DebugLogManager
    /// </summary>
    private void OnSaveLogsClicked()
    {
        DebugLogManager.Instance.SaveLogs();
    }

    /// <summary>
    /// Cleanup: removes button click listener to prevent memory leaks
    /// </summary>

    void OnDestroy()
    {
        if (saveLogsButton != null)
        {
            saveLogsButton.onClick.RemoveListener(OnSaveLogsClicked);
        }
    }
}