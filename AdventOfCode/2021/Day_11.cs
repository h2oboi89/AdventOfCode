using AdventOfCode.Common;

namespace AdventOfCode._2021;

internal class Day_11 : BaseDay
{
    private readonly Octopus[,] input = new Octopus[0, 0];
    private readonly Octopus[,] testInput = new Octopus[0, 0];

    public Day_11(string inputFile)
    {
        var parsed = new List<int[]>();

        static Octopus[,] ProcessInput(List<int[]> input)
        {
            var processedInput = new Octopus[input.Count, input[0].Length];

            processedInput.All((x, y) => processedInput[y, x] = new Octopus(input[y][x]));

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

    private class Octopus
    {
        public int Value { get; set; }

        public bool Flashed { get; set; }

        public Octopus(int value)
        {
            Value = value;
            Flashed = false;
        }

        public void Reset()
        {
            if (Flashed)
            {
                Value = 0;
                Flashed = false;
            }
        }

        public bool Flash()
        {
            if (!Flashed && Value > 9)
            {
                Flashed = true;
                return true;
            }

            return false;
        }

        public bool Increase()
        {
            if (!Flashed)
            {
                Value++;
                return true;
            }

            return false;
        }
    }

    private static int ExecuteStep(Octopus[,] input)
    {
        var flashCount = 0;

        // energy stage 
        input.All((x, y) => input[y, x].Value++);

        // flash stage
        var processing = new Queue<Point>();

        input.All((p) => processing.Enqueue(p));

        while (processing.Any())
        {
            var p = processing.Dequeue();

            var octopus = input.GetValue(p);

            if (octopus.Flash())
            {
                flashCount++;

                foreach (var neighbor in input.GetNeighborPoints(p, true))
                {
                    var n = input.GetValue(neighbor);

                    if (n.Increase())
                    {
                        processing.Enqueue(neighbor);
                    }
                }
            }
        }

        // reset stage
        input.All((x, y) => input[y, x].Reset());

        return flashCount;
    }

    private static int Simulate(Octopus[,] input, int steps)
    {
        var clone = input.Clone((o) => new Octopus(o.Value));

        int flashCount = 0;

        for (var i = 0; i < steps; i++)
        {
            flashCount += ExecuteStep(clone);
        }

        return flashCount;
    }

    private static int SimulateSync(Octopus[,] input)
    {
        var clone = input.Clone((o) => new Octopus(o.Value));

        var count = clone.Length;

        var i = 1;
        while (true)
        {
            var flashed = ExecuteStep(clone);

            if (flashed == count)
            {
                return i;
            }

            i++;
        }
    }

    [Test]
    public TestResult Test1() => ExecuteTest(1656, () => Simulate(testInput, 100));

    [Test]
    public TestResult Test2() => ExecuteTest(195, () => SimulateSync(testInput));

    [Part]
    public string Solve1() => $"{Simulate(input, 100)}";

    [Part]
    public string Solve2() => $"{SimulateSync(input)}";
}
