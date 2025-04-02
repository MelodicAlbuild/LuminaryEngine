using LuminaryEngine.Engine.Core.Rendering.Textures;
using SDL2;

namespace LuminaryEngine.Engine.Core.ResourceManagement;

public class ResourceCache
{
    private IntPtr _renderer;
    private Dictionary<string, Texture> _textureCache;
    
    private TextureLoadingSystem _textureLoadingSystem;
    
    public ResourceCache(IntPtr renderer, TextureLoadingSystem textureLoadingSystem)
    {
        _renderer = renderer;
        _textureLoadingSystem = textureLoadingSystem;
        _textureCache = new Dictionary<string, Texture>();
    }

    public Texture GetTexture(string textureId)
    {
        if (_textureCache.ContainsKey(textureId))
        {
            return _textureCache[textureId];
        }
        
        string texturePath = Path.Combine("Assets", "Textures", textureId);
        
        Texture texture = _textureLoadingSystem.LoadTexture(_renderer, texturePath);
        texture.AssignTextureId(textureId);
        
        _textureCache[textureId] = texture;
        return texture;
    }

    public void ClearCache()
    {
        foreach (var texture in _textureCache.Values)
        {
            texture.Destroy();
        }
        _textureCache.Clear();
    }
}