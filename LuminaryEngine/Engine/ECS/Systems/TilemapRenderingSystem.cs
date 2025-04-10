﻿using System.Numerics;
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

    public TilemapRenderingSystem(Renderer renderer, ResourceCache resourceCache, Camera camera, World world) : base(world)
    {
        _renderer = renderer;
        _resourceCache = resourceCache;
        _camera = camera;
    }

    public void Draw()
    {
        bool first = true;
        
        // Get the LDtk project from the world
        LDtkProject ldtkWorld = _world.GetLDtkWorld();
        foreach (var level in ldtkWorld.Levels)
        {
            foreach (var layer in level.LayerInstances)
            {
                if (layer.Type != "Tiles") continue;

                foreach (var tile in layer.GridTiles)
                {
                    // Get the texture for the tile
                    Texture texture = _resourceCache.GetSpritesheet(layer.TilesetRelPath.Split('/')[layer.TilesetRelPath.Split('/').Length - 1]);
                    if (texture == null)
                    {
                        throw new UnknownTextureException($"Failed to load texture: {layer.TilesetUid}");
                    }

                    if (first)
                    {
                        Console.WriteLine("Tile 0,0 PositionPx: " + tile.PositionPx[0] + ", " + tile.PositionPx[1]);
                        Console.WriteLine("Camera: (" + _camera.X + ", " + _camera.Y + ")");

                        first = false;
                    }

                    // Calculate the destination rectangle for the tile
                    SDL.SDL_Rect destRect = new SDL.SDL_Rect
                    {
                        x = tile.PositionPx[0] - _camera.X,
                        y = tile.PositionPx[1] - _camera.Y,
                        w = 16,
                        h = 16
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
                            w = 16,
                            h = 16
                        },
                        DestRect = destRect,
                        ZOrder = layer.LayerDefUid
                    };

                    // Enqueue the render command
                    _renderer.EnqueueRenderCommand(command);
                }
            }
        }
    }

    public override void Update()
    {
        // No update logic needed for this system
    }
}