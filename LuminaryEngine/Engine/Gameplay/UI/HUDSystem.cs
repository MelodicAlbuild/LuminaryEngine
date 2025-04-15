using LuminaryEngine.Engine.Core.Rendering;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class HUDSystem
{
    private List<UIComponent> _uiComponents = new();

    // Adds a component to the HUD
    public void AddComponent(UIComponent component)
    {
        _uiComponents.Add(component);
    }

    public UIComponent GetComponent(int id)
    {
        return _uiComponents[id];
    }

    // Removes a component from the HUD
    public void RemoveComponent(UIComponent component)
    {
        _uiComponents.Remove(component);
    }

    // Renders all components in the HUD
    public void Render(Renderer renderer)
    {
        foreach (var component in _uiComponents)
        {
            component.Render(renderer);
        }
    }

    // Handles events for all HUD components
    public void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        foreach (var component in _uiComponents)
        {
            component.HandleEvent(sdlEvent);
        }
    }
}