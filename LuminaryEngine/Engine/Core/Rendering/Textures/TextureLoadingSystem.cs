using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Textures;

public class TextureLoadingSystem
{
    public Texture LoadTexture(IntPtr renderer, string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Texture file not found: {filePath}");
            return null;
        }
        
        IntPtr textureHandle = SDL_image.IMG_LoadTexture(renderer, filePath);
        if (textureHandle == IntPtr.Zero)
        {
            Console.WriteLine($"Failed to load texture: {filePath}, Error: {SDL.SDL_GetError()}");
            return null;
        }
        
        SDL.SDL_QueryTexture(textureHandle, out var format, out var access, out var width, out var height);
        
        return new Texture(textureHandle, format, access, width, height);
    }
    
    public void UnloadTexture(Texture texture)
    {
        if (texture != null && texture.Handle != IntPtr.Zero)
        {
            texture.Destroy();
        }
    }
}