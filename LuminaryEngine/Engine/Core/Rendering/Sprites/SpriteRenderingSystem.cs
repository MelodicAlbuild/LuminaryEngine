using System.Numerics;
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

    public SpriteRenderingSystem(Renderer renderer, ResourceCache resourceCache, World world) : base(world)
    {
        _renderer = renderer;
        _resourceCache = resourceCache;
    }

    public void Draw()
    {
        // Get the camera position
        Vector2 cameraPosition = Vector2.Zero;
        foreach (var cameraEntity in _world.GetEntitiesWithComponents(typeof(CameraComponent)))
        {
            var transformComponent = cameraEntity.GetComponent<TransformComponent>();
            cameraPosition = transformComponent.Position;
            break; // Assuming only one camera
        }
        
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(SpriteComponent), typeof(TransformComponent)))
        {
            var spriteComponent = entity.GetComponent<SpriteComponent>();
            var transformComponent = entity.GetComponent<TransformComponent>();
            
            Texture texture = _resourceCache.GetTexture(spriteComponent.TextureId);
            if (texture == null)
            {
                throw new UnknownTextureException($"Failed to load texture: {spriteComponent.TextureId}");
            }
            
            int scaledWidth = (int)(texture.Width * transformComponent.Scale.X);
            int scaledHeight = (int)(texture.Height * transformComponent.Scale.Y);
            
            Vector2 screenPosition = transformComponent.Position - cameraPosition;
            
            SDL.SDL_Rect destRect = new SDL.SDL_Rect
            {
                x = (int)screenPosition.X,
                y = (int)screenPosition.Y,
                w = scaledWidth,
                h = scaledHeight
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