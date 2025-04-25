using LuminaryEngine.Engine.Exceptions;

namespace LuminaryEngine.Engine.ECS;

public class Entity
{
    public int Id { get; private set; }
    private Dictionary<Type, IComponent> _components;

    public Entity(int id)
    {
        Id = id;
        _components = new Dictionary<Type, IComponent>();
    }

    public void AddComponent(IComponent component)
    {
        _components[component.GetType()] = component;
    }

    public T GetComponent<T>() where T : IComponent
    {
        if (_components.TryGetValue(typeof(T), out var component))
        {
            return (T)component;
        }

        throw new UnknownComponentException();
    }

    public bool HasComponent<T>() where T : IComponent
    {
        return _components.ContainsKey(typeof(T));
    }

    public bool HasComponent(Type componentType)
    {
        return _components.ContainsKey(componentType);
    }

    public void RemoveComponent<T>() where T : IComponent
    {
        _components.Remove(typeof(T));
    }
}