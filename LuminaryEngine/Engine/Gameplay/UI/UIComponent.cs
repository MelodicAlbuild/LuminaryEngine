using LuminaryEngine.Engine.Core.Rendering;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public abstract class UIComponent
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsVisible { get; set; } = true;
    public int ZIndex { get; set; } // Default to top-most layer
    public bool IsFocused { get; set; } = false;

    public UIComponent(int x, int y, int width, int height, int zIndex = int.MaxValue - 1)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        ZIndex = zIndex;
    }

    public void EnqueueRender(Renderer renderer, IntPtr texture, SDL.SDL_Rect? sourceRect)
    {
        if (!IsVisible) return;

        SDL.SDL_Rect destRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height };

        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawTexture,
            Texture = texture,
            SourceRect = sourceRect,
            DestRect = destRect,
            ZOrder = ZIndex
        });
    }

    public abstract void Render(Renderer renderer);
    public abstract void HandleEvent(SDL.SDL_Event sdlEvent);
    public abstract void SetFocus(bool isFocused);
}