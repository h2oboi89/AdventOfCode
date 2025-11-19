using System.Text.RegularExpressions;
using AdventOfCode.Extensions;

namespace AdventOfCode._2021;

internal class Day_21 : BaseDay
{
    private readonly List<int> testPlayers = [];
    private readonly List<int> partPlayers = [];

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

        public Player(int postion, int score = 0)
        {
            Position = postion;
            Score = score;
        }

        public void Move(int roll)
        {
            Position = ((Position - 1 + roll) % 10) + 1;
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

        public static bool operator ==(Player a, Player b) => a.Equals(b);

        public static bool operator !=(Player a, Player b) => !(a == b);
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

    private static class DiracDice
    {
        private static readonly List<int> _values = [1, 2, 3];

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

        public static readonly IReadOnlyDictionary<int, int> Rolls;

        static DiracDice()
        {
            var rolls = new Dictionary<int, int>();

            foreach (var permutation in Permute())
            {
                var sum = permutation.Sum();

                if (!rolls.ContainsKey(sum)) rolls[sum] = 0;

                rolls[sum]++;
            }

            Rolls = rolls;
        }
    }

    private static IEnumerable<int> Repeat(Func<int> func, int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return func();
        }
    }

    private static (int winner, int loser, int rolls) PlayDeterministicGame(IEnumerable<int> playerPositions)
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

    private static ulong PlayQuantumGames(IEnumerable<int> playerPositions)
    {
        var players = playerPositions.Select(position => new Player(position)).ToList();

        var universes = new Dictionary<(Player player1, Player player2, int currentPlayer), ulong>()
        {
            { (players.First(), players.Last(), 0), 1 }
        };

        ulong player1Wins = 0, player2Wins = 0;
        var win = 21;


        while(universes.Any())
        {
            var newUniverses = new Dictionary<(Player, Player, int), ulong>();

            foreach(var (universe, count) in universes)
            {
                foreach(var (roll, frequency) in DiracDice.Rolls)
                {
                    var p1 = universe.player1.Clone(); 
                    var p2 = universe.player2.Clone();
                    var occurences = count * (ulong)frequency;

                    if (universe.currentPlayer == 0)
                    {
                        p1.Move(roll);
                        if (p1.Score >= win)
                        {
                            player1Wins += occurences;
                            continue;
                        }
                    }
                    else
                    {
                        p2.Move(roll);
                        if (p2.Score >= win)
                        {
                            player2Wins += occurences;
                            continue;
                        }
                    }

                    newUniverses.AddOrUpdate((p1, p2, (universe.currentPlayer + 1) % 2), occurences);
                }
            }

            universes = newUniverses;
        }

        return Math.Max(player1Wins, player2Wins);
    }

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

    [DayTest]
    public TestResult Test2() => ExecuteTest((ulong)444_356_092_776_315, () => PlayQuantumGames(testPlayers));

    [DayPart]
    public string Solve1()
    {
        var (_, lose, rolls) = PlayDeterministicGame(partPlayers);

        return $"{lose * rolls}";
    }

    [DayPart]
    public string Solve2() => $"{PlayQuantumGames(partPlayers)}";
}
