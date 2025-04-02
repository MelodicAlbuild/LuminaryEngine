using SDL2;

namespace LuminaryEngine.Engine.Core.ResourceManagement;

public class ResourceCache
{
    private IntPtr _renderer;
    private Dictionary<string, IntPtr> _textureCache;
    
    public ResourceCache(IntPtr renderer)
    {
        _renderer = renderer;
        _textureCache = new Dictionary<string, IntPtr>();
    }

    public IntPtr GetTexture(string textureId)
    {
        if (_textureCache.ContainsKey(textureId))
        {
            return _textureCache[textureId];
        }
        
        string texturePath = Path.Combine("Assets", "Textures", textureId);
        
        IntPtr texture = SDL_image.IMG_LoadTexture(_renderer, texturePath);
        if (texture == IntPtr.Zero)
        {
            Console.WriteLine($"Failed to load texture: {texturePath}, Error: {SDL.SDL_GetError()}");
            return IntPtr.Zero;
        }
        
        _textureCache[textureId] = texture;
        return texture;
    }

    public void ClearCache()
    {
        foreach (var texture in _textureCache.Values)
        {
            SDL.SDL_DestroyTexture(texture);
        }
        _textureCache.Clear();
    }
}