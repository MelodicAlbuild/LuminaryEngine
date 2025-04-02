namespace LuminaryEngine.Engine.Core.GameLoop;

using static SDL2.SDL;

public class Game
{
    private IntPtr _window;
    private IntPtr _renderer;
    private bool _isRunning;

    public Game()
    {
        _isRunning = true;
    }

    private bool Initialize()
    {
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL_Init Error: {SDL_GetError()}");
            return false;
        }
        
        _window = SDL_CreateWindow("Luminary Engine", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1280, 720, SDL_WindowFlags.SDL_WINDOW_SHOWN);
        
        if (_window == IntPtr.Zero)
        {
            Console.WriteLine($"SDL_CreateWindow Error: {SDL_GetError()}");
            return false;
        }

        _renderer = SDL_CreateRenderer(_window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
        
        if (_renderer == IntPtr.Zero)
        {
            Console.WriteLine($"SDL_CreateRenderer Error: {SDL_GetError()}");
            SDL_DestroyWindow(_window);
            return false;
        }

        return true;
    }

    public void Run()
    {
        if(!Initialize())
        {
            Console.WriteLine("Failed to initialize Game.");
            return;
        }
        
        LoadContent();
        
        while(_isRunning) 
        {
            HandleEvents();
            Update();
            Draw();
        }
        
        UnloadContent();
        Shutdown();
    }

    private void HandleEvents()
    {
        SDL_Event e;
        while (SDL_PollEvent(out e) != 0)
        {
            if (e.type == SDL_EventType.SDL_QUIT)
            {
                _isRunning = false;
            }
            
            // TODO: Handle Input System
        }
    }

    private void Update()
    {
        // TODO: Handle Update
    }

    private void Draw()
    {
        SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
        SDL_RenderClear(_renderer);
        
        // TODO: Handle Other Rendering
        
        SDL_RenderPresent(_renderer);
    }

    private void LoadContent()
    {
        // TODO: Handle Load
    }

    private void UnloadContent()
    {
        // TODO: Handle Unload
    }
    
    private void Shutdown()
    {
        SDL_DestroyRenderer(_renderer);
        SDL_DestroyWindow(_window);
        SDL_Quit();
    }
}