namespace AdventOfCode.Common;

internal abstract class Point
{
    protected readonly int[] coordinates = Array.Empty<int>();

    protected Point(int size) { coordinates = new int[size]; }

    protected enum Coordinate { X = 0, Y = 1, Z = 2, }

    protected int Get(Coordinate coor) => coordinates[(int)coor];
    protected void Set(Coordinate coor, int value) => coordinates[(int)coor] = value;

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
}

internal class Point2D : Point
{
    public Point2D(int x, int y) : base(2)
    {
        Set(Coordinate.X, x);
        Set(Coordinate.Y, y);
    }

    public int X => Get(Coordinate.X);

    public int Y => Get(Coordinate.Y);
}

internal class Point3D : Point
{
    public Point3D(int x, int y, int z) : base(3)
    {
        Set(Coordinate.X, x);
        Set(Coordinate.Y, y);
        Set(Coordinate.Z, z);
    }

    public int X => Get(Coordinate.X);

    public int Y => Get(Coordinate.Y);

    public int Z => Get(Coordinate.Z);
}
