using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class SpriteComponent : IComponent
{
    public string TextureId { get; set; }
    public SDL.SDL_Rect? SourceRect { get; set; }
    public int ZIndex { get; set; } = 0;
    public bool IsShifted { get; set; } = true;

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

    public SpriteComponent(string textureId, int zIndex)
    {
        TextureId = textureId;
        ZIndex = zIndex;
    }

    public SpriteComponent(string textureId, SDL.SDL_Rect sourceRect, int zIndex)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
        ZIndex = zIndex;
    }
    
    public SpriteComponent(string textureId, SDL.SDL_Rect sourceRect, int zIndex, bool isShifted)
    {
        TextureId = textureId;
        SourceRect = sourceRect;
        ZIndex = zIndex;
        IsShifted = isShifted;
    }
}