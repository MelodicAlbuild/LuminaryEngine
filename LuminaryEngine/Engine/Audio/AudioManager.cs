using SDL2;

namespace LuminaryEngine.Engine.Audio;

public class AudioManager : IDisposable
{
    private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();

    private Dictionary<int, string>
        playingChannels = new Dictionary<int, string>(); // Track which sound is playing on which channel

    public AudioManager()
    {
        if (SDL_mixer.Mix_Init(SDL_mixer.MIX_InitFlags.MIX_INIT_MP3) ==
            0) // Initialize OGG support (add others as needed)
        {
            Console.WriteLine($"SDL_mixer Init Error: {SDL_mixer.Mix_GetError()}");
            // Handle error
        }

        if (SDL_mixer.Mix_OpenAudio(44100, SDL_mixer.MIX_DEFAULT_FORMAT, 2, 2048) == -1)
        {
            Console.WriteLine($"SDL_mixer OpenAudio Error: {SDL_mixer.Mix_GetError()}");
            // Handle error
        }
    }

    public Sound LoadSound(string soundId, string filePath)
    {
        if (sounds.ContainsKey(soundId))
        {
            return sounds[soundId];
        }

        IntPtr chunk = SDL_mixer.Mix_LoadWAV(filePath);
        if (chunk == IntPtr.Zero)
        {
            Console.WriteLine($"SDL_mixer LoadWAV Error: {SDL_mixer.Mix_GetError()}");
            return null;
        }

        Sound sound = new Sound(filePath, chunk);
        sounds.Add(soundId, sound);
        return sound;
    }

    public void PlaySound(string soundId, float volume = 1.0f, bool loop = false)
    {
        if (sounds.TryGetValue(soundId, out Sound sound))
        {
            int loops = loop ? -1 : 0; // -1 for infinite loop, 0 for no loop
            int channel = SDL_mixer.Mix_PlayChannel(-1, sound.Chunk, loops);
            if (channel != -1)
            {
                SDL_mixer.Mix_Volume(channel, (int)(volume * SDL_mixer.MIX_MAX_VOLUME));
                playingChannels[channel] = soundId;
            }
        }
        else
        {
            Console.WriteLine($"AudioManager: Sound '{soundId}' not found.");
        }
    }

    public void StopSound(string soundId)
    {
        List<int> channelsToStop = new List<int>();
        foreach (var pair in playingChannels)
        {
            if (pair.Value == soundId)
            {
                channelsToStop.Add(pair.Key);
            }
        }

        foreach (int channel in channelsToStop)
        {
            SDL_mixer.Mix_HaltChannel(channel);
            playingChannels.Remove(channel);
        }
    }

    public void StopAllSounds()
    {
        SDL_mixer.Mix_HaltChannel(-1); // Stop all channels
        playingChannels.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foreach (var sound in sounds.Values)
        {
            sound.Dispose();
        }

        sounds.Clear();

        SDL_mixer.Mix_CloseAudio();
        SDL_mixer.Mix_Quit();
    }

    ~AudioManager()
    {
        Dispose(false);
    }
}