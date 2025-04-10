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

    // Clear Command
    public byte ClearR { get; set; }
    public byte ClearG { get; set; }
    public byte ClearB { get; set; }
    public byte ClearA { get; set; }
}

public enum RenderCommandType
{
    DrawTexture,
    Clear,
    FadeFrame,
    FadeFrameHold
}