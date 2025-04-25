using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Input;

public abstract class InputSystem : LuminSystem
{
    public InputSystem(World world) : base(world)
    {
    }

    public abstract void HandleEvents(SDL.SDL_Event e);
}