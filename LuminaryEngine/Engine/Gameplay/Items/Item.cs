namespace LuminaryEngine.Engine.Gameplay.Items;

public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string TextureId { get; set; }
    public bool IsEquipped { get; set; }
    public string ItemId { get; set; }
    public ItemType Type { get; set; }
}