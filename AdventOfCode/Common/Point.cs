using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode.Common;

internal struct Point
{
    private readonly int[] Coordinates = Array.Empty<int>();

    private enum Coordinate { X = 0, Y = 1, Z = 2, }

    private Point(int dimensions) { Coordinates = new int[dimensions]; }

    private Point(IEnumerable<int> coordinates) { Coordinates = coordinates.ToArray(); }

    private int Get(Coordinate coordinate) => Coordinates[(int)coordinate];

    private void Set(Coordinate coordinate, int value) => Coordinates[(int)coordinate] = value;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Point other) return false;

        return Coordinates.SequenceEqual(other.Coordinates);
    }

    public override int GetHashCode()
    {
        var hash = Coordinates.Length;

        foreach (var coor in Coordinates)
        {
            hash ^= coor.GetHashCode();
        }

        return hash;
    }

    public override string ToString() => $"( {string.Join(", ", Coordinates)} )";

    private int[] Distance(Point other)
    {
        var distances = new int[Coordinates.Length];

        for (var i = 0; i < Coordinates.Length; i++)
        {
            distances[i] = Math.Abs(Coordinates[i] - other.Coordinates[i]);
        }

        return distances;
    }

    public static Point operator -(Point p) => new(p.Coordinates.Select(c => -c));

    public static Point operator +(Point a, Point b) => new(a.Coordinates.Zip(b.Coordinates).Select(pair => pair.First + pair.Second));

    public struct D2 {
        private readonly Point point;

        public D2() => point = new Point(2);

        public D2(int x, int y) : this()
        {
            point.Set(Coordinate.X, x);
            point.Set(Coordinate.Y, y);
        }

        private D2(Point p) => point = p;

        public int X => point.Get(Coordinate.X);

        public int Y => point.Get(Coordinate.Y);

        public static D2 operator -(D2 p) => new(-p.point);

        public static D2 operator +(D2 a, D2 b) => new(a.point + b.point);

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
            point.Set(Coordinate.X, x);
            point.Set(Coordinate.Y, y);
            point.Set(Coordinate.Z, z);
        }

        private D3(Point p) => point = p;

        public int X => point.Get(Coordinate.X);

        public int Y => point.Get(Coordinate.Y);

        public int Z => point.Get(Coordinate.Z);

        public static D3 operator -(D3 p) => new(-p.point);

        public static D3 operator +(D3 a, D3 b) => new(a.point + b.point);

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

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is not D3 other) return false;

            return point.Equals(other.point);
        }

        public override int GetHashCode() => point.GetHashCode();

        public override string ToString() => point.ToString();
    }
}
