using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class Recipe
{
    [JsonProperty("recipeId")]
    public string RecipeID { get; set; }
    [JsonProperty("result")]
    public RecipeResult Result { get; set; }
    [JsonProperty("requiredSpiritEssences")]
    public Dictionary<string, int> RequiredSpiritEssences { get; set; } // EssenceID : Quantity
    [JsonProperty("requiredItems")]
    public Dictionary<string, int> RequiredItems { get; set; } // ItemID    : Quantity
    [JsonProperty("craftingStationTag")]
    public string CraftingStationTag { get; set; } // Tag to identify compatible crafting stations
}

public class RecipeResult
{
    [JsonProperty("itemId")]
    public string ResultItemID { get; set; }
    [JsonProperty("count")]
    public int Count { get; set; }
    [JsonProperty("isSpiritEssence")]
    public bool IsSpiritEssence { get; set; }
}