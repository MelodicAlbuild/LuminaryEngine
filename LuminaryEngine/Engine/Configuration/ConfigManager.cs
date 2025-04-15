using Newtonsoft.Json.Linq;

namespace LunimaryEngine.Engine.Configuration;

public static class ConfigManager
{
    private static readonly string ConfigFilePath = "Assets/Config/appsettings.json";

    /// <summary>
    /// Retrieves a value from the configuration file.
    /// </summary>
    public static string GetConfigValue(string key)
    {
        if (!File.Exists(ConfigFilePath))
        {
            throw new FileNotFoundException("Configuration file not found.");
        }

        var json = File.ReadAllText(ConfigFilePath);
        var config = JObject.Parse(json);

        return config[key]?.ToString() ?? throw new KeyNotFoundException($"Key '{key}' not found in configuration.");
    }
}