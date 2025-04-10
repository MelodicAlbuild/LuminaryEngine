using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.Gameplay.Player;
using SDL2;

namespace LuminaryEngine.Engine.ECS.Systems;

public class PlayerMovementSystem : LuminSystem
{
    private float _speed = 0.75f;
    private int _tileSize = 32;
    
    private GameTime _gameTime;
    
    public PlayerMovementSystem(World world, GameTime gameTime) : base(world)
    {
        _gameTime = gameTime;
    }
    
    public PlayerMovementSystem(World world, float speed, GameTime gameTime) : base(world)
    {
        _speed = speed;
        _gameTime = gameTime;
    }

    public override void Update()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(TransformComponent), typeof(InputStateComponent)))
        {
            // Assume entity has InputComponent, TransformComponent, and SmoothMovementComponent.
            var input = entity.GetComponent<InputStateComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var smoothMove = entity.GetComponent<SmoothMovementComponent>();

            // When a movement input is detected and no move is currently in progress:
            if (!smoothMove.IsMoving && IsMovementKeyPressed(input, entity, out Vector2 direction))
            {
                // Calculate new target position based on a grid move
                Vector2 newTarget = transform.Position + (direction * smoothMove.TileSize);
                if (IsValidTarget(newTarget))
                {
                    smoothMove.TargetPosition = newTarget;
                    smoothMove.IsMoving = true;
                }
            }

            // If movement is in progress, smoothly interpolate position
            if (smoothMove.IsMoving)
            {
                Vector2 toTarget = smoothMove.TargetPosition - transform.Position;
                float distance = toTarget.Length();

                // Calculate how far to move this frame based on speed.
                float moveStep = smoothMove.Speed * (float)_gameTime.DeltaTime;

                // Use Lerp for a smooth gradual approach.
                // Compute an interpolation factor relative to the remaining distance.
                float lerpFactor = moveStep / distance;
                lerpFactor = Math.Clamp(lerpFactor, 0, 1);
                transform.Position = Vector2.Lerp(transform.Position, smoothMove.TargetPosition, lerpFactor);

                // When close enough (with a tolerance to account for floating-point imprecision),
                // finalize the position.
                if (Vector2.Distance(transform.Position, smoothMove.TargetPosition) < 0.1f)
                {
                    transform.Position = smoothMove.TargetPosition;
                    smoothMove.IsMoving = false;
                }
            }
        }
    }
    
    private bool IsValidTarget(Vector2 target)
    {
        return !_world.IsTileSolid((int)(target.X / 32), (int)(target.Y / 32));
    }
    
    private bool IsMovementKeyPressed(InputStateComponent isc, Entity entity, out Vector2 direction)
    {
        direction = Vector2.Zero;
        AnimationComponent anim = entity.GetComponent<AnimationComponent>();
        SmoothMovementComponent smoothMove = entity.GetComponent<SmoothMovementComponent>();

        if (isc.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_W))
        {
            if (anim.State is not { CurrentAnimation: "WalkUp" })
            {
                anim.PlayAnimation("WalkUp");
            }
            direction.Y -= 1;
        } else if (isc.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_S))
        {
            if (anim.State is not { CurrentAnimation: "WalkDown" })
            {
                anim.PlayAnimation("WalkDown");
            }
            direction.Y += 1;
        } else if (isc.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_A))
        {
            if (anim.State is not { CurrentAnimation: "WalkLeft" })
            {
                anim.PlayAnimation("WalkLeft");
            }
            direction.X -= 1;
        } else if (isc.PressedKeys.Contains(SDL.SDL_Scancode.SDL_SCANCODE_D))
        {
            if (anim.State is not { CurrentAnimation: "WalkRight" })
            {
                anim.PlayAnimation("WalkRight");
            }
            direction.X += 1;
        }
        else
        {
            if (!smoothMove.IsMoving)
            {
                anim.StopAnimation();
            }
        }

        return direction != Vector2.Zero;
    }
}