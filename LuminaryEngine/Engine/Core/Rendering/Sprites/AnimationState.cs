namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class AnimationState
{
    public string CurrentAnimation { get; set; }
    public int CurrentFrame { get; set; }
    public float ElapsedTime { get; set; }

    public AnimationState(string currentAnimation)
    {
        CurrentAnimation = currentAnimation;
        CurrentFrame = 0;
        ElapsedTime = 0f;
    }
}