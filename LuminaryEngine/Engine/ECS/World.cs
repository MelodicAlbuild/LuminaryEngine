using System.Numerics;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.Exceptions;
using LuminaryEngine.Engine.Gameplay.Player;
using LuminaryEngine.Extras;
using LuminaryEngine.ThirdParty.LDtk;
using LuminaryEngine.ThirdParty.LDtk.Models;

namespace LuminaryEngine.Engine.ECS;

public class World
{
    private Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
    private int _nextEntityId = 0;
    
    private int _currentLevelId = 0;

    private LDtkProject _ldtkWorld;
    private Dictionary<int, int[,]> _collisionMaps;
    private Dictionary<int, List<Vector2>> _entityMaps;
    
    private bool _isTransitioning = false;
    private bool _hasFaded = true;
    
    private Renderer _renderer;
    
    public World(LDtkLoadResponse response, Renderer renderer)
    {
        _ldtkWorld = response.Project;
        _collisionMaps = response.CollisionMaps;
        _entityMaps = response.EntityMaps;
        
        _renderer = renderer;
    }

    public void Update()
    {
        if (!_renderer.IsFading())
        {
            _hasFaded = true;
        }
    }

    public Entity CreateEntity()
    {
        Entity newEntity = new Entity(_nextEntityId++);
        _entities[newEntity.Id] = newEntity;
        return newEntity;
    }

    public void DestroyEntity(Entity entity)
    {
        if(_entities.ContainsKey(entity.Id))
        {
            _entities.Remove(entity.Id);
        }
    }

    public Entity GetEntity(int entityId)
    {
        if (_entities.TryGetValue(entityId, out var entity))
        {
            return entity;
        }

        throw new UnknownEntityException();
    }

    public List<Entity> GetEntities()
    {
        return new List<Entity>(_entities.Values);
    }
    
    public List<Entity> GetEntitiesWithComponents(params Type[] componentTypes)
    {
        List<Entity> results = new List<Entity>();

        foreach (Entity entity in _entities.Values)
        {
            bool hasAllComponents = true;
            foreach (Type componentType in componentTypes)
            {
                if (!entity.HasComponent(componentType))
                {
                    hasAllComponents = false;
                    break;
                }
            }
            
            if (hasAllComponents)
            {
                results.Add(entity);
            }
        }
        
        return results;
    }
    
    public LDtkProject GetLDtkWorld()
    {
        return _ldtkWorld;
    }
    
    public bool IsTileSolid(int x, int y)
    {
        if (_collisionMaps.TryGetValue(_currentLevelId, out var collisionMap))
        {
            if (x >= 0 && x < collisionMap.GetLength(0) && y >= 0 && y < collisionMap.GetLength(1))
            {
                return collisionMap[x, y] == 1;
            }
        }
        
        return false;
    }

    public LDtkEntityInstance GetEntityInstance(Vector2 position)
    {
        return GetCurrentLevel().LayerInstances.Find(o => o.Type == "Entities").EntityInstances
            .Find(o => o.PositionPx[0] == (int)position.X * 32 && o.PositionPx[1] == (int)position.Y * 32);
    }
    
    public bool IsEntityAtPosition(Vector2 position)
    {
        return _entityMaps[_currentLevelId].Any(entity => entity == position);
    }

    public LDtkLevel GetCurrentLevel()
    {
        return _ldtkWorld.Levels[_currentLevelId];
    }

    public void SwitchLevel(int newLevelId, Vector2 exitLocation)
    {
        if (newLevelId < 0 || newLevelId >= _ldtkWorld.Levels.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newLevelId), "Invalid level ID.");
        }
        
        SwitchLevel(newLevelId, exitLocation);
    }
    
    private async void SwitchLevelInternal(int newLevelId, Vector2 exitLocation)
    {
        _isTransitioning = true;
        
        Entity player = GetEntitiesWithComponents(typeof(PlayerComponent))[0];
        
        Vector2 dirVector = Vector2.Normalize(exitLocation - player.GetComponent<TransformComponent>().Position);
        
        player.GetComponent<SmoothMovementComponent>().TargetPosition = player.GetComponent<TransformComponent>().Position + (dirVector * player.GetComponent<SmoothMovementComponent>().TileSize);
        player.GetComponent<SmoothMovementComponent>().IsMoving = true;
        
        await TaskEx.WaitUntilNot(player.GetComponent<SmoothMovementComponent>().GetIsMoving);
        
        foreach (Entity entity in GetEntitiesWithComponents(typeof(SmoothMovementComponent)))
        {
            entity.GetComponent<SmoothMovementComponent>().Freeze();
        }
        
        foreach (Entity entity in GetEntitiesWithComponents(typeof(AnimationComponent)))
        {
            entity.GetComponent<AnimationComponent>().StopAnimation();
        }
        
        _renderer.StartFade(true, 2.0f, true);
        _hasFaded = false;

        await TaskEx.WaitUntil(HasFaded);
        
        int oldLevelId = _currentLevelId;
        
        _currentLevelId = newLevelId;
        
        // Optionally, clear and reload entities specific to the level
        //_entities.Clear();
        // TODO: Handle level-specific entities

        int[] exitPx = _ldtkWorld.Levels[_currentLevelId].LayerInstances.Find(o => o.Type == "Entities").EntityInstances
            .Find(o => o.Identifier == "building_interact" &&
                       o.FieldInstances.Find(o => o.Identifier == "interaction").Value.ToString() == "exit" && o.FieldInstances.Find(o => o.Identifier == "buildingId").Value.ToString() == oldLevelId.ToString()).PositionPx;
        
        player.GetComponent<TransformComponent>().Position = new Vector2(exitPx[0], exitPx[1]);
        
        await TaskEx.WaitMs(100);
        
        _renderer.StartFade(false, 2.0f, false);
        _hasFaded = false;

        await TaskEx.WaitUntil(HasFaded);
        
        _isTransitioning = false;
    }
    
    public bool HasFaded()
    {
        return _hasFaded;
    }
    
    public bool IsTransitioning()
    {
        return _isTransitioning;
    }
}