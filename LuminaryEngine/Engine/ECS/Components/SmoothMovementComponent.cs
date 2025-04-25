using System.Numerics;

namespace LuminaryEngine.Engine.ECS.Components;

/// <summary>
/// Component for handling smooth movement between tiles.
/// </summary>
public class SmoothMovementComponent : IComponent
{
    /// <summary>
    /// The target position the entity is moving toward.
    /// </summary>
    public Vector2 TargetPosition { get; set; }

    /// <summary>
    /// Flag indicating whether the entity is currently moving.
    /// </summary>
    public bool IsMoving { get; set; }

    /// <summary>
    /// Movement speed in units per second.
    /// </summary>
    public float Speed { get; set; }

    /// <summary>
    /// Size of one tile; used to calculate target positions from input.
    /// </summary>
    public float TileSize { get; set; }

    /// <summary>
    /// Initializes a new instance of the SmoothMovementComponent.
    /// </summary>
    /// <param name="speed">Movement speed (units/second).</param>
    /// <param name="tileSize">The tile size.</param>
    public SmoothMovementComponent(float speed, float tileSize)
    {
        Speed = speed;
        TileSize = tileSize;
        IsMoving = false;
        TargetPosition = Vector2.Zero;
    }

    public void Freeze()
    {
        // Reset the component to its initial state.
        IsMoving = false;
        TargetPosition = Vector2.Zero;
    }

    public bool GetIsMoving()
    {
        // Return the current moving state.
        return IsMoving;
    }
}