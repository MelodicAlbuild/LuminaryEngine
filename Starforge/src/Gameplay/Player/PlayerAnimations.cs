using LuminaryEngine.Engine.Core.Rendering.Sprites;
using SDL2;

namespace Starforge.Gameplay.Player;

public class PlayerAnimations
{
    public static List<Animation> WalkAnimations = new List<Animation>()
    {
        // Down
        new Animation(
            "WalkDown",
            [
                new SDL.SDL_Rect { x = 0, y = 0, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 32, y = 0, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 64, y = 0, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 96, y = 0, w = 32, h = 48 }
            ],
            0.2f, // Frame duration in seconds
            true  // Looping
        ),
        // Left
        new Animation(
            "WalkLeft",
            [
                new SDL.SDL_Rect { x = 0, y = 48, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 32, y = 48, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 64, y = 48, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 96, y = 48, w = 32, h = 48 }
            ],
            0.2f, // Frame duration in seconds
            true  // Looping
        ),
        // Right
        new Animation(
            "WalkRight",
            [
                new SDL.SDL_Rect { x = 0, y = 96, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 32, y = 96, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 64, y = 96, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 96, y = 96, w = 32, h = 48 }
            ],
            0.2f, // Frame duration in seconds
            true  // Looping
        ),
        // Up
        new Animation(
            "WalkUp",
            [
                new SDL.SDL_Rect { x = 0, y = 144, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 32, y = 144, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 64, y = 144, w = 32, h = 48 },
                new SDL.SDL_Rect { x = 96, y = 144, w = 32, h = 48 }
            ],
            0.2f, // Frame duration in seconds
            true  // Looping
        )
    };
}