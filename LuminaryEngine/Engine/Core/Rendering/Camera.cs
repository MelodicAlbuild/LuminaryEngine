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
}