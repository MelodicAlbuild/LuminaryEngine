using System.Diagnostics;
using System.Numerics;
using LuminaryEngine.Engine.Audio;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Fonts;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.ECS.Systems;
using LuminaryEngine.Engine.Gameplay.Crafting;
using LuminaryEngine.Engine.Gameplay.Dialogue;
using LuminaryEngine.Engine.Gameplay.Items;
using LuminaryEngine.Engine.Gameplay.Player;
using LuminaryEngine.Engine.Gameplay.SaveLoad;
using LuminaryEngine.Engine.Gameplay.Spirits;
using LuminaryEngine.Engine.Gameplay.UI;
using LuminaryEngine.Engine.Settings;
using LuminaryEngine.ThirdParty.LDtk;
using SDL2;

namespace LuminaryEngine.Engine.Core.GameLoop;

using static SDL2.SDL;

public class Game
{
    private IntPtr _window;
    private bool _isRunning;
    private World _world;
    private SaveData _saveData;
    private SaveLoadSystem _saveLoadSystem;

    private Renderer _renderer;
    private ResourceCache _resourceCache;
    private GameTime _gameTime;
    private SpriteRenderingSystem _spriteRenderingSystem;
    private TextureLoadingSystem _textureLoadingSystem;
    private FontLoadingSystem _fontLoadingSystem;
    private KeyboardInputSystem _keyboardInputSystem;
    private MouseInputSystem _mouseInputSystem;
    private PlayerMovementSystem _playerMovementSystem;
    private AudioManager _audioManager;
    private AudioSystem _audioSystem;
    private TilemapRenderingSystem _tilemapRenderingSystem;
    private AnimationSystem _animationSystem;
    private UISystem _uiSystem;
    private DialogueBox _dialogueBox;

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
        _saveLoadSystem = new SaveLoadSystem();

        if (_saveLoadSystem.SaveExists("BaseSave.lumsav") && !DevModeConfig.IgnoreSaves)
        {
            _saveData = _saveLoadSystem.LoadGame("BaseSave.lumsav");
        }
    }

    private bool Initialize()
    {
        // Initialize SDL
        if (SDL_Init(SDL_INIT_VIDEO) < 0)
        {
            LuminLog.Error($"SDL_Init Error: {SDL_GetError()}");
            return false;
        }

        // Create Window
        _window = SDL_CreateWindow("Luminary Engine", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, DISPLAY_WIDTH,
            DISPLAY_HEIGHT, SDL_WindowFlags.SDL_WINDOW_SHOWN);
        if (_window == IntPtr.Zero)
        {
            LuminLog.Error($"SDL_CreateWindow Error: {SDL_GetError()}");
            return false;
        }

        // Create Renderer
        _renderer = new Renderer(_window);

        // Initialize SDL_image
        if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) < 0)
        {
            LuminLog.Error($"SDL_image_Init Error: {SDL_image.IMG_GetError()}");
            _renderer.Destroy();
            SDL_DestroyWindow(_window);
            return false;
        }

        // Initialize SDL_ttf
        if (SDL_ttf.TTF_Init() < 0)
        {
            LuminLog.Error($"SDL_ttf_Init Error: {SDL_ttf.TTF_GetError()}");
            SDL_image.IMG_Quit();
            _renderer.Destroy();
            SDL_DestroyWindow(_window);
            return false;
        }

        // Initialize SDL_mixer
        if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) < 0)
        {
            LuminLog.Error($"SDL_mixer_Init Error: {SDL_mixer.Mix_GetError()}");
            SDL_ttf.TTF_Quit();
            SDL_image.IMG_Quit();
            _renderer.Destroy();
            SDL_DestroyWindow(_window);
            return false;
        }

        // SDL Hints
        SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_SCALE_QUALITY, "0");

        // Initialize Texture Loading System
        _textureLoadingSystem = new TextureLoadingSystem();

        // Font Loading System
        _fontLoadingSystem = new FontLoadingSystem();

        // Initialize Audio Manager
        _audioManager = new AudioManager();

        // Initialize Resource Cache
        _resourceCache = new ResourceCache(_renderer.GetRenderer(), _textureLoadingSystem, _fontLoadingSystem,
            _audioManager);

        _resourceCache.GetFont("Pixel-Upd", 36);

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

        #region Setup Default Keybinds

        InputMappingSystem.Instance.MapActionToKey(ActionType.MoveUp, SDL_Scancode.SDL_SCANCODE_W);
        InputMappingSystem.Instance.MapActionToKey(ActionType.MoveDown, SDL_Scancode.SDL_SCANCODE_S);
        InputMappingSystem.Instance.MapActionToKey(ActionType.MoveLeft, SDL_Scancode.SDL_SCANCODE_A);
        InputMappingSystem.Instance.MapActionToKey(ActionType.MoveRight, SDL_Scancode.SDL_SCANCODE_D);

        InputMappingSystem.Instance.MapActionToKey(ActionType.MenuUp, SDL_Scancode.SDL_SCANCODE_UP);
        InputMappingSystem.Instance.MapActionToKey(ActionType.MenuDown, SDL_Scancode.SDL_SCANCODE_DOWN);
        InputMappingSystem.Instance.MapActionToKey(ActionType.MenuLeft, SDL_Scancode.SDL_SCANCODE_LEFT);
        InputMappingSystem.Instance.MapActionToKey(ActionType.MenuRight, SDL_Scancode.SDL_SCANCODE_RIGHT);

        InputMappingSystem.Instance.MapActionToKey(ActionType.Interact, SDL_Scancode.SDL_SCANCODE_E);
        InputMappingSystem.Instance.MapActionToKey(ActionType.OpenOptions, SDL_Scancode.SDL_SCANCODE_P);
        InputMappingSystem.Instance.MapActionToKey(ActionType.OpenInventory, SDL_Scancode.SDL_SCANCODE_I);

        InputMappingSystem.Instance.MapActionToKey(ActionType.Escape, SDL_Scancode.SDL_SCANCODE_ESCAPE);
        
        #endregion

        if (_saveData != null)
        {
            _world.LoadInteractionData(_saveData.InteractionData);
            _world.SwitchLevel(_saveData.CurrentMap, Vector2.One, false);
        }
        else
        {
            _world.SwitchLevel(0, Vector2.One, false);
        }

        return true;
    }

    public void Run()
    {
        if (!Initialize())
        {
            LuminLog.Error("Failed to initialize Game.");
            return;
        }

        LoadContent();

        while (_isRunning)
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
            _uiSystem.HandleEvent(e);

            if (e.type == SDL_EventType.SDL_KEYDOWN)
            {
                foreach (Entity entitiesWithComponent in _world.GetEntitiesWithComponents(typeof(PlayerComponent)))
                {
                    if (!_dialogueBox.IsVisible)
                    {
                        entitiesWithComponent.GetComponent<PlayerComponent>().HandleInput(e);
                    }
                }

                if (e.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_0)
                {
                    _world.GetEntitiesWithComponents(typeof(PlayerComponent))[0].GetComponent<InventoryComponent>()
                        .AddItem("basic_item");
                }

                if (e.key.keysym.scancode == SDL_Scancode.SDL_SCANCODE_1)
                {
                    LuminLog.Debug("Inventory:");
                    foreach (var keyValuePair in _world.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                 .GetComponent<InventoryComponent>().GetInventory())
                    {
                        LuminLog.Debug(keyValuePair.Key + ": " + keyValuePair.Value);
                    }
                }
            }
        }
    }

    protected virtual void Update()
    {
        _gameTime.Update();

        _playerMovementSystem.Update();
        _audioSystem.Update();

        _dialogueBox.Update(_gameTime.DeltaTime);

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

        _uiSystem.Render(_renderer);

        // Render the fade overlay if applicable
        _renderer.RenderFade();

        _renderer.Present();
    }

    protected virtual void LoadContent()
    {
        ItemManager.Instance.LoadItems();
        SpiritEssenceManager.Instance.LoadSpiritEssence();

        new CraftingSystem();

        MenuSystem settingsMenuSystem = new MenuSystem();
        settingsMenuSystem.AddComponent(new SettingsMenu(5, 55, 630, 250));
        _uiSystem.RegisterMenu("Settings", settingsMenuSystem);

        DialogueBox dialogueBox = new DialogueBox(125, 255, 390, 85);
        _dialogueBox = dialogueBox;
        _dialogueBox.SetVisible(false);
        _uiSystem.AddUIComponent(dialogueBox);
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

        UpdateSave();
        _saveLoadSystem.SaveGame(_saveData, "BaseSave.lumsav");

        LuminLog.FinalizeLog();
    }

    private void UpdateSave()
    {
        if (_saveData == null)
        {
            _saveData = new SaveData();
        }

        Entity player = _world.GetEntitiesWithComponents(typeof(PlayerComponent))[0];
        _saveData.InventoryItems = player.GetComponent<InventoryComponent>().GetInventory();
        _saveData.SpiritEssences = player.GetComponent<InventoryComponent>().GetSpiritEssences();
        _saveData.SavePosition(player.GetComponent<TransformComponent>().Position);
        _saveData.CurrentMap = _world.GetCurrentLevelId();
        _saveData.InteractionData = _world.GetInteractionData();
        _saveData.PlayerFacingDirection = _playerMovementSystem.GetDirection();
        _saveData.SaveTimestamp = DateTime.UtcNow;
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
    public PlayerMovementSystem PlayerMovementSystem => _playerMovementSystem;
    public DialogueBox DialogueBox => _dialogueBox;
    public UISystem UISystem => _uiSystem;
    public SaveData SaveData => _saveData;
}