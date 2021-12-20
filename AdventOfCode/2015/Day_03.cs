using AdventOfCode.Common;

namespace AdventOfCode._2015;

internal class Day_03 : BaseDay
{
    private readonly IEnumerable<Direction> input;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private class House
    {
        public readonly Point2D Address;

        public int Presents = 0;

        public House(Point2D address)
        {
            Address = address;
        }

        public override string ToString() => $"{Address} : {Presents}";
    }

    public Day_03(string inputFile)
    {
        input = ParseInput(File.ReadAllText(inputFile));
    }

    private static IEnumerable<Direction> ParseInput(string input)
    {
        foreach (var c in input)
        {
            switch (c)
            {
                case '^': yield return Direction.Up; break;
                case 'v': yield return Direction.Down; break;
                case '<': yield return Direction.Left; break;
                case '>': yield return Direction.Right; break;
            }
        }
    }

    private static IEnumerable<House> Travel(IEnumerable<Direction> directions)
    {
        var houses = new List<House> { new House(new Point2D()) };

        var current = houses[0];

        current.Presents++; // takes care of initial present

        foreach (var direction in directions)
        {
            var newAddr = direction switch
            {
                Direction.Up => new Point2D(current.Address.X, current.Address.Y + 1),
                Direction.Down => new Point2D(current.Address.X, current.Address.Y - 1),
                Direction.Left => new Point2D(current.Address.X - 1, current.Address.Y),
                Direction.Right => new Point2D(current.Address.X + 1, current.Address.Y),
                _ => throw new InvalidOperationException("WTF...")
            };

            var nextHouse = houses.FirstOrDefault(x => x.Address.Equals(newAddr));

            if (nextHouse == default)
            {
                current = new House(newAddr);
                houses.Add(current);
            }
            else
            {
                current = nextHouse;
            }

            current.Presents++;
        }

        return houses;
    }

    private static int SplitTravel(IEnumerable<Direction> directions)
    {
        var santaDirections = directions.Where((_, index) => index % 2 == 0);
        var roboSantaDirections = directions.Where((_, index) => index % 2 != 0);

        var santaHouses = Travel(santaDirections);
        var roboHouses = Travel(roboSantaDirections);

        return santaHouses.Union(roboHouses).DistinctBy(h => h.Address).Count();
    }

    [DayTest]
    public static TestResult Test1()
    {
        var testValues = new List<(string, object)>
        {
            (">", 2),
            ("^>v<", 4),
            ("^v^v^v^v^v", 2)
        };

        return ExecuteTests(testValues, (i) => Travel(ParseInput(i)).Count());
    }

    [DayTest]
    public static TestResult Test2()
    {
        var testValues = new List<(string, object)>
        {
            ("^v", 3),
            ("^>v<", 3),
            ("^v^v^v^v^v", 11)
        };

        return ExecuteTests(testValues, (i) => SplitTravel(ParseInput(i)));
    }

    [DayPart]
    public string Solve1() => $"{Travel(input).Count()}";

    [DayPart]
    public string Solve2() => $"{SplitTravel(input)}";
}
