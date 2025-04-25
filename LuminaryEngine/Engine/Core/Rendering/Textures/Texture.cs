namespace LuminaryEngine.Engine.Core.Rendering.Textures;

public class Texture
{
    public IntPtr Handle { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public uint Format { get; private set; }
    public int Access { get; private set; }

    public string TextureId { get; private set; }

    public Texture(IntPtr handle, uint format, int access, int width, int height)
    {
        Handle = handle;
        Format = format;
        Access = access;
        Width = width;
        Height = height;
    }

    public void AssignTextureId(string textureId)
    {
        TextureId = textureId;
    }

    public void Destroy()
    {
        if (Handle != IntPtr.Zero)
        {
            SDL2.SDL.SDL_DestroyTexture(Handle);
            Handle = IntPtr.Zero;
        }
    }
}