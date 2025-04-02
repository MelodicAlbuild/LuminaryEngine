using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class SpriteComponent : IComponent
{
    public string TextureId { get; set; }
    public SDL.SDL_Rect? SourceRect { get; set; }

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
}