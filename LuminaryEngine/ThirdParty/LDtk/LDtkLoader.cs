using System.Collections;
using System.Numerics;
using LuminaryEngine.Engine.Gameplay.Dialogue;
using LuminaryEngine.Engine.Gameplay.NPC;
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

            // If the JSON does not explicitly include layerId for each layer instance,
            // assign it from the UID as a default.
            if(project?.Levels != null)
            {
                foreach(var level in project.Levels)
                {
                    if(level.LayerInstances != null)
                    {
                        foreach(var layer in level.LayerInstances)
                        {
                            if(layer.LayerId == 0)  // assuming a 0 value means unassigned
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
                                    collisionMaps.Add(int.Parse(level.Identifier.Split("_")[1]), ArrayReflection.ReflectOverAntiDiagonal(intGridValues));
                                }
                            } else if (layer.Type == "Entities")
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
                                                entities.Add(new Vector2(entity.PositionPx[0] / 32, entity.PositionPx[1] / 32));
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
                                                    string[] diStrings = ((IEnumerable)entity.FieldInstances.Find(o => o.Identifier == "npcDialogue")!.Value).Cast<object>()
                                                        .Select(x => x.ToString())
                                                        .ToArray();

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
                                                    
                                                    if (dialogue.Count > 1)
                                                    {
                                                        n1 = dialogue[^1];
                                                    }
                                                    else
                                                    {
                                                        n1 = dialogue[0];
                                                    }
                                        
                                                    npcs.Add(new NPCData()
                                                    {
                                                        Type = t,
                                                        Interactive = Convert.ToBoolean(entity.FieldInstances.Find(o => o.Identifier == "interactive")!.Value),
                                                        TextureName = (string)entity.FieldInstances.Find(o => o.Identifier == "textureName")!.Value,
                                                        Dialogue = n1,
                                                        Position = new Vector2(entity.PositionPx[0], entity.PositionPx[1]),
                                                    });
                                                    break;
                                            }
                                        }
                                    
                                        npcMaps.Add(int.Parse(level.Identifier.Split("_")[1]), npcs);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new LDtkLoadResponse()
            {
                Project = project,
                CollisionMaps = collisionMaps,
                EntityMaps = entityMaps,
                NPCs = npcMaps
            };
        }
    }
}