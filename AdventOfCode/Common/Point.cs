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

    public Point2D Distance(Point2D other)
    {
        var distances = base.Distance(other);

        return new Point2D(distances[(int)Coordinate.X], distances[(int)Coordinate.Y]);
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

    public Point3D Distance(Point3D other)
    {
        var distances = base.Distance(other);

        return new Point3D(distances[(int)Coordinate.X], distances[(int)Coordinate.Y], distances[(int)Coordinate.Z]);
    }
}
