namespace UnoTetris.Models;

public enum TetrominoType
{
    I, O, T, S, Z, J, L
}

public class Tetromino
{
    public TetrominoType Type { get; }
    public int[,] Shape { get; private set; }
    public string Color { get; }
    public int X { get; set; }
    public int Y { get; set; }

    private static readonly Dictionary<TetrominoType, (int[,] shape, string color)> TetrominoData = new()
    {
        { TetrominoType.I, (new[,] { { 1, 1, 1, 1 } }, "#00F0F0") },
        { TetrominoType.O, (new[,] { { 1, 1 }, { 1, 1 } }, "#F0F000") },
        { TetrominoType.T, (new[,] { { 0, 1, 0 }, { 1, 1, 1 } }, "#A000F0") },
        { TetrominoType.S, (new[,] { { 0, 1, 1 }, { 1, 1, 0 } }, "#00F000") },
        { TetrominoType.Z, (new[,] { { 1, 1, 0 }, { 0, 1, 1 } }, "#F00000") },
        { TetrominoType.J, (new[,] { { 1, 0, 0 }, { 1, 1, 1 } }, "#0000F0") },
        { TetrominoType.L, (new[,] { { 0, 0, 1 }, { 1, 1, 1 } }, "#F0A000") }
    };

    public Tetromino(TetrominoType type)
    {
        Type = type;
        var data = TetrominoData[type];
        Shape = (int[,])data.shape.Clone();
        Color = data.color;
        X = 3;
        Y = 0;
    }

    public void Rotate()
    {
        int rows = Shape.GetLength(0);
        int cols = Shape.GetLength(1);
        int[,] rotated = new int[cols, rows];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                rotated[j, rows - 1 - i] = Shape[i, j];
            }
        }

        Shape = rotated;
    }

    public Tetromino Clone()
    {
        var clone = new Tetromino(Type)
        {
            Shape = (int[,])Shape.Clone(),
            X = X,
            Y = Y
        };
        return clone;
    }
}
