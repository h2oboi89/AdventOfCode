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
            static string Sort(string s)
            {
                var chars = s.ToArray();

                Array.Sort(chars);

                return new(chars);
            }

            static IEnumerable<string> ParsePart(string part) => part.Split(' ').Select(s => Sort(s));

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

    private static int CountUniqueLengthDigits(IEnumerable<(IEnumerable<string> patterns, IEnumerable<string> digits)> input)
    {
        var digitCount = new Dictionary<int, int>() { { 1, 0 }, { 4, 0 }, { 7, 0 }, { 8, 0 } };

        foreach (var (_, digits) in input)
        {
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
        }

        var sum = 0;

        foreach (var kvp in digitCount)
        {
            sum += kvp.Value;
        }

        return sum;
    }

    private static Dictionary<string, int> DecodePatterns(IEnumerable<string> patterns)
    {
        static IEnumerable<char> FindUniqueSegments(IEnumerable<string> strings)
        {
            var chars = new Dictionary<char, int>();

            foreach (var s in strings)
            {
                foreach (var c in s)
                {
                    if (!chars.ContainsKey(c))
                    {
                        chars.Add(c, 1);
                    }
                    else
                    {
                        chars[c]++;
                    }
                }
            }

            return chars.Where(kvp => kvp.Value == 1).Select(kvp => kvp.Key);
        }

        static char FindSegmentNotInPattern(IEnumerable<char> segments, string pattern)
        {
            foreach (var segment in segments)
            {
                if (!pattern.Contains(segment))
                {
                    return segment;
                }
            }

            throw new ArgumentException("All segments in pattern");
        }

        static int PatternOverlap(string a, string b)
        {
            var count = 0;

            foreach (var c in a)
            {
                if (b.Contains(c))
                {
                    count++;
                }
            }

            return count;
        }

        var digits = new Dictionary<int, string>();

        var len5 = new List<string>();
        var len6 = new List<string>();

        foreach (var pattern in patterns)
        {
            switch(pattern.Length)
            {
                case 2: digits[1] = pattern; break;
                case 3: digits[7] = pattern; break;
                case 4: digits[4] = pattern; break;
                case 5: len5.Add(pattern); break; // 2, 3, 5
                case 6: len6.Add(pattern); break; // 0, 6, 9
                case 7: digits[8] = pattern; break;
            }
        }

        // Unsolved = 0, 2, 3, 5, 6, 9
        var eb = FindUniqueSegments(len5);
        var e = FindSegmentNotInPattern(eb, digits[4]);

        foreach(var p in len5)
        {
            if (p.Contains(e))
            {
                digits[2] = p;
            }
        }

        len5.Remove(digits[2]);

        foreach(var p in len6)
        {
            if (!p.Contains(e))
            {
                digits[9] = p;
            }
        }

        len6.Remove(digits[9]);

        // Unsolved = 0, 3, 5, 6
        foreach(var p in len5)
        {
            switch(PatternOverlap(digits[1], p))
            {
                case 1: digits[5] = p; break;
                case 2: digits[3] = p; break;
            }
        }

        foreach (var p in len6)
        {
            switch (PatternOverlap(digits[1], p))
            {
                case 1: digits[6] = p; break;
                case 2: digits[0] = p; break;
            }
        }

        // All solved
        return digits.ToDictionary(x=> x.Value, x => x.Key);
    }

    private static int DecodeDigits(Dictionary<string, int> patterns, IEnumerable<string> digits)
    {
        var value = 0;

        foreach(var d in digits)
        {
            value *= 10;

            value += patterns[d];
        }

        return value;
    }

    private static int Decode(IEnumerable<(IEnumerable<string> patterns, IEnumerable<string> digits)> input)
    {
        var sum = 0;

        foreach(var (patterns, digits) in input)
        {
            var value = DecodeDigits(DecodePatterns(patterns), digits);

            sum += value;
        }

        return sum;
    }

    [Test]
    public TestResult Test1() => ExecuteTest(26, () => CountUniqueLengthDigits(testInput));

    [Test]
    public TestResult Test2() => ExecuteTest(61229, () => Decode(testInput));

    [Part]
    public string Solve1() => $"{CountUniqueLengthDigits(input)}";

    [Part]
    public string Solve2() => $"{Decode(input)}";
}
