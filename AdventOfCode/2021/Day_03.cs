namespace AdventOfCode._2021;

internal class Day_03 : BaseDay
{
    private readonly int inputWidth, testInputWidth;
    private readonly List<int> input = new(), testInput = new();
    private readonly List<string> testValues = new()
    {
        "00100",
        "11110",
        "10110",
        "10111",
        "10101",
        "01111",
        "00111",
        "11100",
        "10000",
        "11001",
        "00010",
        "01010",
    };

    public Day_03(string inputFile)
    {
        var lines = File.ReadAllLines(inputFile);

        inputWidth = lines[0].Length;

        foreach (var line in lines)
        {
            input.Add(Convert.ToInt32(line, 2));
        }

        testInputWidth = testValues[0].Length;

        foreach(var value in testValues)
        {
            testInput.Add(Convert.ToInt32(value, 2));
        }
    }

    private static int BitPosition(int i, int width) => width - i;

    private static int GetBit(int value, int bitPosition) => (value >> bitPosition - 1) & 1;

    private static (int mostCommon, int leastCommon) PositionCommon(List<int> values, int bitPosition)
    {
        var zeroCount = 0;
        var oneCount = 0;

        foreach (var value in values)
        {
            var bit = GetBit(value, bitPosition);

            if (bit == 0)
            {
                zeroCount++;
            }

            if (bit == 1)
            {
                oneCount++;
            }
        }

        if (zeroCount > oneCount)
        {
            return (0, 1);
        }
        else
        {
            return (1, 0);
        }
    }

    private static List<int> Filter(List<int> values, int filterBit, int bitPosition)
    {
        bool CheckBit(int value, int bit, int requiredValue)
        {
            return GetBit(value, bitPosition) == requiredValue;
        }

        if (values.Count > 1)
        {
            values = values.Where(x => CheckBit(x, bitPosition, filterBit)).ToList();
        }

        return values;
    }

    private static int Solve1(List<int> input, int width)
    {
        var gamma = 0;
        var epsilon = 0;

        static int ShiftOr(int value, int bit) => (value << 1) | bit;

        for (var i = 0; i < width; i++)
        {
            var (gammaBit, epsilonBit) = PositionCommon(input, BitPosition(i, width));

            gamma = ShiftOr(gamma, gammaBit);
            epsilon = ShiftOr(epsilon, epsilonBit);
        }

        return gamma * epsilon;
    }

    private static int Solve2(List<int> input, int width)
    {
        var o2Rating = input;
        var co2Rating = input;

        for (var i = 0; i < width; i++)
        {
            var bitPosition = BitPosition(i, width);

            var (o2Filter, _) = PositionCommon(o2Rating, bitPosition);
            o2Rating = Filter(o2Rating, o2Filter, bitPosition);

            var (_, co2Filter) = PositionCommon(co2Rating, bitPosition);
            co2Rating = Filter(co2Rating, co2Filter, bitPosition);
        }

        return o2Rating[0] * co2Rating[0];
    }

    [Test]
    public bool Test1() => ExecuteTest(string.Empty, 198, (_) => Solve1(testInput, testInputWidth));

    [Test]
    public bool Test2() => ExecuteTest(string.Empty, 230, (_) => Solve2(testInput, testInputWidth));

    [Part]
    public string Solve_1() => $"{Solve1(input, inputWidth)}";

    [Part]
    public string Solve_2() => $"{Solve2(input, inputWidth)}";
}
