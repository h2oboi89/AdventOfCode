namespace AdventOfCode._2015;

internal class Day_02 : BaseDay
{
    private readonly List<(int a, int b, int c)> input = new();

    public Day_02(string inputFile)
    {
        input = ParseInput(File.ReadAllLines(inputFile));
    }

    private static List<(int, int, int)> ParseInput(IEnumerable<string> input)
    {
        var dimensions = new List<(int, int, int)>();

        foreach (var line in input)
        {
            dimensions.Add(ParseInput(line));
        }

        return dimensions;
    }

    private static (int, int, int) ParseInput(string input)
    {
        var parts = input.Split('x').Select(x => int.Parse(x)).ToList();

        parts.Sort();

        return (parts[0], parts[1], parts[2]);
    }

    private static int SurfaceArea(int a, int b, int c) => 2 * ((a * b) + (b * c) + (c * a));

    private static int Extra(int a, int b) => a * b;

    private static int WrapPackage(int a, int b, int c) => SurfaceArea(a, b, c) + Extra(a, b);

    private static int Volume(int a, int b, int c) => a * b * c;

    private static int Perimeter(int a, int b) => 2 * (a + b);

    private static int TieRibbon(int a, int b, int c) => Volume(a, b, c) + Perimeter(a, b);

    [DayTest]
    public static TestResult Test_1()
    {
        var testValues = new List<(string input, object expected)>
        {
            ("2x3x4", 58),
            ("1x1x10", 43),
        };

        return ExecuteTests(testValues, (i) =>
        {
            var (a, b, c) = ParseInput(i);

            return WrapPackage(a, b, c);
        });
    }

    [DayTest]
    public static TestResult Test_2()
    {
        var testValues = new List<(string input, object expected)>
        {
            ("2x3x4", 34),
            ("1x1x10", 14),
        };

        return ExecuteTests(testValues, (i) =>
        {
            var (a, b, c) = ParseInput(i);

            return TieRibbon(a, b, c);
        });
    }

    [DayPart]
    public string Solve_1()
    {
        var total = 0;

        foreach (var (a, b, c) in input)
        {
            total += WrapPackage(a, b, c);
        }

        return new($"{total}");
    }

    [DayPart]
    public string Solve_2()
    {
        var total = 0;

        foreach (var (a, b, c) in input)
        {
            total += TieRibbon(a, b, c);
        }

        return new($"{total}");
    }
}
