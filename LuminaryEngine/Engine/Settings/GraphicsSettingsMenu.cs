﻿using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Settings;

public class GraphicsSettingsMenu : UIComponent
{
    private string[] _options = { "Resolution", "Fullscreen", "Texture Quality" };
    private int _selectedOptionIndex = 0;

    private string[] _resolutions = { "1920x1080", "1280x720", "800x600" };
    private int _selectedResolutionIndex = 0;

    private bool _isFullscreen = false;
    private string[] _textureQualities = { "Low", "Medium", "High" };
    private int _selectedTextureQualityIndex = 2;

    public GraphicsSettingsMenu(int x, int y, int width, int height,
        int zIndex = int.MaxValue)
        : base(x, y, width, height, zIndex)
    {
    }

    public override void Render(Renderer renderer)
    {
        int offsetY = Y + 10;

        for (int i = 0; i < _options.Length; i++)
        {
            var isSelected = i == _selectedOptionIndex;
            var value = i switch
            {
                0 => _resolutions[_selectedResolutionIndex],
                1 => _isFullscreen ? "Enabled" : "Disabled",
                2 => _textureQualities[_selectedTextureQualityIndex],
                _ => ""
            };

            var color = isSelected
                ? new SDL.SDL_Color { r = 255, g = 255, b = 0, a = 255 } // Highlighted
                : new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }; // Normal

            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.DrawText,
                Font = ResourceCache.DefaultFont.Handle,
                Text = $"{_options[i]}: {value}",
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
                    case ActionType.MoveUp:
                        _selectedOptionIndex = (_selectedOptionIndex - 1 + _options.Length) % _options.Length;
                        break;

                    case ActionType.MoveDown:
                        _selectedOptionIndex = (_selectedOptionIndex + 1) % _options.Length;
                        break;

                    case ActionType.MoveLeft:
                        AdjustOption(false);
                        break;

                    case ActionType.MoveRight:
                        AdjustOption(true);
                        break;

                    case ActionType.Interact: // Use 'Interact' action for toggling fullscreen or selecting options
                        if (_selectedOptionIndex == 1) // Fullscreen toggle
                        {
                            _isFullscreen = !_isFullscreen;
                            Console.WriteLine($"Fullscreen: {(_isFullscreen ? "Enabled" : "Disabled")}");
                        }

                        break;
                }
            }
        }
    }

    private void AdjustOption(bool increase)
    {
        switch (_selectedOptionIndex)
        {
            case 0: // Resolution
                _selectedResolutionIndex = (_selectedResolutionIndex + (increase ? 1 : -1) + _resolutions.Length) %
                                           _resolutions.Length;
                Console.WriteLine($"Resolution set to: {_resolutions[_selectedResolutionIndex]}");
                break;

            case 2: // Texture Quality
                _selectedTextureQualityIndex =
                    (_selectedTextureQualityIndex + (increase ? 1 : -1) + _textureQualities.Length) %
                    _textureQualities.Length;
                Console.WriteLine($"Texture Quality set to: {_textureQualities[_selectedTextureQualityIndex]}");
                break;
        }
    }
}