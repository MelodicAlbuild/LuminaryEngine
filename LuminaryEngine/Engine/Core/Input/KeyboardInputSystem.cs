using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Input;

public class KeyboardInputSystem : InputSystem
{
    public KeyboardInputSystem(World world) : base(world)
    {
    }

    public override void HandleEvents(SDL.SDL_Event e)
    {
        if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            foreach (var entity in _world.GetEntitiesWithComponents(typeof(InputStateComponent)))
            {
                var inputState = entity.GetComponent<InputStateComponent>();
                inputState.PressedKeys.Add(e.key.keysym.scancode);

                // Check if any action is triggered
                foreach (ActionType action in System.Enum.GetValues(typeof(ActionType)))
                {
                    if (InputMappingSystem.Instance.IsActionTriggered(action, inputState.PressedKeys))
                    {
                        // Handle the triggered action (example: log or trigger event)
                        LuminLog.Debug($"Action triggered: {action}");
                    }
                }
            }
        }
        else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
        {
            foreach (var entity in _world.GetEntitiesWithComponents(typeof(InputStateComponent)))
            {
                var inputState = entity.GetComponent<InputStateComponent>();
                inputState.PressedKeys.Remove(e.key.keysym.scancode);
            }
        }
    }

    public override void Update()
    {
    }
}