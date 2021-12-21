using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_21 : BaseDay
{
    private readonly List<int> testPlayers = new();
    private readonly List<int> partPlayers = new();

    private static readonly Regex playerRegex = new(@"Player (?<id>\d) starting position: (?<position>\d)");

    public Day_21(string inputFile)
    {
        var players = new List<int>();

        foreach (var line in File.ReadLines(inputFile))
        {
            if (line.StartsWith("###"))
            {
                testPlayers.AddRange(players);
                players.Clear();
                continue;
            }

            if (line.StartsWith("!!!"))
            {
                partPlayers.AddRange(players);
                players.Clear();
                continue;
            }

            var match = playerRegex.Match(line);

            players.Add(int.Parse(match.Groups["position"].Value));
        }
    }

    private class Player
    {
        public int Position;
        public int Score;

        private Player(int postion, int score)
        {
            Position = postion;
            Score = score;
        }

        public Player(int position) : this(position, 0) { }

        public void Move(int roll)
        {
            Position = Board.Move(Position, roll);
            Score += Position;
        }

        public override string ToString() => $"{Position} {Score}";

        public Player Clone() => new(Position, Score);

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Player other) return false;

            return Position == other.Position && Score == other.Score;
        }

        public override int GetHashCode() => HashCode.Combine(Position, Score);
    }

    private class DeterministicDice
    {
        private int _value;

        private readonly int _sides = 100;

        public DeterministicDice() { Reset(); }

        public void Reset() => _value = 1;

        public int Roll()
        {
            var roll = _value++;

            if (_value > _sides)
            {
                Reset();
            }

            return roll;
        }
    }

    private class DiracDice
    {
        private static readonly List<int> _values = new() { 1, 2, 3 };

        private static IEnumerable<List<int>> Permute()
        {
            foreach (var a in _values)
            {
                foreach (var b in _values)
                {
                    foreach (var c in _values)
                    {
                        yield return new List<int> { a, b, c };
                    }
                }
            }
        }

        private static readonly List<List<int>> Permutations = Permute().ToList();

        public static IEnumerable<IEnumerable<int>> Roll3()
        {
            foreach (var permutation in Permutations)
            {
                yield return permutation;
            }
        }
    }

    private static class Board
    {
        private const int _size = 10;

        public static int Move(int start, int step) => ((start - 1 + step) % _size) + 1;
    }

    private static IEnumerable<int> Repeat(Func<int> func, int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return func();
        }
    }

    private static (int winner, int loser, int rolls) PlayDeterministicGame(List<int> playerPositions)
    {
        var dice = new DeterministicDice();

        var players = playerPositions.Select(position => new Player(position)).ToList();

        var currentPlayer = 0;
        var rolls = 0;
        var rollTimes = 3;

        while (!players.Any(p => p.Score >= 1000))
        {
            var player = players[currentPlayer];

            var roll = Repeat(dice.Roll, rollTimes).Sum();
            rolls += rollTimes;

            player.Move(roll);

            currentPlayer = (currentPlayer + 1) % players.Count;
        }

        var results = players.OrderByDescending(p => p.Score);

        return (results.First().Score, results.Last().Score, rolls);
    }

    private static ulong PlayQuantumGames(List<(int id, int position)> playerInfo)
    {
        // how do we play and store a lot of games simultaneously?

        throw new NotImplementedException();
    }

    [DayTest]
    public static TestResult BoardMoveTest() => ExecuteTest(2, () => Board.Move(7, 5));

    [DayTest]
    public TestResult MultipleMovesTest()
    {
        var testValues = new List<((Player player, List<int> rolls), int expected)>()
        {
            ((new Player(testPlayers[0]), new List<int>() { 1 + 2 + 3, 7 + 8 + 9, 13 + 14 + 15 }), 6),
            ((new Player(testPlayers[1]), new List<int>() { 4 + 5 + 6, 10 + 11 + 12, 16 + 17 + 18 }), 7)
        };

        return ExecuteTests(testValues, (input) =>
        {
            foreach (var roll in input.rolls)
            {
                input.player.Move(roll);
            }

            return input.player.Position;
        });
    }

    [DayTest]
    public TestResult Test1() => ExecuteTest(739785, () =>
    {
        var (_, lose, rolls) = PlayDeterministicGame(testPlayers);

        return lose * rolls;
    });

    [DayPart]
    public string Solve1()
    {
        var (_, lose, rolls) = PlayDeterministicGame(partPlayers);

        return $"{lose * rolls}";
    }
}
