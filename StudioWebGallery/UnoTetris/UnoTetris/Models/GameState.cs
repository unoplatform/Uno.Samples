namespace UnoTetris.Models;

public class GameState
{
    public const int GridWidth = 10;
    public const int GridHeight = 20;

    public string[,] Grid { get; } = new string[GridHeight, GridWidth];
    public Tetromino? CurrentPiece { get; set; }
    public Tetromino? NextPiece { get; set; }
    public int Score { get; set; }
    public int Level { get; set; } = 1;
    public int LinesCleared { get; set; }
    public bool IsGameOver { get; set; }

    private readonly Random _random = new();
    private readonly TetrominoType[] _allTypes = Enum.GetValues<TetrominoType>();

    public GameState()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        for (int i = 0; i < GridHeight; i++)
        {
            for (int j = 0; j < GridWidth; j++)
            {
                Grid[i, j] = string.Empty;
            }
        }
    }

    public void Reset()
    {
        InitializeGrid();
        Score = 0;
        Level = 1;
        LinesCleared = 0;
        IsGameOver = false;
        CurrentPiece = null;
        NextPiece = null;
        SpawnNewPiece();
    }

    public void SpawnNewPiece()
    {
        if (NextPiece == null)
        {
            NextPiece = CreateRandomPiece();
        }

        CurrentPiece = NextPiece;
        NextPiece = CreateRandomPiece();

        // Check if game over
        if (CurrentPiece != null && !CanPlacePiece(CurrentPiece))
        {
            IsGameOver = true;
        }
    }

    private Tetromino CreateRandomPiece()
    {
        var type = _allTypes[_random.Next(_allTypes.Length)];
        return new Tetromino(type);
    }

    public bool CanPlacePiece(Tetromino piece)
    {
        int rows = piece.Shape.GetLength(0);
        int cols = piece.Shape.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (piece.Shape[i, j] == 0) continue;

                int gridY = piece.Y + i;
                int gridX = piece.X + j;

                if (gridX < 0 || gridX >= GridWidth || gridY >= GridHeight)
                    return false;

                if (gridY >= 0 && !string.IsNullOrEmpty(Grid[gridY, gridX]))
                    return false;
            }
        }

        return true;
    }

    public bool MoveDown(bool addScore = true)
    {
        if (CurrentPiece == null) return false;

        CurrentPiece.Y++;
        if (!CanPlacePiece(CurrentPiece))
        {
            CurrentPiece.Y--;
            LockPiece();
            return false;
        }
        
        // Soft drop bonus: 1 point per row (only when manually moved)
        if (addScore)
        {
            Score += 1;
        }
        return true;
    }

    public void MoveLeft()
    {
        if (CurrentPiece == null) return;

        CurrentPiece.X--;
        if (!CanPlacePiece(CurrentPiece))
        {
            CurrentPiece.X++;
        }
    }

    public void MoveRight()
    {
        if (CurrentPiece == null) return;

        CurrentPiece.X++;
        if (!CanPlacePiece(CurrentPiece))
        {
            CurrentPiece.X--;
        }
    }

    public void Rotate()
    {
        if (CurrentPiece == null) return;

        var clone = CurrentPiece.Clone();
        clone.Rotate();

        if (CanPlacePiece(clone))
        {
            CurrentPiece.Rotate();
        }
    }

    public void HardDrop()
    {
        if (CurrentPiece == null) return;

        int dropDistance = 0;
        while (true)
        {
            CurrentPiece.Y++;
            if (!CanPlacePiece(CurrentPiece))
            {
                CurrentPiece.Y--;
                break;
            }
            dropDistance++;
        }
        
        // Hard drop bonus: 2 points per row dropped
        Score += dropDistance * 2;
        
        LockPiece();
    }

    private void LockPiece()
    {
        if (CurrentPiece == null) return;

        int rows = CurrentPiece.Shape.GetLength(0);
        int cols = CurrentPiece.Shape.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (CurrentPiece.Shape[i, j] == 1)
                {
                    int gridY = CurrentPiece.Y + i;
                    int gridX = CurrentPiece.X + j;

                    if (gridY >= 0 && gridY < GridHeight && gridX >= 0 && gridX < GridWidth)
                    {
                        Grid[gridY, gridX] = CurrentPiece.Color;
                    }
                }
            }
        }

        // Award points for placing a piece
        Score += 10 * Level;
        
        ClearLines();
        SpawnNewPiece();
    }

    private void ClearLines()
    {
        int linesCleared = 0;

        for (int i = GridHeight - 1; i >= 0; i--)
        {
            bool isLineFull = true;
            for (int j = 0; j < GridWidth; j++)
            {
                if (string.IsNullOrEmpty(Grid[i, j]))
                {
                    isLineFull = false;
                    break;
                }
            }

            if (isLineFull)
            {
                linesCleared++;
                // Move all rows above down
                for (int k = i; k > 0; k--)
                {
                    for (int j = 0; j < GridWidth; j++)
                    {
                        Grid[k, j] = Grid[k - 1, j];
                    }
                }

                // Clear top row
                for (int j = 0; j < GridWidth; j++)
                {
                    Grid[0, j] = string.Empty;
                }

                i++; // Check the same row again
            }
        }

        if (linesCleared > 0)
        {
            LinesCleared += linesCleared;
            
            // More generous scoring for line clears
            // 1 line: 100 points
            // 2 lines: 300 points
            // 3 lines: 500 points
            // 4 lines (Tetris): 800 points
            int[] lineScores = { 0, 100, 300, 500, 800 };
            int baseScore = lineScores[Math.Min(linesCleared, 4)];
            Score += baseScore * Level;
            
            Level = 1 + LinesCleared / 10;
        }
    }
}
