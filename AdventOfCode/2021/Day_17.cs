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

            var targetArea = new TargetArea(new Point2D(x1, y1), new Point2D(x2, y2));

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
        private readonly Point2D Start;
        private readonly Point2D End;

        public TargetArea() : this(new Point2D(), new Point2D()) { }

        public TargetArea(Point2D start, Point2D end) { Start = start; End = end; }

        /// <summary>
        /// Determines relative position of <paramref name="point"/> to this <see cref="TargetArea"/>.
        /// Logic is similar to <see cref="IComparer{T}.Compare(T?, T?)"/>
        /// </summary>
        /// <param name="point"><see cref="Point2D"/> to check.</param>
        /// <returns><see cref="Point2D"/> comparing <paramref name="point"/> location to this <see cref="TargetArea"/></returns>
        public Point2D CheckLocation(Point2D point)
        {
            int x = 0;
            if (point.X < Start.X) x = -1;
            //if (point.X >= Start.X && point.X <= End.X) x = 0;
            if (point.X > End.X) x = 1;

            int y = 0;
            if (point.Y < Start.Y) y = -1;
            //if (point.Y >= Start.Y && point.Y <= End.Y) y = 0;
            if (point.Y > End.Y) y = 1;

            return new(x, y);
        }
    }

    private static (bool hit, int maxHeight) Launch(Point2D velocity, TargetArea area)
    {
        var position = new Point2D();
        var maxY = position.Y;

        var initialRelation = area.CheckLocation(position);

        static (Point2D p, Point2D v) Update(Point2D point, Point2D velocity)
        {
            var p = point + velocity;

            var dx = 0;
            if (velocity.X > 0) dx = -1;
            if (velocity.X < 0) dx = 1;

            return (p, new Point2D(velocity.X + dx, velocity.Y - 1));
        }

        while (true)
        {
            (position, velocity) = Update(position, velocity);

            if (position.Y > maxY) maxY = position.Y;

            var relativePosition = area.CheckLocation(position);

            // in target area
            if (relativePosition.Equals(new Point2D()))
            {
                return (true, maxY);
            }

            // in line with target area
            if (relativePosition.X == 0 && relativePosition.Y == initialRelation.Y ||
                relativePosition.Y == 0 && relativePosition.X == initialRelation.X)
            {
                continue;
            }

            // overshot target area
            if (relativePosition.X != initialRelation.X || relativePosition.Y != initialRelation.Y)
            {
                return (false, int.MinValue);
            }
        }
    }

    private static (Point2D bestVelocity, int maxHeight) CalculateBestVelocity(TargetArea area)
    {
        var result = (new Point2D(), maxY: 0);

        var velocities = Enumerable.Range(0, 100).CartesianProduct().Select(pair => new Point2D(pair.a, pair.b));

        foreach (var velocity in velocities)
        {
            var (hit, maxHeight) = Launch(velocity, area);

            if (hit && maxHeight > result.maxY)
            {
                result = (velocity, maxHeight);
            }
        }

        return result;
    }

    private static IEnumerable<Point2D> CalculateAllVelocities(TargetArea area)
    {
        var xRange = Enumerable.Range(0, 500);
        var yRange = Enumerable.Range(-500, 1000);

        var velocities = xRange.CartesianProduct(yRange).Select(pair => new Point2D(pair.a, pair.b));

        foreach (var v in velocities)
        {
            var (hit, _) = Launch(v, area);

            if (hit) yield return v;
        }
    }

    [DayTest]
    public TestResult Test1()
    {
        var testValues = new List<(Point2D velocity, bool expected)>()
        {
            (new(7,2), true),
            (new(6,3), true),
            (new(9,0), true),
            (new(17,-4), false),
        };

        return ExecuteTests(testValues, (velocity) => Launch(velocity, testArea).hit);
    }

    [DayTest]
    public TestResult Test2() => ExecuteTest((new Point2D(6, 9), 45), () => CalculateBestVelocity(testArea));

    [DayTest]
    public TestResult Test3() => ExecuteTest(112, () => CalculateAllVelocities(testArea).Count());

    [DayPart]
    public string Solve1() => $"{CalculateBestVelocity(partArea).maxHeight}";

    [DayPart]
    public string Solve2() => $"{CalculateAllVelocities(partArea).Count()}";
}
