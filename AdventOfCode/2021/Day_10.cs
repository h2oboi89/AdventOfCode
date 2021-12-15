namespace AdventOfCode._2021;

internal class Day_10 : BaseDay
{
    public readonly IEnumerable<string> input;
    public readonly IEnumerable<string> testInput;

    private const char NULL = '\0';

    public Day_10(string intputFile)
    {
        var values = new List<string>();
        var test = true;

        foreach (var line in File.ReadAllLines(intputFile))
        {
            if (line.StartsWith("#"))
            {
                if (test)
                {
                    test = false;
                    testInput = values;
                    values = new();
                }
                else
                {
                    input = values;
                }

                continue;
            }

            values.Add(line);
        }
    }

    private static readonly Dictionary<char, int> InvalidPoints = new() { { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 }, };
    private static readonly Dictionary<char, ulong> CompletePoints = new() { { ')', 1 }, { ']', 2 }, { '}', 3 }, { '>', 4 }, };

    private static (int index, Stack<char>? stack) CheckLine(string line)
    {
        var chunks = new Stack<char>();

        static bool CheckAndPop(Stack<char> chunks, char c)
        {
            var expected = c switch
            {
                ')' => '(',
                ']' => '[',
                '}' => '{',
                '>' => '<',
                _ => NULL
            };

            if (expected == NULL || chunks.Peek() != expected)
            {
                return false;
            }
            else
            {
                chunks.Pop();

                return true;
            }
        }

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            switch (c)
            {
                case '(':
                case '[':
                case '{':
                case '<':
                    chunks.Push(c);
                    break;
                default:
                    if (!chunks.Any() || !CheckAndPop(chunks, c))
                    {
                        return (i, null);
                    }
                    break;
            }
        }

        return (-1, chunks);
    }

    private static IEnumerable<(string line, int index)> FindCorrupted(IEnumerable<string> input)
    {
        foreach (var line in input)
        {
            var (index, _) = CheckLine(line);

            if (index != -1)
            {
                yield return (line, index);
            }
        }
    }

    private static IEnumerable<Stack<char>> FindIncomplete(IEnumerable<string> input)
    {
        foreach (var line in input)
        {
            var (index, stack) = CheckLine(line);

            if (index == -1 && stack != null)
            {
                yield return stack;
            }
        }
    }

    private static int CalculateCorruptionScore(IEnumerable<string> input)
    {
        var score = 0;

        foreach (var (line, index) in FindCorrupted(input))
        {
            score += InvalidPoints[line[index]];
        }

        return score;
    }

    private static ulong CalculateIncompleteScore(IEnumerable<string> input)
    {
        var scores = new List<ulong>();

        foreach (var line in FindIncomplete(input))
        {
            var completion = new List<char>();

            while(line.Any())
            {
                var c = line.Pop();

                var required = c switch {
                    '(' => ')',
                    '[' => ']',
                    '{' => '}',
                    '<' => '>',
                    _ => NULL
                };
                
                completion.Add(required);
            }

            ulong score = 0;

            foreach(var c in completion)
            {
                score *= 5;
                score += CompletePoints[c];
            }

            scores.Add(score);
        }

        scores.Sort();

        var middleIndex = (scores.Count / 2);

        return scores[middleIndex];
    }

    [Test]
    public TestResult Test1() => ExecuteTest(26397, () => CalculateCorruptionScore(testInput));

    [Test]
    public TestResult Test2() => ExecuteTest((ulong)288957, () => CalculateIncompleteScore(testInput));

    [Part]
    public string Solve1() => $"{CalculateCorruptionScore(input)}";

    [Part]
    public string Solve2() => $"{CalculateIncompleteScore(input)}";
}
