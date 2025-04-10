using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;

namespace LuminaryEngine.Engine.Core.Rendering;

public class Camera
{
    public int X { get; set; }
    public int Y { get; set; }

    public Camera(int initialX, int initialY)
    {
        X = initialX;
        Y = initialY;
    }
    
    public void Follow(Vector2 target)
    {
        // Center the camera on the target immediately.
        //int clampedX = Math.Clamp(desiredPosition.X, MapBounds.Left, MapBounds.Right - Game.DISPLAY_WIDTH);
        //int clampedY = Math.Clamp(desiredPosition.Y, MapBounds.Top, MapBounds.Bottom - Game.DISPLAY_HEIGHT);
        X = (int)target.X;
        Y = (int)target.Y;
    }
}