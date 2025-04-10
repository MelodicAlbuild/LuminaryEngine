using LuminaryEngine.Engine.Exceptions;
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
    public World(LDtkProject ldtkWorld, Dictionary<int, int[,]> cMaps)
    private Renderer _renderer;
    public World(LDtkLoadResponse response, Renderer renderer)
    {
        _ldtkWorld = response.Project;
        _collisionMaps = response.CollisionMaps;
        _entityMaps = response.EntityMaps;
        
        _renderer = renderer;
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
}