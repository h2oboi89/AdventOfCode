using System.Text.RegularExpressions;

namespace AdventOfCode.Common;

internal abstract class Point
{
    protected readonly int[] coordinates = Array.Empty<int>();

    protected Point(int dimensions) { coordinates = new int[dimensions]; }

    protected Point(IEnumerable<int> coords) { coordinates = coords.ToArray(); }

    protected enum Coordinate { X = 0, Y = 1, Z = 2, }

    protected int Get(Coordinate coordinate) => coordinates[(int)coordinate];
    protected void Set(Coordinate coordinate, int value) => coordinates[(int)coordinate] = value;

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;

        if (obj is not Point other) return false;

        return coordinates.SequenceEqual(other.coordinates);
    }

    public override int GetHashCode()
    {
        var hash = coordinates.Length;

        foreach (var coor in coordinates)
        {
            hash ^= coor.GetHashCode();
        }

        return hash;
    }

    public override string ToString() => $"( {string.Join(", ", coordinates)} )";

    protected int[] Distance(Point other)
    {
        var distances = new int[coordinates.Length];

        for (var i = 0; i < coordinates.Length; i++)
        {
            distances[i] = Math.Abs(coordinates[i] - other.coordinates[i]);
        }

        return distances;
    }
}

internal class Point2D : Point
{
    public Point2D() : base(2) { }

    public Point2D(int x, int y) : this()
    {
        Set(Coordinate.X, x);
        Set(Coordinate.Y, y);
    }

    private Point2D(IEnumerable<int> coordinates) : base(coordinates) { }

    public int X => Get(Coordinate.X);

    public int Y => Get(Coordinate.Y);

    public static Point2D operator -(Point2D p) => new(p.coordinates.Select(c => -c));

    public static Point2D operator +(Point2D a, Point2D b) => new(a.coordinates.Zip(b.coordinates).Select(pair => pair.First + pair.Second));

    public Point2D Distance(Point2D other) => new(base.Distance(other));

    public IEnumerable<Point2D> GetNeighbors()
    {
        // top left
        yield return new Point2D(X - 1, Y - 1);

        // top
        yield return new Point2D(X, Y - 1);

        // top right
        yield return new Point2D(X + 1, Y - 1);

        // left
        yield return new Point2D(X - 1, Y);

        // self
        yield return this;

        // right
        yield return new Point2D(X + 1, Y);

        // bottom left
        yield return new Point2D(X - 1, Y + 1);

        // bottom
        yield return new Point2D(X, Y + 1);

        // bottom right
        yield return new Point2D(X + 1, Y + 1);
    }

    private static readonly Regex parseRegex = new(@"\( (?<x>-?\d+), (?<y>-?\d+) \)");

    public static Point2D? Parse(string input)
    {
        var match = parseRegex.Match(input);

        if (match.Success)
        {
            return new Point2D(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value));
        }

        return null;
    }
}

internal class Point3D : Point
{
    public Point3D() : base(3) { }

    public Point3D(int x, int y, int z) : this()
    {
        Set(Coordinate.X, x);
        Set(Coordinate.Y, y);
        Set(Coordinate.Z, z);
    }

    private Point3D(IEnumerable<int> coordinates) : base(coordinates) { }

    public int X => Get(Coordinate.X);

    public int Y => Get(Coordinate.Y);

    public int Z => Get(Coordinate.Z);

    public static Point3D operator -(Point3D p) => new(p.coordinates.Select(c => -c));

    public static Point3D operator +(Point3D a, Point3D b) => new(a.coordinates.Zip(b.coordinates).Select(pair => pair.First + pair.Second));

    public Point3D Distance(Point3D other) => new(base.Distance(other));

    private static readonly Regex parseRegex = new(@"\( (?<x>-?\d+), (?<y>-?\d+), (?<z>-?\d+) \)");

    public static Point3D? Parse(string input)
    {
        var match = parseRegex.Match(input);

        if (match.Success)
        {
            return new Point3D(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value), int.Parse(match.Groups["z"].Value));
        }

        return null;
    }
}
