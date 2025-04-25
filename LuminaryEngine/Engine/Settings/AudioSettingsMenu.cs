using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Settings;

public class AudioSettingsMenu : UIComponent
{
    private int _masterVolume = 50;
    private int _musicVolume = 50;
    private int _sfxVolume = 50;
    private int _selectedOptionIndex = 0;

    private readonly string[] _options = { "Master Volume", "Music Volume", "SFX Volume" };

    public AudioSettingsMenu(int x, int y, int width, int height, int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
    }

    public override void Render(Renderer renderer)
    {
        // Render backdrop
        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.DrawRectangle,
            RectColor = new SDL.SDL_Color() { r = 0, g = 0, b = 0, a = 255 }, // Semi-transparent black
            DestRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height },
            Filled = true,
            ZOrder = ZIndex - 1 // Ensure backdrop is behind menu items
        });

        int offsetY = Y + 10;

        for (int i = 0; i < _options.Length; i++)
        {
            var isSelected = i == _selectedOptionIndex;
            var volume = i switch
            {
                0 => _masterVolume,
                1 => _musicVolume,
                2 => _sfxVolume,
                _ => 0
            };

            var color = isSelected
                ? new SDL.SDL_Color { r = 255, g = 255, b = 0, a = 255 } // Highlighted
                : new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }; // Normal

            if (!IsFocused)
            {
                color = new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 150 }; // Dimmed color when not focused
            }

            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.DrawText,
                Font = ResourceCache.DefaultFont.Handle,
                Text = $"{_options[i]}: {volume}%",
                TextColor = color,
                DestRect = new SDL.SDL_Rect { x = X + 10, y = offsetY, w = Width - 20, h = 30 },
                ZOrder = ZIndex
            });

            offsetY += 40;
        }
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            // Use the InputMappingSystem to handle navigation and selection
            var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                { sdlEvent.key.keysym.scancode });

            foreach (var action in triggeredActions)
            {
                switch (action)
                {
                    case ActionType.MenuUp:
                        _selectedOptionIndex = (_selectedOptionIndex - 1 + _options.Length) % _options.Length;
                        LuminLog.Debug($"Selected: {_options[_selectedOptionIndex]}");
                        break;

                    case ActionType.MenuDown:
                        _selectedOptionIndex = (_selectedOptionIndex + 1) % _options.Length;
                        LuminLog.Debug($"Selected: {_options[_selectedOptionIndex]}");
                        break;

                    case ActionType.MenuLeft:
                        AdjustVolume(-5);
                        break;

                    case ActionType.MenuRight:
                        AdjustVolume(5);
                        break;
                }
            }
        }
    }

    private void AdjustVolume(int delta)
    {
        switch (_selectedOptionIndex)
        {
            case 0:
                _masterVolume = Math.Clamp(_masterVolume + delta, 0, 100);
                LuminLog.Debug($"Master Volume: {_masterVolume}%");
                break;

            case 1:
                _musicVolume = Math.Clamp(_musicVolume + delta, 0, 100);
                LuminLog.Debug($"Music Volume: {_musicVolume}%");
                break;

            case 2:
                _sfxVolume = Math.Clamp(_sfxVolume + delta, 0, 100);
                LuminLog.Debug($"SFX Volume: {_sfxVolume}%");
                break;
        }
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
    }
}