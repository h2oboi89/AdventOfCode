﻿using AdventOfCode.Common;
using AdventOfCode.Extensions;

namespace AdventOfCode._2021;

internal class Day_09 : BaseDay
{
    private readonly int[,] input = new int[0,0];
    private readonly int[,] testInput = new int[0, 0];

    public Day_09(string inputFile)
    {
        var parsed = new List<int[]>();

        static int[,] ProcessInput(List<int[]> input)
        {
            var processedInput = new int[input.Count, input[0].Length];

            processedInput.All((x, y) => processedInput[y, x] = input[y][x]);

            return processedInput;
        }

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
                lineValues.Add(c - '0');
            }

            parsed.Add(lineValues.ToArray());
        }
    }

    private static IEnumerable<Point.D2> FindMinimums(int[,] input)
    {
        for (var y = 0; y < input.GetLength(0); y++)
        {
            for (var x = 0; x < input.GetLength(1); x++)
            {
                var current = input[y, x];

                var isMin = true;

                foreach (var neighbor in input.GetNeighborPoints(new Point.D2(x, y), false).Select(input.GetValue))
                {
                    if (current >= neighbor)
                    {
                        isMin = false;
                        break;
                    }
                }

                if (isMin)
                {
                    yield return new Point.D2(x, y);
                }
            }
        }
    }

    private static int FindRisk(int[,] input) => FindMinimums(input).Sum(m => input.GetValue(m) + 1);

    private static IEnumerable<List<Point.D2>> FindBasins(int[,] input)
    {
        foreach (var min in FindMinimums(input))
        {
            var basin = new List<Point.D2>();
            var unprocessed = new Queue<Point.D2>();

            unprocessed.Enqueue(min);

            while (unprocessed.Any())
            {
                var current = unprocessed.Dequeue();

                if (input.GetValue(current) == 9)
                {
                    continue;
                }

                if (!basin.Contains(current))
                {
                    basin.Add(current);
                }
                else
                {
                    continue;
                }

                foreach (var neighbor in input.GetNeighborPoints(current, false))
                {
                    unprocessed.Enqueue(neighbor);
                }
            }

            yield return basin;
        }
    }

    private static int FindLargestBasins(int[,] input) => FindBasins(input).OrderByDescending(b => b.Count).Take(3).Select(b => b.Count).Product();

    [DayTest]
    public TestResult Test1() => ExecuteTest(15, () => FindRisk(testInput));

    [DayTest]
    public TestResult Test2() => ExecuteTest(1134, () => FindLargestBasins(testInput));

    [DayPart]
    public string Solve1() => $"{FindRisk(input)}";

    [DayPart]
    public string Solve2() => $"{FindLargestBasins(input)}";
}
