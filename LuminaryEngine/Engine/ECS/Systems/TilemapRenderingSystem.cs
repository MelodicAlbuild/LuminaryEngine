using System.Numerics;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.Exceptions;
using LuminaryEngine.ThirdParty.LDtk.Models;
using SDL2;

namespace LuminaryEngine.Engine.ECS.Systems;

public class TilemapRenderingSystem : LuminSystem
{
    private Renderer _renderer;
    private ResourceCache _resourceCache;
    private Camera _camera;

    public TilemapRenderingSystem(Renderer renderer, ResourceCache resourceCache, Camera camera, World world) :
        base(world)
    {
        _renderer = renderer;
        _resourceCache = resourceCache;
        _camera = camera;
    }

    public void Draw()
    {
        if (_world.GetCurrentLevelId() < 0) return;

        Texture black = _resourceCache.GetTexture("black.png");

        // Get the LDtk project from the world
        LDtkProject ldtkWorld = _world.GetLDtkWorld();

        // Get the current level from the world
        LDtkLevel currentLevel = _world.GetCurrentLevel();

        foreach (var layer in currentLevel.LayerInstances)
        {
            if (layer.Type != "Tiles") continue;

            foreach (var tile in layer.GridTiles)
            {
                // Get the texture for the tile
                Texture texture =
                    _resourceCache.GetSpritesheet(
                        layer.TilesetRelPath.Split('/')[layer.TilesetRelPath.Split('/').Length - 1]);
                if (texture == null)
                {
                    throw new UnknownTextureException($"Failed to load texture: {layer.TilesetUid}");
                }

                // Calculate the destination rectangle for the tile
                SDL.SDL_Rect destRect = new SDL.SDL_Rect
                {
                    x = (int)Math.Round((tile.PositionPx[0] - _camera.X), MidpointRounding.AwayFromZero),
                    y = (int)Math.Round((tile.PositionPx[1] - _camera.Y), MidpointRounding.AwayFromZero),
                    w = 32,
                    h = 32
                };

                // Create the render command
                RenderCommand command = new RenderCommand()
                {
                    Type = RenderCommandType.DrawTexture,
                    Texture = texture.Handle,
                    SourceRect = new SDL.SDL_Rect
                    {
                        x = tile.SrcRect[0],
                        y = tile.SrcRect[1],
                        w = 32,
                        h = 32
                    },
                    DestRect = destRect,
                    ZOrder = ldtkWorld.Definitions.LayerDefs.Find(o => o.Uid == layer.LayerDefUid)!.Doc.zIndex
                };

                // Enqueue the render command
                _renderer.EnqueueRenderCommand(command);

                if (_world.IsTileSolid(tile.PositionPx[0] / 32, tile.PositionPx[1] / 32) &&
                    DevModeConfig.ShowCollisionBoxes)
                {
                    RenderCommand command2 = new RenderCommand()
                    {
                        Type = RenderCommandType.DrawTexture,
                        Texture = black.Handle,
                        DestRect = destRect,
                        ZOrder = 250
                    };

                    // Enqueue the render command
                    _renderer.EnqueueRenderCommand(command2);
                }
            }
        }
    }

    public override void Update()
    {
        // No update logic needed for this system
    }
}