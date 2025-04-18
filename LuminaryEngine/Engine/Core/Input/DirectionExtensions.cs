using System.Numerics;

namespace LuminaryEngine.Engine.Core.Input;

public static class DirectionExtensions
{
    public static Vector2 ToVector2(this Direction direction)
    {
        return direction switch
        {
            Direction.North => new Vector2(0, -1),
            Direction.South => new Vector2(0, 1),
            Direction.West => new Vector2(-1, 0),
            Direction.East => new Vector2(1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static bool ComparedTo(this Direction direction, Direction other)
    {
        return other.ToVector2() == direction.ToVector2();
    }
}