using Newtonsoft.Json;

namespace LuminaryEngine.ThirdParty.LDtk
{
    public class LDtkLoader
    {
        /// <summary>
        /// Loads an entire LDtk project from a JSON file.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the LDtk JSON file.</param>
        /// <returns>An LDtkProject object populated with all available properties.</returns>
        public static Models.LDtkProject LoadProject(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException("LDtk file not found", filePath);
            }

            string json = System.IO.File.ReadAllText(filePath);
            var project = JsonConvert.DeserializeObject<Models.LDtkProject>(json);

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
                        }
                    }
                }
            }

            return project;
        }
    }
}