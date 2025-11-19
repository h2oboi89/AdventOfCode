namespace AdventOfCode._2021;

internal class Day_02 : BaseDay
{
    private readonly List<(Direction direction, int value)> input = [];

    private readonly List<(Direction direction, int value)> testInput =
    [
        (Direction.forward, 5),
        (Direction.down, 5),
        (Direction.forward, 8),
        (Direction.up, 3),
        (Direction.down, 8),
        (Direction.forward, 2),
    ];

    private enum Direction
    {
        Unknown,
        forward,
        up,
        down
    }

    public Day_02(string inputFile)
    {
        foreach (var line in File.ReadAllLines(inputFile))
        {
            var parts = line.Split(' ');

            var direction = Direction.Unknown;

            switch (parts[0])
            {
                case nameof(Direction.forward):
                    direction = Direction.forward;
                    break;
                case nameof(Direction.up):
                    direction = Direction.up;
                    break;
                case nameof(Direction.down):
                    direction = Direction.down;
                    break;
            }

            var v = int.Parse(parts[1]);

            input.Add((direction, v));
        }
    }

    private static int Solve1(List<(Direction, int)> values)
    {
        var horizontal = 0;
        var depth = 0;

        foreach (var (direction, value) in values)
        {
            switch (direction)
            {
                case Direction.forward:
                    horizontal += value;
                    break;
                case Direction.up:
                    depth -= value;
                    break;
                case Direction.down:
                    depth += value;
                    break;
            }
        }

        return horizontal * depth;
    }

    private static int Solve2(List<(Direction, int)> values)
    {
        var horizontal = 0;
        var depth = 0;
        var aim = 0;

        foreach (var (direction, value) in values)
        {
            switch (direction)
            {
                case Direction.forward:
                    horizontal += value;
                    depth += aim * value;
                    break;
                case Direction.up:
                    aim -= value;
                    break;
                case Direction.down:
                    aim += value;
                    break;
            }
        }

        return horizontal * depth;
    }

    [DayTest]
    public TestResult Test1() => ExecuteTest(testInput, 150, Solve1);

    [DayTest]
    public TestResult Test2() => ExecuteTest(testInput, 900, Solve2);

    [DayPart]
    public string Solve_1() => $"{Solve1(input)}";

    [DayPart]
    public string Solve_2() => $"{Solve2(input)}";
}
