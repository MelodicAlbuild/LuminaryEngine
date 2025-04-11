using LuminaryEngine.Engine.Core.Rendering;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class MenuSystem
{
    private List<UIComponent> _menuComponents = new();
    private bool _isActive = false;

    // Adds a component to the menu
    public void AddComponent(UIComponent component)
    {
        _menuComponents.Add(component);
    }

    // Removes a component from the menu
    public void RemoveComponent(UIComponent component)
    {
        _menuComponents.Remove(component);
    }

    // Activates the menu (enables rendering and event handling)
    public void Activate()
    {
        _isActive = true;
    }

    // Deactivates the menu (disables rendering and event handling)
    public void Deactivate()
    {
        _isActive = false;
    }

    // Renders all components in the menu when active
    public void Render(Renderer renderer)
    {
        if (!_isActive) return;

        foreach (var component in _menuComponents)
        {
            component.Render(renderer);
        }
    }

    // Handles events for all menu components when active
    public void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (!_isActive) return;

        foreach (var component in _menuComponents)
        {
            component.HandleEvent(sdlEvent);
        }
    }
}