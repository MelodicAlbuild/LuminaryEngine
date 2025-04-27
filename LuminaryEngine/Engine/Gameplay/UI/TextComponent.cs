using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Fonts;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class TextComponent : UIComponent
{
    public string Text { get; private set; }
    public Font Font { get; set; }
    public SDL.SDL_Color Color { get; set; }

    // Store precomputed lines for rendering
    private List<string> wrappedLines = new();

    public TextComponent(
        string text,
        Font font,
        SDL.SDL_Color color,
        int x,
        int y,
        int width,
        int height,
        int zIndex = int.MaxValue
    ) : base(x, y, width, height, zIndex)
    {
        Text = text;
        Font = font;
        Color = color;

        // Wrap the initial text
        WrapText(Text);
    }

    /// <summary>
    /// Wraps the provided text to fit within the width of the component.
    /// </summary>
    public void WrapText(string text)
    {
        if (Font == null || string.IsNullOrEmpty(text)) return;

        wrappedLines.Clear();
        string[] lines = text.Split('\n'); // Split the text into lines based on newlines
        int spaceWidth, lineHeight;
        SDL_ttf.TTF_SizeText(Font.Handle, " ", out spaceWidth, out lineHeight);

        foreach (var line in lines)
        {
            string[] words = line.Split(' '); // Further split each line into words
            string currentLine = "";

            foreach (var word in words)
            {
                int textWidth, textHeight;
                SDL_ttf.TTF_SizeText(Font.Handle, (currentLine + word).Trim(), out textWidth, out textHeight);

                if (textWidth > Width && !string.IsNullOrEmpty(currentLine))
                {
                    wrappedLines.Add(currentLine.Trim());
                    currentLine = word + " ";
                }
                else
                {
                    currentLine += word + " ";
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                wrappedLines.Add(currentLine.Trim());
            }
        }
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible || Font == null) return;

        int lineHeight;
        SDL_ttf.TTF_SizeText(Font.Handle, "A", out _, out lineHeight);

        int currentY = Y;
        foreach (var line in wrappedLines)
        {
            if (currentY + lineHeight > Y + Height) break;

            SDL.SDL_Rect destRect = new SDL.SDL_Rect
            {
                x = X,
                y = currentY,
                w = Width,
                h = lineHeight
            };

            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.DrawText,
                Font = Font.Handle,
                Text = line,
                TextColor = Color,
                DestRect = destRect,
                ZOrder = ZIndex
            });

            currentY += lineHeight;
        }
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        // Handle events if needed
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
    }

    public void SetText(string text)
    {
        Text = text;
        WrapText(text); // Recalculate wrapped lines with the new text
    }
    
    public void SetColor(SDL.SDL_Color color)
    {
        Color = color;
    }
}