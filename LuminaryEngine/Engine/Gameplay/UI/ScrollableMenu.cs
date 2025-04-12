using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class ScrollableMenuWithBackdrop : UIComponent
    {
        private List<string> _options;
        private int _selectedOptionIndex;
        private int _scrollOffset;
        private int _visibleOptionsCount; // Number of options visible at a time

        private SDL.SDL_Color _backdropColor = new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 150 }; // Semi-transparent black

        public ScrollableMenuWithBackdrop(int x, int y, int width, int height, List<string> options, int visibleOptionsCount, int zIndex = int.MaxValue)
            : base(x, y, width, height, zIndex)
        {
            _options = options;
            _selectedOptionIndex = 0;
            _scrollOffset = 0;
            _visibleOptionsCount = visibleOptionsCount;
        }

        public override void Render(Renderer renderer)
        {
            // Render backdrop
            renderer.EnqueueRenderCommand(new RenderCommand
            {
                Type = RenderCommandType.DrawRectangle,
                RectColor = _backdropColor,
                DestRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height },
                Filled = true,
                ZOrder = ZIndex - 1 // Ensure backdrop is behind menu items
            });

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

                Console.WriteLine(ResourceCache.DefaultFont.Handle);
                
                renderer.EnqueueRenderCommand(new RenderCommand
                {
                    Type = RenderCommandType.DrawText,
                    Font = ResourceCache.DefaultFont.Handle,
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
            // Use InputMappingSystem to determine triggered actions
            if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN || sdlEvent.type == SDL.SDL_EventType.SDL_MOUSEWHEEL)
            {
                var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode> { sdlEvent.key.keysym.scancode });

                foreach (var action in triggeredActions)
                {
                    switch (action)
                    {
                        case ActionType.MoveUp:
                            MoveSelectionUp();
                            break;

                        case ActionType.MoveDown:
                            MoveSelectionDown();
                            break;

                        case ActionType.MoveLeft: // Optional: Could handle left-navigation or adjustments
                            break;

                        case ActionType.MoveRight: // Optional: Could handle right-navigation or adjustments
                            break;

                        case ActionType.Interact:
                            SelectOption();
                            break;
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
            Console.WriteLine($"Moved selection up to: {_options[_selectedOptionIndex]}");
        }

        private void MoveSelectionDown()
        {
            _selectedOptionIndex = Math.Min(_options.Count - 1, _selectedOptionIndex + 1);
            UpdateScrollOffset();
            Console.WriteLine($"Moved selection down to: {_options[_selectedOptionIndex]}");
        }

        private void SelectOption()
        {
            Console.WriteLine($"Selected option: {_options[_selectedOptionIndex]}");
            // Add logic to handle selecting the currently highlighted option
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
    }