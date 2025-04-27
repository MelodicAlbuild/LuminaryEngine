using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class UISystem
{
    private Dictionary<string, HUDSystem> _hudSystems = new();
    private Dictionary<string, MenuSystem> _menuSystems = new();
    private List<UIComponent> _uiComponents = new();

    private string _activeHUD = null;
    private string _activeMenu = null;

    // Adds a new HUD system with a unique identifier
    public void RegisterHUD(string id, HUDSystem hudSystem)
    {
        if (!_hudSystems.ContainsKey(id))
        {
            _hudSystems[id] = hudSystem;
        }
    }

    // Removes an existing HUD system by its identifier
    public void UnregisterHUD(string id)
    {
        if (_hudSystems.ContainsKey(id))
        {
            _hudSystems.Remove(id);
        }
    }

    // Activates a specific HUD system by its identifier
    public void ActivateHUD(string id)
    {
        if (_hudSystems.ContainsKey(id))
        {
            _activeHUD = id;
        }
    }

    // Deactivates the currently active HUD system
    public void DeactivateHUD()
    {
        _activeHUD = null;
    }

    // Adds a new Menu system with a unique identifier
    public void RegisterMenu(string id, MenuSystem menuSystem)
    {
        if (!_menuSystems.ContainsKey(id))
        {
            _menuSystems[id] = menuSystem;
        }
    }

    // Removes an existing Menu system by its identifier
    public void UnregisterMenu(string id)
    {
        if (_menuSystems.ContainsKey(id))
        {
            _menuSystems.Remove(id);
        }
    }

    // Activates a specific Menu system by its identifier
    public void ActivateMenu(string id)
    {
        if (_menuSystems.ContainsKey(id))
        {
            _activeMenu = id;
            _menuSystems[id].Activate();
        }
    }

    // Deactivates the currently active Menu system
    public void DeactivateMenu()
    {
        if (_activeMenu != null && _menuSystems.ContainsKey(_activeMenu))
        {
            _menuSystems[_activeMenu].Deactivate();
            _activeMenu = null;
        }
    }

    // Adds a UI component to the system
    public void AddUIComponent(UIComponent component)
    {
        _uiComponents.Add(component);
    }

    // Removes a UI component from the system
    public void RemoveUIComponent(UIComponent component)
    {
        _uiComponents.Remove(component);
    }

    // Renders the currently active HUD and Menu systems
    public void Render(Renderer renderer)
    {
        // Render active HUD
        if (_activeHUD != null && _hudSystems.ContainsKey(_activeHUD))
        {
            _hudSystems[_activeHUD].Render(renderer);
        }

        // Render active Menu
        if (_activeMenu != null && _menuSystems.ContainsKey(_activeMenu))
        {
            _menuSystems[_activeMenu].Render(renderer);
        }

        // Render UI components
        foreach (var component in _uiComponents)
        {
            component.Render(renderer);
        }
    }

    // Handles events for the currently active HUD and Menu systems
    public void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                { sdlEvent.key.keysym.scancode });

            if (triggeredActions.Contains(ActionType.OpenOptions))
            {
                if (_activeMenu == "Settings")
                {
                    DeactivateMenu();
                }
                else
                {
                    ActivateMenu("Settings");
                }
            }
            else if (triggeredActions.Contains(ActionType.OpenInventory))
            {
                if (_activeMenu == "Inventory")
                {
                    DeactivateMenu();
                }
                else
                {
                    ActivateMenu("Inventory");
                }
            }
        }

        // Handle events for active HUD
        if (_activeHUD != null && _hudSystems.ContainsKey(_activeHUD))
        {
            _hudSystems[_activeHUD].HandleEvent(sdlEvent);
        }

        // Handle events for active Menu
        if (_activeMenu != null && _menuSystems.ContainsKey(_activeMenu))
        {
            _menuSystems[_activeMenu].HandleEvent(sdlEvent);
        }

        // Handle events for UI components
        foreach (var component in _uiComponents)
        {
            component.HandleEvent(sdlEvent);
        }
    }

    public HUDSystem GetHUDSystem(string id)
    {
        return _hudSystems[id];
    }

    public MenuSystem GetMenuSystem(string id)
    {
        return _menuSystems[id];
    }
    
    public bool IsMenuActive(string id)
    {
        return _activeMenu == id;
    }
    
    public bool IsHUDActive(string id)
    {
        return _activeHUD == id;
    }
}