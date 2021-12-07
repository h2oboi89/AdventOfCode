using AdventOfCode.Common;
using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode._2015;

internal class Day_06 : BaseDay
{
    private readonly IEnumerable<Range> instructions;

    private static readonly Regex lineRegex = new(@".*(?<action>on|off|toggle)\s(?<startX>\d+),(?<startY>\d+)\sthrough\s(?<endX>\d+),(?<endY>\d+)");

    public Day_06(string inputFile)
    {
        var values = new List<Range>();

        foreach (var line in File.ReadAllLines(inputFile))
        {
            var captured = lineRegex.Match(line);

            var action = captured.Groups["action"].Value switch
            {
                "on" => Action.On,
                "off" => Action.Off,
                "toggle" => Action.Toggle,
                _ => Action.Unknown
            };

            var startX = int.Parse(captured.Groups["startX"].Value);
            var startY = int.Parse(captured.Groups["startY"].Value);
            var endX = int.Parse(captured.Groups["endX"].Value);
            var endY = int.Parse(captured.Groups["endY"].Value);

            values.Add(new Range(new Point(startX, startY), new Point(endX, endY), action));
        }

        instructions = values;
    }

    private enum Action { Unknown, On, Off, Toggle }

    private class Range : IEnumerable<Point>
    {
        public readonly Point Start, End;
        public readonly Action Action;

        public Range(Point start, Point end, Action action)
        {
            Start = start; End = end; Action = action;
        }

        public IEnumerator<Point> GetEnumerator()
        {
            for (var x = Start.X; x <= End.X; x++)
            {
                for (var y = Start.Y; y <= End.Y; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private abstract class Grid
    {
        public abstract void On(Point point);
        public abstract void Off(Point point);
        public abstract void Toggle(Point point);
        public abstract int Lit { get; }
    }

    private class BoolGrid : Grid
    {
        private readonly bool[,] grid = new bool[1000, 1000];

        private void Set(Point point, bool state) => grid[point.Y, point.X] = state;

        public override void On(Point point) => Set(point, true);

        public override void Off(Point point) => Set(point, false);

        public override void Toggle(Point point) => Set(point, !grid[point.Y, point.X]);

        public override int Lit
        {
            get
            {
                var on = 0;

                foreach (var p in grid)
                {
                    if (p) on++;
                }

                return on;
            }
        }
    }

    private class IntGrid : Grid
    {
        private readonly int[,] grid = new int[1000, 1000];

        public override void On(Point point)
        {
            if (grid[point.Y, point.X] == int.MaxValue) return;

            grid[point.Y, point.X]++;
        }

        public override void Off(Point point)
        {
            if (grid[point.Y, point.X] == 0) return;

            grid[point.Y, point.X]--;
        }

        public override void Toggle(Point point) { On(point); On(point); }

        public override int Lit
        {
            get
            {
                int on = 0;

                foreach (var p in grid)
                {
                    on += p;
                }

                return on;
            }
        }
    }

    private static int FollowInstructions(Grid grid, IEnumerable<Range> instructions)
    {
        foreach (var instruction in instructions)
        {
            Action<Point> method = instruction.Action switch
            {
                Action.On => grid.On,
                Action.Off => grid.Off,
                Action.Toggle => grid.Toggle,
                _ => throw new ArgumentException($"Unknown action: {instruction.Action}"),
            };

            foreach (var point in instruction)
            {
                method(point);
            }
        }

        return grid.Lit;
    }

    [Test]
    public static TestResult Test1()
    {
        var instructions = new List<Range>
        {
            new Range(new Point(0, 0), new Point(999, 999), Action.On),
            new Range(new Point(0, 0), new Point(999, 0), Action.Toggle),
            new Range(new Point(499, 499), new Point(500, 500), Action.Off),
        };

        return ExecuteTest((1_000 * 1_000) - 1_000 - 4, () => FollowInstructions(new BoolGrid(), instructions));
    }

    [Test]
    public static TestResult Test2()
    {
        var instructions = new List<Range>
        {
            new Range(new Point(0, 0), new Point(0, 0), Action.On),
            new Range(new Point(0, 0), new Point(999, 999), Action.Toggle),
        };

        return ExecuteTest(1 + 2_000_000, () => FollowInstructions(new IntGrid(), instructions));
    }

    [Part]
    public string Solve1() => $"{FollowInstructions(new BoolGrid(), instructions)}";

    [Part]
    public string Solve2() => $"{FollowInstructions(new IntGrid(), instructions)}";
}
