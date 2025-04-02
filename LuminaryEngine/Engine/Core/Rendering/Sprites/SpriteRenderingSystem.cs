using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class SpriteRenderingSystem
{
    private IntPtr _renderer;
    private ResourceCache _resourceCache;
    private World _world;

    public SpriteRenderingSystem(IntPtr renderer, ResourceCache resourceCache, World world)
    {
        _renderer = renderer;
        _resourceCache = resourceCache;
        _world = world;
    }

    public void Draw()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(SpriteComponent), typeof(TransformComponent)))
        {
            var spriteComponent = entity.GetComponent<SpriteComponent>();
            var transformComponent = entity.GetComponent<TransformComponent>();
            
            IntPtr texture = _resourceCache.GetTexture(spriteComponent.TextureId);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Failed to load texture: {spriteComponent.TextureId}");
                continue;
            }
            
            SDL.SDL_Rect destRect = new SDL.SDL_Rect
            {
                x = (int)transformComponent.Position.X,
                y = (int)transformComponent.Position.Y,
                w = 100,
                h = 100
            };
            
            if (spriteComponent.SourceRect.HasValue)
            {
                var spriteComponentSourceRect = spriteComponent.SourceRect!.Value;
                SDL.SDL_RenderCopy(_renderer, texture, ref spriteComponentSourceRect, ref destRect);
            }
            else
            {
                SDL.SDL_RenderCopy(_renderer, texture, IntPtr.Zero, ref destRect);
            }
        }
    }
}