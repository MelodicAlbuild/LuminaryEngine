using System.Numerics;
using LuminaryEngine.Engine.Gameplay.Stations;

namespace LuminaryEngine.Engine.Gameplay.Crafting;

public class CraftingStation : IStation
{
    public string TextureId { get; set; }
    public Vector2 Position { get; set; }
    public string StationTag { get; set; }
}