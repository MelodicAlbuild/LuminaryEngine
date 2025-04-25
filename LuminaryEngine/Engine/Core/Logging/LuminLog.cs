using System.Security.Cryptography;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Extras;
using LunimaryEngine.Engine.Configuration;

namespace LuminaryEngine.Engine.Core.Logging;

public static class LuminLog
{
    private static readonly string _tempLogFilePath = "logs_temp.lme"; // Temporary file during runtime
    private static readonly string _finalizedLogFilePath = "logs.lme"; // Finalized file after program close
    private static string _encryptionPassword; // Replace with a secure password

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }

    public static void Log(string message, LogLevel level = LogLevel.INFO)
    {
        if (level == LogLevel.DEBUG && !DevModeConfig.IsDebugEnabled)
        {
            // Skip debug logs if debug mode is disabled
            return;
        }

        string logMessage = FormatLogMessage(message, level);

        // Write the log message to the temporary log file
        File.AppendAllText(_tempLogFilePath, logMessage + Environment.NewLine);

        // Optionally, log debug messages to the console
        if (level == LogLevel.DEBUG)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[DEBUG]: {message}");
            Console.ResetColor();
        }
    }

    private static string FormatLogMessage(string message, LogLevel level)
    {
        // Custom proprietary format: [LEVEL]::[Timestamp]::[Message]
        return $"{level}::{DateTime.Now:yyyy-MM-dd HH:mm:ss}::{message}";
    }

    public static void FinalizeLog()
    {
        _encryptionPassword = ConfigManager.GetConfigValue("EncryptionKey");

        if (!File.Exists(_tempLogFilePath))
        {
            Console.WriteLine("No logs to finalize.");
            return;
        }

        try
        {
            // Read all log entries from the temporary file
            string plainTextLogs = File.ReadAllText(_tempLogFilePath);

            // Encrypt the logs and write to the encrypted log file
            string encryptedLogs = EncryptionUtils.Encrypt(plainTextLogs, _encryptionPassword);
            File.WriteAllText(_finalizedLogFilePath, encryptedLogs);

            // Delete the temporary log file
            File.Delete(_tempLogFilePath);

            Console.WriteLine("Logs have been finalized and saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while finalizing logs: {ex.Message}");
        }
    }

    public static void Debug(string message)
    {
        Log(message, LogLevel.DEBUG);
    }

    public static void Info(string message)
    {
        Log(message, LogLevel.INFO);
    }

    public static void Warning(string message)
    {
        Log(message, LogLevel.WARNING);
    }

    public static void Error(string message)
    {
        Log(message, LogLevel.ERROR);
    }
}