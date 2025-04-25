using LuminaryEngine.Engine.Gameplay.Player;
using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Items;

public class ItemManager
{
    private static ItemManager instance = null;

    private ItemManager()
    {
    }

    public static ItemManager Instance
    {
        get { return instance ??= new ItemManager(); }
    }

    private Dictionary<string, Item> _items = new();

    public void RegisterItem(string id, Item item)
    {
        if (!_items.ContainsKey(id))
        {
            _items[id] = item;
        }
    }

    public void UnregisterItem(string id)
    {
        if (_items.ContainsKey(id))
        {
            _items.Remove(id);
        }
    }

    public Item GetItem(string id)
    {
        if (_items.ContainsKey(id))
        {
            return _items[id];
        }

        return null;
    }

    public void LoadItems()
    {
        if (!File.Exists(Path.Combine("Assets", "Items", "items.json")))
        {
            throw new FileNotFoundException("Items JSON file not found", Path.Combine("Assets", "Items", "items.json"));
        }

        JsonConvert.DeserializeObject<List<ItemJSON>>(File.ReadAllText(Path.Combine("Assets", "Items", "items.json")))
            ?.ForEach(itemJson =>
            {
                var item = new Item();
                item.Name = itemJson.Name;
                item.Description = itemJson.Description;
                item.TextureId = itemJson.TextureId;
                item.Type = (ItemType)itemJson.Type;
                item.ItemId = itemJson.ItemId;
                RegisterItem(itemJson.ItemId, item);
            });
    }
}