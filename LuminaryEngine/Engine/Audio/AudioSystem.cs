using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Audio;

public class AudioSystem : LuminSystem
{
    private AudioManager _audioManager;

    public AudioSystem(World world, AudioManager audioManager) : base(world)
    {
        _audioManager = audioManager;
    }

    public override void Update()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(AudioSource)))
        {
            var audioSource = entity.GetComponent<AudioSource>();

            if (audioSource.PlayOnAwake)
            {
                _audioManager.PlaySound(audioSource.SoundId, audioSource.Volume, audioSource.Loop);
                audioSource.PlayOnAwake = false; // Play only once on awake
            }

            // Implement logic to handle other audio source properties (volume, pitch, pan, etc.)
        }
    }
}