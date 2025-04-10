using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.Exceptions;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class SpriteRenderingSystem : LuminSystem
{
    private Renderer _renderer;
    private ResourceCache _resourceCache;
    private Camera _camera;

    public SpriteRenderingSystem(Renderer renderer, ResourceCache resourceCache, Camera camera, World world) : base(world)
    {
        _renderer = renderer;
        _resourceCache = resourceCache;
        _camera = camera;
    }

    public void Draw()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(SpriteComponent), typeof(TransformComponent)))
        {
            var spriteComponent = entity.GetComponent<SpriteComponent>();
            var transformComponent = entity.GetComponent<TransformComponent>();
            
            if (entity.HasComponent<AnimationComponent>())
            {
                var animationComponent = entity.GetComponent<AnimationComponent>();
                if (animationComponent.State != null)
                {
                    // Use the current frame's source rectangle from the animation
                    var animation = animationComponent.Animations[animationComponent.State.CurrentAnimation];
                    spriteComponent.SourceRect = animation.Frames[animationComponent.State.CurrentFrame];
                }
                else
                {
                    if (animationComponent.DeadAnim == 1)
                    {
                        spriteComponent.SourceRect = animationComponent.Frame;
                    }
                    else
                    {
                        spriteComponent.SourceRect = spriteComponent.SourceRect;
                    }
                }
            }
            
            Texture texture = _resourceCache.GetTexture(spriteComponent.TextureId);
            if (texture == null)
            {
                throw new UnknownTextureException($"Failed to load texture: {spriteComponent.TextureId}");
            }
            
            SDL.SDL_Rect destRect = new SDL.SDL_Rect
            {
                x = (int)Math.Floor(transformComponent.Position.X) - (int)_camera.X,
                y = (int)Math.Floor(transformComponent.Position.Y) - 16 - (int)_camera.Y,
                w = (int)(spriteComponent.SourceRect.Value.w * transformComponent.Scale.X),
                h = (int)(spriteComponent.SourceRect.Value.h * transformComponent.Scale.Y)
            };

            RenderCommand command = new RenderCommand()
            {
                Type = RenderCommandType.DrawTexture,
                Texture = texture.Handle,
                SourceRect = spriteComponent.SourceRect,
                DestRect = destRect,
                ZOrder = spriteComponent.ZIndex
            };
            
            _renderer.EnqueueRenderCommand(command);
        }
    }

    public override void Update()
    {
        
    }
}