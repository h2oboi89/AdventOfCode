using AdventOfCode.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_17 : BaseDay
{
    private readonly TargetArea partArea = new();
    private readonly TargetArea testArea = new();

    private static readonly Regex targetRegex = new(@".*x=(?<x1>-?\d+)\.\.(?<x2>-?\d+).*y=(?<y1>-?\d+)\.\.(?<y2>-?\d+)");

    public Day_17(string inputFile)
    {

        var test = true;
        foreach (var line in File.ReadAllLines(inputFile))
        {
            var captured = targetRegex.Match(line);

            var x1 = int.Parse(captured.Groups["x1"].Value);
            var x2 = int.Parse(captured.Groups["x2"].Value);
            var y1 = int.Parse(captured.Groups["y1"].Value);
            var y2 = int.Parse(captured.Groups["y2"].Value);

            var targetArea = new TargetArea(new Point(x1, y1), new Point(x2, y2));

            if (test)
            {
                testArea = targetArea;
                test = false;
            }
            else
            {
                partArea = targetArea;
            }
        }
    }

    private class TargetArea
    {
        private readonly Point Start;
        private readonly Point End;

        public TargetArea() { Start = new Point(0, 0); End = new Point(0, 0); }

        public TargetArea(Point start, Point end) { Start = start; End = end; }

        public (int x, int y) CheckLocation(Point point)
        {
            int x = 0;
            if (point.X < Start.X) x = -1;
            if (point.X >= Start.X && point.X <= End.X) x = 0;
            if (point.X > End.X) x = 1;

            int y = 0;
            if (point.Y < Start.Y) y = -1;
            if (point.Y >= Start.Y && point.Y <= End.Y) y = 0;
            if (point.Y > End.Y) y = 1;

            return (x, y);
        }
    }

    private static (bool hit, int maxHeight) Launch(int x, int y, TargetArea area)
    {
        var position = new Point(0, 0);
        var maxY = position.Y;

        var initialRelation = area.CheckLocation(position);

        static (Point p, int x, int y) Update(Point point, int x, int y)
        {
            var p = new Point(point.X + x, point.Y + y);

            if (x > 0) x--;
            if (x < 0) x++;

            y--;

            return (p, x, y);
        }

        while (true)
        {
            (position, x, y) = Update(position, x, y);

            if (position.Y > maxY) maxY = position.Y;

            var relativePosition = area.CheckLocation(position);

            if (relativePosition.Equals((0, 0)))
            {
                return (true, maxY);
            }

            if (relativePosition.x == 0 && relativePosition.y == initialRelation.y ||
                relativePosition.y == 0 && relativePosition.x == initialRelation.x)
            {
                continue;
            }

            if (relativePosition.x != initialRelation.x || relativePosition.y != initialRelation.y)
            {
                return (false, int.MinValue);
            }
        }
    }

    private static (int x, int y, int maxHeight) CalculateBestVelocity(TargetArea area)
    {
        var result = (x: 0, y: 0, maxY: 0);

        var velocities = Enumerable.Range(0, 100).CartesianProduct();

        foreach (var (x, y) in velocities)
        {
            var (hit, maxHeight) = Launch(x, y, area);

            if (hit && maxHeight > result.maxY)
            {
                result = (x, y, maxHeight);
            }
        }

        return result;
    }

    private static IEnumerable<(int x, int y)> CalculateAllVelocities(TargetArea area)
    {
        var xRange = Enumerable.Range(0, 500);
        var yRange = Enumerable.Range(-500, 1000);

        var velocities = xRange.CartesianProduct(yRange);

        foreach (var (x, y) in velocities)
        {
            var (hit, _) = Launch(x, y, area);

            if (hit) yield return (x, y);
        }
    }

    [Test]
    public TestResult Test1()
    {
        var testValues = new List<((int x, int y), bool expected)>()
        {
            ((7,2), true),
            ((6,3), true),
            ((9,0), true),
            ((17,-4), false),
        };

        return ExecuteTests(testValues, (i) => Launch(i.x, i.y, testArea).hit);
    }

    [Test]
    public TestResult Test2() => ExecuteTest((6, 9, 45), () => CalculateBestVelocity(testArea));

    [Test]
    public TestResult Test3() => ExecuteTest(112, () => CalculateAllVelocities(testArea).Count());

    [Part]
    public string Solve1() => $"{CalculateBestVelocity(partArea).maxHeight}";

    [Part]
    public string Solve2() => $"{CalculateAllVelocities(partArea).Count()}";
}
