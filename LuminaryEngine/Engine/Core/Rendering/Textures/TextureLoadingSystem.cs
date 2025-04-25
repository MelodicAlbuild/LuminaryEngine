using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.Exceptions;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Textures;

public class TextureLoadingSystem
{
    public Texture LoadTexture(IntPtr renderer, string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new UnknownTextureException($"Texture file not found: {filePath}");
        }

        IntPtr textureHandle = SDL_image.IMG_LoadTexture(renderer, filePath);
        if (textureHandle == IntPtr.Zero)
        {
            throw new UnknownTextureException($"Failed to load texture: {filePath}, Error: {SDL.SDL_GetError()}");
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