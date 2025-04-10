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
    
    public World(LDtkProject ldtkWorld, Dictionary<int, int[,]> cMaps)
    {
        _ldtkWorld = ldtkWorld;
        _collisionMaps = cMaps;
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

    public LDtkLevel GetCurrentLevel()
    {
        return _ldtkWorld.Levels[_currentLevelId];
    }
}