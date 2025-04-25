using SDL2;

namespace LuminaryEngine.Engine.Audio;

public class Sound : IDisposable
{
    internal IntPtr Chunk { get; private set; }
    public string FilePath { get; private set; }

    public Sound(string filePath, IntPtr chunk)
    {
        FilePath = filePath;
        Chunk = chunk;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (Chunk != IntPtr.Zero)
        {
            SDL_mixer.Mix_FreeChunk(Chunk);
            Chunk = IntPtr.Zero;
        }
    }

    ~Sound()
    {
        Dispose(false);
    }
}