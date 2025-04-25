using LuminaryEngine.Engine.Gameplay.Player;
using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class CraftingSystem
{
    private readonly Dictionary<string, Recipe> _recipes = new();

    public CraftingSystem()
    {
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        const string filePath = "recipes.json";
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Recipe file not found at {filePath}");
        }

        var jsonContent = File.ReadAllText(filePath);
        var recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonContent);

        if (recipes == null) return;

        foreach (var recipe in recipes)
        {
            _recipes.TryAdd(recipe.RecipeID, recipe);
        }
    }

    public bool CanCraft(string recipeID, InventoryComponent inventory)
    {
        if (!_recipes.TryGetValue(recipeID, out var recipe))
        {
            return false;
        }

        // Check required items
        if (recipe.RequiredItems.Any(item => !inventory.HasItem(item.Key, item.Value)))
        {
            return false;
        }

        // Check required spirit essences
        return recipe.RequiredSpiritEssences.All(essence => inventory.HasSpiritEssence(essence.Key, essence.Value));
    }

    public bool Craft(string recipeID, InventoryComponent inventory)
    {
        if (!CanCraft(recipeID, inventory))
        {
            return false;
        }

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
        return knowledge.KnownRecipeIDs
            .Where(recipeID => _recipes.TryGetValue(recipeID, out var recipe) &&
                               recipe.CraftingStationTag == craftingStationTag)
            .Select(recipeID => _recipes[recipeID])
            .ToList();
    }
}