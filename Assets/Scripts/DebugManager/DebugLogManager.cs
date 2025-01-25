using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Core debug logging system that captures, manages, and exports Unity debug logs.
/// Implements Singleton pattern for global access to logging functionality.
/// 
/// Key features:
/// - Automatic log capture with timestamp and type
/// - Stack trace preservation for error investigation
/// - File-based log export with session information
/// - Memory management through log entry limiting
/// - Cross-platform file system integration
/// 
/// Design patterns:
/// - Singleton: Ensures single logging instance
/// - Observer: Subscribes to Unity's logging system
/// - Builder: Uses StringBuilder for efficient log construction
/// 
/// Usage:
/// - Automatically captures all Debug.Log calls
/// - Provides SaveLogs() method for exporting to file
/// - Manages log rotation to prevent memory issues
/// 
/// Performance considerations:
/// - Limits maximum log entries to prevent memory overflow
/// - Uses StringBuilder for efficient string concatenation
/// - Implements proper cleanup on destruction
/// </summary>
public class DebugLogManager : singleton<DebugLogManager>
{
    // Efficient string building for log assembly
    private StringBuilder logBuilder;
    // Storage for captured log entries
    private List<LogEntry> logEntries;
    // Target path for log file export
    private string logFilePath;
    // Prevent memory issues with too many logs
    private const int MAX_LOGS = 1000;


    /// <summary>
    /// Represents a single log entry with complete debug information
    /// Serializable for potential data persistence
    /// </summary>
    [Serializable]
    private struct LogEntry
    {
        public string timestamp;
        public string type;
        public string message;
        public string stackTrace;
    }

    public override void Awake()
    {
        base.Awake();
        InitializeLogger();
    }

    /// <summary>
    /// Initializes logging system and sets up file path -- refactored and need to be putet in Awake or OnEnable
    /// </summary>
    private void InitializeLogger()
    {
        logBuilder = new StringBuilder();
        logEntries = new List<LogEntry>();
        
        // Get desktop path and create a timestamped filename
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        logFilePath = Path.Combine(desktopPath, $"unity_debug_logs_{timestamp}.txt");
        
        // Subscribe to log messages
        Application.logMessageReceived += HandleLog;
        
        Debug.Log($"DebugLogManager initialized. Logs will be saved to: {logFilePath}");
    }
    /// <summary>
    /// Processes incoming log messages and manages log storage
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Create new log entry
        LogEntry entry = new LogEntry
        {
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            type = type.ToString(),
            message = logString,
            stackTrace = stackTrace
        };

        // Add to collection with overflow protection
        logEntries.Add(entry);

        // Maintain max log count
        if (logEntries.Count > MAX_LOGS)
        {
            logEntries.RemoveAt(0);
        }
    }

    /// <summary>
    /// Saves all collected logs to a file
    /// </summary>
    public void SaveLogs()
    {
        try
        {
            logBuilder.Clear();
            
            // Add session info at the top of the file
            logBuilder.AppendLine($"Unity Debug Logs - Session: {DateTime.Now}");
            logBuilder.AppendLine($"Project: {Application.productName}");
            logBuilder.AppendLine($"Version: {Application.version}");
            logBuilder.AppendLine("----------------------------------------");
            logBuilder.AppendLine();

            // Build the log content
            foreach (var entry in logEntries)
            {
                logBuilder.AppendLine($"[{entry.timestamp}] [{entry.type}] {entry.message}");
                if (!string.IsNullOrEmpty(entry.stackTrace))
                {
                    logBuilder.AppendLine("Stack Trace:");
                    logBuilder.AppendLine(entry.stackTrace);
                }
                logBuilder.AppendLine("----------------------------------------");
            }

            // Write to file
            File.WriteAllText(logFilePath, logBuilder.ToString());
            Debug.Log($"Logs saved successfully to: {logFilePath}");

            // Open the containing folder in explorer (Windows) or finder (macOS)
            #if UNITY_EDITOR_WIN
                System.Diagnostics.Process.Start("explorer.exe", "/select," + logFilePath);
            #elif UNITY_EDITOR_OSX
                System.Diagnostics.Process.Start("open", "-R " + logFilePath);
            #endif
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save logs: {e.Message}");
        }
    }

    /// <summary>
    /// Clears all stored logs
    /// </summary>
    public void ClearLogs()
    {
        logEntries.Clear();
        logBuilder.Clear();
        Debug.Log("Logs cleared successfully.");
    }

    private void OnDestroy()
    {
        // Unsubscribe from log messages
        Application.logMessageReceived -= HandleLog;
    }
}