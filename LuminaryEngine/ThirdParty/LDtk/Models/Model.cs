using LuminaryEngine.ThirdParty.LDtk.Converters;
using Newtonsoft.Json;

namespace LuminaryEngine.ThirdParty.LDtk.Models
{
    // Root object representing the entire LDtk project.
    public class LDtkProject
    {
        [JsonProperty("jsonVersion")] public string JsonVersion { get; set; }

        [JsonProperty("ldtkVersion")] public string LdtkVersion { get; set; }

        [JsonProperty("bgColor")] public string BgColor { get; set; }

        [JsonProperty("worldGridSize")] public int WorldGridSize { get; set; }

        [JsonProperty("defaultLevelBgColor")] public string DefaultLevelBgColor { get; set; }

        [JsonProperty("levels")] public List<LDtkLevel> Levels { get; set; }

        [JsonProperty("defs")] public LDtkDefinitions Definitions { get; set; }
    }

    // Definitions for various reusable parts of the LDtk project.
    public class LDtkDefinitions
    {
        [JsonProperty("tilesets")] public List<LDtkTileset> Tilesets { get; set; }

        [JsonProperty("enums")] public List<LDtkEnumDef> Enums { get; set; }

        [JsonProperty("layers")] public List<LDtkLayerDef> LayerDefs { get; set; }

        [JsonProperty("entities")] public List<LDtkEntityDef> EntityDefs { get; set; }
    }

    public class LDtkTileset
    {
        [JsonProperty("identifier")] public string Identifier { get; set; }

        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("relPath")] public string RelPath { get; set; }

        [JsonProperty("pxWid")] public int PixelWidth { get; set; }

        [JsonProperty("pxHei")] public int PixelHeight { get; set; }

        [JsonProperty("tileGridSize")] public int TileGridSize { get; set; }

        [JsonProperty("spacing")] public int Spacing { get; set; }

        [JsonProperty("padding")] public int Padding { get; set; }

        [JsonProperty("tags")] public string[] Tags { get; set; }
    }

    public class LDtkEnumDef
    {
        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("identifier")] public string Identifier { get; set; }

        [JsonProperty("values")] public List<LDtkEnumValue> Values { get; set; }
    }

    public class LDtkEnumValue
    {
        [JsonProperty("id")] public string Id { get; set; }

        [JsonProperty("identifier")] public string Identifier { get; set; }

        // Typically a rectangle: [x, y, width, height]
        [JsonProperty("tileSrcRect")] public int[] TileSrcRect { get; set; }

        [JsonProperty("optional")] public bool Optional { get; set; }
    }

    public class LDtkLayerDef
    {
        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("identifier")] public string Identifier { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("infinite")] public bool Infinite { get; set; }

        [JsonProperty("gridSize")] public int GridSize { get; set; }

        [JsonProperty("displayOpacity")] public double DisplayOpacity { get; set; }

        [JsonProperty("offsetX")] public int OffsetX { get; set; }

        [JsonProperty("offsetY")] public int OffsetY { get; set; }

        [JsonProperty("doc")]
        [JsonConverter(typeof(StringToObjectConverter))]
        public LDtkDocDef Doc { get; set; }

        // Additional properties can be added as needed.
    }

    public class LDtkDocDef
    {
        public int zIndex { get; set; }
    }

    public class LDtkEntityDef
    {
        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("identifier")] public string Identifier { get; set; }

        [JsonProperty("width")] public int Width { get; set; }

        [JsonProperty("height")] public int Height { get; set; }

        [JsonProperty("tile")] public int? Tile { get; set; }

        [JsonProperty("tileRect")] public int[] TileRect { get; set; }

        [JsonProperty("fields")] public List<LDtkFieldDef> Fields { get; set; }
    }

    public class LDtkFieldDef
    {
        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("identifier")] public string Identifier { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        // Default value can be a number, string, boolean, etc.
        [JsonProperty("defaults")] public object DefaultValue { get; set; }
    }

    public class LDtkLevel
    {
        [JsonProperty("identifier")] public string Identifier { get; set; }

        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("pxWid")] public int PixelWidth { get; set; }

        [JsonProperty("pxHei")] public int PixelHeight { get; set; }

        [JsonProperty("worldX")] public int WorldX { get; set; }

        [JsonProperty("worldY")] public int WorldY { get; set; }

        [JsonProperty("bgColor")] public string BgColor { get; set; }

        // For external level files.
        [JsonProperty("externalRelPath")] public string ExternalRelPath { get; set; }

        [JsonProperty("externalFileChecksum")] public string ExternalFileChecksum { get; set; }

        [JsonProperty("layerInstances")] public List<LDtkLayerInstance> LayerInstances { get; set; }
    }

    public class LDtkLayerInstance
    {
        // The user-friendly name of the layer.
        [JsonProperty("__identifier")] public string Identifier { get; set; }

        // The type of layer (Tile, IntGrid, Entities, AutoLayer, etc.).
        [JsonProperty("__type")] public string Type { get; set; }

        // Unique ID for this layer instance.
        [JsonProperty("uid")] public int Uid { get; set; }

        // The unique id for the corresponding layer definition.
        [JsonProperty("layerDefUid")] public int LayerDefUid { get; set; }

        // *** Added Property: LayerId ***
        // This property can be used as an alias or additional identifier.
        [JsonProperty("layerId")] public int LayerId { get; set; }

        [JsonProperty("visible")] public bool Visible { get; set; }

        [JsonProperty("__gridSize")] public int GridSize { get; set; }

        [JsonProperty("pxOffsetX")] public int PxOffsetX { get; set; }

        [JsonProperty("pxOffsetY")] public int PxOffsetY { get; set; }

        [JsonProperty("tileSize")] public int TileSize { get; set; }

        [JsonProperty("__tilesetDefUid")] public int? TilesetUid { get; set; }

        [JsonProperty("__tilesetRelPath")] public string TilesetRelPath { get; set; }

        // For tile layers:
        [JsonProperty("gridTiles")] public List<LDtkTileInstance> GridTiles { get; set; }

        // For auto layers:
        [JsonProperty("autoTiles")] public List<LDtkAutoTile> AutoTiles { get; set; }

        // For entity layers:
        [JsonProperty("entityInstances")] public List<LDtkEntityInstance> EntityInstances { get; set; }

        // For int-grid layers:
        [JsonProperty("intGridCsv")] public List<int> IntGrid { get; set; }

        [JsonProperty("__cWid")] public int CellWidth { get; set; }

        [JsonProperty("__cHei")] public int CellHeight { get; set; }

        // Custom field instances attached to the layer.
        [JsonProperty("fieldInstances")] public List<LDtkFieldInstance> FieldInstances { get; set; }
    }

    public class LDtkTileInstance
    {
        // Position in pixels, e.g., [x, y].
        [JsonProperty("px")] public int[] PositionPx { get; set; }

        // The source rectangle in the tileset image: [srcX, srcY, width, height].
        [JsonProperty("src")] public int[] SrcRect { get; set; }

        [JsonProperty("f")] public int FlipFlags { get; set; }
    }

    public class LDtkAutoTile
    {
        [JsonProperty("px")] public int[] PositionPx { get; set; }

        [JsonProperty("t")] public int TileId { get; set; }

        [JsonProperty("f")] public int FlipFlags { get; set; }
    }

    public class LDtkEntityInstance
    {
        [JsonProperty("uid")] public int Uid { get; set; }

        [JsonProperty("defUid")] public int DefUid { get; set; }

        [JsonProperty("__identifier")] public string Identifier { get; set; }

        [JsonProperty("width")] public int Width { get; set; }

        [JsonProperty("height")] public int Height { get; set; }

        [JsonProperty("pivotX")] public double PivotX { get; set; }

        [JsonProperty("pivotY")] public double PivotY { get; set; }

        [JsonProperty("isTile")] public bool IsTile { get; set; }

        // Only populated if the entity is represented as a tile.
        [JsonProperty("tile")] public LDtkTileInstance Tile { get; set; }

        // Position in pixels.
        [JsonProperty("px")] public int[] PositionPx { get; set; }

        [JsonProperty("fieldInstances")] public List<LDtkFieldInstance> FieldInstances { get; set; }
    }

    public class LDtkFieldInstance
    {
        [JsonProperty("__identifier")] public string Identifier { get; set; }

        [JsonProperty("__type")] public string Type { get; set; }

        [JsonProperty("defUid")] public int DefUid { get; set; }

        [JsonProperty("__value")] public object Value { get; set; }

        // Depending on the field type, only one of these may be populated.
        [JsonProperty("real")] public double? RealValue { get; set; }

        [JsonProperty("integer")] public int? IntegerValue { get; set; }

        [JsonProperty("string")] public string StringValue { get; set; }

        [JsonProperty("bool")] public bool? BoolValue { get; set; }

        // Additional complex types can be represented if needed.
        [JsonProperty("dungeon")] public object DungeonValue { get; set; }
    }
}