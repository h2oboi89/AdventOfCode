using AdventOfCode.Extensions;

namespace AdventOfCode._2021;

internal class Day_06 : BaseDay
{
    private readonly IEnumerable<int> input = new List<int>();
    private readonly IEnumerable<int> testInput = new List<int>();

    public Day_06(string intputFile)
    {
        var values = new List<int>();
        var isTest = true;

        foreach (var line in File.ReadAllLines(intputFile))
        {
            if (line.StartsWith("#")) // marks end of input section (test and input)
            {
                if (isTest)
                {
                    testInput = values;
                }
                else
                {
                    input = values;
                }

                values = new List<int>();
                isTest = false;
                continue;
            }

            // parse initial fish values
            values.AddRange(line.ParseCommaSeparatedInt32s());
        }
    }

    private class Cohort
    {
        public int Timer;
        public ulong Count = 0;

        public Cohort(int timer)
        {
            Timer = timer;
        }
    }

    private static List<Cohort> GenerateCohorts(IEnumerable<int> fish)
    {
        var cohorts = new List<Cohort>();

        for (var i = 0; i < 9; i++)
        {
            cohorts.Add(new(i));
        }

        foreach (var f in fish)
        {
            foreach (var cohort in cohorts)
            {
                if (cohort.Timer == f)
                {
                    cohort.Count++;
                }
            }
        }

        return cohorts;
    }

    private static ulong Simulate(IEnumerable<int> fish, int days)
    {
        var cohorts = GenerateCohorts(fish);

        for (var i = 0; i < days; i++)
        {
            ulong resetFish = 0;

            // update cohorts
            foreach (var cohort in cohorts)
            {
                cohort.Timer--;

                // spawn new fish
                if (cohort.Timer == -1)
                {
                    cohort.Timer = 8;
                    resetFish = cohort.Count;
                }
            }

            // reset fish that spawned new fish
            foreach (var cohort in cohorts)
            {
                if (cohort.Timer == 6)
                {
                    cohort.Count += resetFish;
                }
            }
        }

        ulong count = 0;

        foreach(var cohort in cohorts) {
            count += cohort.Count;
        }

        return count;
    }

    [DayTest]
    public TestResult Test1_1() => ExecuteTest((ulong)26, () => Simulate(testInput, 18));

    [DayTest]
    public TestResult Test1_2() => ExecuteTest((ulong)5934, () => Simulate(testInput, 80));

    [DayPart]
    public string Part1() => $"{Simulate(input, 80)}";

    [DayPart]
    public string Part2() => $"{Simulate(input, 256)}";
}
