using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Gameplay.NPC;

public class NPCComponent : IComponent
{
    private NPCData _data;

    public NPCComponent(NPCData data)
    {
        _data = data;
    }

    public NPCData GetData()
    {
        return _data;
    }
}