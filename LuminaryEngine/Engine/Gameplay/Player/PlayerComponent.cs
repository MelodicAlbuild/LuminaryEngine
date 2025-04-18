using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.ECS.Systems;
using LuminaryEngine.Engine.Gameplay.NPC;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.Player;

public class PlayerComponent : IComponent
{
    private World _world;
    private PlayerMovementSystem _playerMovementSystem;
    private Game _game;
    
    public PlayerComponent(World world, PlayerMovementSystem playerMovementSystem, Game game)
    {
        _world = world;
        _playerMovementSystem = playerMovementSystem;
        _game = game;
    }
    
    public void HandleInput(SDL.SDL_Event sdlEvent)
    {
        if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
        {
            var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
                { sdlEvent.key.keysym.scancode });

            if (triggeredActions.Contains(ActionType.Interact))
            {
                Vector2 position =
                    _world.GetEntitiesWithComponents(typeof(TransformComponent), typeof(PlayerComponent))[0]
                        .GetComponent<TransformComponent>().Position;
                Vector2 direction = _playerMovementSystem.GetDirection().ToVector2() * 32;
                Vector2 target = position + direction;
                
                if (_world.IsInteractableAtPosition(target))
                {
                    NPCData data = _world.GetInteractableInstance(target);
                    if (data != null)
                    {
                        switch (data.Type)
                        {
                            case NPCType.Dialogue:
                                _game.DialogueBox.SetDialogue(data.Dialogue);
                                break;
                        }
                    }
                }
            }
        }
    }
}