using UnoTetris.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace UnoTetris;

public sealed partial class MainPage : Page
{
    private const int CellSize = 30;
    private const int NextPieceCellSize = 24;
    private readonly GameState _gameState;
    private DispatcherTimer? _gameTimer;
    private readonly Random _random = new();
    private bool _isAutoPlay = false;

    public MainPage()
    {
        this.InitializeComponent();
        _gameState = new GameState();
        InitializeGame();
        
        // Enable keyboard input
        this.KeyDown += MainPage_KeyDown;
        this.Loaded += (s, e) => this.Focus(FocusState.Programmatic);
    }

    private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (_gameState.IsGameOver || _gameTimer == null) return;

        switch (e.Key)
        {
            case VirtualKey.Left:
                _gameState.MoveLeft();
                UpdateUI();
                e.Handled = true;
                break;
            
            case VirtualKey.Right:
                _gameState.MoveRight();
                UpdateUI();
                e.Handled = true;
                break;
            
            case VirtualKey.Down:
                _gameState.MoveDown();
                UpdateUI();
                e.Handled = true;
                break;
            
            case VirtualKey.Up:
            case VirtualKey.Space:
                _gameState.Rotate();
                UpdateUI();
                e.Handled = true;
                break;
            
            case VirtualKey.Enter:
                _gameState.HardDrop();
                UpdateUI();
                e.Handled = true;
                break;
        }
    }

    private void InitializeGame()
    {
        DrawGrid();
        _gameState.Reset();
        UpdateUI();
    }

    private void DrawGrid()
    {
        GameCanvas.Children.Clear();

        // Draw grid cells
        for (int i = 0; i < GameState.GridHeight; i++)
        {
            for (int j = 0; j < GameState.GridWidth; j++)
            {
                var rect = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 37, 37, 56)),
                    StrokeThickness = 0.5,
                    Fill = new SolidColorBrush(Colors.Transparent)
                };

                Canvas.SetLeft(rect, j * CellSize);
                Canvas.SetTop(rect, i * CellSize);
                GameCanvas.Children.Add(rect);
            }
        }
    }

    private void UpdateUI()
    {
        // Clear dynamic elements (keep grid)
        var gridCells = GameCanvas.Children.Where(c => c is Rectangle).Take(GameState.GridWidth * GameState.GridHeight).ToList();
        GameCanvas.Children.Clear();
        foreach (var cell in gridCells)
        {
            GameCanvas.Children.Add(cell);
        }

        // Draw locked pieces
        for (int i = 0; i < GameState.GridHeight; i++)
        {
            for (int j = 0; j < GameState.GridWidth; j++)
            {
                if (!string.IsNullOrEmpty(_gameState.Grid[i, j]))
                {
                    var rect = CreateCell(_gameState.Grid[i, j]);
                    Canvas.SetLeft(rect, j * CellSize);
                    Canvas.SetTop(rect, i * CellSize);
                    GameCanvas.Children.Add(rect);
                }
            }
        }

        // Draw current piece
        if (_gameState.CurrentPiece != null)
        {
            DrawPiece(_gameState.CurrentPiece, GameCanvas, CellSize);
        }

        // Draw next piece
        DrawNextPiece();

        // Update stats
        ScoreText.Text = _gameState.Score.ToString();
        LevelText.Text = _gameState.Level.ToString();
        LinesText.Text = _gameState.LinesCleared.ToString();

        // Check game over
        if (_gameState.IsGameOver)
        {
            StopGame();
            ShowGameOver();
        }
    }

    private Rectangle CreateCell(string color)
    {
        var rect = new Rectangle
        {
            Width = CellSize - 2,
            Height = CellSize - 2,
            Fill = new SolidColorBrush(ParseColor(color)),
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
            StrokeThickness = 1,
            RadiusX = 3,
            RadiusY = 3
        };

        return rect;
    }

    private void DrawPiece(Tetromino piece, Canvas canvas, int cellSize)
    {
        int rows = piece.Shape.GetLength(0);
        int cols = piece.Shape.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (piece.Shape[i, j] == 1)
                {
                    var rect = new Rectangle
                    {
                        Width = cellSize - 2,
                        Height = cellSize - 2,
                        Fill = new SolidColorBrush(ParseColor(piece.Color)),
                        Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
                        StrokeThickness = 1,
                        RadiusX = 3,
                        RadiusY = 3
                    };

                    Canvas.SetLeft(rect, (piece.X + j) * cellSize);
                    Canvas.SetTop(rect, (piece.Y + i) * cellSize);
                    canvas.Children.Add(rect);
                }
            }
        }
    }

    private void DrawNextPiece()
    {
        NextPieceCanvas.Children.Clear();

        if (_gameState.NextPiece == null) return;

        var piece = _gameState.NextPiece;
        int rows = piece.Shape.GetLength(0);
        int cols = piece.Shape.GetLength(1);

        double offsetX = (NextPieceCanvas.Width - cols * NextPieceCellSize) / 2;
        double offsetY = (NextPieceCanvas.Height - rows * NextPieceCellSize) / 2;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (piece.Shape[i, j] == 1)
                {
                    var rect = new Rectangle
                    {
                        Width = NextPieceCellSize - 2,
                        Height = NextPieceCellSize - 2,
                        Fill = new SolidColorBrush(ParseColor(piece.Color)),
                        Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
                        StrokeThickness = 1,
                        RadiusX = 2,
                        RadiusY = 2
                    };

                    Canvas.SetLeft(rect, offsetX + j * NextPieceCellSize);
                    Canvas.SetTop(rect, offsetY + i * NextPieceCellSize);
                    NextPieceCanvas.Children.Add(rect);
                }
            }
        }
    }

    private Windows.UI.Color ParseColor(string hex)
    {
        hex = hex.TrimStart('#');
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        return ColorHelper.FromArgb(255, r, g, b);
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartGame();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StopGame();
    }

    private void PlayAgain_Click(object sender, RoutedEventArgs e)
    {
        GameOverOverlay.Visibility = Visibility.Collapsed;
        InitializeGame();
        StartGame();
    }

    private void StartGame()
    {
        if (_gameState.IsGameOver)
        {
            InitializeGame();
        }

        StartButton.IsEnabled = false;
        StartButton.Opacity = 0.5;
        StopButton.IsEnabled = true;
        StopButton.Opacity = 1.0;

        _gameTimer = new DispatcherTimer();
        _gameTimer.Interval = TimeSpan.FromMilliseconds(GetGameSpeed());
        _gameTimer.Tick += GameTimer_Tick;
        _gameTimer.Start();
    }

    private void StopGame()
    {
        _gameTimer?.Stop();
        _gameTimer = null;

        StartButton.IsEnabled = true;
        StartButton.Opacity = 1.0;
        StopButton.IsEnabled = false;
        StopButton.Opacity = 0.5;
    }

    private void GameTimer_Tick(object? sender, object e)
    {
        // Auto-play AI logic (only if auto-play mode is enabled)
        if (_isAutoPlay)
        {
            PerformAutoMove();
        }

        // Move piece down (no score for automatic drops)
        _gameState.MoveDown(false);
        
        UpdateUI();

        // Adjust speed based on level
        if (_gameTimer != null)
        {
            _gameTimer.Interval = TimeSpan.FromMilliseconds(GetGameSpeed());
        }
    }

    private void PerformAutoMove()
    {
        if (_gameState.CurrentPiece == null) return;

        // Simple AI: Random moves for demonstration
        // In a real AI, you'd evaluate positions and choose the best one
        int action = _random.Next(100);

        if (action < 15)
        {
            _gameState.Rotate();
        }
        else if (action < 40)
        {
            _gameState.MoveLeft();
        }
        else if (action < 65)
        {
            _gameState.MoveRight();
        }
        // else just let it fall
    }

    private double GetGameSpeed()
    {
        // Speed increases with level (faster = smaller interval)
        return Math.Max(100, 500 - (_gameState.Level - 1) * 40);
    }

    private void ShowGameOver()
    {
        FinalScoreText.Text = _gameState.Score.ToString();
        GameOverOverlay.Visibility = Visibility.Visible;
    }

    private void AutoPlayToggle_Changed(object sender, RoutedEventArgs e)
    {
        _isAutoPlay = AutoPlayToggle.IsChecked == true;
    }
}
