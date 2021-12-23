using AdventOfCode.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_22 : BaseDay
{
    private readonly Regex lineRegex = new(@"(?<action>on|off) x=(?<x_min>-?\d+)\.\.(?<x_max>-?\d+),y=(?<y_min>-?\d+)\.\.(?<y_max>-?\d+),z=(?<z_min>-?\d+)\.\.(?<z_max>-?\d+)");

    private readonly List<Instruction> test1Instructions = new();
    private readonly List<Instruction> test2Instructions = new();
    private readonly List<Instruction> partInstructions = new();

    public Day_22(string inputFile)
    {
        var instructions = new List<Instruction>();

        static Point.D3 GetPoint(Match match, string name)
        {
            static int GetCoordinate(Match match, char coordinate, string name) => int.Parse(match.Groups[$"{coordinate}_{name}"].Value);

            return new Point.D3(GetCoordinate(match, 'x', name), GetCoordinate(match, 'y', name), GetCoordinate(match, 'z', name));
        }

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("###"))
            {
                test1Instructions.AddRange(instructions);
                instructions.Clear();
                continue;
            }

            if (line.StartsWith("$$$"))
            {
                test2Instructions.AddRange(instructions);
                instructions.Clear();
                continue;
            }

            if (line.StartsWith("!!!"))
            {
                partInstructions.AddRange(instructions);
                instructions.Clear();
                continue;
            }

            var match = lineRegex.Match(line);

            instructions.Add(new(GetPoint(match, "min"), GetPoint(match, "max"), match.Groups["action"].Value == "on"));
        }
    }

    private class Instruction
    {
        public readonly Point.D3 Start;
        public readonly Point.D3 End;
        public readonly bool On;

        public Instruction(Point.D3 start, Point.D3 end, bool on)
        {
            Start = start; End = end; On = on;
        }

        public bool InLimit(int min, int max)
        {
            static bool InLimit(int i, int min, int max) => i >= min && i <= max;

            if (!InLimit(Start.X, min, max)) return false;
            if (!InLimit(Start.Y, min, max)) return false;
            if (!InLimit(Start.Z, min, max)) return false;

            if (!InLimit(End.X, min, max)) return false;
            if (!InLimit(End.Y, min, max)) return false;
            if (!InLimit(End.Z, min, max)) return false;

            return true;
        }

        public override string ToString() => $"{Start} {End} {On}";
    }

    private class Cuboid
    {
        private readonly Point.D3 Min;
        private readonly Point.D3 Max;
        private readonly sbyte Sign;

        public Cuboid(Point.D3 min, Point.D3 max, sbyte sign)
        {
            Min = min; Max = max; Sign = sign;
        }

        public bool Intersect(Cuboid other)
        {
            static bool CheckDimension(int aMin, int aMax, int bMin, int bMax) => aMin <= bMax && aMax >= bMin;

            if (!CheckDimension(Min.X, Max.X, other.Min.X, other.Max.X)) return false;

            if (!CheckDimension(Min.Y, Max.Y, other.Min.Y, other.Max.Y)) return false;

            if (!CheckDimension(Min.Z, Max.Z, other.Min.Z, other.Max.Z)) return false;

            return true;
        }

        public Cuboid Intersection(Cuboid other)
        {
            static (int min, int max) GetDimension(int aMin, int aMax, int bMin, int bMax) => (Math.Max(aMin, bMin), Math.Min(aMax, bMax));

            var (xMin, xMax) = GetDimension(Min.X, Max.X, other.Min.X, other.Max.X);
            var (yMin, yMax) = GetDimension(Min.Y, Max.Y, other.Min.Y, other.Max.Y);
            var (zMin, zMax) = GetDimension(Min.Z, Max.Z, other.Min.Z, other.Max.Z);

            var sign = Sign * other.Sign;
            if (Sign == other.Sign) sign = -Sign;
            if (Sign == 1 && other.Sign == -1) sign = 1;

            return new Cuboid(new Point.D3(xMin, yMin, zMin), new Point.D3(xMax, yMax, zMax), (sbyte)sign);
        }

        public long Volume
        {
            get
            {
                static long GetDimension(int min, int max) => max - min + 1;

                return Sign * GetDimension(Min.X, Max.X) * GetDimension(Min.Y, Max.Y) * GetDimension(Min.Z, Max.Z);
            }
        }

        public override string ToString() => $"{Min} {Max} {Volume}";
    }

    private static long Execute(IEnumerable<Instruction> instructions, int min, int max)
    {
        var cuboids = new List<Cuboid>();

        foreach (var instruction in instructions.Where(i => i.InLimit(min, max)))
        {
            var current = new Cuboid(instruction.Start, instruction.End, (sbyte)(instruction.On ? 1 : -1));

            var intersections = new List<Cuboid>();

            foreach (var cuboid in cuboids)
            {
                if (current.Intersect(cuboid))
                {
                    intersections.Add(current.Intersection(cuboid));
                }
            }

            cuboids.AddRange(intersections);

            if (current.Volume > 0)
            {
                cuboids.Add(current);
            }
        }

        long result = 0;

        foreach (var cuboid in cuboids)
        {
            result += cuboid.Volume;
        }

        return result;
    }

    [DayTest]
    public TestResult ParseTest() => ExecuteTest(22, () => test1Instructions.Count);

    [DayTest]
    public TestResult Test1() => ExecuteTest(590_784, () => Execute(test1Instructions, -50, 50));

    [DayTest]
    public TestResult Test2() => ExecuteTest(2_758_514_936_282_235, () => Execute(test2Instructions, int.MinValue, int.MaxValue));

    [DayPart]
    public string Solve1() => $"{Execute(partInstructions, -50, 50)}";

    [DayPart]
    public string Solve2() => $"{Execute(partInstructions, int.MinValue, int.MaxValue)}";
}
