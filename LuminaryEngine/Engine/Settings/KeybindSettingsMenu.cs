using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Settings;

public class KeybindSettingsMenu : UIComponent
{
    private List<ActionType> _actions;
    private int _selectedActionIndex;
    private bool _isRebinding; // Indicates if we are waiting for a key press to rebind an action

    public KeybindSettingsMenu(int x, int y, int width, int height,
        int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        _actions = new List<ActionType>((ActionType[])Enum.GetValues(typeof(ActionType)));
        _selectedActionIndex = 0;
        _isRebinding = false;
    }

    public override void Render(Renderer renderer)
    {
        int offsetY = Y + 10;

        for (int i = 0; i < _actions.Count; i++)
        {
            var action = _actions[i];
            var isSelected = i == _selectedActionIndex;
            var assignedKey = InputMappingSystem.Instance.GetKeyForAction(action)?.ToString() ?? "None";

            var color = isSelected
                ? new SDL.SDL_Color { r = 255, g = 255, b = 0, a = 255 } // Highlighted
                : new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }; // Normal

            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.DrawText,
                Font = ResourceCache.DefaultFont.Handle,
                Text = $"{action}: {assignedKey}",
                TextColor = color,
                DestRect = new SDL.SDL_Rect { x = X + 10, y = offsetY, w = Width - 20, h = 30 },
                ZOrder = ZIndex
            });

            offsetY += 40;
        }

        if (_isRebinding)
        {
            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.DrawText,
                Font = ResourceCache.DefaultFont.Handle,
                Text = "Press a key to rebind...",
                TextColor = new SDL.SDL_Color { r = 255, g = 0, b = 0, a = 255 },
                DestRect = new SDL.SDL_Rect { x = X + 10, y = Y + Height - 40, w = Width - 20, h = 30 },
                ZOrder = ZIndex
            });
        }
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (_isRebinding && sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            // Use the InputMappingSystem to handle the keybinding
            var pressedKey = sdlEvent.key.keysym.scancode;

            // Check if the key is already assigned to another action
            foreach (var action in _actions)
            {
                if (InputMappingSystem.Instance.GetKeyForAction(action) == pressedKey)
                {
                    Console.WriteLine($"Key {pressedKey} is already assigned to {action}. Choose another key.");
                    _isRebinding = false; // Exit rebind mode
                    return;
                }
            }

            // Rebind the selected action
            var selectedAction = _actions[_selectedActionIndex];
            InputMappingSystem.Instance.MapActionToKey(selectedAction, pressedKey);
            Console.WriteLine($"Bound {selectedAction} to {pressedKey}.");
            _isRebinding = false; // Exit rebind mode
        }
        else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            // Handle navigation and selection using the InputMappingSystem
            var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                { sdlEvent.key.keysym.scancode });

            foreach (var action in triggeredActions)
            {
                switch (action)
                {
                    case ActionType.MoveUp:
                        _selectedActionIndex = (_selectedActionIndex - 1 + _actions.Count) % _actions.Count;
                        break;

                    case ActionType.MoveDown:
                        _selectedActionIndex = (_selectedActionIndex + 1) % _actions.Count;
                        break;

                    case ActionType.Interact: // Use 'Interact' action as "Select/Rebind" action
                        _isRebinding = true;
                        Console.WriteLine($"Rebinding {_actions[_selectedActionIndex]}... Press a key.");
                        break;
                }
            }
        }
    }
}