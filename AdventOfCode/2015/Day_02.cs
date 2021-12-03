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

    private static int SurfaceArea(int length, int width, int height) =>
            2 * ((length * width) + (width * height) + (height * length));

    private static int Extra(int a, int b) => a * b;

    private static int Volume(int length, int width, int height) => length * width * height;

    private static int Perimeter(int a, int b) => 2 * (a + b);

    [Test]
    public static bool Test_1()
    {
        var testValues = new List<(string input, object expected)>
        {
            ("2x3x4", 58),
            ("1x1x10", 43),
        };

        return ExecuteTests(testValues, (i) =>
        {
            var (a, b, c) = ParseInput(i);

            return SurfaceArea(a, b, c) + Extra(a, b);
        });
    }

    [Test]
    public static bool Test_2()
    {
        var testValues = new List<(string input, object expected)>
        {
            ("2x3x4", 34),
            ("1x1x10", 14),
        };

        return ExecuteTests(testValues, (i) =>
        {
            var (a, b, c) = ParseInput(i);

            return Volume(a, b, c) + Perimeter(a, b);
        });
    }

    [Part]
    public string Solve_1()
    {
        var total = 0;

        foreach (var (a, b, c) in input)
        {
            var surfaceArea = SurfaceArea(a, b, c);
            var extra = a * b;

            total += surfaceArea + extra;
        }

        return new($"{total}");
    }

    [Part]
    public string Solve_2() {
        var total = 0;

        foreach (var (a, b, c) in input)
        {
            total += Volume(a, b, c) + Perimeter(a, b);
        }

        return new($"{total}");
    }
}
