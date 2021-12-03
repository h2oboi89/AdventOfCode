namespace AdventOfCode._2015;

internal class Day_01 : BaseDay
{
    private readonly List<int> directions;

    public Day_01(string inputFile)
    {
        directions = ParseDirections(File.ReadAllText(inputFile));
    }

    private static List<int> ParseDirections(string input)
    {
        var directions = new List<int>();

        foreach (var c in input)
        {
            switch (c)
            {
                case '(':
                    directions.Add(1);
                    break;
                case ')':
                    directions.Add(-1);
                    break;
            }
        }

        return directions;
    }

    private static int FollowDirections(List<int> directions)
    {
        var floor = 0;

        foreach (var direction in directions)
        {
            floor += direction;
        }

        return floor;
    }

    private static int FindBasement(List<int> directions)
    {
        var floor = 0;
        var position = 1;

        foreach (var direction in directions)
        {
            floor += direction;

            if (floor == -1)
            {
                break;
            }

            position++;
        }

        return position;
    }

    [Test]
    public static bool Test_1()
    {
        var testValues = new List<(string input, object expected)>
        {
            ("(())", 0),
            ("()()", 0),
            ("(((", 3),
            ("(()(()(", 3),
            ("))(((((", 3),
            ("())", -1),
            ("))(", -1),
            (")))", -3),
            (")())())", -3),
        };

        return ExecuteTests(testValues, (i) => FollowDirections(ParseDirections(i)));
    }

    [Test]
    public static bool Test_2()
    {
        var testValues = new List<(string input, object expected)>
        {
            (")", 1),
            ("()())", 5),
        };

        return ExecuteTests(testValues, (i) => FindBasement(ParseDirections(i)));
    }

    [Part]
    public string Solve_1() => $"{FollowDirections(directions)}";

    [Part]
    public string Solve_2() => $"{FindBasement(directions)}";
}
