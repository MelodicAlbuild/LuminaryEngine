using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using LuminaryEngine.Extras;
using LunimaryEngine.Engine.Configuration;

namespace LuminaryEngine.Engine.Gameplay.SaveLoad;

/// <summary>
/// Handles saving and loading of game data to/from encrypted files.
/// </summary>
public class SaveLoadSystem
{
    private readonly string _saveDirectory;
    private readonly string _encryptionPassword;

    public SaveLoadSystem(string saveDirectory = "Saves")
    {
        _saveDirectory = saveDirectory;
        _encryptionPassword = ConfigManager.GetConfigValue("EncryptionKey");

        // Ensure save directory exists
        if (!Directory.Exists(_saveDirectory))
        {
            Directory.CreateDirectory(_saveDirectory);
        }
    }

    /// <summary>
    /// Saves the game data to an encrypted file.
    /// </summary>
    /// <param name="saveData">The data to save.</param>
    /// <param name="fileName">The name of the save file.</param>
    public void SaveGame(SaveData saveData, string fileName)
    {
        string filePath = Path.Combine(_saveDirectory, fileName);

        try
        {
            // Serialize the save data to JSON
            var options = new JsonSerializerOptions { WriteIndented = false };
            string jsonData = JsonSerializer.Serialize(saveData, options);

            // Encrypt the JSON data
            string encryptedData = EncryptionUtils.Encrypt(jsonData, _encryptionPassword);

            // Write encrypted data to file
            File.WriteAllText(filePath, encryptedData);

            Console.WriteLine($"Game saved successfully to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save game: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the game data from an encrypted file.
    /// </summary>
    /// <param name="fileName">The name of the save file.</param>
    /// <returns>The loaded game data.</returns>
    public SaveData LoadGame(string fileName)
    {
        string filePath = Path.Combine(_saveDirectory, fileName);

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Save file not found: {filePath}");
            return null;
        }

        try
        {
            // Read encrypted data from file
            string encryptedData = File.ReadAllText(filePath);

            // Decrypt the data
            string jsonData = EncryptionUtils.Decrypt(encryptedData, _encryptionPassword);

            // Deserialize the JSON data to SaveData object
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            SaveData saveData = JsonSerializer.Deserialize<SaveData>(jsonData, options);

            return saveData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Deletes a save file.
    /// </summary>
    /// <param name="fileName">The name of the save file to delete.</param>
    public void DeleteSave(string fileName)
    {
        string filePath = Path.Combine(_saveDirectory, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine($"Save file deleted: {filePath}");
        }
        else
        {
            Console.WriteLine($"Save file not found: {filePath}");
        }
    }

    /// <summary>
    /// Checks if a save file exists.
    /// </summary>
    /// <param name="fileName">The name of the save file.</param>
    /// <returns>True if the save file exists; otherwise, false.</returns>
    public bool SaveExists(string fileName)
    {
        string filePath = Path.Combine(_saveDirectory, fileName);
        return File.Exists(filePath);
    }
}