﻿namespace AdventOfCode._2021;

internal class Day_06 : BaseDay
{
    private readonly IEnumerable<int> PartValues, TestValues;

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
                    TestValues = values;
                }
                else
                {
                    PartValues = values;
                }

                values = new List<int>();
                isTest = false;
                continue;
            }

            // parse pair of points
            values.AddRange(line.Split(",").Select(v => int.Parse(v)));
        }
    }

    private class Cohort : IComparable<Cohort>
    {
        public int Timer;
        public ulong Count = 0;

        public Cohort(int timer)
        {
            Timer = timer;
        }

        public int CompareTo(Cohort? other)
        {
            if (other == null) return 1;

            return Timer.CompareTo(other.Timer);
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

            // reset fish that spawned
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

    [Test]
    public bool Test1_1() => ExecuteTest(string.Empty, (ulong)26, (_) => Simulate(TestValues, 18));

    [Test]
    public bool Test1_2() => ExecuteTest(string.Empty, (ulong)5934, (_) => Simulate(TestValues, 80));

    [Part]
    public string Part1() => $"{Simulate(PartValues, 80)}";

    [Part]
    public string Part2() => $"{Simulate(PartValues, 256)}";
}