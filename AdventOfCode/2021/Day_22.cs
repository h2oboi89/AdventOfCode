using AdventOfCode.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_22 : BaseDay
{
    private readonly Regex lineRegex = new(@"(?<action>on|off) x=(?<x_min>-?\d+)\.\.(?<x_max>-?\d+),y=(?<y_min>-?\d+)\.\.(?<y_max>-?\d+),z=(?<z_min>-?\d+)\.\.(?<z_max>-?\d+)");

    private readonly List<(bool on, Point.D3 start, Point.D3 end)> testInstructions = new();
    private readonly List<(bool on, Point.D3 start, Point.D3 end)> partInstructions = new();

    public Day_22(string inputFile)
    {
        var instructions = new List<(bool, Point.D3, Point.D3)>();

        static Point.D3 GetPoint(Match match, string name)
        {
            static int GetCoordinate(Match match, char coordinate, string name) => int.Parse(match.Groups[$"{coordinate}_{name}"].Value);

            return new Point.D3(GetCoordinate(match, 'x', name), GetCoordinate(match, 'y', name), GetCoordinate(match, 'z', name));
        }

        foreach(var line in File.ReadAllLines(inputFile))
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

            instructions.Add((match.Groups["action"].Value == "on", GetPoint(match, "min"), GetPoint(match, "max")));
        }
    }

    private static IEnumerable<Point.D3> Enumerate(Point.D3 start, Point.D3 end)
    {
        throw new NotImplementedException();
    }

    private static bool InRange(Point.D3 point, Point.D3 start, Point.D3 end)
    {
        throw new NotImplementedException();
    }

    [DayTest]
    public TestResult ParseTest() => ExecuteTest(22, () => testInstructions.Count);
}
