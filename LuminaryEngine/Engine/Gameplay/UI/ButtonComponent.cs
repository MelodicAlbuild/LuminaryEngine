using LuminaryEngine.Engine.Core.Rendering;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class ButtonComponent : UIComponent
{
    public SDL.SDL_Color BackgroundColor { get; set; }
    public Action OnClick { get; set; }
    private TextComponent _textComponent;

    public ButtonComponent(
        string label,
        IntPtr font,
        SDL.SDL_Color labelColor,
        SDL.SDL_Color backgroundColor,
        int x,
        int y,
        int width,
        int height,
        int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
        BackgroundColor = backgroundColor;

        // Initialize the TextComponent for the button's label
        _textComponent = new TextComponent(label, font, labelColor, x + 10, y + 10, width - 20, height - 20, zIndex + 1); // Padding for the label
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible) return;

        // Enqueue button background
        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.Clear, // Custom command for solid color
            ClearR = BackgroundColor.r,
            ClearG = BackgroundColor.g,
            ClearB = BackgroundColor.b,
            ClearA = BackgroundColor.a,
            DestRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height },
            ZOrder = ZIndex
        });

        // Render the integrated TextComponent
        _textComponent.Render(renderer);
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (sdlEvent.type == SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN)
        {
            int mouseX = sdlEvent.button.x;
            int mouseY = sdlEvent.button.y;

            if (mouseX >= X && mouseX <= X + Width && mouseY >= Y && mouseY <= Y + Height)
            {
                OnClick?.Invoke();
            }
        }
    }

    // Update the button's label text
    public void SetLabel(string newLabel)
    {
        _textComponent.Text = newLabel;
    }

    // Update the button's label position (useful if the button's position changes)
    public void UpdateTextPosition()
    {
        _textComponent.X = X + 10; // Adjust for padding
        _textComponent.Y = Y + 10; // Adjust for padding
    }
}