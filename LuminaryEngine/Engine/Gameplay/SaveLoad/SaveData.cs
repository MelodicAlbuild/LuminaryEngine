using System.Numerics;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Extras;

namespace LuminaryEngine.Engine.Gameplay.SaveLoad;

/// <summary>
/// Represents the data structure for saving game state.
/// </summary>
[Serializable]
public class SaveData
{
    // Player-related data
    public string PlayerName { get; set; }
    public float PlayerPositionX { get; set; }
    public float PlayerPositionY { get; set; }
    public Direction PlayerFacingDirection { get; set; }

    // Inventory-related data
    public Dictionary<string, int> InventoryItems { get; set; }
    public Dictionary<string, int> SpiritEssences { get; set; }

    // Map Data
    public int CurrentMap { get; set; }
    public Dictionary<int, List<SerializableVector2>> InteractionData { get; set; }

    // Save timestamp
    public DateTime SaveTimestamp { get; set; }

    public SaveData()
    {
        SaveTimestamp = DateTime.Now;
        InventoryItems = new Dictionary<string, int>();
    }

    public void SavePosition(Vector2 position)
    {
        PlayerPositionX = position.X;
        PlayerPositionY = position.Y;
    }
}