using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class SpriteRaisedComponent : IComponent
{
    public string TextureId { get; set; }
    public SDL.SDL_Rect? SourceRect { get; set; }
    public int ZIndex { get; set; } = 0;

    public SpriteRaisedComponent(string textureId)
    {
        TextureId = textureId;
        SourceRect = null;
    }

    public SpriteRaisedComponent(string textureId, SDL.SDL_Rect sourceRect)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
    }

    public SpriteRaisedComponent(string textureId, int zIndex)
    {
        TextureId = textureId;
        ZIndex = zIndex;
    }

    public SpriteRaisedComponent(string textureId, SDL.SDL_Rect sourceRect, int zIndex)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
        ZIndex = zIndex;
    }
}