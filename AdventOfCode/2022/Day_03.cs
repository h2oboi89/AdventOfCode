using AdventOfCode.Extensions;

namespace AdventOfCode._2022
{
    internal class Day_03 : BaseDay
    {
        private readonly List<string> input = [];

        private readonly string testInput = """
            vJrwpWtwJgWrhcsFMMfFFhFp
            jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
            PmmdzqPrVvPwwTWBwg
            wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
            ttgJtRGJQctTZtZT
            CrZsJsPPZsGzwwsLwLmpwMDw
            """;

        public Day_03(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        private class Bag
        {
            public char[] A { get; init; } = Array.Empty<char>();
            public char[] B { get; init; } = Array.Empty<char>();

            public IEnumerable<char> All => A.Concat(B);

            public char Common => A.Intersect(B).First();

            public static Bag Parse(string bag)
            {
                var length = bag.Length;

                var a = bag.Take(length / 2).ToArray();
                var b = bag.Skip(a.Length).ToArray();

                return new Bag() { A = a, B = b };
            }

            public static char Badge(IEnumerable<Bag> bags)
            {
                var b = bags.ToArray();

                return b[0].All.Intersect(b[1].All).Intersect(b[2].All).First();
            }

            public override string ToString() => new(All.ToArray());
        }

        private static int Score(IEnumerable<char> items)
        {
            var score = 0;

            foreach (var item in items)
            {
                score += item switch
                {
                    var i when i >= 'a' && i <= 'z' => item - 'a' + 1,
                    var i when i >= 'A' && i <= 'Z' => item - 'A' + 26 + 1,
                    _ => 0
                };
            }

            return score;
        }

        [DayTest]
        public TestResult Test_1() => ExecuteTest(157, () => Score(testInput.Split(Environment.NewLine).Select(Bag.Parse).Select(b => b.Common)));

        [DayTest]
        public TestResult Test_2() => ExecuteTest(70, () => Score(testInput.Split(Environment.NewLine).Select(Bag.Parse).Batch(3).Select(Bag.Badge)));

        [DayPart]
        public string Solve_1() => $"{Score(input.Select(Bag.Parse).Select(b => b.Common))}";

        [DayPart]
        public string Solve_2() => $"{Score(input.Select(Bag.Parse).Batch(3).Select(Bag.Badge))}";
    }
}
