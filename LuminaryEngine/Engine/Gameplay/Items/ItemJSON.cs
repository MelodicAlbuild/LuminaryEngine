using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Items;

public class ItemJSON
{
    [JsonProperty("itemId")] public string ItemId { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("description")] public string Description { get; set; }
    [JsonProperty("textureId")] public string TextureId { get; set; }
    [JsonProperty("type")] public int Type { get; set; }
}