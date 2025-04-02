using SDL2;

namespace LuminaryEngine.Engine.Core.GameLoop;

public class GameTime
{
    public float DeltaTime { get; private set; }
    public float TotalTime { get; private set; }

    private ulong previousTicks;

    public GameTime()
    {
        previousTicks = SDL.SDL_GetTicks();
        DeltaTime = 0f;
        TotalTime = 0f;
    }

    public void Update()
    {
        ulong currentTicks = SDL.SDL_GetTicks();
        DeltaTime = (currentTicks - previousTicks) / 1000.0f; // Convert milliseconds to seconds
        TotalTime += DeltaTime;
        previousTicks = currentTicks;
    }
}