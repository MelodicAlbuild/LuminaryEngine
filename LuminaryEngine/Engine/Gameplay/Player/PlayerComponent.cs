using System.Numerics;
using LuminaryEngine.Engine.Core.GameLoop;
using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.ECS;
using LuminaryEngine.Engine.ECS.Components;
using LuminaryEngine.Engine.ECS.Systems;
using LuminaryEngine.Engine.Gameplay.Crafting;
using LuminaryEngine.Engine.Gameplay.Dialogue;
using LuminaryEngine.Engine.Gameplay.Items;
using LuminaryEngine.Engine.Gameplay.NPC;
using LuminaryEngine.Engine.Gameplay.Spirits;
using LuminaryEngine.Engine.Gameplay.Stations;
using LuminaryEngine.Engine.Gameplay.UI;
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
                    switch (_world.GetInteractableType(target))
                    {
                        case InteractableType.NPC:
                        {
                            NPCData data = _world.GetNPCInstance(target);
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
                                                DialogueNode node = data.Dialogue;
                                            if (data.IsSpiritEssence)
                                            {
                                                _game.World.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                                    .GetComponent<InventoryComponent>()
                                                    .AddSpiritEssence(data.ItemId, data.ItemAmount);
                                                
                                                SpiritEssence e = SpiritEssenceManager.Instance.GetSpiritEssence(data.ItemId);
                                                
                                                if (node.Choices == null)
                                                {
                                                    node.Choices = new List<DialogueNode>();
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {e.Name}"));
                                                }
                                                else if (node.Choices.Count == 0)
                                                {
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {e.Name}"));
                                                }
                                                else
                                                {
                                                    DialogueNode nodeNew = node.Choices[0];
                                                    while (nodeNew.Choices != null && nodeNew.Choices.Count > 0)
                                                    {
                                                        nodeNew = nodeNew.Choices[0];
                                                    }

                                                    nodeNew.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {e.Name}"));
                                                }
                                            }
                                            else
                                            {
                                                _game.World.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                                    .GetComponent<InventoryComponent>()
                                                    .AddItem(data.ItemId, data.ItemAmount);
                                                
                                                Item i = ItemManager.Instance.GetItem(data.ItemId);
                                                
                                                if (node.Choices == null)
                                                {
                                                    node.Choices = new List<DialogueNode>();
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {i.Name}"));
                                                }
                                                else if (node.Choices.Count == 0)
                                                {
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {i.Name}"));
                                                }
                                                else
                                                {
                                                    DialogueNode nodeNew = node.Choices[0];
                                                    while (nodeNew.Choices != null && nodeNew.Choices.Count > 0)
                                                    {
                                                        nodeNew = nodeNew.Choices[0];
                                                    }

                                                    nodeNew.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {i.Name}"));
                                                }
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
                                            DialogueNode node = data.Dialogue;
                                            if (data.IsSpiritEssence)
                                            {
                                                _game.World.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                                    .GetComponent<InventoryComponent>()
                                                    .AddSpiritEssence(data.ItemId, data.ItemAmount);
                                                
                                                SpiritEssence e = SpiritEssenceManager.Instance.GetSpiritEssence(data.ItemId);
                                                
                                                if (node.Choices == null)
                                                {
                                                    node.Choices = new List<DialogueNode>();
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {e.Name}"));
                                                }
                                                else if (node.Choices.Count == 0)
                                                {
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {e.Name}"));
                                                }
                                                else
                                                {
                                                    DialogueNode nodeNew = node.Choices[0];
                                                    while (nodeNew.Choices != null && nodeNew.Choices.Count > 0)
                                                    {
                                                        nodeNew = nodeNew.Choices[0];
                                                    }

                                                    nodeNew.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {e.Name}"));
                                                }
                                            }
                                            else
                                            {
                                                _game.World.GetEntitiesWithComponents(typeof(PlayerComponent))[0]
                                                    .GetComponent<InventoryComponent>()
                                                    .AddItem(data.ItemId, data.ItemAmount);
                                                
                                                Item i = ItemManager.Instance.GetItem(data.ItemId);
                                                
                                                if (node.Choices == null)
                                                {
                                                    node.Choices = new List<DialogueNode>();
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {i.Name}"));
                                                }
                                                else if (node.Choices.Count == 0)
                                                {
                                                    node.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {i.Name}"));
                                                }
                                                else
                                                {
                                                    DialogueNode nodeNew = node.Choices[0];
                                                    while (nodeNew.Choices != null && nodeNew.Choices.Count > 0)
                                                    {
                                                        nodeNew = nodeNew.Choices[0];
                                                    }

                                                    nodeNew.Choices.Add(
                                                        new DialogueNode($"You received {data.ItemAmount}x {i.Name}"));
                                                }
                                            }

                                            _game.DialogueBox.SetDialogue(node);
                                            data.HasInteracted = true;
                                        }

                                        break;
                                }
                            }

                            break;
                        }
                        case InteractableType.Station:
                        {
                            IStation station = _world.GetStationInstance(target);

                            if (station is CraftingStation craftingStation)
                            {
                                if (!_game.UISystem.IsMenuActive("Crafting"))
                                {
                                    (_game.UISystem.GetMenuSystem("Crafting") as CraftingMenuSystem).SetStation(craftingStation);
                                    _game.UISystem.ActivateMenu("Crafting");
                                }
                            }
                            
                            break;
                        }
                    }
                }
            }
        }
    }
}