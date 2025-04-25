namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class Recipe
{
    public string RecipeID { get; set; }
    public string ResultItemID { get; set; }
    public Dictionary<string, int> RequiredSpiritEssences { get; set; } // EssenceID : Quantity
    public Dictionary<string, int> RequiredItems { get; set; }          // ItemID    : Quantity
    public string CraftingStationTag { get; set; } // Tag to identify compatible crafting stations
}