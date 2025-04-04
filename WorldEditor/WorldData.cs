using System.Windows.Media;

namespace WorldEditor;

public class TileInfo
{
    public int TileId { get; set; }
    public ImageSource ImageSource { get; set; } // Store the relative path to the cropped tile
}

public class ObjectInfo
{
    public string ObjectId { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

public class WorldLayer
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Tile[,] Tiles { get; set; }
}

public class Tile
{
    public int TileId { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}

public class WorldData
{
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public List<WorldLayer> Layers { get; set; }
    public List<WorldObject> Objects { get; set; } = new List<WorldObject>();
}

public class WorldObject
{
    public string ObjectId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
}