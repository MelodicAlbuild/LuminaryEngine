using System.Numerics;
using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Input;

public class InputStateComponent : IComponent
{
    public HashSet<SDL.SDL_Scancode> PressedKeys { get; set; } = new HashSet<SDL.SDL_Scancode>();
    public HashSet<byte> PressedMouseButtons { get; set; } = new HashSet<byte>();
    public Vector2 MousePosition { get; set; } = Vector2.Zero;
}