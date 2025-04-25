using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Gameplay.Player;

public class InventoryComponent : IComponent
{
    private readonly Dictionary<string, int> _items = new Dictionary<string, int>();
    public int Capacity { get; set; } = 30; // Default inventory capacity
    public int UsedSlots => _items.Count;
    public int RemainingSlots => Capacity - UsedSlots;

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
            // Optionally handle inventory full (e.g., queue for later, drop item)
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
            // No event system, handle UI updates externally
            return true;
        }

        if (_items[itemID] == quantity)
        {
            _items.Remove(itemID);
            // No event system, handle UI updates externally
            return true;
        }

        // _items[itemID] < quantity
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
        else
        {
            LuminLog.Warning($"Attempted to shrink inventory by an invalid amount: {amount}");
        }
    }
}