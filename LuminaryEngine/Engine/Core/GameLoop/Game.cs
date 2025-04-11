using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography.Xml;
using LuminaryEngine.Engine.Audio;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.ECS.Systems;
using LuminaryEngine.Engine.Gameplay.Player;
using LuminaryEngine.Engine.Gameplay.UI;
using LuminaryEngine.ThirdParty.LDtk;
using LuminaryEngine.ThirdParty.LDtk.Models;
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
    private AudioManager _audioManager;
    private AudioSystem _audioSystem;
    private TilemapRenderingSystem _tilemapRenderingSystem;
    private AnimationSystem _animationSystem;
    private UISystem _uiSystem;
    
    private Camera _camera;
    
    private Stopwatch _stopwatch;
    private int _frameCount;
    private float _frameRate;
    
    public static readonly int DISPLAY_WIDTH = 640;
    public static readonly int DISPLAY_HEIGHT = 360;

    public Game()
    {
        _isRunning = true;
        _gameTime = new GameTime();
        
        _stopwatch = new Stopwatch();
        _frameCount = 0;
        _frameRate = 0.0f;
        
        _uiSystem = new UISystem();
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
        _window = SDL_CreateWindow("Luminary Engine", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, DISPLAY_WIDTH, DISPLAY_HEIGHT, SDL_WindowFlags.SDL_WINDOW_SHOWN);
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
        _textureLoadingSystem = new TextureLoadingSystem();
        
        // Initialize Audio Manager
        _audioManager = new AudioManager();
        
        // Initialize Resource Cache
        _resourceCache = new ResourceCache(_renderer.GetRenderer(), _textureLoadingSystem, _audioManager);
        
        // Load LDtk World
        LDtkLoadResponse resp = LDtkLoader.LoadProject($"Assets/World/World.ldtk");
        
        // Initialize World
        _world = new World(resp, _renderer);
        
        // Initialize Camera
        _camera = new Camera(0, 0, _world);
        
        // Initialize Systems
        _spriteRenderingSystem = new SpriteRenderingSystem(_renderer, _resourceCache, _camera, _world);
        _keyboardInputSystem = new KeyboardInputSystem(_world);
        _mouseInputSystem = new MouseInputSystem(_world);
        _playerMovementSystem = new PlayerMovementSystem(_world, _gameTime);
        _audioSystem = new AudioSystem(_world, _audioManager);
        _tilemapRenderingSystem = new TilemapRenderingSystem(_renderer, _resourceCache, _camera, _world);
        _animationSystem = new AnimationSystem(_world, _gameTime);
        
        // Start the stopwatch
        _stopwatch.Start();

        return true;
    }
    
    private void InitializeUISystem()
    {
        // Example HUD setup
        var gameplayHUD = new HUDSystem();
        gameplayHUD.AddComponent(new ImageComponent(texture: _resourceCache.GetTexture("health_bar.png"), x: 10, y: 10, width: 200, height: 20));
        gameplayHUD.AddComponent(new TextComponent("Score: 0", font: LoadFont("arial.ttf", 24), color: new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }, x: 10, y: 40, width: 200, height: 30));
        _uiSystem.RegisterHUD("GameplayHUD", gameplayHUD);
        _uiSystem.ActivateHUD("GameplayHUD");

        // Example Menu setup
        var mainMenu = new MenuSystem();
        var startButton = new ButtonComponent("Start", LoadFont("arial.ttf", 24), new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }, new SDL.SDL_Color { r = 0, g = 128, b = 255, a = 255 }, 100, 100, 200, 50);
        startButton.OnClick = () => Console.WriteLine("Game Started!");
        mainMenu.AddComponent(startButton);

        var exitButton = new ButtonComponent("Exit", LoadFont("arial.ttf", 24), new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }, new SDL.SDL_Color { r = 255, g = 0, b = 0, a = 255 }, 100, 160, 200, 50);
        exitButton.OnClick = () => _isRunning = false;
        mainMenu.AddComponent(exitButton);

        _uiSystem.RegisterMenu("MainMenu", mainMenu);
        _uiSystem.ActivateMenu("MainMenu");
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
            CalculateFrameRate();
            UpdateWindowTitle();
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

    protected virtual void Update()
    {
        _gameTime.Update();
        
        _playerMovementSystem.Update();
        _audioSystem.Update();
        
        _renderer.UpdateFade(_gameTime.DeltaTime);
        _world.Update();
        
        _animationSystem.Update();
        
        Entity player = _world.GetEntitiesWithComponents(typeof(PlayerComponent))[0];
        TransformComponent playerTransform = player.GetComponent<TransformComponent>();
        
        _camera.Follow(playerTransform.Position);
    }

    private void Draw()
    {
        _renderer.Clear(0, 0, 0, 255);
        
        _tilemapRenderingSystem.Draw();
        _spriteRenderingSystem.Draw();
        
        // Render the fade overlay if applicable
        _renderer.RenderFade();
        
        _renderer.Present();
    }

    protected virtual void LoadContent()
    {
        
    }

    protected virtual void UnloadContent()
    {
        
    }
    
    private void Shutdown()
    {
        _renderer.Destroy();
        
        SDL_DestroyWindow(_window);
        SDL_image.IMG_Quit();
        SDL_Quit();
    }
    
    private void CalculateFrameRate()
    {
        // Increment frame count
        _frameCount++;

        // Calculate elapsed time
        float elapsedTime = (float)_stopwatch.Elapsed.TotalSeconds;

        // If more than a second has passed, calculate the frame rate
        if (elapsedTime >= 1.0f)
        {
            _frameRate = _frameCount / elapsedTime;

            // Reset frame count and stopwatch
            _frameCount = 0;
            _stopwatch.Restart();
        }
    }

    private void UpdateWindowTitle()
    {
        // Update the window title with the frame rate
        string title = $"Luminary Engine - FPS: {_frameRate:F0}";
        SDL_SetWindowTitle(_window, title);
    }
    
    public ResourceCache ResourceCache => _resourceCache;
    public Renderer Renderer => _renderer;
    public GameTime GameTime => _gameTime;
    public World World => _world;
}