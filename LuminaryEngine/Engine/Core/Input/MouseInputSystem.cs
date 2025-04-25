using System.Numerics;
using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Input;

public class MouseInputSystem : InputSystem
{
    public MouseInputSystem(World world) : base(world)
    {
    }

    public override void HandleEvents(SDL.SDL_Event e)
    {
        if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
        {
            foreach (var entity in _world.GetEntitiesWithComponents(typeof(InputStateComponent)))
            {
                var inputState = entity.GetComponent<InputStateComponent>();
                inputState.PressedMouseButtons.Add(e.button.button);
            }
        }
        else if (e.type == SDL.SDL_EventType.SDL_MOUSEBUTTONUP)
        {
            foreach (var entity in _world.GetEntitiesWithComponents(typeof(InputStateComponent)))
            {
                var inputState = entity.GetComponent<InputStateComponent>();
                inputState.PressedMouseButtons.Remove(e.button.button);
            }
        }
        else if (e.type == SDL.SDL_EventType.SDL_MOUSEMOTION)
        {
            foreach (var entity in _world.GetEntitiesWithComponents(typeof(InputStateComponent)))
            {
                var inputState = entity.GetComponent<InputStateComponent>();
                inputState.MousePosition = new Vector2(e.motion.x, e.motion.y);
            }
        }
    }

    public override void Update()
    {
    }
}