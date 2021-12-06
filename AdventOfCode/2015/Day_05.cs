namespace AdventOfCode._2015;

internal class Day_05 : BaseDay
{
    private readonly IEnumerable<string> input;
    private const char EMPTY = ' ';

    public Day_05(string inputFile)
    {
        var values = new List<string>();

        foreach (var line in File.ReadAllLines(inputFile))
        {
            values.Add(line.Trim());
        }

        input = values;
    }

    private static bool IsNice1(string value)
    {
        var vowelCount = 0;
        var previous = EMPTY;
        var hasDouble = false;

        static bool IsVowel(char c)
        {
            return c switch
            {
                'a' or 'e' or 'i' or 'o' or 'u' => true,
                _ => false,
            };
        }

        static bool IsBadPair(char previous, char current)
        {
            return $"{previous}{current}" switch
            {
                "ab" or "cd" or "pq" or "xy" => true,
                _ => false,
            };
        }

        foreach (var c in value)
        {
            if (IsVowel(c))
            {
                vowelCount++;
            }

            if (c.Equals(previous))
            {
                hasDouble = true;
            }

            if (IsBadPair(previous, c)) return false;

            previous = c;
        }

        return vowelCount >= 3 && hasDouble;
    }

    private class Pair
    {
        public readonly string Chars;
        public readonly int Index;

        public Pair(string chars, int index)
        {
            Chars = chars; Index = index;
        }
    }

    private static bool IsNice2(string value)
    {
        var pairs = new List<Pair>();
        var hasSplitRepeat = false;

        var p1 = EMPTY;
        var p0 = EMPTY;

        static bool CheckPairs(IEnumerable<Pair> pairs)
        {
            foreach (var pair in pairs)
            {
                foreach (var other in pairs)
                {
                    if (pair.Chars.Equals(other.Chars) && other.Index - pair.Index > 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        foreach(var (c, i) in value.Select((c, i) => (c, i)))
        {
            if (!p0.Equals(EMPTY))
            {
                pairs.Add(new Pair($"{p0}{c}", i));
            }

            if (c.Equals(p1))
            {
                hasSplitRepeat = true;
            }

            p1 = p0;
            p0 = c;
        }

        return hasSplitRepeat && CheckPairs(pairs);
    }

    private static int CheckList(IEnumerable<string> values, Func<string, bool> checkRules)
    {
        var nice = 0;

        foreach (var value in values)
        {
            if (checkRules(value)) nice++;
        }

        return nice;
    }

    [Test]
    public static TestResult Test1()
    {
        var testValues = new List<(string, object)>
        {
            ("ugknbfddgicrmopn", true),
            ("aaa", true),
            ("jchzalrnumimnmhp", false),
            ("haegwjzuvuyypxyu", false),
            ("dvszwmarrgswjxmb", false),
        };

        return ExecuteTests(testValues, (i) => IsNice1(i));
    }

    [Test]
    public static TestResult Test2()
    {
        var testValues = new List<(string, object)>
        {
            ("qjhvhtzxzqqjkmpb", true),
            ("xxyxx", true),
            ("uurcxstgmygtbstg", false),
            ("ieodomkazucvgmuy", false),
        };

        return ExecuteTests(testValues, (i) => IsNice2(i));
    }

    [Part]
    public string Part1() => $"{CheckList(input, IsNice1)}";

    [Part]
    public string Part2() => $"{CheckList(input, IsNice2)}";
}
