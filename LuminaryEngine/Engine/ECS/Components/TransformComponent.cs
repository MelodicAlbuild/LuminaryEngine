using System.Numerics;

namespace LuminaryEngine.Engine.ECS.Components;

public class TransformComponent : IComponent
{
    public Vector2 Position { get; set; }
    public Vector2 PrecisePosition { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; }

    public TransformComponent()
    {
        Position = Vector2.Zero;
        PrecisePosition = Vector2.Zero;
        Rotation = 0f;
        Scale = Vector2.One;
    }

    public TransformComponent(float x, float y) : this()
    {
        Position = new Vector2(x, y);
        PrecisePosition = new Vector2(x, y);
    }

    public TransformComponent(float x, float y, float rotation) : this(x, y)
    {
        Rotation = rotation;
    }

    public TransformComponent(float x, float y, float rotation, float scaleX, float scaleY) : this(x, y, rotation)
    {
        Scale = new Vector2(scaleX, scaleY);
    }
}