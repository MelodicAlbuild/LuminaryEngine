using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering;

public struct RenderCommand
{
    public RenderCommandType Type { get; set; }
    public IntPtr Texture { get; set; }
    public SDL.SDL_Rect? SourceRect { get; set; }
    public SDL.SDL_Rect DestRect { get; set; }
}

public enum RenderCommandType
{
    DrawTexture
}