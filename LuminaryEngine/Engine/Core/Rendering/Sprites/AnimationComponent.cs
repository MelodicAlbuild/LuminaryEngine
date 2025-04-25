using LuminaryEngine.Engine.ECS;
using SDL2;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class AnimationComponent : IComponent
{
    public Dictionary<string, Animation> Animations { get; set; }
    public AnimationState State { get; set; }
    public SDL.SDL_Rect Frame { get; set; }
    public int DeadAnim { get; set; }

    public AnimationComponent()
    {
        Animations = new Dictionary<string, Animation>();
        State = null;
    }

    public void AddAnimation(Animation animation)
    {
        Animations[animation.Name] = animation;
    }

    public void AddAnimations(IEnumerable<Animation> animations)
    {
        foreach (var animation in animations)
        {
            Animations[animation.Name] = animation;
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (Animations.ContainsKey(animationName))
        {
            State = new AnimationState(animationName);
        }
    }

    public void StopAnimation()
    {
        if (State != null)
        {
            DeadAnim = 1;
            Frame = Animations[State.CurrentAnimation].Frames[0];
            State = null;
        }
    }
}