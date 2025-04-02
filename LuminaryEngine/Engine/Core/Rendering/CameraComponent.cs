using System.Numerics;
using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Core.Rendering;

public class CameraComponent : IComponent
{
    public Vector2 Offset { get; set; }

    public CameraComponent()
    {
        Offset = Vector2.Zero;
    }
    
    public CameraComponent(Vector2 offset)
    {
        Offset = offset;
    }
}