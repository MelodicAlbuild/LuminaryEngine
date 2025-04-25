using LuminaryEngine.Engine.Gameplay.Player;
using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class CraftingSystem
{
    private Dictionary<string, Recipe> _recipes;

    public CraftingSystem()
    {
        _recipes = new Dictionary<string, Recipe>();
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        string filePath = "recipes.json";
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Recipe file not found at {filePath}");

        var jsonContent = File.ReadAllText(filePath);
        var recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonContent);

        foreach (var recipe in recipes)
        {
            if (!_recipes.ContainsKey(recipe.RecipeID))
                _recipes[recipe.RecipeID] = recipe;
        }
    }

    public bool CanCraft(string recipeID, InventoryComponent inventory)
    {
        if (!_recipes.ContainsKey(recipeID))
            return false;

        var recipe = _recipes[recipeID];

        // Check required items
        foreach (var item in recipe.RequiredItems)
        {
            if (!inventory.HasItem(item.Key, item.Value))
                return false;
        }

        // Check required spirit essences
        foreach (var essence in recipe.RequiredSpiritEssences)
        {
            if (!inventory.HasSpiritEssence(essence.Key, essence.Value))
                return false;
        }

        return true;
    }

    public bool Craft(string recipeID, InventoryComponent inventory)
    {
        if (!CanCraft(recipeID, inventory))
            return false;

        var recipe = _recipes[recipeID];

        // Remove required items
        foreach (var item in recipe.RequiredItems)
        {
            inventory.RemoveItem(item.Key, item.Value);
        }

        // Remove required spirit essences
        foreach (var essence in recipe.RequiredSpiritEssences)
        {
            inventory.RemoveSpiritEssence(essence.Key, essence.Value);
        }

        // Add the crafted item
        inventory.AddItem(recipe.ResultItemID, 1);

        return true;
    }

    public List<Recipe> GetKnownRecipesForStation(string craftingStationTag, CraftingKnowledgeComponent knowledge)
    {
        var knownRecipes = new List<Recipe>();

        foreach (var recipeID in knowledge.KnownRecipeIDs)
        {
            if (_recipes.ContainsKey(recipeID) &&
                _recipes[recipeID].CraftingStationTag == craftingStationTag)
            {
                knownRecipes.Add(_recipes[recipeID]);
            }
        }

        return knownRecipes;
    }
}