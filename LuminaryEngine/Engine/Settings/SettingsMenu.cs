using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Fonts;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Settings;

public class SettingsMenu : UIComponent
{
    private ScrollableMenuWithBackdrop _scrollableMenu;
    private Dictionary<string, UIComponent> _categoryMenus;
    private UIComponent _currentMenu;

    public SettingsMenu(int x, int y, int width, int height, int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        // Define categories
        var categories = new List<string>
            { "Keybindings", 
                "Audio", 
                "Graphics", 
                //"Gameplay", 
                //"Network", 
                //"Accessibility" 
            };

        // Create the scrollable menu
        _scrollableMenu = new ScrollableMenuWithBackdrop(x, y, width, 200, categories, 5, zIndex); // 5 visible items

        // Map categories to their respective menus
        _categoryMenus = new Dictionary<string, UIComponent>
        {
            { "Keybindings", new KeybindSettingsMenu(x, y + 220, width, height - 220) },
            { "Audio", new AudioSettingsMenu(x, y + 220, width, height - 220) },
            { "Graphics", new GraphicsSettingsMenu(x, y + 220, width, height - 220) },
            //{ "Gameplay", new GameplaySettingsMenu(x, y + 220, width, height - 220) },
            //{ "Network", new NetworkSettingsMenu(x, y + 220, width, height - 220) },
            //{ "Accessibility", new AccessibilitySettingsMenu(x, y + 220, width, height - 220) }
        };

        // Set the initial menu
        _currentMenu = _categoryMenus["Keybindings"];
    }

    public override void Render(Renderer renderer)
    {
        // Render the scrollable menu for categories
        _scrollableMenu.Render(renderer);

        // Render the currently selected sub-menu
        _currentMenu.Render(renderer);
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        // Handle input for the scrollable menu
        _scrollableMenu.HandleEvent(sdlEvent);

        // Check if the user selects a new category
        var selectedCategory = _scrollableMenu.GetSelectedOption();
        if (_categoryMenus.TryGetValue(selectedCategory, out var newMenu))
        {
            _currentMenu = newMenu;
        }

        // Handle input for the currently selected sub-menu
        _currentMenu.HandleEvent(sdlEvent);
    }
}