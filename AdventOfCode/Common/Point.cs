using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode.Common;

internal struct Point
{
    private readonly int[] _coordinates = Array.Empty<int>();

    private const int X = 0;
    private const int Y = 1;
    private const int Z = 2;

    private Point(int dimensions) { _coordinates = new int[dimensions]; }

    private Point(int[] coordinates) { _coordinates = coordinates; }

    private int[] Distance(Point other)
    {
        var distances = new int[_coordinates.Length];

        for (var i = 0; i < _coordinates.Length; i++)
        {
            distances[i] = Math.Abs(_coordinates[i] - other._coordinates[i]);
        }

        return distances;
    }

    public static Point operator -(Point p)
    {
        var newCoordinates = new int[p._coordinates.Length];

        for (var i = 0; i < newCoordinates.Length; i++)
        {
            newCoordinates[i] = -p._coordinates[i];
        }

        return new Point(newCoordinates);
    }

    public static Point operator +(Point a, Point b)
    {
        var newCoordinates = new int[a._coordinates.Length];

        for (var i = 0; i < newCoordinates.Length; i++)
        {
            newCoordinates[i] = a._coordinates[i] + b._coordinates[i];
        }

        return new Point(newCoordinates);
    }

    public static bool operator ==(Point a, Point b) => a.Equals(b);

    public static bool operator !=(Point a, Point b) => !(a == b);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Point other) return false;

        if (_coordinates.Length != other._coordinates.Length) return false;

        for (var i = 0; i < _coordinates.Length; i++)
        {
            if (_coordinates[i] != other._coordinates[i]) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hash = _coordinates.Length;

        for (var i = 0; i < _coordinates.Length; i++)
        {
            hash ^= _coordinates[i];
        }

        return hash;
    }

    public override string ToString() => $"( {string.Join(", ", _coordinates)} )";

    public struct D2
    {
        private readonly Point point;

        public D2() => point = new Point(2);

        public D2(int x, int y) : this()
        {
            point._coordinates[Point.X] = x;
            point._coordinates[Point.Y] = y;
        }

        private D2(Point p) => point = p;

        public int X => point._coordinates[Point.X];

        public int Y => point._coordinates[Point.Y];

        public D2 Distance(D2 other) => new(new Point(point.Distance(other.point)));

        public IEnumerable<D2> GetNeighbors()
        {
            // top left
            yield return new D2(X - 1, Y - 1);

            // top
            yield return new D2(X, Y - 1);

            // top right
            yield return new D2(X + 1, Y - 1);

            // left
            yield return new D2(X - 1, Y);

            // self
            yield return this;

            // right
            yield return new D2(X + 1, Y);

            // bottom left
            yield return new D2(X - 1, Y + 1);

            // bottom
            yield return new D2(X, Y + 1);

            // bottom right
            yield return new D2(X + 1, Y + 1);
        }

        private static readonly Regex parseRegex = new(@"\( (?<x>-?\d+), (?<y>-?\d+) \)");

        public static D2? Parse(string input)
        {
            var match = parseRegex.Match(input);

            if (match.Success)
            {
                return new D2(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value));
            }

            return null;
        }

        public static D2 operator -(D2 p) => new(-p.point);

        public static D2 operator +(D2 a, D2 b) => new(a.point + b.point);

        public static bool operator ==(D2 a, D2 b) => a.Equals(b);

        public static bool operator !=(D2 a, D2 b) => !(a == b);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not D2 other) return false;

            return point.Equals(other.point);
        }

        public override int GetHashCode() => point.GetHashCode();

        public override string ToString() => point.ToString();
    }

    internal struct D3
    {
        private readonly Point point;

        public D3() => point = new Point(3);

        public D3(int x, int y, int z) : this()
        {
            point._coordinates[Point.X] = x;
            point._coordinates[Point.Y] = y;
            point._coordinates[Point.Z] = z;
        }

        private D3(Point p) => point = p;

        public int X => point._coordinates[Point.X];

        public int Y => point._coordinates[Point.Y];

        public int Z => point._coordinates[Point.Z];

        public D3 Distance(D3 other) => new(new Point(point.Distance(other.point)));

        private static readonly Regex parseRegex = new(@"\( (?<x>-?\d+), (?<y>-?\d+), (?<z>-?\d+) \)");

        public static D3? Parse(string input)
        {
            var match = parseRegex.Match(input);

            if (match.Success)
            {
                return new D3(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value), int.Parse(match.Groups["z"].Value));
            }

            return null;
        }

        public static D3 operator -(D3 p) => new(-p.point);

        public static D3 operator +(D3 a, D3 b) => new(a.point + b.point);

        public static bool operator ==(D3 a, D3 b) => a.Equals(b);

        public static bool operator !=(D3 a, D3 b) => !(a == b);

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not D3 other) return false;

            return point.Equals(other.point);
        }

        public override int GetHashCode() => point.GetHashCode();

        public override string ToString() => point.ToString();
    }
}
