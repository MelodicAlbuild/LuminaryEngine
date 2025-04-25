using Newtonsoft.Json;

namespace LuminaryEngine.Engine.Gameplay.Spirits;

public class SpiritEssenceManager
{
    private static SpiritEssenceManager instance = null;

    private SpiritEssenceManager()
    {
    }

    public static SpiritEssenceManager Instance
    {
        get { return instance ??= new SpiritEssenceManager(); }
    }

    private Dictionary<string, SpiritEssence> _items = new();

    public void RegisterSpiritEssence(string id, SpiritEssence item)
    {
        if (!_items.ContainsKey(id))
        {
            _items[id] = item;
        }
    }

    public void UnregisterSpiritEssence(string id)
    {
        if (_items.ContainsKey(id))
        {
            _items.Remove(id);
        }
    }

    public SpiritEssence GetSpiritEssence(string id)
    {
        if (_items.ContainsKey(id))
        {
            return _items[id];
        }

        return null;
    }

    public void LoadSpiritEssence()
    {
        if (!File.Exists(Path.Combine("Assets", "SpiritEssence", "spiritessence.json")))
        {
            throw new FileNotFoundException("Spirit Essence JSON file not found", Path.Combine("Assets", "SpiritEssence", "spiritessence.json"));
        }

        JsonConvert.DeserializeObject<List<SpiritEssenceJSON>>(File.ReadAllText(Path.Combine("Assets", "SpiritEssence", "spiritessence.json")))
            ?.ForEach(spiritEssenceJson =>
            {
                var spiritEssence = new SpiritEssence();
                spiritEssence.EssenceID = spiritEssenceJson.EssenceID;
                spiritEssence.Name = spiritEssenceJson.Name;
                spiritEssence.Description = spiritEssenceJson.Description;
                spiritEssence.TextureId = spiritEssenceJson.TextureId;
                spiritEssence.Type = (SpiritType)spiritEssenceJson.Type;
                spiritEssence.Tier = (SpiritTier)spiritEssenceJson.Tier;
                spiritEssence.PropertyMultipliers = spiritEssenceJson.PropertyMultipliers;
                RegisterSpiritEssence(spiritEssenceJson.EssenceID, spiritEssence);
            });
    }
}