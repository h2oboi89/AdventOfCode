using System.Diagnostics.CodeAnalysis;

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
        public readonly Address Address;

        public int Presents = 0;

        public House(Address address)
        {
            Address = address;
        }
    }

    private class Address
    {
        public readonly int X, Y;

        public Address(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Address other) return false;

            return other.X == X & other.Y == Y;
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public override string ToString() => $"( {X}, {Y} )";
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
        var houses = new List<House> { new House(new Address(0, 0)) };

        var current = houses[0];

        current.Presents++; // takes care of initial present

        foreach (var direction in directions)
        {
            var newAddr = direction switch
            {
                Direction.Up => new Address(current.Address.X, current.Address.Y + 1),
                Direction.Down => new Address(current.Address.X, current.Address.Y - 1),
                Direction.Left => new Address(current.Address.X - 1, current.Address.Y),
                Direction.Right => new Address(current.Address.X + 1, current.Address.Y),
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

    [Test]
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

    [Test]
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

    [Part]
    public string Solve1() => $"{Travel(input).Count()}";

    [Part]
    public string Solve2() => $"{SplitTravel(input)}";
}
