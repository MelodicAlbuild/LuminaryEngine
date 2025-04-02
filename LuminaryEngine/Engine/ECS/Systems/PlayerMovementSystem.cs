using System.Numerics;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.ECS.Components;
using SDL2;

namespace LuminaryEngine.Engine.ECS.Systems;

public class PlayerMovementSystem : LuminSystem
{
    private float _speed = 5f;
    
    public PlayerMovementSystem(World world) : base(world)
    {
    }
    
    public PlayerMovementSystem(World world, float speed) : base(world)
    {
        _speed = speed;
    }

    public override void Update()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(TransformComponent), typeof(InputStateComponent)))
        {
            var transform = entity.GetComponent<TransformComponent>();
            var inputState = entity.GetComponent<InputStateComponent>();

            Vector2 movement = Vector2.Zero;

            if (inputState.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_W))
            {
                movement.Y -= _speed;
            }

            if (inputState.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_S))
            {
                movement.Y += _speed;
            }

            if (inputState.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_A))
            {
                movement.X -= _speed;
            }

            if (inputState.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_D))
            {
                movement.X += _speed;
            }

            transform.Position += movement;
        }
    }
}