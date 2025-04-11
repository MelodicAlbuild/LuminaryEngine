using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Fonts;

public class Font
{
    public IntPtr Handle { get; private set; }
    public int FontSize { get; private set; }
    public string FontId { get; private set; }

    public Font(IntPtr handle, int fontSize, string fontId)
    {
        Handle = handle;
        FontSize = fontSize;
        FontId = fontId;
    }

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            SDL_ttf.TTF_CloseFont(Handle);
            Handle = IntPtr.Zero;
        }
    }
}