using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WorldEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private ObservableCollection<TileInfo> tilesetTiles = new ObservableCollection<TileInfo>();

    public ObservableCollection<TileInfo> TilesetTiles
    {
        get { return tilesetTiles; }
        set
        {
            tilesetTiles = value;
            OnPropertyChanged(nameof(TilesetTiles));
        }
    }

    private ObservableCollection<ObjectInfo> objects = new ObservableCollection<ObjectInfo>();

    public ObservableCollection<ObjectInfo> Objects
    {
        get { return objects; }
        set
        {
            objects = value;
            OnPropertyChanged(nameof(Objects));
        }
    }

    private ObservableCollection<WorldLayer> worldLayers = new ObservableCollection<WorldLayer>();

    public ObservableCollection<WorldLayer> WorldLayers
    {
        get { return worldLayers; }
        set
        {
            worldLayers = value;
            OnPropertyChanged(nameof(WorldLayers));
        }
    }

    private WorldLayer selectedLayer;

    public WorldLayer SelectedLayer
    {
        get { return selectedLayer; }
        set
        {
            selectedLayer = value;
            OnPropertyChanged(nameof(SelectedLayer));
            OnPropertyChanged(nameof(IsLayerSelected));
            UpdatePropertyPanel();
        }
    }

    public bool IsLayerSelected => SelectedLayer != null;

    private int mapWidth = 150;
    private int mapHeight = 100;
    private int tileWidth = 32;
    private int tileHeight = 32;

    private TileInfo selectedTile;
    private ObjectInfo selectedObject;
    private bool isDrawing;
    private bool isErasing;
    private Point startPoint;

    private string currentFilePath;

    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadTileset();
        LoadObjects();
        NewWorld();
        MapCanvas.MouseMove += MapCanvas_MouseMove_UpdateCoordinates;
    }

    private void LoadTileset()
    {
        try
        {
            string tilesetPath = "images/tileset.png"; // Adjust path if needed
            BitmapImage tilesetImage = new BitmapImage(new Uri($"pack://application:,,,/{tilesetPath}"));

            int tileWidthInPixels = 32; // Set your tile width
            int tileHeightInPixels = 32; // Set your tile height

            int columns = (int)(tilesetImage.PixelWidth / (double)tileWidthInPixels);
            int rows = (int)(tilesetImage.PixelHeight / (double)tileHeightInPixels);

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var croppedBitmap = new CroppedBitmap(
                        tilesetImage,
                        new System.Windows.Int32Rect(col * tileWidthInPixels, row * tileHeightInPixels,
                            tileWidthInPixels, tileHeightInPixels)
                    );
                    TilesetTiles.Add(new TileInfo { TileId = row * columns + col, ImageSource = croppedBitmap });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading tileset: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void LoadObjects()
    {
        Objects.Add(new ObjectInfo
        {
            ObjectId = "Tree",
            Properties = new Dictionary<string, object> { { "Collision", true }, { "Interactable", false } }
        });
        Objects.Add(new ObjectInfo
        {
            ObjectId = "Chest",
            Properties = new Dictionary<string, object>
                { { "Collision", true }, { "Interactable", true }, { "ContainsItem", "Sword" } }
        });
        Objects.Add(new ObjectInfo
        {
            ObjectId = "NPC_Guard",
            Properties = new Dictionary<string, object>
                { { "Collision", true }, { "Interactable", true }, { "DialogueId", "guard_greeting" } }
        });
        Objects.Add(new ObjectInfo { ObjectId = "SpawnPoint_Player" });
    }

    private void NewWorld()
    {
        WorldLayers.Clear();
        SelectedLayer = null;
        WorldLayers.Add(new WorldLayer
            { Name = "Ground", Width = mapWidth, Height = mapHeight, Tiles = new Tile[mapWidth, mapHeight] });
        WorldLayers.Add(new WorldLayer
            { Name = "Objects", Width = mapWidth, Height = mapHeight, Tiles = new Tile[mapWidth, mapHeight] });
        SelectedLayer = WorldLayers[0];
        DrawMap();
        currentFilePath = null;
    }

    private void OpenWorld()
    {
        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
        openFileDialog.Filter = "World Files (*.world)|*.world|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            currentFilePath = openFileDialog.FileName;
            try
            {
                string jsonString = File.ReadAllText(currentFilePath);
                var worldData = JsonSerializer.Deserialize<WorldData>(jsonString);
                if (worldData != null)
                {
                    mapWidth = worldData.TileWidth;
                    mapHeight = worldData.TileHeight;
                    tileWidth = worldData.TileWidth;
                    tileHeight = worldData.TileHeight;
                    WorldLayers.Clear();
                    foreach (var layerData in worldData.Layers)
                    {
                        var newLayer = new WorldLayer
                            { Name = layerData.Name, Width = mapWidth, Height = mapHeight, Tiles = layerData.Tiles };
                        WorldLayers.Add(newLayer);
                    }

                    SelectedLayer = WorldLayers[0];
                    DrawMap();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading world: {ex.Message}", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private void SaveWorld()
    {
        if (string.IsNullOrEmpty(currentFilePath))
        {
            SaveWorldAs();
        }
        else
        {
            SaveWorldToFile(currentFilePath);
        }
    }

    private void SaveWorldAs()
    {
        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
        saveFileDialog.Filter = "World Files (*.world)|*.world|All files (*.*)|*.*";
        if (saveFileDialog.ShowDialog() == true)
        {
            currentFilePath = saveFileDialog.FileName;
            SaveWorldToFile(currentFilePath);
        }
    }

    private void SaveWorldToFile(string filePath)
    {
        var worldData = new WorldData
        {
            TileWidth = mapWidth,
            TileHeight = mapHeight,
            Layers = WorldLayers.ToList()
        };

        try
        {
            string jsonString = JsonSerializer.Serialize(worldData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving world: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DrawMap()
    {
        MapCanvas.Children.Clear();
        if (SelectedLayer == null) return;

        for (int y = 0; y < SelectedLayer.Height; y++)
        {
            for (int x = 0; x < SelectedLayer.Width; x++)
            {
                var tile = SelectedLayer.Tiles[x, y];
                if (tile != null && tile.TileId < TilesetTiles.Count)
                {
                    var image = new Image
                    {
                        Source = TilesetTiles[tile.TileId].ImageSource,
                        Width = tileWidth,
                        Height = tileHeight
                    };
                    Canvas.SetLeft(image, x * tileWidth);
                    Canvas.SetTop(image, y * tileHeight);
                    MapCanvas.Children.Add(image);
                }
            }
        }

        DrawGrid();
    }

    private void DrawGrid()
    {
        if (SelectedLayer == null) return;
        int gridWidth = SelectedLayer.Width * tileWidth;
        int gridHeight = SelectedLayer.Height * tileHeight;

        for (int x = 0; x <= SelectedLayer.Width; x++)
        {
            var line = new Line
            {
                X1 = x * tileWidth,
                Y1 = 0,
                X2 = x * tileWidth,
                Y2 = gridHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5
            };
            MapCanvas.Children.Add(line);
        }

        for (int y = 0; y <= SelectedLayer.Height; y++)
        {
            var line = new Line
            {
                X1 = 0,
                Y1 = y * tileHeight,
                X2 = gridWidth,
                Y2 = y * tileHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 0.5
            };
            MapCanvas.Children.Add(line);
        }
    }

    private Button selectedTileButton;
    
    private void Tile_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        selectedTile = (TileInfo)button.DataContext;
        selectedObject = null;

        // Reset the border of the previously selected button (if any)
        if (selectedTileButton != null)
        {
            selectedTileButton.BorderBrush = Brushes.Transparent;
            selectedTileButton.BorderThickness = new Thickness(0);
        }

        // Highlight the newly selected button
        button.BorderBrush = Brushes.Red; // Or any color you prefer
        button.BorderThickness = new Thickness(2);
        selectedTileButton = button;
    }

    private void Object_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        selectedObject = (ObjectInfo)button.DataContext;
        selectedTile = null;
    }

    private void MapCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (SelectedLayer == null) return;
        isDrawing = true;
        startPoint = e.GetPosition(MapCanvas);
        DrawTileAtMousePosition(startPoint);
    }

    private void MapCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDrawing && e.LeftButton == MouseButtonState.Pressed && SelectedLayer != null)
        {
            Point currentPoint = e.GetPosition(MapCanvas);
            if (Math.Abs(currentPoint.X - startPoint.X) >= tileWidth ||
                Math.Abs(currentPoint.Y - startPoint.Y) >= tileHeight)
            {
                DrawTileAtMousePosition(currentPoint);
                startPoint = currentPoint;
            }
        }
    }

    private void MapCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        isDrawing = false;
    }

    private void MapCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (SelectedLayer == null) return;
        isErasing = true; // Add a new boolean variable for erasing
        startPoint = e.GetPosition(MapCanvas);
        EraseTileAtMousePosition(startPoint);
    }

    private void DrawTileAtMousePosition(Point position)
    {
        if (SelectedLayer == null || selectedTile == null) return;

        int x = (int)(position.X / tileWidth);
        int y = (int)(position.Y / tileHeight);

        if (x >= 0 && x < SelectedLayer.Width && y >= 0 && y < SelectedLayer.Height)
        {
            SelectedLayer.Tiles[x, y] = new Tile { TileId = selectedTile.TileId };
            DrawMap();
        }
    }

    private void MapCanvas_MouseMove_UpdateCoordinates(object sender, MouseEventArgs e)
    {
        if (isDrawing && e.LeftButton == MouseButtonState.Pressed && SelectedLayer != null)
        {
            Point currentPoint = e.GetPosition(MapCanvas);
            if (Math.Abs(currentPoint.X - startPoint.X) >= tileWidth || Math.Abs(currentPoint.Y - startPoint.Y) >= tileHeight)
            {
                DrawTileAtMousePosition(currentPoint);
                startPoint = currentPoint;
            }
        }
        else if (isErasing && e.RightButton == MouseButtonState.Pressed && SelectedLayer != null) // Add erasing logic
        {
            Point currentPoint = e.GetPosition(MapCanvas);
            if (Math.Abs(currentPoint.X - startPoint.X) >= tileWidth || Math.Abs(currentPoint.Y - startPoint.Y) >= tileHeight)
            {
                EraseTileAtMousePosition(currentPoint);
                startPoint = currentPoint;
            }
        }
    }
    
    private void MapCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        isErasing = false;
    }
    
    private void EraseTileAtMousePosition(Point position)
    {
        if (SelectedLayer == null) return;

        int x = (int)(position.X / tileWidth);
        int y = (int)(position.Y / tileHeight);

        if (x >= 0 && x < SelectedLayer.Width && y >= 0 && y < SelectedLayer.Height)
        {
            SelectedLayer.Tiles[x, y] = null; // Set the tile to null to "erase" it
            DrawMap();
        }
    }

    private void AddLayer_Click(object sender, RoutedEventArgs e)
    {
        var newLayer = new WorldLayer
        {
            Name = $"Layer {WorldLayers.Count + 1}", Width = mapWidth, Height = mapHeight,
            Tiles = new Tile[mapWidth, mapHeight]
        };
        WorldLayers.Add(newLayer);
        SelectedLayer = newLayer;
    }

    private void RemoveLayer_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedLayer != null)
        {
            WorldLayers.Remove(SelectedLayer);
            SelectedLayer = WorldLayers.FirstOrDefault();
        }
    }

    private void MoveLayerUp_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedLayer != null)
        {
            int index = WorldLayers.IndexOf(SelectedLayer);
            if (index > 0)
            {
                WorldLayers.Move(index, index - 1);
            }
        }
    }

    private void MoveLayerDown_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedLayer != null)
        {
            int index = WorldLayers.IndexOf(SelectedLayer);
            if (index < WorldLayers.Count - 1)
            {
                WorldLayers.Move(index, index + 1);
            }
        }
    }

    private void UpdatePropertyPanel()
    {
        PropertyPanel.Children.Clear();
        if (SelectedLayer != null)
        {
            var textBlock = new TextBlock
                { Text = $"Selected Layer: {SelectedLayer.Name}", FontWeight = FontWeights.Bold };
            PropertyPanel.Children.Add(textBlock);
        }
    }

    private void NewWorld_Click(object sender, RoutedEventArgs e) => NewWorld();
    private void OpenWorld_Click(object sender, RoutedEventArgs e) => OpenWorld();
    private void SaveWorld_Click(object sender, RoutedEventArgs e) => SaveWorld();
    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

    private void Undo_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show("Undo functionality not implemented yet.");

    private void Redo_Click(object sender, RoutedEventArgs e) =>
        MessageBox.Show("Redo functionality not implemented yet.");

    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}