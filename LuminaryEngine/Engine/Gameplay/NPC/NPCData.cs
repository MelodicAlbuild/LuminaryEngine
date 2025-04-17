using System.Numerics;
using LuminaryEngine.Engine.Gameplay.Dialogue;

namespace LuminaryEngine.Engine.Gameplay.NPC;

public class NPCData
{
    public Vector2 Position { get; set; }
    public string TextureName { get; set; }
    public NPCType Type { get; set; }
    public bool Interactive { get; set; }
    public DialogueNode Dialogue { get; set; }
}