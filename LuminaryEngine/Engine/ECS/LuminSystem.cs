namespace LuminaryEngine.Engine.ECS;

public abstract class LuminSystem
{
    protected World _world;

    public LuminSystem(World world)
    {
        _world = world;
    }

    public abstract void Update();
}