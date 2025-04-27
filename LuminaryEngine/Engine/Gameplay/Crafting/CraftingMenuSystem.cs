using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.Player;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class CraftingMenuSystem : MenuSystem
    {
        private ScrollableMenu _recipeList;
        private TextComponent _recipeDetails;
        private TextComponent _resultDetails;
        private ButtonComponent _craftButton;
        private CraftingSystem _craftingSystem;
        private InventoryComponent _inventory;
        private CraftingKnowledgeComponent _knowledge;
        private ResourceCache _resourceCache;
        private UISystem _uiSystem;

        private bool _isDetailsFocused = false;
        
        private string _currentCraftingStation = "DefaultStation"; // Default crafting station

        private int _x, _y, _width, _height, _zIndex;

        public CraftingMenuSystem(CraftingSystem craftingSystem, InventoryComponent inventory, CraftingKnowledgeComponent knowledge,
            ResourceCache resourceCache, UISystem uiSystem, int x, int y, int width, int height, int zIndex = int.MaxValue)
        {
            _craftingSystem = craftingSystem;
            _inventory = inventory;
            _resourceCache = resourceCache;
            _knowledge = knowledge;
            _uiSystem = uiSystem;

            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _zIndex = zIndex;

            InitializeComponents();
        }
        
        public override void Render(Renderer renderer)
        {
            if (_recipeList.HasSelectedMenuItem())
            {
                _isDetailsFocused = true;
                _recipeList.SetFocus(false);
            }
            else
            {
                _isDetailsFocused = false;
                _recipeList.SetFocus(true);
            }
            
            // Render the background rectangle
            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.ClearUI, // Custom command for solid color
                ClearR = 0, // Background color (R)
                ClearG = 0, // Background color (G)
                ClearB = 0, // Background color (B)
                ClearA = 255, // Transparency (A)
                DestRect = new SDL.SDL_Rect { x = _x, y = _y, w = _width, h = _height },
                ZOrder = _zIndex - 5 // Render behind other components
            });

            base.Render(renderer); // Render child components
        }

        private void InitializeComponents()
        {
            // Initialize the recipe list
            _recipeList = new ScrollableMenu(
                _x + 10, _y + 10, _width / 2 - 20, _height - 20,
                new List<string>(), 5, zIndex: _zIndex - 1, horizontal: false, interactive: true
            );
            _recipeList.SetFont(_resourceCache.GetFont("Pixel", 28));
            _recipeList.SetFocus(true);
            AddComponent(_recipeList);

            // Initialize recipe details
            _recipeDetails = new TextComponent(
                "Select a recipe to view details.",
                _resourceCache.GetFont("Pixel", 16),
                new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 },
                _x + _width / 2 + 10, _y + 10, _width / 2 - 20, _height - 80,
                zIndex: _zIndex - 1
            );
            AddComponent(_recipeDetails);

            // Initialize Craft button
            _craftButton = new ButtonComponent(
                "Craft (" + SDL.SDL_GetScancodeName(InputMappingSystem.Instance.GetKeyForAction(ActionType.Interact)!.Value) + ")", _resourceCache.GetFont("Pixel", 28),
                new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 },
                new SDL.SDL_Color { r = 0, g = 128, b = 0, a = 255 },
                _x + _width / 2 + 10, _y + _height - 60, _width / 2 - 20, 50, _zIndex - 1
            );
            _craftButton.OnClick += CraftSelectedRecipe;
            AddComponent(_craftButton);

            _resultDetails = new TextComponent(" ",
                _resourceCache.GetFont("Pixel", 16),
                new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 },
                _x + _width / 2 + 10, _y + _height - 90, _width / 2 - 20, _height - 80,
                zIndex: _zIndex - 1);
            AddComponent(_resultDetails);

            RefreshRecipeList();
        }
        
        private void UpdateRecipeDetails()
        {
            if (_isDetailsFocused)
            {
                _recipeDetails.SetColor(new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }); // White color for details
                _craftButton.Undim();
            }
            else
            {
                _recipeDetails.SetColor(new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 150 }); // White color for details
                _craftButton.Dim();
            }
            
            if (!_recipeList.HasSelection()) return;

            string selectedRecipeID = _recipeList.GetSelectedOption();
            if (!_craftingSystem.TryGetRecipe(selectedRecipeID, out var recipe)) return;

            string details = $"Recipe ID: {recipe.RecipeID}\n" +
                             $"Result: {recipe.Result.ResultItemID} x{recipe.Result.Count}\n" +
                             $"Required Items:\n";

            foreach (var item in recipe.RequiredItems)
            {
                details += $"- {item.Key} x{item.Value}\n";
            }

            details += "Required Spirit Essences:\n";
            foreach (var essence in recipe.RequiredSpiritEssences)
            {
                details += $"- {essence.Key} x{essence.Value}\n";
            }

            _recipeDetails.SetText(details);
        }

        private void RefreshRecipeList()
        {
            _recipeList.Clear();
            var recipes = _craftingSystem.GetKnownRecipesForStation(_currentCraftingStation, _knowledge);
            foreach (var recipe in recipes)
            {
                _recipeList.AddOption(recipe.RecipeID);
            }
        }

        private void CraftSelectedRecipe()
        {
            if (!_recipeList.HasSelection()) return;

            string selectedRecipe = _recipeList.GetSelectedOption();
            if (_craftingSystem.CanCraft(selectedRecipe, _inventory))
            {
                _resultDetails.SetColor(new SDL.SDL_Color { r = 0, g = 255, b = 0, a = 255 }); // Red color for error messages
                _craftingSystem.Craft(selectedRecipe, _inventory);
                _resultDetails.SetText($"Successfully crafted {selectedRecipe}!");
            }
            else
            {
                _resultDetails.SetColor(new SDL.SDL_Color { r = 255, g = 0, b = 0, a = 255 }); // Red color for error messages
                _resultDetails.SetText($"Cannot craft {selectedRecipe}. Missing requirements.");
            }
        }

        public override void Activate()
        {
            base.Activate();
            
            _recipeList.SetFocus(true);
        }
        
        public override void Deactivate()
        {
            base.Deactivate();
            
            _recipeList.SetFocus(false);
        }

        public override void HandleEvent(SDL.SDL_Event sdlEvent)
        {
            if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                var triggeredAction = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                    { sdlEvent.key.keysym.scancode });

                if (triggeredAction.Contains(ActionType.Escape) && !_recipeList.HasSelectedMenuItem())
                {
                    _uiSystem.DeactivateMenu();
                }
            
                if (triggeredAction.Contains(ActionType.Escape) && _recipeList.HasSelectedMenuItem())
                {
                    _recipeList.FreeOption();
                }

                if (triggeredAction.Contains(ActionType.Interact) && _recipeList.HasSelectedMenuItem())
                {
                    _craftButton.OnClick?.Invoke();
                }

                if (!_recipeList.HasSelectedMenuItem())
                {
                    _recipeList.HandleEvent(sdlEvent);
                }
            }
            
            UpdateRecipeDetails();
        }
        
        public void SetStation(CraftingStation station)
        {
            _currentCraftingStation = station.StationTag;
            RefreshRecipeList();
        }
    }