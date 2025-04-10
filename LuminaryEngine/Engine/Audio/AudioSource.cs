using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Audio;

public class AudioSource : IComponent
{
    public string SoundId { get; set; }
    public float Volume { get; set; } = 1.0f;
    public float Pitch { get; set; } = 1.0f;
    public float Pan { get; set; } = 0.0f; // -1.0 (left) to 1.0 (right)
    public bool PlayOnAwake { get; set; } = false;
    public bool Loop { get; set; } = false;

    public AudioSource(string soundId)
    {
        SoundId = soundId;
    }
}