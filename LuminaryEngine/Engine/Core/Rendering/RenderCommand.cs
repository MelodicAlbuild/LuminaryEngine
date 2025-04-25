using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering;

public struct RenderCommand
{
    public RenderCommandType Type { get; set; }

    // Draw Texture Command
    public IntPtr Texture { get; set; }
    public SDL.SDL_Rect? SourceRect { get; set; }
    public SDL.SDL_Rect DestRect { get; set; }
    public float ZOrder { get; set; }

    // Draw Text Command
    public IntPtr Font { get; set; } // SDL2 TTF Font
    public string Text { get; set; }
    public SDL.SDL_Color TextColor { get; set; }

    // Clear Command
    public byte ClearR { get; set; }
    public byte ClearG { get; set; }
    public byte ClearB { get; set; }
    public byte ClearA { get; set; }

    // Draw Rectangle Command
    public SDL.SDL_Color RectColor { get; set; }
    public bool Filled { get; set; }
}

public enum RenderCommandType
{
    DrawTexture,
    Clear,
    FadeFrame,
    FadeFrameHold,
    ClearUI,
    DrawText,
    DrawRectangle
}