using AdventOfCode.Extensions;
using System.Text.RegularExpressions;

namespace AdventOfCode._2022
{
    internal partial class Day_05 : BaseDay
    {
        private readonly List<string> input = [];

        private const string testInput = """
            [D]    
        [N] [C]    
        [Z] [M] [P]
         1   2   3 

        move 1 from 2 to 1
        move 3 from 1 to 3
        move 2 from 2 to 1
        move 1 from 1 to 2    
        """;

        public Day_05(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        private partial class Instruction
        {
            public readonly int Count;
            public readonly int Source;
            public readonly int Destination;

            [GeneratedRegex("move (?<count>\\d+) from (?<source>\\d+) to (?<destination>\\d+)")]
            private static partial Regex InstructionRegex();

            private Instruction(int count, int source, int destination)
            {
                Count = count; Source = source; Destination = destination;
            }

            public static Instruction Parse(string instruction)
            {
                var matched = InstructionRegex().Match(instruction);

                if (matched.Success)
                {
                    return new Instruction(
                        int.Parse(matched.Groups["count"].Value),
                        int.Parse(matched.Groups["source"].Value),
                        int.Parse(matched.Groups["destination"].Value)
                    );
                }

                return new Instruction(0, 0, 0);
            }

            public override string ToString() => $"move {Count} from {Source} to {Destination}";
        }

        private static (List<Stack<char>> stacks, Queue<Instruction> instructions) Parse(IEnumerable<string> input) {
            var stacks = new List<Stack<char>>();
            var instructions = new Queue<Instruction>();

            var stacksDone = false;

            void ParseStackInfo(string stackLine)
            {
                if (string.IsNullOrEmpty(stackLine.Trim())) // empty line between stacks and instructions
                {
                    stacksDone = true;
                    return;
                }

                var parts = stackLine.SplitInParts(4).Select(p => p.Trim()).ToArray();

                while (stacks.Count < parts.Length)
                {
                    stacks.Add(new Stack<char>());
                }

                if (int.TryParse(parts[0], out _)) return; // we found the stack IDs

                var i = 0;
                foreach (var part in parts)
                {
                    if (!string.IsNullOrEmpty(part))
                    {
                        stacks[i].Push(part[1]);
                    }

                    i++;
                }
            }

            void ReverseStacks(List<Stack<char>> stacks)
            {
                for (var i = 0; i < stacks.Count; i++)
                {
                    stacks[i] = new Stack<char>(stacks[i]);
                }
            }

            foreach (var line in input)
            {
                if (!stacksDone)
                {
                    ParseStackInfo(line);
                }
                else
                {
                    instructions.Enqueue(Instruction.Parse(line));
                }
            }

            ReverseStacks(stacks);

            return (stacks, instructions);
        }

        private static string Execute1((List<Stack<char>> stacks, Queue<Instruction> instructions) input)
        {
            var (stacks, instructions) = input;

            while(instructions.Any())
            {
                var instruction = instructions.Dequeue();
                var source = instruction.Source - 1;
                var destination = instruction.Destination - 1;

                for(var i = 0; i < instruction.Count; i++)
                {
                    var crate = stacks[source].Pop();

                    stacks[destination].Push(crate);
                }
            }

            return new string(stacks.Select(s => s.Pop()).ToArray());
        }

        private static string Execute2((List<Stack<char>> stacks, Queue<Instruction> instructions) input)
        {
            var (stacks, instructions) = input;

            while (instructions.Any())
            {
                var instruction = instructions.Dequeue();
                var source = instruction.Source - 1;
                var destination = instruction.Destination - 1;

                var temp = new Stack<char>();

                for (var i = 0; i < instruction.Count; i++)
                {
                    var crate = stacks[source].Pop();

                    temp.Push(crate);
                }

                while(temp.Any())
                {
                    stacks[destination].Push(temp.Pop());
                }
            }

            return new string(stacks.Select(s => s.Pop()).ToArray());
        }

        [DayTest]
        public static TestResult Test_1() => ExecuteTest("CMZ", () => Execute1(Parse(testInput.Split(Environment.NewLine))));

        [DayTest]
        public static TestResult Test_2() => ExecuteTest("MCD", () => Execute2(Parse(testInput.Split(Environment.NewLine))));

        [DayPart]
        public string Solve_1() => $"{Execute1(Parse(input))}";

        [DayPart]
        public string Solve_2() => $"{Execute2(Parse(input))}";
    }
}
