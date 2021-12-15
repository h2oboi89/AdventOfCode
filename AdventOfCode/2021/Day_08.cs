namespace AdventOfCode._2021;

internal class Day_08 : BaseDay
{
    private readonly IEnumerable<(IEnumerable<string> patterns, IEnumerable<string> digits)> testInput;
    private readonly IEnumerable<(IEnumerable<string> patterns, IEnumerable<string> digits)> input;

    public Day_08(string inputFile)
    {
        var test = true;
        var parsed = new List<(IEnumerable<string> patterns, IEnumerable<string> digits)>();

        static (IEnumerable<string>, IEnumerable<string>) ParseInput(string input)
        {
            static IEnumerable<string> ParsePart(string part) => part.Split(' ');

            var parts = input.Split(" | ");

            return (ParsePart(parts[0]), ParsePart(parts[1]));
        }

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                if (test)
                {
                    testInput = parsed;
                    parsed = new List<(IEnumerable<string> patterns, IEnumerable<string> digits)>();
                    test = false;
                }
                else
                {
                    input = parsed;
                }

                continue;
            }

            parsed.Add(ParseInput(line));
        }
    }

    private enum Segment { A, B, C, D, E, F, G }

    private class SegmentMap
    {
        private List<char> guesses = new() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' };

        public readonly Segment Segment;

        public SegmentMap(Segment segment)
        {
            Segment = segment;
        }

        public string Possibilities => string.Join(' ', guesses);

        public bool Solved => guesses.Count == 1;

        public void Set(char c) => guesses = new List<char> { c };

        public void Remove(char c) => guesses.Remove(c);
    }

    //private readonly Dictionary<int, List<char>> digitMap = new()
    //{
    //    { 0, new List<char>() { 'a', 'b', 'c', 'e', 'f', 'g' } },
    //    { 1, new List<char>() { 'c', 'f' } },
    //    { 2, new List<char>() { 'a', 'c', 'd', 'e', 'g' } },
    //    { 3, new List<char>() { 'a', 'c', 'd', 'f', 'g' } },
    //    { 4, new List<char>() { 'b', 'c', 'd', 'f' } },
    //    { 5, new List<char>() { 'a', 'b', 'd', 'f', 'g' } },
    //    { 6, new List<char>() { 'a', 'b', 'd', 'e', 'f', 'g' } },
    //    { 7, new List<char>() { 'a', 'c', 'f' } },
    //    { 8, new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g' } },
    //    { 9, new List<char>() { 'a', 'b', 'c', 'd', 'f', 'g' } },
    //};

    private class Digit
    {
        public readonly string Segments;

        public readonly int Value;

        public Digit(int value, string segments) { Value = value; Segments = segments; }
    }

    private static int CountUniqueLengthDigits(IEnumerable<string> digits)
    {
        var digitCount = new Dictionary<int, int>() { { 1, 0 }, { 4, 0 }, { 7, 0 }, { 8, 0 } };

        foreach (var digit in digits)
        {
            switch (digit.Length)
            {
                case 2: digitCount[1]++; break;
                case 3: digitCount[7]++; break;
                case 4: digitCount[4]++; break;
                case 7: digitCount[8]++; break;
            }
        }

        var sum = 0;

        foreach (var kvp in digitCount)
        {
            sum += kvp.Value;
        }

        return sum;
    }

    [Test]
    public TestResult Test1()
    {
        var sum = 0;

        foreach (var (_, digits) in testInput)
        {
            sum += CountUniqueLengthDigits(digits);
        }

        return ExecuteTest(26, () => sum);
    }

    [Part]
    public string Solve1()
    {
        var sum = 0;

        foreach (var (_, digits) in input)
        {
            sum += CountUniqueLengthDigits(digits);
        }

        return $"{sum}";
    }
}
