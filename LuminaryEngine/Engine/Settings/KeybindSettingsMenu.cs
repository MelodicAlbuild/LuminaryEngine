using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Settings;

public class KeybindSettingsMenu : UIComponent
{
    private List<ActionType> _actions;
    private bool _isRebinding; // Indicates if we are waiting for a key press to rebind an action
    private ScrollableMenu _scrollableMenu;

    public KeybindSettingsMenu(int x, int y, int width, int height,
        int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        _actions = new List<ActionType>((ActionType[])Enum.GetValues(typeof(ActionType)));
        _isRebinding = false;

        List<string> assignedKeys = new List<string>();
        foreach (var action in _actions)
        {
            var assignedKey = InputMappingSystem.Instance.GetKeyForAction(action);
            if (assignedKey != null)
            {
                assignedKeys.Add(action + ": " + SDL.SDL_GetScancodeName(assignedKey!.Value));
            }
            else
            {
                assignedKeys.Add(action + ": None");
            }
        }

        _scrollableMenu = new ScrollableMenu(X, Y, Width, Height, assignedKeys, 5, ZIndex);
    }

    public override void Render(Renderer renderer)
    {
        // Render backdrop
        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawRectangle,
            RectColor = new SDL.SDL_Color() { r = 0, g = 0, b = 0, a = 255 }, // Semi-transparent black
            DestRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height },
            Filled = true,
            ZOrder = ZIndex - 1 // Ensure backdrop is behind menu items
        });

        _scrollableMenu.Render(renderer);

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
            foreach (var action in _actions.Where(action =>
                         InputMappingSystem.Instance.GetKeyForAction(action) == pressedKey))
            {
                LuminLog.Debug($"Key {pressedKey} is already assigned to {action}. Choose another key.");
                _isRebinding = false; // Exit rebind mode
                return;
            }

            // Rebind the selected action
            Enum.TryParse(_scrollableMenu.GetSelectedOption().Split(":")[0], out ActionType selectedAction);
            InputMappingSystem.Instance.MapActionToKey(selectedAction, pressedKey);
            LuminLog.Debug($"Bound {selectedAction} to {pressedKey}.");
            _scrollableMenu.UpdateCurrentOption($"{selectedAction}: {SDL.SDL_GetScancodeName(pressedKey)}");
            _isRebinding = false; // Exit rebind mode
        }
        else if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            _scrollableMenu.HandleEvent(sdlEvent);

            var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                { sdlEvent.key.keysym.scancode });

            if (triggeredActions.Contains(ActionType.Interact))
            {
                _isRebinding = true;
            }
        }
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;

        _scrollableMenu.SetFocus(isFocused);
    }
}