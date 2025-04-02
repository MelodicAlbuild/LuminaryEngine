using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class SpriteComponent : IComponent
{
    public string TextureId { get; set; }
    public SDL.SDL_Rect? SourceRect { get; set; }
    public int ZIndex { get; set; } = 0;

    public SpriteComponent(string textureId)
    {
        TextureId = textureId;
        SourceRect = null;
    }

    public SpriteComponent(string textureId, SDL.SDL_Rect sourceRect)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
    }
    
    public SpriteComponent(string textureId, int ZIndex)
    {
        TextureId = textureId;
        ZIndex = ZIndex;
    }
    
    public SpriteComponent(string textureId, SDL.SDL_Rect sourceRect, int ZIndex)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
        ZIndex = ZIndex;
    }
}