using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering;

public class Renderer
{
    private IntPtr _renderer;
    private Queue<RenderCommand> _commandQueue = new Queue<RenderCommand>();
    
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
        SDL.SDL_SetRenderDrawColor(_renderer, r, g, b, a);
        SDL.SDL_RenderClear(_renderer);
    }
    
    public void EnqueueRenderCommand(RenderCommand command)
    {
        _commandQueue.Enqueue(command);
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
        while (_commandQueue.Count > 0) // Check if the queue is not empty
        {
            RenderCommand command = _commandQueue.Dequeue(); // Use Dequeue

            switch (command.Type)
            {
                case RenderCommandType.DrawTexture:
                    DrawTexture(command.Texture, command.SourceRect, command.DestRect);
                    break;
                // Handle other command types here
            }
        }

        SDL.SDL_RenderPresent(_renderer);
    }
    
    public void Destroy()
    {
        if (_renderer != IntPtr.Zero)
        {
            SDL.SDL_DestroyRenderer(_renderer);
            _renderer = IntPtr.Zero;
        }
    }
}