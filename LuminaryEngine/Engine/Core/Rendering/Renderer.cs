using LuminaryEngine.Engine.Core.Logging;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering;

public class Renderer
{
    private IntPtr _renderer;
    private List<RenderCommand> renderQueue = new List<RenderCommand>();

    private bool _isFading = false;
    private float _fadeDuration = 0f; // Duration of the fade (seconds).
    private float _fadeElapsed = 0f; // Time elapsed since starting fade (seconds).
    private bool _fadeIn = true; // true for fade in (black->clear), false for fade out (clear->black).
    private bool _holdFade = false; // Whether to hold the fade at the end.

    public Renderer(IntPtr window)
    {
        _renderer = SDL.SDL_CreateRenderer(window, -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

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
                    SDL.SDL_SetRenderDrawColor(_renderer, command.ClearR, command.ClearG, command.ClearB,
                        command.ClearA);
                    SDL.SDL_RenderClear(_renderer);
                    break;
                case RenderCommandType.ClearUI:
                    SDL.SDL_SetRenderDrawColor(_renderer, command.ClearR, command.ClearG, command.ClearB,
                        command.ClearA);
                    SDL.SDL_Rect dRect = command.DestRect;
                    ;
                    SDL.SDL_RenderFillRect(_renderer, ref dRect);
                    break;
                case RenderCommandType.DrawText:
                    DrawText(command.Font, command.Text, command.TextColor, command.DestRect);
                    break;
                case RenderCommandType.FadeFrame:
                    RenderFadeOverlay();
                    break;
                case RenderCommandType.FadeFrameHold:
                    RenderBlackOverlay();
                    break;
                case RenderCommandType.DrawRectangle:
                    DrawRectangle(command.DestRect, command.RectColor, command.Filled);
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

    private void DrawText(IntPtr font, string text, SDL.SDL_Color color, SDL.SDL_Rect destRect)
    {
        // Calculate the actual size of the text
        int textWidth, textHeight;
        SDL_ttf.TTF_SizeUTF8(font, text, out textWidth, out textHeight);

        if (textWidth <= 0 || textHeight <= 0)
        {
            return;
        }

        // Adjust the destRect to maintain the aspect ratio
        destRect.w = textWidth;
        destRect.h = textHeight;

        // Render the text
        IntPtr surface = SDL_ttf.TTF_RenderUTF8_Solid(font, text, color);
        if (surface == IntPtr.Zero)
        {
            throw new Exception($"Failed to render text surface: {SDL.SDL_GetError()}");
        }

        IntPtr texture = SDL.SDL_CreateTextureFromSurface(_renderer, surface);
        if (texture == IntPtr.Zero)
        {
            SDL.SDL_FreeSurface(surface);
            throw new Exception($"Failed to create texture from text surface: {SDL.SDL_GetError()}");
        }

        SDL.SDL_FreeSurface(surface);

        // Render the texture
        SDL.SDL_RenderCopy(_renderer, texture, IntPtr.Zero, ref destRect);

        SDL.SDL_DestroyTexture(texture);
    }

    public IntPtr GetRenderer()
    {
        return _renderer;
    }

    public void UpdateFade(float deltaTime)
    {
        if (_isFading)
        {
            _fadeElapsed += deltaTime;
            if (_fadeElapsed >= _fadeDuration)
            {
                _fadeElapsed = _fadeDuration;
                _isFading = false;
            }
        }
    }

    public bool IsFading()
    {
        return _isFading;
    }

    public void StartFade(bool fadeIn, float duration, bool hold)
    {
        _fadeIn = fadeIn;
        _fadeDuration = duration;
        _fadeElapsed = 0f;
        _isFading = true;
        _holdFade = hold;
    }

    public void DrawRectangle(SDL.SDL_Rect rect, SDL.SDL_Color color, bool filled)
    {
        // Set the renderer color
        SDL.SDL_SetRenderDrawColor(_renderer, color.r, color.g, color.b, color.a);

        if (filled)
        {
            // Fill the rectangle
            SDL.SDL_RenderFillRect(_renderer, ref rect);
        }
        else
        {
            // Draw the outline of the rectangle
            SDL.SDL_RenderDrawRect(_renderer, ref rect);
        }

        // Reset the renderer color (optional, depending on your rendering pipeline)
        SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
    }

    public void RenderFade()
    {
        RenderCommand command = new RenderCommand()
        {
            Type = RenderCommandType.FadeFrame,
            ZOrder = float.MaxValue // Ensure fade is rendered last
        };

        EnqueueRenderCommand(command);

        if (_holdFade && !_isFading)
        {
            RenderCommand command2 = new RenderCommand()
            {
                Type = RenderCommandType.FadeFrameHold,
                ZOrder = float.MaxValue // Ensure fade is rendered last
            };

            EnqueueRenderCommand(command2);
        }
    }

    private void RenderFadeOverlay()
    {
        if (!_isFading) return;

        // Calculate fade progress (0.0 to 1.0)
        float progress = _fadeElapsed / _fadeDuration;
        byte alpha;
        // For fade-in: alpha goes from 255 (fully black) to 0 (clear).
        // For fade-out: alpha goes from 0 (clear) to 255 (black).
        if (!_fadeIn)
        {
            alpha = (byte)((1f - progress) * 255);
        }
        else
        {
            alpha = (byte)(progress * 255);
        }

        // Set the renderer to use blended drawing.
        SDL.SDL_SetRenderDrawBlendMode(_renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
        // Set draw color to black with the calculated alpha.
        SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, alpha);

        SDL.SDL_Rect viewport = new SDL.SDL_Rect()
        {
            x = 0,
            y = 0,
            w = 640, // Assuming a fixed width for the viewport
            h = 360 // Assuming a fixed height for the viewport
        };

        // Render a rectangle covering the entire viewport.
        SDL.SDL_RenderFillRect(_renderer, ref viewport);
    }

    private void RenderBlackOverlay()
    {
        // Set the renderer to use blended drawing.
        SDL.SDL_SetRenderDrawBlendMode(_renderer, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
        // Set draw color to black with the calculated alpha.
        SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);

        SDL.SDL_Rect viewport = new SDL.SDL_Rect()
        {
            x = 0,
            y = 0,
            w = 640, // Assuming a fixed width for the viewport
            h = 360 // Assuming a fixed height for the viewport
        };

        // Render a rectangle covering the entire viewport.
        SDL.SDL_RenderFillRect(_renderer, ref viewport);
    }
}