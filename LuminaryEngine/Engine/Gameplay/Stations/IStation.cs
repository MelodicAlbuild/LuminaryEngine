using System.Numerics;

namespace LuminaryEngine.Engine.Gameplay.Stations;

public interface IStation
{
    public string TextureId { get; set; }
    public Vector2 Position { get; set; }
}