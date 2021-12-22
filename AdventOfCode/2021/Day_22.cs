using AdventOfCode.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_22 : BaseDay
{
    private readonly Regex lineRegex = new(@"(?<action>on|off) x=(?<x_min>-?\d+)\.\.(?<x_max>-?\d+),y=(?<y_min>-?\d+)\.\.(?<y_max>-?\d+),z=(?<z_min>-?\d+)\.\.(?<z_max>-?\d+)");

    private readonly List<Instruction> testInstructions = new();
    private readonly List<Instruction> partInstructions = new();

    public Day_22(string inputFile)
    {
        var instructions = new List<Instruction>();

        static Point.D3 GetPoint(Match match, string name)
        {
            static int GetCoordinate(Match match, char coordinate, string name) => int.Parse(match.Groups[$"{coordinate}_{name}"].Value);

            return new Point.D3(GetCoordinate(match, 'x', name), GetCoordinate(match, 'y', name), GetCoordinate(match, 'z', name));
        }

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("###"))
            {
                testInstructions.AddRange(instructions);
                instructions.Clear();
                continue;
            }

            if (line.StartsWith("!!!"))
            {
                partInstructions.AddRange(instructions);
                instructions.Clear();
                continue;
            }

            var match = lineRegex.Match(line);

            instructions.Add(new(GetPoint(match, "min"), GetPoint(match, "max"), match.Groups["action"].Value == "on"));
        }
    }

    private class Instruction
    {
        public readonly Point.D3 Start;
        public readonly Point.D3 End;
        public readonly bool On;

        public Instruction(Point.D3 start, Point.D3 end, bool on)
        {
            Start = start; End = end; On = on;
        }

        public Instruction Limit(int min, int max)
        {
            var startX = Math.Max(min, Start.X);
            var startY = Math.Max(min, Start.Y);
            var StartZ = Math.Max(min, Start.Z);

            var endX = Math.Min(max, End.X);
            var endY = Math.Min(max, End.Y);
            var endZ = Math.Min(max, End.Z);

            return new Instruction(new Point.D3(startX, startY, StartZ), new Point.D3(endX, endY, endZ), On);
        }

        public override string ToString() => $"{Start} {End} {On}";
    }

    private static IEnumerable<Point.D3> Enumerate(Point.D3 start, Point.D3 end)
    {
        for (var x = start.X; x <= end.X; x++)
        {
            for (var y = start.Y; y <= end.Y; y++)
            {
                for (var z = start.Z; z <= end.Z; z++)
                {
                    yield return new Point.D3(x, y, z);
                }
            }
        }
    }

    private static bool InRange(Point.D3 point, Point.D3 start, Point.D3 end) => point >= start && point <= end;

    private static HashSet<Point.D3> Execute(HashSet<Point.D3> reactor, Instruction instruction)
    {
        if (instruction.On)
        {
            foreach (var point in Enumerate(instruction.Start, instruction.End))
            {
                reactor.Add(point);
            }
        }
        else
        {
            foreach (var point in reactor)
            {
                if (InRange(point, instruction.Start, instruction.End))
                {
                    reactor.Remove(point);
                }
            }
        }

        return reactor;
    }
    private static HashSet<Point.D3> Execute(IEnumerable<Instruction> instructions, int min, int max)
    {
        // FIXME: test data takes 10 minutes
        // Next idea: keep track of "on" ranges
        // Break into smaller cuboids based on edges of the other range and how they intersect this range
        // - if both on: get unique range and discard duplicates
        // - if one off: eliminate overlap range and keep rest
        var reactor = new HashSet<Point.D3>();

        foreach(var instruction in instructions)
        {
            reactor = Execute(reactor, instruction.Limit(min, max));
        }

        return reactor;
    }

    [DayTest]
    public TestResult ParseTest() => ExecuteTest(22, () => testInstructions.Count);

    [DayTest]
    public TestResult Test1() => ExecuteTest(590_784, () => Execute(testInstructions, -50, 50).Count);

    [DayPart]
    public string Solve1() => $"{Execute(partInstructions, -50, -50).Count}";
}
