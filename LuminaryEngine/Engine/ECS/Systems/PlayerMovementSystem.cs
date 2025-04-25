using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.ThirdParty.LDtk.Models;
using SDL2;

namespace LuminaryEngine.Engine.ECS.Systems;

public class PlayerMovementSystem : LuminSystem
{
    private float _speed = 0.75f;
    private Direction _direction = Direction.South;

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
        foreach (var entity in
                 _world.GetEntitiesWithComponents(typeof(TransformComponent), typeof(InputStateComponent)))
        {
            // Assume entity has InputComponent, TransformComponent, and SmoothMovementComponent.
            var input = entity.GetComponent<InputStateComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var smoothMove = entity.GetComponent<SmoothMovementComponent>();

            // When a movement input is detected and no move is currently in progress:
            if (!smoothMove.IsMoving && IsMovementKeyPressed(input, entity, out Vector2 direction) &&
                !_world.IsTransitioning())
            {
                // Calculate new target position based on a grid move
                Vector2 newTarget = transform.Position + (direction * smoothMove.TileSize);
                if (IsValidTarget(newTarget, entity))
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

    private bool IsValidTarget(Vector2 target, Entity entity)
    {
        if (_world.IsTileSolid((int)(target.X / 32), (int)(target.Y / 32)))
        {
            OnCollide(target, entity);
            return false;
        }

        return true;
    }

    private void OnCollide(Vector2 target, Entity entity)
    {
        Vector2 gridTarget = new Vector2(target.X / 32, target.Y / 32);
        if (_world.IsEntityAtPosition(gridTarget))
        {
            // Handle collision with another entity
            LDtkEntityInstance entityInstance = _world.GetEntityInstance(gridTarget);
            if (entityInstance != null)
            {
                Console.WriteLine($"Collision with {entityInstance.Identifier}");
                switch (entityInstance.Identifier)
                {
                    case "building_interact":
                        // Handle building interaction
                        if (entityInstance.FieldInstances.Find(o => o.Identifier == "buildingId") != null)
                        {
                            int bId = Convert.ToInt32(entityInstance.FieldInstances
                                .Find(o => o.Identifier == "buildingId").Value);
                            _world.SwitchLevel(bId, target);
                        }

                        break;
                }
            }
        }
    }

    private bool IsMovementKeyPressed(InputStateComponent isc, Entity entity, out Vector2 direction)
    {
        direction = Vector2.Zero;
        AnimationComponent anim = entity.GetComponent<AnimationComponent>();
        SmoothMovementComponent smoothMove = entity.GetComponent<SmoothMovementComponent>();

        var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(isc.PressedKeys);

        foreach (var action in triggeredActions)
        {
            switch (action)
            {
                case ActionType.MoveUp:
                    if (direction == Vector2.Zero)
                    {
                        if (anim.State is not { CurrentAnimation: "WalkUp" })
                        {
                            anim.PlayAnimation("WalkUp");
                        }

                        direction.Y -= 1;
                        _direction = Direction.North;
                    }

                    break;
                case ActionType.MoveDown:
                    if (direction == Vector2.Zero)
                    {
                        if (anim.State is not { CurrentAnimation: "WalkDown" })
                        {
                            anim.PlayAnimation("WalkDown");
                        }

                        direction.Y += 1;
                        _direction = Direction.South;
                    }

                    break;
                case ActionType.MoveLeft:
                    if (direction == Vector2.Zero)
                    {
                        if (anim.State is not { CurrentAnimation: "WalkLeft" })
                        {
                            anim.PlayAnimation("WalkLeft");
                        }

                        direction.X -= 1;
                        _direction = Direction.West;
                    }

                    break;
                case ActionType.MoveRight:
                    if (direction == Vector2.Zero)
                    {
                        if (anim.State is not { CurrentAnimation: "WalkRight" })
                        {
                            anim.PlayAnimation("WalkRight");
                        }

                        direction.X += 1;
                        _direction = Direction.East;
                    }

                    break;
            }
        }

        if (triggeredActions.Count <= 0)
        {
            if (!smoothMove.IsMoving)
            {
                anim.StopAnimation();
            }
        }

        return direction != Vector2.Zero;
    }

    public Direction GetDirection()
    {
        return _direction;
    }

    public void SetDirection(Direction direction, AnimationComponent anim = null)
    {
        _direction = direction;

        if (anim != null)
        {
            switch (direction)
            {
                case Direction.North:
                    anim.PlayAnimation("WalkUp");
                    break;
                case Direction.South:
                    anim.PlayAnimation("WalkDown");
                    break;
                case Direction.West:
                    anim.PlayAnimation("WalkLeft");
                    break;
                case Direction.East:
                    anim.PlayAnimation("WalkRight");
                    break;
            }

            anim.StopAnimation();
        }
    }
}