using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Gameplay.UI;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.Dialogue;

public class DialogueBox : UIComponent
{
    private DialogueUISystem _dialogueUISystem;

    public DialogueBox(int x, int y, int width, int height, int zIndex = 2147483646) : base(x, y, width, height, zIndex)
    {
        _dialogueUISystem = new DialogueUISystem(x, y, width, height);
    }

    public void SetDialogue(DialogueNode dialogue)
    {
        _dialogueUISystem.StartDialogue(dialogue);
    }

    public override void Render(Renderer renderer)
    {
        if (!IsVisible) return;

        RenderCommand command = new RenderCommand()
        {
            Type = RenderCommandType.DrawRectangle,
            RectColor = new SDL.SDL_Color() { r = 255, g = 255, b = 255, a = 255 }, // Semi-transparent black
            DestRect = new SDL.SDL_Rect { x = X, y = Y, w = Width, h = Height },
            Filled = true,
            ZOrder = ZIndex - 1 // Ensure backdrop is behind menu items
        };

        renderer.EnqueueRenderCommand(command); // Render the background rectangle

        // Render the dialogue UI system
        _dialogueUISystem.Render(renderer);
    }

    public void Update(float deltaTime)
    {
        IsVisible = _dialogueUISystem.IsVisible;
        _dialogueUISystem.Update(deltaTime);
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        _dialogueUISystem.HandleEvent(sdlEvent);
    }

    public override void SetFocus(bool isFocused)
    {
        IsFocused = isFocused;
        _dialogueUISystem.SetFocus(isFocused);
    }

    public void SetVisible(bool isVisible)
    {
        IsVisible = isVisible;
        _dialogueUISystem.IsVisible = isVisible;
    }
}