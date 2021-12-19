using AdventOfCode.Common;
using System.Text;

namespace AdventOfCode._2021;

internal class Day_05 : BaseDay
{
    private class PointPair
    {
        public readonly Point2D Start, End;

        public PointPair(Point2D start, Point2D end) { Start = start; End = end; }

        public bool IsHorizonal => Start.Y == End.Y;

        public bool IsVertical => Start.X == End.X;

        public bool IsDiagonal => !IsHorizonal && !IsVertical;

        public override string ToString() => $"[ {Start}, {End} ] {IsDiagonal}";

        private int NumPoints
        {
            get
            {
                var dx = Math.Abs(Start.X - End.X) + 1;
                var dy = Math.Abs(Start.Y - End.Y) + 1;

                return Math.Max(dx, dy);
            }
        }

        public IEnumerable<Point2D> GetLine()
        {
            var dx = 1; var dy = 1;

            if (End.X < Start.X) dx= -1;
            if (End.Y < Start.Y) dy = -1;

            if (IsHorizonal) dy = 0;
            if (IsVertical) dx = 0;

            var p = new Point2D(Start.X, Start.Y);
            for(var i = 0; i < NumPoints; i++)
            {
                yield return p;

                p = new Point2D(p.X + dx, p.Y + dy);
            }
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

    private readonly (int dimension, IEnumerable<PointPair> values) PartValues, TestValues;

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

            var points = new List<Point2D>();
            foreach (var part in parts)
            {
                var coords = part.Split(",").Select(c => int.Parse(c));
                var x = coords.First(); var y = coords.Last();

                CheckMax(x, y);

                points.Add(new Point2D(x, y));
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

    [DayTest]
    public TestResult Test1() => ExecuteTest(5, () => MapVents(TestValues.dimension, TestValues.values, false, 2));

    [DayTest]
    public TestResult Test2() => ExecuteTest(12, () => MapVents(TestValues.dimension, TestValues.values, true, 2));

    [DayPart]
    public string Part1() => $"{MapVents(PartValues.dimension, PartValues.values, false, 2)}";

    [DayPart]
    public string Part2() => $"{MapVents(PartValues.dimension, PartValues.values, true, 2)}";
}
