using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.YEAR
{
    internal class Day_Template : BaseDay
    {
        private readonly List<string> input = [];

        private const string testInput = """
            
        """;

        public Day_Template(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        // TODO: code to solve
        private static int DO_THE_THING(IEnumerable<string> input)
        {
            return 314;
        }

        [DayTest]
        public static TestResult Test_1() => ExecuteTest(42, () => DO_THE_THING(testInput.Split(Environment.NewLine)));

        [DayTest]
        public static TestResult Test_2() => ExecuteTest(42, () => DO_THE_THING(testInput.Split(Environment.NewLine)));

        [DayPart]
        public string Solve_1() => $"{DO_THE_THING(input)}";

        [DayPart]
        public string Solve_2() => $"{DO_THE_THING(input)}";
    }
}
