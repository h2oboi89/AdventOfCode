namespace AdventOfCode.Common;

internal class Point
{
    public readonly int X, Y;

    public Point(int x, int y) { X = x; Y = y; }

    public override string ToString() => $"( {X}, {Y} )";

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is not Point other) return false;

        return other.X == X & other.Y == Y;
    }

    public override int GetHashCode() => (X, Y).GetHashCode();
}
