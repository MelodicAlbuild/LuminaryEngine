using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Fonts;
using LuminaryEngine.Engine.Core.ResourceManagement;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class ScrollableMenu : UIComponent
{
    private List<string> _options;
    private int _selectedOptionIndex;
    private int _scrollOffset;
    private int _visibleOptionsCount; // Number of options visible at a time
    private bool _hasSelectedMenuItem = false; // Flag to check if a menu item has been selected
    
    private Font _defaultFont;

    private bool _horizontal;
    private bool _interactive;

    private SDL.SDL_Color _backdropColor = new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 150 }; // Semi-transparent black

    public ScrollableMenu(int x, int y, int width, int height, List<string> options, int visibleOptionsCount,
        int zIndex = int.MaxValue, bool horizontal = false, bool interactive = false)
        : base(x, y, width, height, zIndex)
    {
        _interactive = interactive;
        _horizontal = horizontal;
        _options = options;
        _selectedOptionIndex = 0;
        _scrollOffset = 0;
        _visibleOptionsCount = visibleOptionsCount;
        _defaultFont = ResourceCache.DefaultFont;
    }

    public override void Render(Renderer renderer)
    {
        if (_horizontal)
        {
            RenderHorizontal(renderer);
        }
        else
        {
            RenderVertical(renderer);
        }
    }

    private void RenderHorizontal(Renderer renderer)
    {
        // Render visible options
        int offsetX = X + 10;
        for (int i = 0; i < _visibleOptionsCount; i++)
        {
            int optionIndex = _scrollOffset + i;

            if (optionIndex >= _options.Count)
                break;

            var isSelected = optionIndex == _selectedOptionIndex;
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
                Font = _defaultFont.Handle,
                Text = _options[optionIndex],
                TextColor = color,
                DestRect = new SDL.SDL_Rect { x = offsetX, y = Y + 5, w = Width - 20, h = 30 },
                ZOrder = ZIndex
            });

            int w, h;
            SDL_ttf.TTF_SizeText(_defaultFont.Handle, _options[optionIndex], out w, out h);

            offsetX += w + 10; // Space between menu items
        }
    }

    private void RenderVertical(Renderer renderer)
    {
        // Render visible options
        int offsetY = Y + 10;
        for (int i = 0; i < _visibleOptionsCount; i++)
        {
            int optionIndex = _scrollOffset + i;

            if (optionIndex >= _options.Count)
                break;

            var isSelected = optionIndex == _selectedOptionIndex;
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
                Font = _defaultFont.Handle,
                Text = _options[optionIndex],
                TextColor = color,
                DestRect = new SDL.SDL_Rect { x = X + 10, y = offsetY, w = Width - 20, h = 30 },
                ZOrder = ZIndex
            });

            offsetY += 40; // Space between menu items
        }
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (_hasSelectedMenuItem) return;

        // Use InputMappingSystem to determine triggered actions
        if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN || sdlEvent.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
        {
            var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                { sdlEvent.key.keysym.scancode });

            if (_horizontal)
            {
                foreach (var action in triggeredActions)
                {
                    switch (action)
                    {
                        case ActionType.MenuLeft:
                            MoveSelectionUp();
                            break;

                        case ActionType.MenuRight:
                            MoveSelectionDown();
                            break;

                        case ActionType.Interact:
                            if (_interactive)
                            {
                                SelectOption();
                            }

                            break;
                    }
                }
            }
            else
            {
                foreach (var action in triggeredActions)
                {
                    switch (action)
                    {
                        case ActionType.MenuUp:
                            MoveSelectionUp();
                            break;

                        case ActionType.MenuDown:
                            MoveSelectionDown();
                            break;

                        case ActionType.Interact:
                            if (_interactive)
                            {
                                SelectOption();
                            }

                            break;
                    }
                }
            }

            // Handle mouse wheel scrolling
            if (sdlEvent.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
            {
                if (sdlEvent.wheel.y > 0) // Scroll up
                    MoveSelectionUp();
                else if (sdlEvent.wheel.y < 0) // Scroll down
                    MoveSelectionDown();
            }
        }
    }

    private void MoveSelectionUp()
    {
        _selectedOptionIndex = Math.Max(0, _selectedOptionIndex - 1);
        UpdateScrollOffset();
        LuminLog.Debug($"Moved selection up to: {_options[_selectedOptionIndex]}");
    }

    private void MoveSelectionDown()
    {
        _selectedOptionIndex = Math.Min(_options.Count - 1, _selectedOptionIndex + 1);
        UpdateScrollOffset();
        LuminLog.Debug($"Moved selection down to: {_options[_selectedOptionIndex]}");
    }

    private void SelectOption()
    {
        LuminLog.Debug($"Selected option: {_options[_selectedOptionIndex]}");
        _hasSelectedMenuItem = true; // Set the flag to indicate a menu item has been selected
        // Add logic to handle selecting the currently highlighted option
    }

    public bool HasSelectedMenuItem()
    {
        return _hasSelectedMenuItem;
    }

    public void FreeOption()
    {
        _hasSelectedMenuItem = false;
    }

    private void UpdateScrollOffset()
    {
        // Ensure the selected option is always visible
        if (_selectedOptionIndex < _scrollOffset)
        {
            _scrollOffset = _selectedOptionIndex;
        }
        else if (_selectedOptionIndex >= _scrollOffset + _visibleOptionsCount)
        {
            _scrollOffset = _selectedOptionIndex - _visibleOptionsCount + 1;
        }
    }

    public string GetSelectedOption()
    {
        if (_selectedOptionIndex >= 0 && _selectedOptionIndex < _options.Count)
        {
            return _options[_selectedOptionIndex];
        }

        return null; // Return null if no valid option is selected
    }

    public void UpdateCurrentOption(string opt)
    {
        _options[_selectedOptionIndex] = opt;
    }

    public override void SetFocus(bool focus)
    {
        IsFocused = focus;
    }
    
    // New Methods
    public void AddOption(string option)
    {
        _options.Add(option);
    }

    public void Clear()
    {
        _options.Clear();
        _selectedOptionIndex = 0;
        _scrollOffset = 0;
    }

    public bool HasSelection()
    {
        return _selectedOptionIndex >= 0 && _selectedOptionIndex < _options.Count;
    }
    
    public void SetFont(Font font)
    {
        _defaultFont = font;
    }
}