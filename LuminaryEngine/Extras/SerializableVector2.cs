using System.Numerics;
using Newtonsoft.Json;

namespace LuminaryEngine.Extras;

[Serializable]
public class SerializableVector2
{
    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    // Default constructor for serialization
    public SerializableVector2()
    {
        X = 0;
        Y = 0;
    }

    // Constructor to initialize values
    public SerializableVector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public SerializableVector2(Vector2 vector)
    {
        X = vector.X;
        Y = vector.Y;
    }

    // Overriding ToString for better debugging and logging
    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    // Equality members
    public override bool Equals(object? obj)
    {
        if (obj is not SerializableVector2 other) return false;
        return Math.Abs(X - other.X) < 0.0001f && Math.Abs(Y - other.Y) < 0.0001f;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    
    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }
}