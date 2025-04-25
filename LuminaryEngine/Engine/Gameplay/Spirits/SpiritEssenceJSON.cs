using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Spirits;

public class SpiritEssenceJSON
{
    [JsonProperty("essenceId")]
    public string EssenceID { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("textureId")]
    public string TextureId { get; set; }
    [JsonProperty("spiritType")]
    public int Type { get; set; }
    [JsonProperty("spiritTier")]
    public int Tier { get; set; }

    [JsonProperty("spiritProperties")]
    public Dictionary<string, float> PropertyMultipliers { get; set; } // How much it affects stats
}