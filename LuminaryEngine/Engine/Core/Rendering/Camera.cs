using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Core.Rendering;

public class Camera
{
    public float X { get; set; }
    public float Y { get; set; }

    private World _world;
    
    public Camera(int initialX, int initialY, World world)
    {
        X = initialX;
        Y = initialY;
        _world = world;
    }
    
    public void Follow(Vector2 target)
    {
        // Center the camera on the target immediately.
        Vector2 desiredPosition = target - new Vector2(Game.DISPLAY_WIDTH * 0.5f, Game.DISPLAY_HEIGHT * 0.5f);
        int clampedX = (int)Math.Clamp(desiredPosition.X, 0, (_world.GetCurrentLevel().PixelWidth) - Game.DISPLAY_WIDTH);
        int clampedY = (int)Math.Clamp(desiredPosition.Y, 0, (_world.GetCurrentLevel().PixelHeight) - Game.DISPLAY_HEIGHT);
        X = clampedX;
        Y =clampedY;
    }
}