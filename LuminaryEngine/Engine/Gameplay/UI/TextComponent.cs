using LuminaryEngine.Engine.Core.Rendering;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class TextComponent : UIComponent
{
    public string Text { get; set; }
    public IntPtr Font { get; set; } // SDL2 TTF Font
    public SDL.SDL_Color Color { get; set; }

    public TextComponent(string text, IntPtr font, SDL.SDL_Color color, int x, int y, int width, int height, int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        Text = text;
        Font = font;
        Color = color;
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible) return;

        IntPtr surface = SDL_ttf.TTF_RenderText_Blended(Font, Text, Color);
        IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer.GetRenderer(), surface);

        SDL.SDL_FreeSurface(surface);

        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawTexture,
            Texture = texture,
            DestRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height },
            ZOrder = ZIndex
        });

        SDL.SDL_DestroyTexture(texture);
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        // Handle events if needed
    }
}