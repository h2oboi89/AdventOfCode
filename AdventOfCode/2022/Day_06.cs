namespace AdventOfCode._2022
{
    internal class Day_06 : BaseDay
    {
        private readonly string input;

        public Day_06(string inputFile)
        {
            this.input = File.ReadAllText(inputFile);
        }

        private static int DetectStart(string input, int windowSize)
        {
            var buffer = new Queue<char>();

            var i = 0;
            while (buffer.Count < windowSize)
            {
                buffer.Enqueue(input[i++]);
            }

            for (; i < input.Length; i++)
            {
                if (buffer.Distinct().Count() == windowSize) break;

                buffer.Enqueue(input[i]);
                buffer.Dequeue();
            }

            return i;
        }

        [DayTest]
        public static TestResult Test_1_1() => ExecuteTest(7, () => DetectStart("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 4));
        [DayTest]
        public static TestResult Test_1_2() => ExecuteTest(5, () => DetectStart("bvwbjplbgvbhsrlpgdmjqwftvncz", 4));
        [DayTest]
        public static TestResult Test_1_3() => ExecuteTest(6, () => DetectStart("nppdvjthqldpwncqszvftbrmjlhg", 4));
        [DayTest]
        public static TestResult Test_1_4() => ExecuteTest(10, () => DetectStart("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 4));
        [DayTest]
        public static TestResult Test_1_5() => ExecuteTest(11, () => DetectStart("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 4));

        [DayTest]
        public static TestResult Test_2_1() => ExecuteTest(19, () => DetectStart("mjqjpqmgbljsphdztnvjfqwrcgsmlb", 14));
        [DayTest]
        public static TestResult Test_2_2() => ExecuteTest(23, () => DetectStart("bvwbjplbgvbhsrlpgdmjqwftvncz", 14));
        [DayTest]
        public static TestResult Test_2_3() => ExecuteTest(23, () => DetectStart("nppdvjthqldpwncqszvftbrmjlhg", 14));
        [DayTest]
        public static TestResult Test_2_4() => ExecuteTest(29, () => DetectStart("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 14));
        [DayTest]
        public static TestResult Test_2_5() => ExecuteTest(26, () => DetectStart("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 14));

        [DayPart]
        public string Solve_1() => $"{DetectStart(input, 4)}";

        [DayPart]
        public string Solve_2() => $"{DetectStart(input, 14)}";
    }
}
