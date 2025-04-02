using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.Exceptions;
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
            
            Texture texture = _resourceCache.GetTexture(spriteComponent.TextureId);
            if (texture == null)
            {
                throw new UnknownTextureException($"Failed to load texture: {spriteComponent.TextureId}");
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
                SDL.SDL_RenderCopy(_renderer, texture.Handle, ref spriteComponentSourceRect, ref destRect);
            }
            else
            {
                SDL.SDL_RenderCopy(_renderer, texture.Handle, IntPtr.Zero, ref destRect);
            }
        }
    }
}