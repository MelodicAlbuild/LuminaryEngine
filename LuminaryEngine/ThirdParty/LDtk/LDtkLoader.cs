using System.Collections;
using System.Diagnostics;
using System.Numerics;
using LuminaryEngine.Engine.Gameplay.Crafting;
using LuminaryEngine.Engine.Gameplay.Dialogue;
using LuminaryEngine.Engine.Gameplay.NPC;
using LuminaryEngine.Engine.Gameplay.Stations;
using LuminaryEngine.Extras;
using LuminaryEngine.ThirdParty.LDtk.Models;
using Newtonsoft.Json;

namespace LuminaryEngine.ThirdParty.LDtk
{
    public struct LDtkLoadResponse
    {
        public LDtkProject Project { get; set; }
        public Dictionary<int, int[,]> CollisionMaps { get; set; }
        public Dictionary<int, List<Vector2>> EntityMaps { get; set; }
        public Dictionary<int, List<NPCData>> NPCs { get; set; }
        public Dictionary<int, List<Vector2>> InteractableMaps { get; set; }
        public Dictionary<int, List<IStation>> CraftingStationMaps { get; set; }
    }

    public class LDtkLoader
    {
        /// <summary>
        /// Loads an entire LDtk project from a JSON file.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the LDtk JSON file.</param>
        /// <returns>An LDtkProject object populated with all available properties.</returns>
        public static LDtkLoadResponse LoadProject(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException("LDtk file not found", filePath);
            }

            string json = System.IO.File.ReadAllText(filePath);
            var project = JsonConvert.DeserializeObject<Models.LDtkProject>(json);

            Dictionary<int, int[,]> collisionMaps = new Dictionary<int, int[,]>();
            Dictionary<int, List<Vector2>> entityMaps = new Dictionary<int, List<Vector2>>();
            Dictionary<int, List<NPCData>> npcMaps = new Dictionary<int, List<NPCData>>();
            Dictionary<int, List<Vector2>> interactableMaps = new Dictionary<int, List<Vector2>>();
            Dictionary<int, List<IStation>> craftingStationMaps = new Dictionary<int, List<IStation>>();

            // If the JSON does not explicitly include layerId for each layer instance,
            // assign it from the UID as a default.
            if (project?.Levels != null)
            {
                foreach (var level in project.Levels)
                {
                    if (level.LayerInstances != null)
                    {
                        List<Vector2> interactables = new List<Vector2>();
                        
                        foreach (var layer in level.LayerInstances)
                        {
                            if (layer.LayerId == 0) // assuming a 0 value means unassigned
                            {
                                layer.LayerId = layer.Uid;
                            }

                            if (layer.Type == "IntGrid")
                            {
                                // Convert the IntGrid values to a 2D array
                                if (layer is { IntGrid: not null, Identifier: "collision_grid" })
                                {
                                    int[,] intGridValues = new int[layer.CellWidth, layer.CellHeight];

                                    for (int i = 0; i < layer.CellHeight; i++)
                                    {
                                        for (int j = 0; j < layer.CellWidth; j++)
                                        {
                                            intGridValues[i, j] = layer.IntGrid[i * layer.CellWidth + j];
                                        }
                                    }

                                    // Assign the 2D array to the layer
                                    collisionMaps.Add(int.Parse(level.Identifier.Split("_")[1]),
                                        ArrayReflection.ReflectOverAntiDiagonal(intGridValues));
                                }
                            }
                            else if (layer.Type == "Entities")
                            {
                                switch (layer.Identifier)
                                {
                                    case "entities":
                                    {
                                        List<Vector2> entities = new List<Vector2>();

                                        foreach (var entity in layer.EntityInstances)
                                        {
                                            // Assuming the entity has a PositionPx property
                                            if (entity.PositionPx != null && entity.PositionPx.Length == 2)
                                            {
                                                entities.Add(new Vector2(entity.PositionPx[0] / 32,
                                                    entity.PositionPx[1] / 32));
                                            }
                                        }

                                        // Add the entities to the dictionary with the level ID as the key
                                        entityMaps.Add(int.Parse(level.Identifier.Split("_")[1]), entities);
                                        break;
                                    }
                                    case "npcs":
                                    {
                                        List<NPCData> npcs = new List<NPCData>();

                                        foreach (var entity in layer.EntityInstances)
                                        {
                                            NPCType t = (NPCType)Convert.ToInt32(entity.FieldInstances.Find(o =>
                                                o.Identifier == "npcType")!.Value);

                                            switch (t)
                                            {
                                                case NPCType.Dialogue:
                                                    string[] diStrings =
                                                        ((IEnumerable)entity.FieldInstances.Find(o =>
                                                            o.Identifier == "npcDialogue")!.Value).Cast<object>()
                                                        .Select(x => x.ToString())
                                                        .ToArray()!;

                                                    List<DialogueNode> dialogue = new List<DialogueNode>();

                                                    foreach (var s in diStrings)
                                                    {
                                                        dialogue.Add(new DialogueNode(s));
                                                    }

                                                    dialogue.Reverse();

                                                    for (int i = 0; i < dialogue.Count - 1; i++)
                                                    {
                                                        dialogue[i + 1].Choices.Add(dialogue[i]);
                                                    }

                                                    DialogueNode n1;

                                                    n1 = dialogue.Count > 1 ? dialogue[^1] : dialogue[0];

                                                    NPCData d = new NPCData()
                                                    {
                                                        Type = t,
                                                        Interactive = Convert.ToBoolean(
                                                            entity.FieldInstances.Find(o =>
                                                                o.Identifier == "interactive")!.Value),
                                                        TextureName =
                                                            (string)entity.FieldInstances.Find(o =>
                                                                o.Identifier == "textureName")!.Value,
                                                        Dialogue = n1,
                                                        Position = new Vector2(entity.PositionPx[0],
                                                            entity.PositionPx[1]),
                                                    };

                                                    npcs.Add(d);

                                                    if (d.Interactive)
                                                    {
                                                        interactables.Add(new Vector2(entity.PositionPx[0],
                                                            entity.PositionPx[1]));
                                                    }

                                                    break;
                                                case NPCType.ItemGiver:
                                                    string[] diStrings1 =
                                                        ((IEnumerable)entity.FieldInstances.Find(o =>
                                                            o.Identifier == "npcDialogue")!.Value).Cast<object>()
                                                        .Select(x => x.ToString())
                                                        .ToArray()!;

                                                    List<DialogueNode> dialogue1 = new List<DialogueNode>();

                                                    foreach (var s in diStrings1)
                                                    {
                                                        dialogue1.Add(new DialogueNode(s));
                                                    }

                                                    dialogue1.Reverse();

                                                    for (int i = 0; i < dialogue1.Count - 1; i++)
                                                    {
                                                        dialogue1[i + 1].Choices.Add(dialogue1[i]);
                                                    }

                                                    DialogueNode nl1;

                                                    nl1 = dialogue1.Count > 1 ? dialogue1[^1] : dialogue1[0];

                                                    bool repeat = Convert.ToBoolean(
                                                        entity.FieldInstances.Find(o =>
                                                            o.Identifier == "isRepeatable")!.Value);

                                                    NPCData d1;

                                                    if (!repeat)
                                                    {
                                                        string[] diStringsError1 =
                                                            ((IEnumerable)entity.FieldInstances.Find(o =>
                                                                o.Identifier == "errorDialogue")!.Value).Cast<object>()
                                                            .Select(x => x.ToString())
                                                            .ToArray()!;

                                                        List<DialogueNode> dialogueError1 = new List<DialogueNode>();

                                                        foreach (var s in diStringsError1)
                                                        {
                                                            dialogueError1.Add(new DialogueNode(s));
                                                        }

                                                        dialogueError1.Reverse();

                                                        for (int i = 0; i < dialogueError1.Count - 1; i++)
                                                        {
                                                            dialogueError1[i + 1].Choices.Add(dialogueError1[i]);
                                                        }

                                                        DialogueNode nlE1;

                                                        nlE1 = dialogueError1.Count > 1
                                                            ? dialogueError1[^1]
                                                            : dialogueError1[0];

                                                        d1 = new NPCData()
                                                        {
                                                            Type = t,
                                                            Interactive = Convert.ToBoolean(
                                                                entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "interactive")!.Value),
                                                            TextureName =
                                                                (string)entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "textureName")!.Value,
                                                            Dialogue = nl1,
                                                            Position = new Vector2(entity.PositionPx[0],
                                                                entity.PositionPx[1]),
                                                            IsSpiritEssence = Convert.ToBoolean(
                                                                entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "isSpiritEssence")!.Value),
                                                            ItemId = (string)entity.FieldInstances.Find(o =>
                                                                o.Identifier == "itemId")!.Value,
                                                            ItemAmount = Convert.ToInt32(
                                                                entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "itemAmount")!.Value),
                                                            IsRepeatable = repeat,
                                                            ErrorDialogue = nlE1
                                                        };
                                                    }
                                                    else
                                                    {
                                                        d1 = new NPCData()
                                                        {
                                                            Type = t,
                                                            Interactive = Convert.ToBoolean(
                                                                entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "interactive")!.Value),
                                                            TextureName =
                                                                (string)entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "textureName")!.Value,
                                                            Dialogue = nl1,
                                                            Position = new Vector2(entity.PositionPx[0],
                                                                entity.PositionPx[1]),
                                                            IsSpiritEssence = Convert.ToBoolean(
                                                                entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "isSpiritEssence")!.Value),
                                                            ItemId = (string)entity.FieldInstances.Find(o =>
                                                                o.Identifier == "itemId")!.Value,
                                                            ItemAmount = Convert.ToInt32(
                                                                entity.FieldInstances.Find(o =>
                                                                    o.Identifier == "itemAmount")!.Value),
                                                            IsRepeatable = repeat
                                                        };
                                                    }

                                                    npcs.Add(d1);

                                                    if (d1.Interactive)
                                                    {
                                                        interactables.Add(new Vector2(entity.PositionPx[0],
                                                            entity.PositionPx[1]));
                                                    }

                                                    break;
                                            }
                                        }

                                        npcMaps.Add(int.Parse(level.Identifier.Split("_")[1]), npcs);
                                        break;
                                    }
                                    case "stations":
                                    {
                                        List<IStation> craftingStations = new List<IStation>();

                                        foreach (var entity in layer.EntityInstances)
                                        {
                                            switch (project.Definitions.EntityDefs.Find(o => o.Uid == entity.DefUid)!
                                                        .Identifier)
                                            {
                                                case "crafting_station":
                                                {
                                                    CraftingStation station = new CraftingStation();
                                                    
                                                    station.TextureId =
                                                        (string)entity.FieldInstances.Find(o =>
                                                            o.Identifier == "textureName")!.Value;
                                                    
                                                    station.Position = new Vector2(entity.PositionPx[0],
                                                        entity.PositionPx[1]);
                                                    
                                                    station.StationTag = (string)entity.FieldInstances.Find(o =>
                                                        o.Identifier == "stationTag")!.Value;
                                                    
                                                    craftingStations.Add(station);
                                                    interactables.Add(new Vector2(entity.PositionPx[0],
                                                        entity.PositionPx[1]));
                                                    break;
                                                }
                                            }
                                        }

                                        // Add the crafting stations to the dictionary with the level ID as the key
                                        craftingStationMaps.Add(int.Parse(level.Identifier.Split("_")[1]),
                                            craftingStations);
                                        break;
                                    }
                                }
                            }
                        }
                        
                        interactableMaps.Add(int.Parse(level.Identifier.Split("_")[1]), interactables);
                    }
                }
            }

            Debug.Assert(project != null, nameof(project) + " != null");
            return new LDtkLoadResponse()
            {
                Project = project,
                CollisionMaps = collisionMaps,
                EntityMaps = entityMaps,
                NPCs = npcMaps,
                InteractableMaps = interactableMaps,
                CraftingStationMaps = craftingStationMaps
            };
        }
    }
}