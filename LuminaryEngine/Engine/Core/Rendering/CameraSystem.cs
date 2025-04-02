using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;

namespace LuminaryEngine.Engine.Core.Rendering;

public class CameraSystem : LuminSystem
{
    // The current camera position
        public Vector2 CameraPosition { get; private set; } = Vector2.Zero;

        // Camera viewport dimensions (should match the window size)
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

        // Map dimensions (we'll need this to prevent the camera from going out of bounds)
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }

        // Smoothing factor (adjust to control camera smoothness)
        public float SmoothingSpeed { get; set; } = 5.0f;
        
        private GameTime _gameTime;

        public CameraSystem(World world, GameTime gameTime) : base(world)
        {
            // Initialize viewport and map dimensions (you'll likely get these from your level loading)
            ViewportWidth = 1280; // Default window width
            ViewportHeight = 720;  // Default window height
            MapWidth = 2048;       // Example map width
            MapHeight = 2048;      // Example map height
            
            _gameTime = gameTime;
        }

        public override void Update()
        {
            // Get delta time from the GameTime class
            float deltaTime = _gameTime.DeltaTime;

            // Find the entity with the CameraComponent (usually the player)
            Entity targetEntity = null;
            foreach (var entity in _world.GetEntitiesWithComponents(typeof(CameraComponent), typeof(TransformComponent)))
            {
                targetEntity = entity;
                break; // Assuming only one camera target
            }

            if (targetEntity == null)
                return; // No camera target, nothing to do

            var cameraComponent = targetEntity.GetComponent<CameraComponent>();
            var transformComponent = targetEntity.GetComponent<TransformComponent>();

            // Calculate the desired camera position (centered on the target)
            Vector2 desiredPosition = transformComponent.Position + cameraComponent.Offset -
                                      new Vector2(ViewportWidth / 2, ViewportHeight / 2);

            // Clamp the camera position to prevent it from going out of map bounds
            desiredPosition.X = Math.Max(0, Math.Min(desiredPosition.X, MapWidth - ViewportWidth));
            desiredPosition.Y = Math.Max(0, Math.Min(desiredPosition.Y, MapHeight - ViewportHeight));

            // Smoothly move the camera towards the desired position
            CameraPosition = Vector2.Lerp(CameraPosition, desiredPosition, SmoothingSpeed * deltaTime);
        }

        // Helper function to transform world coordinates to screen coordinates
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return worldPosition - CameraPosition;
        }
}