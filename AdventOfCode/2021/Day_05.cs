﻿using System.Text;

namespace AdventOfCode._2021;

internal class Day_05 : BaseDay
{
    private class Point
    {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y) { X = x; Y = y; }

        public override string ToString() => $"( {X}, {Y} )";
    }

    private class PointPair
    {
        public readonly Point Start;
        public readonly Point End;

        public PointPair(Point start, Point end) { Start = start; End = end; }

        public bool IsHorizonal => Start.Y == End.Y;

        public bool IsVertical => Start.X == End.X;

        public bool IsDiagonal => !IsHorizonal && !IsVertical;

        public override string ToString() => $"[ {Start}, {End} ] {IsDiagonal}";

        private IEnumerable<Point> GetHorizonalLine
        {
            get
            {
                var start = Start;
                var end = End;

                if (End.X < Start.X)
                {
                    start = End;
                    end = Start;
                }

                for (var x = start.X; x <= end.X; x++)
                {
                    yield return new Point(x, Start.Y);
                }
            }
        }

        private IEnumerable<Point> GetVerticalLine
        {
            get
            {
                var start = Start;
                var end = End;

                if (End.Y < Start.Y)
                {
                    start = End;
                    end = Start;
                }

                for (var y = start.Y; y <= end.Y; y++)
                {
                    yield return new Point(Start.X, y);
                }
            }
        }

        private IEnumerable<Point> GetDiagonalLine
        {
            get
            {
                var xDir = 1; var yDir = 1;

                if (End.X < Start.X) xDir = -1;
                if (End.Y < Start.Y) yDir = -1;

                var p = new Point(Start.X, Start.Y);
                for (var i = 0; i <= Math.Abs(End.X - Start.X); i++)
                {
                    yield return p;

                    p = new Point(p.X + xDir, p.Y + yDir);
                }
            }
        }

        public IEnumerable<Point> GetLine()
        {
            if (IsHorizonal) return GetHorizonalLine;

            if (IsVertical) return GetVerticalLine;

            return GetDiagonalLine;
        }
    }

    private class DangerZone
    {
        public readonly int[,] Points;

        public DangerZone(int dimension)
        {
            Points = new int[dimension, dimension];
        }

        public void AddVent(PointPair line)
        {
            foreach (var point in line.GetLine())
            {
                Points[point.Y, point.X]++;
            }
        }

        public int DangerVents(int level)
        {
            var result = 0;

            foreach (var point in Points)
            {
                if (point >= level) result++;
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var dimension = Points.GetLength(0);
            var numDigits = dimension.NumberOfDigits() - 1;

            for (var x = 0; x < dimension; x++)
            {
                var points = new List<int>();

                for (var y = 0; y < dimension; y++)
                {
                    points.Add(Points[x, y]);
                }

                sb.AppendLine(string.Join(" ", points.Select(p => p.ToString().PadLeft(numDigits))));
            }

            return sb.ToString();
        }
    }

    private readonly (int dimension, IEnumerable<PointPair> values) PartValues;
    private readonly (int dimension, IEnumerable<PointPair> values) TestValues;

    public Day_05(string intputFile)
    {
        var values = new List<PointPair>();
        var isTest = true;
        var max = 0;

        void CheckMax(int x, int y)
        {
            if (x > max) max = x;
            if (y > max) max = y;
        }

        foreach (var line in File.ReadAllLines(intputFile))
        {
            if (line.StartsWith("#")) // marks end of input section (test and input)
            {
                if (isTest)
                {
                    TestValues = (max.NearestPowerOfTen(), values);
                }
                else
                {
                    PartValues = (max.NearestPowerOfTen(), values);
                }

                max = 0;

                values = new List<PointPair>();
                isTest = false;
                continue;
            }

            // parse pair of points
            var parts = line.Split("->").Select(p => p.Trim());

            var points = new List<Point>();
            foreach (var part in parts)
            {
                var coords = part.Split(",").Select(c => int.Parse(c));
                var x = coords.First(); var y = coords.Last();

                CheckMax(x, y);

                points.Add(new Point(x, y));
            }

            values.Add(new PointPair(points.First(), points.Last()));
        }
    }

    private static int MapVents(int dimension, IEnumerable<PointPair> values, bool includeDiagonal, int dangerLevel)
    {
        var dangerZone = new DangerZone(dimension);

        foreach (var value in values)
        {
            if (!includeDiagonal && value.IsDiagonal) continue;

            dangerZone.AddVent(value);
        }

        return dangerZone.DangerVents(dangerLevel);
    }

    [Test]
    public bool Test1() => ExecuteTest(string.Empty, 5, (_) => MapVents(TestValues.dimension, TestValues.values, false, 2));

    [Test]
    public bool Test2() => ExecuteTest(string.Empty, 12, (_) => MapVents(TestValues.dimension, TestValues.values, true, 2));

    [Part]
    public string Part1() => $"{MapVents(PartValues.dimension, PartValues.values, false, 2)}";

    [Part]
    public string Part2() => $"{MapVents(PartValues.dimension, PartValues.values, true, 2)}";
}
