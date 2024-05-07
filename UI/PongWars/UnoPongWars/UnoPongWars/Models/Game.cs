using System.Runtime.CompilerServices;
using Uno.Extensions.Specialized;
using Windows.Foundation;

namespace UnoPongWars.Models;

public record Game(int Width, int Height)
{
    private readonly List<Cell> _cells = [];

    private Point _ball1Direction = new(0, 0);
    private Point _ball2Direction = new(0, 0);

    private int X(Cell cell) => cell.Id % Width;
    private int Y(Cell cell) => cell.Id / Width;
    private int Index(int x, int y) => y * Width + x;
    private int Player(int row) => (row >= Height / 2) ? 1 : 0;

    private Point BallDirection(Cell cell) => cell.Player == 0 ? _ball1Direction : _ball2Direction;
    private Point BallPosition(Cell cell) => new(X(cell), Y(cell));

    private double RandomDirection() => new Random().NextDouble() > 0.5 ? 0.5 : -0.5;

    public int Speed { get; set; } = 900;

    private int NewIndex(Point ballPosition, Point ballDirection, Cell cell)
    {
        var newDirection = CheckBoundary(ballPosition, ballDirection);

        UpdateBallDirection(newDirection, cell.Player);

        var newX = (int)Math.Round(ballPosition.X + ballDirection.X + newDirection.X);
        var newY = (int)Math.Round(ballPosition.Y + ballDirection.Y + newDirection.Y);

        newX = Math.Clamp(newX, 0, Width - 1);
        newY = Math.Clamp(newY, 0, Height - 1);

        return Index(newX, newY);
    }

    private Cell Update(Cell cell)
    {
        _cells[cell.Id] = cell;
        return cell;
    }

    private Point Bounce(Point direction, double angle)
    {
        // Determine bounce direction based on the angle
        if (Math.Abs(Math.Cos(angle)) > Math.Abs(Math.Sin(angle)))
        {
            return new Point(-direction.X, direction.Y);
        }
        else
        {
            return new Point(direction.X, -direction.Y);
        }
    }

    private ImmutableList<Cell> Initialize()
    {
        var ball1RandomValue = RandomDirection();
        var ball2RandomValue = RandomDirection();
        _ball1Direction = new(ball1RandomValue, -ball1RandomValue);
        _ball2Direction = new(-ball2RandomValue, ball2RandomValue);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var hasBall = (y == 5 && x == 5) || (y == Height - 5 && x == Width - 5);

                _cells.Add(new Cell(_cells.Count, Player(y), hasBall));
            }
        }
        return _cells.ToImmutableList();
    }

    public async IAsyncEnumerable<IImmutableList<Cell>> Loop([EnumeratorCancellation] CancellationToken ct)
    {
        var board = Initialize();
        yield return board;

        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(1010 - Speed, ct);
            board = GameLogic(board);
            yield return board;
        }
    }

    private ImmutableList<Cell> GameLogic(ImmutableList<Cell> board)
    {
        var edited = board.ToBuilder();
        foreach (var modifiedCell in Tick())
        {
            edited[modifiedCell.Id] = modifiedCell;
        }
        return edited.ToImmutable();
    }

    private IEnumerable<Cell> Tick()
    {
        if (_cells is not { })
        {
            yield break;
        }

        foreach (var cell in _cells.Where(c => c.HasBall).ToList())
        {
            var ballPosition = BallPosition(cell);
            var ballDirection = BallDirection(cell);

            foreach (var (updatedCell, updatedDirection) in CheckCollisions(cell, ballPosition, ballDirection))
            {
                if (updatedCell.HasBall)
                {
                    yield return cell;
                    continue;
                }

                yield return updatedCell;
                ballDirection = updatedDirection;
            }

            var newIndex = NewIndex(ballPosition, ballDirection, cell);

            if (newIndex != cell.Id)
            {
                yield return Update(_cells[newIndex] with { Player = cell.Player, HasBall = true });
                yield return Update(cell with { HasBall = false });
            }
        }
    }

    private IEnumerable<(Cell, Point)> CheckCollisions(Cell cell, Point ballPosition, Point ballDirection)
    {
        var neighbors = new[]
        {
            (0, -1),
            (0, 1),
            (-1, 0),
            (1, 0)
        };

        foreach (var (xOffset, yOffset) in neighbors)
        {
            var checkX = ballPosition.X + xOffset;
            var checkY = ballPosition.Y + yOffset;

            if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
            {
                var i = (int)checkX;
                var j = (int)checkY;

                var index = Index(i, j);
                var targetCell = _cells[index];

                if (targetCell.Player != cell.Player && !targetCell.HasBall)
                {
                    var newDirection = Bounce(ballDirection, Math.Atan2(yOffset, xOffset));
                    UpdateBallDirection(newDirection, cell.Player);

                    yield return (Update(_cells[index] with { Player = cell.Player }), newDirection);
                }
            }
        }
    }

    private Point CheckBoundary(Point ballPosition, Point ballDirection) =>
        new(CheckBoundaryForAxis(ballPosition.X, ballDirection.X, Width - 1),
            CheckBoundaryForAxis(ballPosition.Y, ballDirection.Y, Height - 1));

    private double CheckBoundaryForAxis(double position, double direction, int maxBoundary)
    {
        if (position + direction > maxBoundary || position + direction < 0)
        {
            return -direction;
        }
        else
        {
            return direction;
        }
    }

    private void UpdateBallDirection(Point boundary, int playerId)
    {
        if (playerId == 0)
        {
            _ball1Direction = boundary;
        }
        else
        {
            _ball2Direction = boundary;
        }
    }
}
