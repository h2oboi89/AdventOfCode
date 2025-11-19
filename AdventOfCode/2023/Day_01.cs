namespace AdventOfCode._2023;
internal class Day_01(string inputFile) : BaseDay
{
    private readonly string _inputFile = inputFile;

    private static int ParseLine(string line, bool advanced)
    {
        var digits = new List<int>();

        for (var i = 0; i < line.Length; i++)
        {
            if (TryParseDigit(line[i], out var digit))
            {
                digits.Add(digit);
                continue;
            }

            if (advanced)
            {
                foreach (var (word, d) in _digitWords)
                {
                    if (CheckDigitWord(line, i, word))
                    {
                        digits.Add(d);
                    }
                }
            }
        }

        return digits.First() * 10 + digits.Last();
    }

    private static bool TryParseDigit(char c, out int digit)
    {
        digit = 0;

        if (char.IsDigit(c))
        {
            digit = int.Parse($"{c}");
            return true;
        }

        return false;
    }

    private static readonly (string, int)[] _digitWords =
    [
        ( "one", 1 ),
        ( "two", 2 ),
        ( "three", 3 ),
        ( "four", 4 ),
        ( "five", 5 ),
        ( "six", 6 ),
        ( "seven", 7 ),
        ( "eight", 8 ),
        ( "nine", 9 ),
    ];

    private static bool CheckDigitWord(string line, int i, string word)
    {
        if (line.Length - i < word.Length)
        {
            return false;
        }

        for (var k = 0; k < word.Length; k++)
        {
            if (line[i + k] != word[k])
            {
                return false;
            }
        }

        return true;
    }

    private static int SumLines(IEnumerable<string> lines, bool advanced)
    {
        var sum = 0;
        foreach (var line in lines)
        {
            var value = ParseLine(line, advanced);
            sum += value;
        }

        return sum;
    }

    private static readonly List<(string, int)> _part1TestLines =
    [
        ( "1abc2", 12 ),
        ( "pqr3stu8vwx", 38 ),
        ( "a1b2c3d4e5f", 15 ),
        ( "treb7uchet", 77),
    ];

    private static readonly List<(string, int)> _part2TestLines =
    [
        ( "two1nine", 29 ),
        ( "eightwothree", 83 ),
        ( "abcone2threexyz", 13 ),
        ( "xtwone3four", 24 ),
        ( "4nineeightseven2", 42 ),
        ( "zoneight234", 14 ),
        ( "7pqrstsixteen", 76 ),
    ];

    [DayTest]
    public static TestResult Test_1_1()
    {
        return ExecuteTests(_part1TestLines, i => ParseLine(i, false));
    }

    [DayTest]
    public static TestResult Test_1_2()
    {
        return ExecuteTest(_part1TestLines, 142, i => SumLines(i.Select(x => x.Item1), false));
    }

    [DayTest]
    public static TestResult Test_2_1()
    {
        return ExecuteTests(_part2TestLines, i => ParseLine(i, true));
    }

    [DayTest]
    public static TestResult Test_2_2()
    {
        return ExecuteTest(_part2TestLines, 281, i => SumLines(i.Select(x => x.Item1), true));
    }

    [DayPart]
    public string Solve_1() => $"{SumLines(File.ReadAllLines(_inputFile), false)}";

    [DayPart]
    public string Solve_2() => $"{SumLines(File.ReadAllLines(_inputFile), true)}";
}
