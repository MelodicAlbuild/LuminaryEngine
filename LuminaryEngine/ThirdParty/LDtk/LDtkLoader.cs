﻿using LuminaryEngine.Extras;
using LuminaryEngine.ThirdParty.LDtk.Models;
using Newtonsoft.Json;

namespace LuminaryEngine.ThirdParty.LDtk
{
    public struct LDtkLoadResponse
    {
        public LDtkProject Project { get; set; }
        public Dictionary<int, int[,]> CollisionMaps { get; set; }
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
                                    //collisionMaps.Add(int.Parse(level.Identifier.Split("_")[1]), intGridValues);
                                }
                            }
                        }
                    }
                }
            }

            return new LDtkLoadResponse()
            {
                Project = project,
                CollisionMaps = collisionMaps
            };
        }
    }
}