using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class ImageComponent : UIComponent
{
    public Texture Texture { get; private set; }
    public SDL.SDL_Rect? SourceRect { get; private set; }

    public ImageComponent(Texture texture, int x, int y, int width, int height, SDL.SDL_Rect? sourceRect = null,
        int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        Texture = texture;
        SourceRect = sourceRect;
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible) return;

        SDL.SDL_Rect destRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height };

        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawTexture,
            Texture = Texture.Handle,
            SourceRect = SourceRect,
            DestRect = destRect,
            ZOrder = ZIndex
        });
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        // Image components are static and don't handle events by default.
    }

    public void SetTexture(Texture newTexture, SDL.SDL_Rect? newSourceRect = null)
    {
        Texture = newTexture;
        SourceRect = newSourceRect;
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
    }
}