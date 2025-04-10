using LuminaryEngine.Engine.Audio;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering.Sprites;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.Gameplay.Player;
using SDL2;
using Starforge.Gameplay.Player;

namespace Starforge;

public class StarforgeGame : Game
{
    protected override void LoadContent()
    {
        base.LoadContent();
        
        Sound bgmMain = ResourceCache.GetSound("background_music.mp3");
        Entity backgroundMusicEntity = World.CreateEntity();
        backgroundMusicEntity.AddComponent(new AudioSource("background_music.mp3") { PlayOnAwake = true, Volume = 0.7f, Loop = true });
        
        Entity player = World.CreateEntity();
        player.AddComponent(new TransformComponent(512, 544));
        player.AddComponent(new SpriteComponent("player.png", new SDL.SDL_Rect()
        {
            x = 0,
            y = 0,
            w = 32,
            h = 48
        }, 18));
        player.AddComponent(new InputStateComponent());
        player.AddComponent(new PlayerComponent());
        player.AddComponent(new SmoothMovementComponent(100f, 32));
        player.AddComponent(new AnimationComponent());
        
        player.GetComponent<AnimationComponent>().AddAnimations(PlayerAnimations.WalkAnimations);
    }
}