using SDL2;

namespace LuminaryEngine.Engine.Core.Input;

public class InputMappingSystem
{
    // Static instance of the InputMappingSystem
    private static InputMappingSystem _instance;

    // Public property to access the singleton instance
    public static InputMappingSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new InputMappingSystem();
            }

            return _instance;
        }
    }

    private Dictionary<ActionType, SDL.SDL_Scancode> _actionKeyMap;

    public InputMappingSystem()
    {
        _actionKeyMap = new Dictionary<ActionType, SDL.SDL_Scancode>();
    }

    // Map an action to a key
    public void MapActionToKey(ActionType action, SDL.SDL_Scancode key)
    {
        _actionKeyMap[action] = key;
    }

    // Get the key mapped to an action
    public SDL.SDL_Scancode? GetKeyForAction(ActionType action)
    {
        return _actionKeyMap.TryGetValue(action, out var key) ? key : null;
    }

    // Check if a specific action is triggered
    public bool IsActionTriggered(ActionType action, HashSet<SDL.SDL_Scancode> pressedKeys)
    {
        var key = GetKeyForAction(action);
        return key.HasValue && pressedKeys.Contains(key.Value);
    }

    // Check all keybinds and return a list of triggered actions
    public List<ActionType> GetTriggeredActions(HashSet<SDL.SDL_Scancode> pressedKeys)
    {
        var triggeredActions = new List<ActionType>();

        foreach (var action in _actionKeyMap.Keys)
        {
            if (IsActionTriggered(action, pressedKeys))
            {
                triggeredActions.Add(action);
            }
        }

        return triggeredActions;
    }

    // Load mappings from a configuration dictionary
    public void LoadMappings(Dictionary<ActionType, SDL.SDL_Scancode> mappings)
    {
        _actionKeyMap = new Dictionary<ActionType, SDL.SDL_Scancode>(mappings);
    }

    // Save mappings to a configuration dictionary
    public Dictionary<ActionType, SDL.SDL_Scancode> SaveMappings()
    {
        return new Dictionary<ActionType, SDL.SDL_Scancode>(_actionKeyMap);
    }
}