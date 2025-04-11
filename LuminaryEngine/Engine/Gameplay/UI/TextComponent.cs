using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Fonts;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class TextComponent : UIComponent
{
    public string Text { get; set; }
    public Font Font { get; set; }
    public SDL.SDL_Color Color { get; set; }

    public TextComponent(string text, Font font, SDL.SDL_Color color, int x, int y, int width, int height, int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        Text = text;
        Font = font;
        Color = color;
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible) return;

        // Calculate text dimensions
        int textWidth, textHeight;
        SDL_ttf.TTF_SizeText(Font.Handle, Text, out textWidth, out textHeight);

        // Adjust the DestRect to maintain aspect ratio
        SDL.SDL_Rect adjustedRect = new SDL.SDL_Rect
        {
            x = X,
            y = Y,
            w = Width,
            h = Height
        };

        float aspectRatio = (float)textWidth / textHeight;
        if (adjustedRect.w / (float)adjustedRect.h > aspectRatio)
        {
            adjustedRect.w = (int)(adjustedRect.h * aspectRatio);
        }
        else
        {
            adjustedRect.h = (int)(adjustedRect.w / aspectRatio);
        }

        // Enqueue the DrawText render command
        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawText,
            Font = Font.Handle,
            Text = Text,
            TextColor = Color,
            DestRect = adjustedRect,
            ZOrder = ZIndex
        });
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        // Handle events if needed
    }
}