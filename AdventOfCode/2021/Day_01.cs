namespace AdventOfCode._2021;

internal class Day_01 : BaseDay
{
    private readonly List<int> input = new();

    private readonly List<int> testInput = new()
    {
        199,
        200,
        208,
        210,
        200,
        207,
        240,
        269,
        260,
        263,
    };

    public Day_01(string inputFile)
    {
        foreach (var line in File.ReadAllLines(inputFile))
        {
            input.Add(int.Parse(line));
        }
    }

    private static int Solve(List<int> input, int windowSize)
    {
        var increases = 0;

        var previousSum = int.MaxValue;
        var values = new Queue<int>();

        foreach (var v in input)
        {
            values.Enqueue(v);

            if (values.Count == windowSize)
            {
                var currentSum = values.Sum();

                if (currentSum > previousSum)
                {
                    increases++;
                }

                previousSum = currentSum;

                values.Dequeue();
            }
        }

        return increases;
    }

    [Test]
    public TestResult Test_1() => ExecuteTest(7, () => Solve(testInput, 1));

    [Test]
    public TestResult Test_2() => ExecuteTest(5, () => Solve(testInput, 3));

    [Part]
    public string Solve_1() => $"{Solve(input, 1)}";

    [Part]
    public string Solve_2() => $"{Solve(input, 3)}";
}
