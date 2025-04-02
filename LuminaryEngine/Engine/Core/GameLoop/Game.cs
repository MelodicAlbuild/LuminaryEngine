using LuminaryEngine.Engine.Audio;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.ECS.Systems;
using SDL2;

namespace LuminaryEngine.Engine.Core.GameLoop;

using static SDL2.SDL;

public class Game
{
    private IntPtr _window;
    private bool _isRunning;
    private World _world;
    
    private Renderer _renderer;
    private ResourceCache _resourceCache;
    private GameTime _gameTime;
    private SpriteRenderingSystem _spriteRenderingSystem;
    private TextureLoadingSystem _textureLoadingSystem;
    private KeyboardInputSystem _keyboardInputSystem;
    private MouseInputSystem _mouseInputSystem;
    private PlayerMovementSystem _playerMovementSystem;
    private CameraSystem _cameraSystem;
    private AudioManager _audioManager;
    private AudioSystem _audioSystem;

    public Game()
    {
        _isRunning = true;
        _world = new World();
        _gameTime = new GameTime();
    }

    private bool Initialize()
    {
        // Initialize SDL
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            Console.WriteLine($"SDL_Init Error: {SDL_GetError()}");
            return false;
        }
        
        // Create Window
        _window = SDL_CreateWindow("Luminary Engine", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1280, 720, SDL_WindowFlags.SDL_WINDOW_SHOWN);
        if (_window == IntPtr.Zero)
        {
            Console.WriteLine($"SDL_CreateWindow Error: {SDL_GetError()}");
            return false;
        }

        // Create Renderer
        _renderer = new Renderer(_window);

        // Initialize SDL_image
        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) < 0)
        {
            Console.WriteLine($"SDL_image_Init Error: {SDL_image.IMG_GetError()}");
            _renderer.Destroy();
            SDL_DestroyWindow(_window);
            return false;
        }
        
        // Initialize Texture Loading System
        _textureLoadingSystem = new TextureLoadingSystem(_world);
        
        // Initialize Audio Manager
        _audioManager = new AudioManager();
        
        // Initialize Resource Cache
        _resourceCache = new ResourceCache(_renderer.GetRenderer(), _textureLoadingSystem, _audioManager);
        
        // Initialize Systems
        _spriteRenderingSystem = new SpriteRenderingSystem(_renderer, _resourceCache, _world);
        _keyboardInputSystem = new KeyboardInputSystem(_world);
        _mouseInputSystem = new MouseInputSystem(_world);
        _playerMovementSystem = new PlayerMovementSystem(_world);
        _cameraSystem = new CameraSystem(_world, _gameTime);
        _audioSystem = new AudioSystem(_world, _audioManager);

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
            
            _keyboardInputSystem.HandleEvents(e);
            _mouseInputSystem.HandleEvents(e);
        }
    }

    private void Update()
    {
        _playerMovementSystem.Update();
        _cameraSystem.Update();
        _audioSystem.Update();
    }

    private void Draw()
    {
        _renderer.Clear(0, 0, 0, 255);
        
        _spriteRenderingSystem.Draw();
        
        _renderer.Present();
    }

    private void LoadContent()
    {
        Sound bgmMain = _resourceCache.GetSound("background_music.mp3");
        Entity backgroundMusicEntity = _world.CreateEntity();
        backgroundMusicEntity.AddComponent(new AudioSource("background_music.mp3") { PlayOnAwake = true, Volume = 0.7f, Loop = true });
        
        Entity player = _world.CreateEntity();
        player.AddComponent(new TransformComponent(100, 100));
        player.AddComponent(new SpriteComponent("player.png", 1));
        player.AddComponent(new InputStateComponent());
        
        Entity camera = _world.CreateEntity();
        camera.AddComponent(new TransformComponent(0, 0));
        camera.AddComponent(new CameraComponent());
    }

    private void UnloadContent()
    {
        // TODO: Handle Unload
    }
    
    private void Shutdown()
    {
        _renderer.Destroy();
        
        SDL_DestroyWindow(_window);
        SDL_image.IMG_Quit();
        SDL_Quit();
    }
}