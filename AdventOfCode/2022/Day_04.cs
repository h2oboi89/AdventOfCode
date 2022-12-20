using AdventOfCode.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode._2022
{
    internal partial class Day_04 : BaseDay
    {
        private readonly List<string> input = new();

        private readonly string testInput = """
            2-4,6-8
            2-3,4-5
            5-7,7-9
            2-8,3-7
            6-6,4-6
            2-6,4-8
            """;

        public Day_04(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        private partial class RangePair
        {
            public Range A;
            public Range B;

            [GeneratedRegex("(?<a_start>\\d+)-(?<a_end>\\d+),(?<b_start>\\d+)-(?<b_end>\\d+)")]
            private static partial Regex ParseRegex();

            public static RangePair Parse(string input)
            {
                var matched = ParseRegex().Match(input);

                if (matched.Success)
                {
                    var a = new Range(
                        int.Parse(matched.Groups["a_start"].Value),
                        int.Parse(matched.Groups["a_end"].Value));

                    var b = new Range(
                        int.Parse(matched.Groups["b_start"].Value),
                        int.Parse(matched.Groups["b_end"].Value));

                    return new RangePair { A = a, B = b };
                }

                return new RangePair { A = new Range(), B = new Range() };
            }
        }

        private static int CountContainedRanges(IEnumerable<RangePair> rangePairs)
        {
            var count = 0;

            foreach (var rangePair in rangePairs)
            {
                if (rangePair.A.ContainedIn(rangePair.B) ||
                    rangePair.B.ContainedIn(rangePair.A))
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountOverlapedRanges(IEnumerable<RangePair> rangePairs)
        {
            var count = 0;

            foreach(var rangePair in rangePairs)
            {
                if (rangePair.A.Overlap(rangePair.B) ||
                    rangePair.B.Overlap(rangePair.A))
                {
                    count++;
                }
            }

            return count;
        }

        [DayTest]
        public TestResult Test_1() => ExecuteTest(2, () => CountContainedRanges(testInput.Split(Environment.NewLine).Select(RangePair.Parse)));

        [DayTest]
        public TestResult Test_2() => ExecuteTest(4, () => CountOverlapedRanges(testInput.Split(Environment.NewLine).Select(RangePair.Parse)));

        [DayPart]
        public string Solve_1() => $"{CountContainedRanges(input.Select(RangePair.Parse))}";

        [DayPart]
        public string Solve_2() => $"{CountOverlapedRanges(input.Select(RangePair.Parse))}";
    }
}
