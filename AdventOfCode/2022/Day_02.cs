namespace AdventOfCode._2022
{
    internal class Day_02 : BaseDay
    {
        private readonly List<string> input = new();

        private const string testInput = """
            A Y
            B X
            C Z
        """;

        private enum Action
        {
            Rock,
            Paper,
            Scissors,
            Error
        }

        public Day_02(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        private static IEnumerable<(string, string)> Parse(IEnumerable<string> input)
        {
            var games = new List<(string, string)>();

            foreach (var line in input)
            {
                var parts = line.Trim().Split(' ');

                games.Add((parts[0], parts[1]));
            }

            return games;
        }

        private static Action MapOpponentAction(string action)
        {
            return action switch
            {
                "A" => Action.Rock,
                "B" => Action.Paper,
                "C" => Action.Scissors,
                _ => Action.Error
            };
        }

        private static (Action, Action) Map_1((string a, string b) game)
        {
            var a = MapOpponentAction(game.a);

            var b = game.b switch
            {
                "X" => Action.Rock,
                "Y" => Action.Paper,
                "Z" => Action.Scissors,
                _ => Action.Error
            };

            return (a, b);
        }

        private static (Action, Action) Map_2((string a, string b) game)
        {
            var a = MapOpponentAction(game.a);

            const string LOSE = "X", DRAW = "Y", WIN = "Z";

            var b = (a, game.b) switch
            {
                (Action.Rock, LOSE) => Action.Scissors,
                (Action.Rock, DRAW) => Action.Rock,
                (Action.Rock, WIN) => Action.Paper,

                (Action.Paper, LOSE) => Action.Rock,
                (Action.Paper, DRAW) => Action.Paper,
                (Action.Paper, WIN) => Action.Scissors,

                (Action.Scissors, LOSE) => Action.Paper,
                (Action.Scissors, DRAW) => Action.Scissors,
                (Action.Scissors, WIN) => Action.Rock,
                _ => throw new Exception("Unexpected input")
            };

            return (a, b);
        }

        private static (int me, int opponent) Play(IEnumerable<(Action, Action)> input)
        {
            var score_o = 0;
            var score_m = 0;

            const int WIN = 6, DRAW = 3, LOSS = 0;
            var actionPoints = new Dictionary<Action, int>
            {
                { Action.Rock, 1 },
                { Action.Paper, 2 },
                { Action.Scissors, 3 }
            };

            foreach (var (act_o, act_m) in input)
            {
                var (points_o, points_m) = (act_o, act_m) switch
                {
                    (Action.Rock, Action.Rock) => (DRAW, DRAW),
                    (Action.Rock, Action.Paper) => (LOSS, WIN),
                    (Action.Rock, Action.Scissors) => (WIN, LOSS),

                    (Action.Paper, Action.Rock) => (WIN, LOSS),
                    (Action.Paper, Action.Paper) => (DRAW, DRAW),
                    (Action.Paper, Action.Scissors) => (LOSS, WIN),

                    (Action.Scissors, Action.Rock) => (LOSS, WIN),
                    (Action.Scissors, Action.Scissors) => (DRAW, DRAW),
                    (Action.Scissors, Action.Paper) => (WIN, LOSS),
                    _ => throw new Exception("Unexpected actions")
                };

                score_o += points_o + actionPoints[act_o];
                score_m += points_m + actionPoints[act_m];
            }

            return (score_m, score_o);
        }

        [DayTest]
        public static TestResult Test_1() => ExecuteTest(15, () => Play(Parse(testInput.Split(Environment.NewLine)).Select(Map_1)).me);

        [DayTest]
        public static TestResult Test_2() => ExecuteTest(12, () => Play(Parse(testInput.Split(Environment.NewLine)).Select(Map_2)).me);

        [DayPart]
        public string Solve_1() => $"{Play(Parse(input).Select(Map_1)).me}";

        [DayPart]
        public string Solve_2() => $"{Play(Parse(input).Select(Map_2)).me}";
    }
}
