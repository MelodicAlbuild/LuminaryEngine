using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class CraftingKnowledgeComponent : IComponent
{
    // A set of known recipe IDs for the player.
    public HashSet<string> KnownRecipeIDs { get; private set; }

    public CraftingKnowledgeComponent()
    {
        KnownRecipeIDs = new HashSet<string>();
    }

    // Adds a recipe to the known recipes
    public bool LearnRecipe(string recipeID)
    {
        return KnownRecipeIDs.Add(recipeID);
    }

    // Checks if a recipe is already known
    public bool KnowsRecipe(string recipeID)
    {
        return KnownRecipeIDs.Contains(recipeID);
    }

    // Removes a recipe from the known recipes
    public bool ForgetRecipe(string recipeID)
    {
        return KnownRecipeIDs.Remove(recipeID);
    }
}