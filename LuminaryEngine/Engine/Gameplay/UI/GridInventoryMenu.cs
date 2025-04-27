using LuminaryEngine.Engine.Core.Logging;
using LuminaryEngine.Engine.Core.Rendering;
using LuminaryEngine.Engine.Core.Rendering.Textures;
using LuminaryEngine.Engine.Core.ResourceManagement;
using LuminaryEngine.Engine.Gameplay.Items;
using LuminaryEngine.Engine.Gameplay.Player;
using LuminaryEngine.Engine.Gameplay.Spirits;
using SDL2;

namespace LuminaryEngine.Engine.Gameplay.UI;

public class GridInventoryMenu : MenuSystem
{
    private const int CellWidth = 64;
    private const int CellHeight = 64;
    private const int MaxColumns = 5; // Number of columns in the grid
    private const int Padding = 10; // Space between grid cells

    private InventoryComponent _inventory;
    private ResourceCache _resourceCache;
    private ScrollableMenu _scrollableMenu;
    
    private string _currentOption = "Items"; // Default selected option

    private int _x, _y, _width, _height, _zIndex;

    public GridInventoryMenu(InventoryComponent inventory, ResourceCache resourceCache, int x, int y, int width,
        int height, int zIndex = Int32.MaxValue)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;
        _inventory = inventory;
        _resourceCache = resourceCache;
        _zIndex = zIndex;
        
        _scrollableMenu = new ScrollableMenu(x, y, width, height, new List<string>() { "Items", "Spirits" }, 2, zIndex: zIndex, horizontal: true);
    }

    public override void Render(Renderer renderer)
    {
        // Render the background rectangle
        renderer.EnqueueRenderCommand(new RenderCommand
        {
            Type = RenderCommandType.ClearUI, // Custom command for solid color
            ClearR = 0, // Background color (R)
            ClearG = 0, // Background color (G)
            ClearB = 0, // Background color (B)
            ClearA = 255, // Transparency (A)
            DestRect = new SDL.SDL_Rect { x = _x, y = _y, w = _width, h = _height },
            ZOrder = _zIndex - 5 // Render behind other components
        });

        base.Render(renderer); // Render child components
    }

    public override void HandleEvent(SDL.SDL_Event sdlEvent)
    {
        _scrollableMenu.HandleEvent(sdlEvent);

        if (_scrollableMenu.GetSelectedOption() != _currentOption)
        {
            _currentOption = _scrollableMenu.GetSelectedOption();
            RefreshGrid();
        }
    }
    
    private void RefreshGrid()
    {
        // Clear existing components
        ClearComponents();

        // Recalculate and render the grid
        Activate();
    }

    public override void Activate()
    {
        base.Activate();

        foreach (UIComponent menuComponent in GetMenuComponents())
        {
            menuComponent.IsVisible = true;
        }
        
        AddComponent(_scrollableMenu);
        _scrollableMenu.SetFocus(true);

        // Set up grid layout for items
        int currentX = _x + Padding;
        int currentY = _y + Padding + 22;
        int columnCount = 0;

        switch (_scrollableMenu.GetSelectedOption())
        {
            case "Items":
            {
                foreach (var item in _inventory.GetInventory())
                {
                    // Create an icon for the item (placeholder image)
                    var itemIcon = new ImageComponent(
                        _resourceCache.GetTexture(ItemManager.Instance.GetItem(item.Key)
                            .TextureId), // Replace with actual texture path
                        currentX, currentY, CellWidth, CellHeight, zIndex: _zIndex - 2
                    );

                    // Create a label for the item amount
                    var itemLabel = new TextComponent(
                        $"x{item.Value}", // Item quantity
                        _resourceCache.GetFont("Pixel", 16), // Default font
                        new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }, // White color
                        currentX + CellWidth - 20, currentY + CellHeight - 20, CellWidth, 20,
                        zIndex: _zIndex - 1 // Render behind the icon
                    );
                    itemLabel.IsVisible = true; // Ensure the label is visible

                    // Add components to the menu's internal list
                    AddComponent(itemIcon);
                    AddComponent(itemLabel);

                    // Move to the next grid cell
                    currentX += CellWidth + Padding;
                    columnCount++;

                    if (columnCount >= MaxColumns)
                    {
                        // Move to the next row
                        currentX = _x + Padding;
                        currentY += CellHeight + Padding;
                        columnCount = 0;
                    }
                }

                break;
            }
            case "Spirits":
            {
                foreach (var spiritEssence in _inventory.GetSpiritEssences())
                {
                    // Create an icon for the spirit essence (placeholder image)
                    var spiritEssenceIcon = new ImageComponent(
                        _resourceCache.GetTexture(SpiritEssenceManager.Instance.GetSpiritEssence(spiritEssence.Key)
                            .TextureId), // Replace with actual texture path
                        currentX, currentY, CellWidth, CellHeight, zIndex: _zIndex - 2
                    );

                    // Create a label for the spirit essence amount
                    var spiritEssenceLabel = new TextComponent(
                        $"x{spiritEssence.Value}", // Item quantity
                        _resourceCache.GetFont("Pixel", 16), // Default font
                        new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 }, // White color
                        currentX + CellWidth - 20, currentY + CellHeight - 20, CellWidth, 20,
                        zIndex: _zIndex - 1 // Render behind the icon
                    );
                    spiritEssenceLabel.IsVisible = true; // Ensure the label is visible

                    // Add components to the menu's internal list
                    AddComponent(spiritEssenceIcon);
                    AddComponent(spiritEssenceLabel);

                    // Move to the next grid cell
                    currentX += CellWidth + Padding;
                    columnCount++;

                    if (columnCount >= MaxColumns)
                    {
                        // Move to the next row
                        currentX = _x + Padding;
                        currentY += CellHeight + Padding;
                        columnCount = 0;
                    }
                }
                
                break;
            }
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        _scrollableMenu.SetFocus(false);
        ClearComponents();
    }
}