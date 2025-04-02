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