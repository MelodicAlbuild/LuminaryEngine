using LuminaryEngine.Engine.Gameplay.Player;
using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class CraftingSystem
{
    public static CraftingSystem Instance;
    
    private readonly Dictionary<string, Recipe> _recipes = new();

    public CraftingSystem()
    {
        Instance = this;
        LoadRecipes();
    }

    private void LoadRecipes()
    {
        var path = Path.Combine("Assets", "Recipes", "recipes.json");
        
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Items JSON file not found", path);
        }

        var jsonContent = File.ReadAllText(path);
        var recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonContent);

        if (recipes == null) return;

        foreach (var recipe in recipes)
        {
            _recipes.TryAdd(recipe.RecipeID, recipe);
        }
    }

    public bool TryGetRecipe(string recipeID, out Recipe recipe)
    {
        return _recipes.TryGetValue(recipeID, out recipe);
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
        if (recipe.Result.IsSpiritEssence)
        {
            inventory.AddSpiritEssence(recipe.Result.ResultItemID, recipe.Result.Count);
        }
        else
        {
            inventory.AddItem(recipe.Result.ResultItemID, recipe.Result.Count);
        }

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