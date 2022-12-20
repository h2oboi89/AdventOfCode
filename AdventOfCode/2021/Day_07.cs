using AdventOfCode.Extensions;

namespace AdventOfCode._2021;

internal class Day_07 : BaseDay
{
    private readonly IEnumerable<int> Positions;

    private static readonly IEnumerable<int> TestPositions = new List<int> { 16, 1, 2, 0, 4, 2, 7, 1, 2, 14 };

    public Day_07(string inputFile)
    {
        Positions = File.ReadAllText(inputFile).ParseCommaSeparatedInt32s();
    }

    private static int SimpleCost(int a, int b) => Math.Abs(a - b);

    private static int ComplexCost(int a, int b)
    {
        var distance = Math.Abs(a - b);

        var cost = 0;

        for (var i = 0; i < distance; i++)
        {
            cost += i + 1;
        }

        return cost;
    }

    private static int FindMin(IEnumerable<int> positions, Func<int, int, int> costFunc)
    {
        var minPos = positions.Min();
        var maxPos = positions.Max();

        (int pos, int cost) min = (0, int.MaxValue);

        for (var x = minPos; x <= maxPos; x++)
        {
            var cost = 0;

            foreach (var p in positions)
            {
                cost += costFunc(p, x);
            }

            if (cost < min.cost)
            {
                min = (x, cost);
            }
        }

        return min.cost;
    }

    [DayTest]
    public static TestResult Test1() => ExecuteTest(37, () => FindMin(TestPositions, SimpleCost));

    [DayTest]
    public static TestResult Test2() => ExecuteTest(168, () => FindMin(TestPositions, ComplexCost));

    [DayPart]
    public string Solve1() => $"{FindMin(Positions, SimpleCost)}";

    [DayPart]
    public string Solve2() => $"{FindMin(Positions, ComplexCost)}";
}
