namespace LuminaryEngine.Engine.Gameplay.Spirits;

public class SpiritEssence
{
    public string EssenceID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string TextureId { get; set; }
    public SpiritType Type { get; set; }
    public SpiritTier Tier { get; set; } // Optional

    public Dictionary<string, float> PropertyMultipliers { get; set; } // How much it affects stats
}