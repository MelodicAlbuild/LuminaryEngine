using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Gameplay.Player;

public class InventoryComponent : IComponent
{
    private readonly Dictionary<string, int> _items = new Dictionary<string, int>();
    private readonly Dictionary<string, int> _spiritEssences = new Dictionary<string, int>();

    public int Capacity { get; set; } = 30; // Default inventory capacity
    public int UsedSlots => _items.Count;
    public int RemainingSlots => Capacity - UsedSlots;

    // Item Management
    public void AddItem(string itemID, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return;
        }

        if (_items.ContainsKey(itemID))
        {
            _items[itemID] += quantity;
        }
        else if (UsedSlots < Capacity)
        {
            _items.Add(itemID, quantity);
        }
        else
        {
            LuminLog.Warning($"Inventory is full, cannot add item: {itemID}");
        }
    }

    public bool RemoveItem(string itemID, int quantity = 1)
    {
        if (quantity <= 0 || !_items.ContainsKey(itemID))
        {
            return false;
        }

        if (_items[itemID] > quantity)
        {
            _items[itemID] -= quantity;
            return true;
        }

        if (_items[itemID] == quantity)
        {
            _items.Remove(itemID);
            return true;
        }

        LuminLog.Warning($"Attempted to remove more of item '{itemID}' than present in inventory.");
        return false;
    }

    public bool HasItem(string itemID, int quantity = 1)
    {
        return _items.ContainsKey(itemID) && _items[itemID] >= quantity;
    }

    public int GetItemCount(string itemID)
    {
        return _items.TryGetValue(itemID, out int count) ? count : 0;
    }

    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(_items); // Return a copy
    }

    public void SetInventory(Dictionary<string, int> items)
    {
        _items.Clear();
        foreach (var item in items)
        {
            if (item.Value > 0)
            {
                _items[item.Key] = item.Value;
            }
        }
    }

    // Spirit Essence Management
    public void AddSpiritEssence(string essenceID, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return;
        }

        if (_spiritEssences.ContainsKey(essenceID))
        {
            _spiritEssences[essenceID] += quantity;
        }
        else
        {
            _spiritEssences.Add(essenceID, quantity);
        }
    }

    public bool RemoveSpiritEssence(string essenceID, int quantity = 1)
    {
        if (quantity <= 0 || !_spiritEssences.ContainsKey(essenceID))
        {
            return false;
        }

        if (_spiritEssences[essenceID] > quantity)
        {
            _spiritEssences[essenceID] -= quantity;
            return true;
        }

        if (_spiritEssences[essenceID] == quantity)
        {
            _spiritEssences.Remove(essenceID);
            return true;
        }

        LuminLog.Warning($"Attempted to remove more of spirit essence '{essenceID}' than present in inventory.");
        return false;
    }

    public bool HasSpiritEssence(string essenceID, int quantity = 1)
    {
        return _spiritEssences.ContainsKey(essenceID) && _spiritEssences[essenceID] >= quantity;
    }

    public int GetSpiritEssenceCount(string essenceID)
    {
        return _spiritEssences.TryGetValue(essenceID, out int count) ? count : 0;
    }

    public Dictionary<string, int> GetSpiritEssences()
    {
        return new Dictionary<string, int>(_spiritEssences); // Return a copy
    }

    public void SetSpiritEssences(Dictionary<string, int> spiritEssences)
    {
        _spiritEssences.Clear();
        foreach (var essence in spiritEssences)
        {
            if (essence.Value > 0)
            {
                _spiritEssences[essence.Key] = essence.Value;
            }
        }
    }

    // Other Inventory Operations
    public bool CanAddItem(string itemID, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return true;
        }

        if (_items.ContainsKey(itemID))
        {
            return true; // Can always add to an existing stack (assuming no max stack size limit here)
        }

        return UsedSlots + 1 <= Capacity;
    }

    public bool IsFull()
    {
        return UsedSlots >= Capacity;
    }

    public void Clear()
    {
        _items.Clear();
        _spiritEssences.Clear();
    }

    public void Expand(int amount)
    {
        if (amount > 0)
        {
            Capacity += amount;
            LuminLog.Debug($"Inventory expanded by {amount}. New capacity: {Capacity}");
        }
        else
        {
            LuminLog.Warning($"Attempted to expand inventory by a non-positive amount: {amount}");
        }
    }

    public void Shrink(int amount)
    {
        if (amount > 0 && Capacity - amount >= UsedSlots)
        {
            Capacity -= amount;
            LuminLog.Debug($"Inventory shrunk by {amount}. New capacity: {Capacity}");
        }
    }
}