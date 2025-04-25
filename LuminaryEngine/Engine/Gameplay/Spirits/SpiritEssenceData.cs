namespace LuminaryEngine.Engine.Gameplay.Spirits;

public class SpiritEssenceData
{
    public string EssenceID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconResourcePath { get; set; }
    public SpiritType Type { get; set; } 
    public SpiritTier Tier { get; set; } // Optional
    public Dictionary<string, float> PropertyMultipliers { get; set; } // How much it affects stats
    // ... other essence properties
}

public enum SpiritType { Fire, Water, Earth, Air, Light, Shadow }
public enum SpiritTier { Basic, Greater, Ancient } // Optional