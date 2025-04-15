namespace LuminaryEngine.Engine.Gameplay.Dialogue;

public class DialogueNode
{
    public string Text { get; set; }
    public List<DialogueNode> Choices { get; set; }

    public DialogueNode(string text)
    {
        Text = text;
        Choices = new List<DialogueNode>();
    }
}