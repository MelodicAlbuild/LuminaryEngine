using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.ECS;

namespace LuminaryEngine.Engine.Core.Rendering.Sprites;

public class AnimationSystem : LuminSystem
{
    private GameTime _gameTime;

    public AnimationSystem(World world, GameTime gameTime) : base(world)
    {
        _gameTime = gameTime;
    }

    public override void Update()
    {
        foreach (var entity in _world.GetEntitiesWithComponents(typeof(AnimationComponent), typeof(SpriteComponent)))
        {
            var animationComponent = entity.GetComponent<AnimationComponent>();
            var spriteComponent = entity.GetComponent<SpriteComponent>();

            if (animationComponent.State == null) continue;

            var state = animationComponent.State;
            var animation = animationComponent.Animations[state.CurrentAnimation];

            // Update elapsed time
            state.ElapsedTime += _gameTime.DeltaTime;

            // Advance frame if needed
            if (state.ElapsedTime >= animation.FrameDuration)
            {
                state.ElapsedTime -= animation.FrameDuration;
                state.CurrentFrame++;

                if (state.CurrentFrame >= animation.Frames.Count)
                {
                    if (animation.IsLooping)
                    {
                        state.CurrentFrame = 0;
                    }
                    else
                    {
                        state.CurrentFrame = animation.Frames.Count - 1; // Stay on the last frame
                    }
                }
            }

            // Update the sprite's source rectangle
            spriteComponent.SourceRect = animation.Frames[state.CurrentFrame];
        }
    }
}