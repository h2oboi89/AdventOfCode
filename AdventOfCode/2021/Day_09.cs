using AdventOfCode.Common;

namespace AdventOfCode._2021;

internal class Day_09 : BaseDay
{
    private readonly int[,] input;
    private readonly int[,] testInput;

    public Day_09(string inputFile)
    {
        var parsed = new List<int[]>();

        var test = true;
        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                if (test)
                {
                    testInput = ProcessInput(parsed);
                    parsed.Clear();
                    test = false;
                }
                else
                {
                    input = ProcessInput(parsed);
                }
                continue;
            }

            var lineValues = new List<int>();
            foreach (var c in line)
            {
                lineValues.Add(c - 48);
            }

            parsed.Add(lineValues.ToArray());
        }
    }

    private static int[,] ProcessInput(List<int[]> input)
    {
        var processedInput = new int[input.Count, input[0].Length];

        for (var y = 0; y < processedInput.GetLength(0); y++)
        {
            for (var x = 0; x < processedInput.GetLength(1); x++)
            {
                processedInput[y, x] = input[y][x];
            }
        }

        return processedInput;
    }

    private static IEnumerable<Point> GetNeighbors(int x, int y, int[,] input)
    {
        // top
        if (y != 0) yield return new Point(x, y - 1);

        // right
        if (x != input.GetLength(1) - 1) yield return new Point(x + 1, y);

        // bottom
        if (y != input.GetLength(0) - 1) yield return new Point(x, y + 1);

        // left
        if (x != 0) yield return new Point(x - 1, y);
    }

    private static int GetValue(Point p, int[,] input) => input[p.Y, p.X];

    private static IEnumerable<Point> FindMinimums(int[,] input)
    {
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                var current = input[y, x];

                var isMin = true;

                foreach (var neighbor in GetNeighbors(x, y, input).Select(n => GetValue(n, input)))
                {
                    if (current >= neighbor)
                    {
                        isMin = false;
                        continue;
                    }
                }

                if (isMin)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }

    private static int FindRisk(int[,] input)
    {
        var risk = 0;

        foreach (var min in FindMinimums(input))
        {
            risk += GetValue(min, input) + 1;
        }

        return risk;
    }

    private static IEnumerable<List<Point>> FindBasins(int[,] input)
    {
        foreach (var min in FindMinimums(input))
        {
            var basin = new List<Point>();
            var unprocessed = new Queue<Point>();

            unprocessed.Enqueue(min);

            while (unprocessed.Any())
            {
                var current = unprocessed.Dequeue();

                if (GetValue(current, input) == 9)
                {
                    continue;
                }

                if (!basin.Contains(current)) {
                    basin.Add(current);
                }
                else
                {
                    continue;
                }

                foreach(var neighbor in GetNeighbors(current.X, current.Y, input))
                {
                    unprocessed.Enqueue(neighbor);
                }
            }

            yield return basin;
        }
    }

    private static int FindLargestBasins(int[,] input)
    {
        var basins = FindBasins(input).OrderByDescending(b => b.Count).Take(3);

        var result = 1;

        foreach(var basin in basins)
        {
            result *= basin.Count;
        }

        return result;
    }

    [Test]
    public TestResult Test1() => ExecuteTest(15, () => FindRisk(testInput));

    [Test]
    public TestResult Test2() => ExecuteTest(1134, () => FindLargestBasins(testInput));

    [Part]
    public string Solve1() => $"{FindRisk(input)}";

    [Part]
    public string Solve2() => $"{FindLargestBasins(input)}";
}
