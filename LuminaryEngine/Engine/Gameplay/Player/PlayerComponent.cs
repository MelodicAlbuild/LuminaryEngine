using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.ECS.Systems;
using LuminaryEngine.Engine.Gameplay.Dialogue;
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
                            case NPCType.ItemGiver:
                                if (data.HasInteracted)
                                {
                                    if (data.IsRepeatable)
                                    {
                                        _game.World.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                            .GetComponent<InventoryComponent>().AddItem(data.ItemId, data.ItemAmount);
                                        DialogueNode node = data.Dialogue;
                                        if (node.Choices == null)
                                        {
                                            node.Choices = new List<DialogueNode>();
                                            node.Choices.Add(
                                                new DialogueNode($"You received {data.ItemAmount}x {data.ItemId}"));
                                        }
                                        else if (node.Choices.Count == 0)
                                        {
                                            node.Choices.Add(
                                                new DialogueNode($"You received {data.ItemAmount}x {data.ItemId}"));
                                        }
                                        else
                                        {
                                            DialogueNode nodeNew = node.Choices[0];
                                            while (nodeNew.Choices != null && nodeNew.Choices.Count > 0)
                                            {
                                                nodeNew = nodeNew.Choices[0];
                                            }

                                            nodeNew.Choices.Add(
                                                new DialogueNode($"You received {data.ItemAmount}x {data.ItemId}"));
                                        }

                                        _game.DialogueBox.SetDialogue(node);
                                    }
                                    else
                                    {
                                        _game.DialogueBox.SetDialogue(data.ErrorDialogue);
                                    }
                                }
                                else
                                {
                                    _game.World.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                        .GetComponent<InventoryComponent>().AddItem(data.ItemId, data.ItemAmount);
                                    DialogueNode node = data.Dialogue;
                                    if (node.Choices == null)
                                    {
                                        node.Choices = new List<DialogueNode>();
                                        node.Choices.Add(
                                            new DialogueNode($"You received {data.ItemAmount}x {data.ItemId}"));
                                    }
                                    else if (node.Choices.Count == 0)
                                    {
                                        node.Choices.Add(
                                            new DialogueNode($"You received {data.ItemAmount}x {data.ItemId}"));
                                    }
                                    else
                                    {
                                        DialogueNode nodeNew = node.Choices[0];
                                        while (nodeNew.Choices != null && nodeNew.Choices.Count > 0)
                                        {
                                            nodeNew = nodeNew.Choices[0];
                                        }

                                        nodeNew.Choices.Add(
                                            new DialogueNode($"You received {data.ItemAmount}x {data.ItemId}"));
                                    }

                                    _game.DialogueBox.SetDialogue(node);
                                    data.HasInteracted = true;
                                }

                                break;
                        }
                    }
                }
            }
        }
    }
}