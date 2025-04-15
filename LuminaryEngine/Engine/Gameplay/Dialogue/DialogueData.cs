namespace LuminaryEngine.Engine.Gameplay.Dialogue;

public class DialogueData
{
    public DialogueNode StartNode { get; set; }
    public List<DialogueNode> Nodes { get; set; }

    public DialogueData()
    {
        Nodes = new List<DialogueNode>();
    }
}