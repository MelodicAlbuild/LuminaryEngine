using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Fonts;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Settings;

public class SettingsMenu : UIComponent
{
    private ScrollableMenu _scrollableMenu;
    private Dictionary<string, UIComponent> _categoryMenus;
    private UIComponent _currentMenu;

    private int _width, _height;

    public SettingsMenu(int x, int y, int width, int height, int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        _height = height;
        _width = width;

        // Define categories
        var categories = new List<string>
        {
            "Keybindings",
            "Audio",
            "Graphics",
            "Gameplay",
            "Network",
            "Accessibility"
        };

        // Create the scrollable menu
        _scrollableMenu = new ScrollableMenu(x, y, width, 150, categories, 5, zIndex, true, true); // 5 visible items

        // Map categories to their respective menus
        _categoryMenus = new Dictionary<string, UIComponent>
        {
            { "Keybindings", new KeybindSettingsMenu(x + 5, y + 40, width, height - 120) },
            { "Audio", new AudioSettingsMenu(x + 5, y + 40, width, height - 120) },
            { "Graphics", new GraphicsSettingsMenu(x + 5, y + 40, width, height - 120) },
            //{ "Gameplay", new GameplaySettingsMenu(x, y + 220, width, height - 220) },
            //{ "Network", new NetworkSettingsMenu(x, y + 220, width, height - 220) },
            //{ "Accessibility", new AccessibilitySettingsMenu(x, y + 220, width, height - 220) }
        };

        // Set the initial menu
        _currentMenu = _categoryMenus["Keybindings"];
    }

    public override void Render(Renderer renderer)
    {
        // Render backdrop
        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawRectangle,
            RectColor = new SDL.SDL_Color() { r = 0, g = 0, b = 0, a = 255 }, // Semi-transparent black
            DestRect = new SDL.SDL_Rect { x = X, y = Y, w = _width, h = _height },
            Filled = true,
            ZOrder = ZIndex - 1 // Ensure backdrop is behind menu items
        });

        // Render the scrollable menu for categories
        _scrollableMenu.Render(renderer);

        // Render the currently selected sub-menu
        _currentMenu.Render(renderer);
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        // Handle input for the currently selected sub-menu
        if (_scrollableMenu.HasSelectedMenuItem())
        {
            _currentMenu.SetFocus(true);
            _scrollableMenu.SetFocus(false);

            if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
            {
                if (sdlEvent.key.keysym.sym == SDL.SDL_Keycode.SDLK_ESCAPE)
                {
                    _scrollableMenu.FreeOption();
                }
            }

            _currentMenu.HandleEvent(sdlEvent);
        }
        else
        {
            _currentMenu.SetFocus(false);
            _scrollableMenu.SetFocus(true);
        }

        // Handle input for the scrollable menu
        _scrollableMenu.HandleEvent(sdlEvent);

        // Check if the user selects a new category
        var selectedCategory = _scrollableMenu.GetSelectedOption();
        if (_categoryMenus.TryGetValue(selectedCategory, out var newMenu))
        {
            _currentMenu = newMenu;
        }
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
    }
}