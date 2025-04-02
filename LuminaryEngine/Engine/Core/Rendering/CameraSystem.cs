using System.Numerics;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;

namespace LuminaryEngine.Engine.Core.Rendering;

public class CameraSystem : LuminSystem
{
    public Vector2 CameraPosition { get; private set; } = Vector2.Zero;

    public CameraSystem(World world) : base(world) { }

    public override void Update()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(CameraComponent), typeof(TransformComponent)))
        {
            var cameraComponent = entity.GetComponent<CameraComponent>();
            var transformComponent = entity.GetComponent<TransformComponent>();

            // Calculate the desired camera position
            Vector2 desiredPosition = transformComponent.Position + cameraComponent.Offset;

            // For simplicity, let's make the camera instantly snap to the target
            // You can implement smoothing or other camera effects here
            CameraPosition = desiredPosition;
        }
    }
    
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        // Convert world position to screen position based on camera position
        return worldPosition - CameraPosition;
    }
}