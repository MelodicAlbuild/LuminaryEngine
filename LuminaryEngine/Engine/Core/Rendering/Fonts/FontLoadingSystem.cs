using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Fonts;

public class FontLoadingSystem
{
    public Font LoadFont(string filePath, int fontSize)
    {
        if (!File.Exists(filePath))
        {
            throw new Exception($"Font file not found: {filePath}");
        }

        IntPtr fontHandle = SDL_ttf.TTF_OpenFont(filePath, fontSize);
        if (fontHandle == IntPtr.Zero)
        {
            throw new Exception($"Failed to load font: {filePath}, Error: {SDL.SDL_GetError()}");
        }

        return new Font(fontHandle, fontSize, Path.GetFileNameWithoutExtension(filePath));
    }

    public void UnloadFont(Font font)
    {
        font.Dispose();
    }
}