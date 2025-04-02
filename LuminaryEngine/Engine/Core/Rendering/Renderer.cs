using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering;

public class Renderer
{
    private IntPtr _renderer;
    private List<RenderCommand> renderQueue = new List<RenderCommand>();
    
    public Renderer(IntPtr window)
    {
        _renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        
        if (_renderer == IntPtr.Zero)
        {
            throw new Exception($"Failed to create renderer: {SDL.SDL_GetError()}");
        }
    }
    
    public void Clear(byte r, byte g, byte b, byte a)
    {
        EnqueueRenderCommand(new RenderCommand()
        {
            Type = RenderCommandType.Clear,
            ClearR = r,
            ClearG = g,
            ClearB = b,
            ClearA = a,
            ZOrder = float.MinValue
        });
    }
    
    public void EnqueueRenderCommand(RenderCommand command)
    {
        int insertIndex = renderQueue.FindIndex(cmd => cmd.ZOrder > command.ZOrder);
        if (insertIndex < 0)
        {
            renderQueue.Add(command);
        }
        else
        {
            renderQueue.Insert(insertIndex, command);
        }
    }
    
    public void DrawTexture(IntPtr texture, SDL.SDL_Rect? sourceRect, SDL.SDL_Rect destRect)
    {
        if (sourceRect.HasValue)
        {
            var sourceRectValue = sourceRect.Value;
            SDL.SDL_RenderCopy(_renderer, texture, ref sourceRectValue, ref destRect);
        }
        else
        {
            SDL.SDL_RenderCopy(_renderer, texture, IntPtr.Zero, ref destRect);
        }
    }
    
    public void Present()
    {
        // Process the entire queue
        foreach (var command in renderQueue)
        {
            switch (command.Type)
            {
                case RenderCommandType.DrawTexture:
                    DrawTexture(command.Texture, command.SourceRect, command.DestRect);
                    break;
                case RenderCommandType.Clear:
                    SDL.SDL_SetRenderDrawColor(_renderer, command.ClearR, command.ClearG, command.ClearB, command.ClearA);
                    SDL.SDL_RenderClear(_renderer);
                    break;
            }
        }

        SDL.SDL_RenderPresent(_renderer);
        renderQueue.Clear();
    }
    
    public void Destroy()
    {
        if (_renderer != IntPtr.Zero)
        {
            SDL.SDL_DestroyRenderer(_renderer);
            _renderer = IntPtr.Zero;
        }
    }

    public IntPtr GetRenderer()
    {
        return _renderer;
    }
}