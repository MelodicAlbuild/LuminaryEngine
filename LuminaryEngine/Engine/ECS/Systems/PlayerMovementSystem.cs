using System.Numerics;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.ECS.Components;
using SDL2;

namespace LuminaryEngine.Engine.ECS.Systems;

public class PlayerMovementSystem : LuminSystem
{
    private float _speed = 0.75f;
    private int _tileSize = 16;
    
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

            Vector2 precise = transform.PrecisePosition;
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

            precise += movement;
            transform.PrecisePosition = precise;

            transform.Position = new Vector2(
                (float)Math.Floor(precise.X / _tileSize) * _tileSize,
                (float)Math.Floor(precise.Y / _tileSize) * _tileSize
            );
        }
    }
}