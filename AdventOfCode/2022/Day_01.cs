namespace AdventOfCode._2022
{
    internal class Day_01 : BaseDay
    {
        private readonly List<string> input = [];

        private const string testInput = """
            1000
            2000
            3000

            4000

            5000
            6000

            7000
            8000
            9000

            10000

        """;

        private class Elf
        {
            public readonly List<int> Calories = [];
            public int Total => Calories.Sum();

            public override string ToString() => $"{Total}";
        }

        public Day_01(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        private static IEnumerable<Elf> Parse(IEnumerable<string> input)
        {
            var elves = new List<Elf>();
            var elf = new Elf();

            foreach (var line in input)
            {
                if (line.Trim() == string.Empty)
                {
                    if (elf != null) elves.Add(elf);
                    elf = new();
                    continue;
                }

                elf.Calories.Add(int.Parse(line));
            }

            return elves;
        }

        private static int MaxCalories(IEnumerable<Elf> elves) => elves.Max(e => e.Total);

        private static int TopThree(IEnumerable<Elf> elves)
        {
            var e = elves.ToList();
            e.Sort((a, b) => a.Total.CompareTo(b.Total));
            e.Reverse();
            return e.Take(3).Sum(e => e.Total);
        }

        [DayTest]
        public static TestResult Test_1() => ExecuteTest(24_000, () => MaxCalories(Parse(testInput.Split(Environment.NewLine))));

        [DayTest]
        public static TestResult Test_2() => ExecuteTest(45_000, () => TopThree(Parse(testInput.Split(Environment.NewLine))));

        [DayPart]
        public string Solve_1() => $"{MaxCalories(Parse(input))}";

        [DayPart]
        public string Solve_2() => $"{TopThree(Parse(input))}";
    }
}
