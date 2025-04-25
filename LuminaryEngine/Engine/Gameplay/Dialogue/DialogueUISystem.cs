using LuminaryEngine.Engine.Core.Input;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.Dialogue;

public class DialogueUISystem : UIComponent
{
    private TextComponent dialogueText;
    private DialogueNode currentNode;
    private string fullText;
    private string displayedText;
    private int currentCharIndex = 0;
    private float typewriterSpeed = 0.05f; // Time in seconds between each character
    private float timeSinceLastChar = 0f;
    private bool isTyping = true;
    private bool isWaitingForInput = false;
    private bool interactedDuringTyping = false;
    private bool startedType = false;

    public DialogueUISystem(int x, int y, int width, int height)
        : base(x, y, width, height)
    {
        // Initialize the text component with proper margins and alignment
        int margin = 10;
        dialogueText = new TextComponent(
            "",
            ResourceCache.DefaultFont,
            new SDL.SDL_Color { r = 0, g = 0, b = 0, a = 255 }, // White color
            x + margin, // X position with margin
            y + margin, // Y position with margin
            width - 2 * margin, // Width reduced by margins
            height - 2 * margin // Height reduced by margins
        );
    }

    public void StartDialogue(DialogueNode startNode)
    {
        currentNode = startNode;
        fullText = currentNode.Text;
        displayedText = ""; // Clear displayedText to avoid flashing
        currentCharIndex = 0;
        timeSinceLastChar = 0f; // Reset the typewriter timer
        isTyping = true;
        isWaitingForInput = false;
        interactedDuringTyping = false; // Reset interaction flag

        // Reset the text component with an empty string
        dialogueText.SetText(" ");
        startedType = true;

        IsVisible = true;
    }

    private void UpdateTypewriterEffect(float deltaTime)
    {
        if (currentNode == null) return;

        if (isTyping)
        {
            timeSinceLastChar += deltaTime;

            if (timeSinceLastChar >= typewriterSpeed && currentCharIndex < fullText.Length)
            {
                displayedText += fullText[currentCharIndex];
                currentCharIndex++;
                timeSinceLastChar = 0f;

                // Update the text box with the current displayed text
                dialogueText.SetText(displayedText);

                // Check if we've reached the end of the current text
                if (currentCharIndex >= fullText.Length)
                {
                    isTyping = false;
                    dialogueText.SetText(displayedText + " \u25bc");
                    isWaitingForInput = true;
                }
            }
        }
    }

    public void HandleInteractKey()
    {
        if (isTyping && !startedType)
        {
            // Skip to the end of the current dialogue text if typing is in progress
            displayedText = fullText;
            dialogueText.SetText(displayedText + " \u25bc");
            isTyping = false;
            isWaitingForInput = true;
            interactedDuringTyping = true;
        }
        else if (isWaitingForInput && !interactedDuringTyping)
        {
            // If there's more dialogue to display, move to the next node
            if (currentNode != null)
            {
                if (currentNode.Choices != null)
                {
                    if (currentNode.Choices.Count > 0)
                    {
                        isWaitingForInput = false;
                        StartDialogue(currentNode.Choices[0]); // Simplified for single-choice progress
                    }
                    else
                    {
                        // End of dialogue
                        dialogueText.SetText(" "); // Clear the text box
                        currentNode = null;

                        IsVisible = false; // Hide the dialogue UI if there are no choices left to display
                    }
                }
                else
                {
                    // End of dialogue
                    dialogueText.SetText(" "); // Clear the text box
                    currentNode = null;

                    IsVisible = false; // Hide the dialogue UI if there are no choices left to display
                }
            }
            else
            {
                // End of dialogue
                dialogueText.SetText(" "); // Clear the text box
                currentNode = null;

                IsVisible = false; // Hide the dialogue UI if there are no choices left to display
            }
        }
        else if (isWaitingForInput && interactedDuringTyping)
        {
            // Clear the interaction flag so the next interaction can proceed
            interactedDuringTyping = false;
        }
        else if (startedType)
        {
            startedType = false;
        }
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible) return;

        // Render the dialogue text component
        dialogueText.Render(renderer);
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        if (!IsVisible) return;

        var triggeredActions = InputMappingSystem.Instance.GetTriggeredActions(new HashSet<SDL.SDL_Scancode>
            { sdlEvent.key.keysym.scancode });

        if (triggeredActions.Contains(ActionType.Interact))
        {
            HandleInteractKey();
        }
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
    }

    public void Update(float deltaTime)
    {
        UpdateTypewriterEffect(deltaTime);
    }
}