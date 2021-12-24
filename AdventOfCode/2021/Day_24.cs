using System.Diagnostics;

namespace AdventOfCode._2021;

internal class Day_24 : BaseDay
{
    private readonly List<(Operation operation, string a, string b)> instructions = new();

    public Day_24(string inputFile)
    {
        foreach (var line in File.ReadLines(inputFile))
        {
            var parts = line.Split(' ');

            var op = ParseOperation(parts[0]);
            var a = parts[1];
            var b = op == Operation.Input ? string.Empty : parts[2];

            instructions.Add((op, a, b));
        }
    }

    private enum Operation
    {
        Input,
        Add,
        Multiply,
        Divide,
        Modulo,
        Equal
    }

    private static Operation ParseOperation(string operation) => operation switch
    {
        "inp" => Operation.Input,
        "add" => Operation.Add,
        "mul" => Operation.Multiply,
        "div" => Operation.Divide,
        "mod" => Operation.Modulo,
        "eql" => Operation.Equal,
        _ => throw new Exception($"Invalid operation: '{operation}"),
    };

    private class ALU
    {
        public long W { get; private set; } = 0;
        public long X { get; private set; } = 0;
        public long Y { get; private set; } = 0;
        public long Z { get; private set; } = 0;

        private void SetRegister(string r, long value)
        {
            switch (r)
            {
                case "w": W = value; break;
                case "x": X = value; break;
                case "y": Y = value; break;
                case "z": Z = value; break;
                default: throw new Exception($"Invalid register: '{r}'");
            }
        }

        private long GetRegister(string r)
        {
            switch (r)
            {
                case "w": return W;
                case "x": return X;
                case "y": return Y;
                case "z": return Z;
                default: throw new Exception($"Invalid register: '{r}'");
            }
        }

        private void Execute(Operation operation, string r, long a, long b)
        {
            switch (operation)
            {
                case Operation.Input: SetRegister(r, a); break;
                case Operation.Add: SetRegister(r, a + b); break;
                case Operation.Multiply: SetRegister(r, a * b); break;
                case Operation.Divide: SetRegister(r, a / b); break;
                case Operation.Modulo: SetRegister(r, a % b); break;
                case Operation.Equal: SetRegister(r, a == b ? 1 : 0); break;
            }
        }

        public ALU Clone() => new() { W = W, X = X, Y = Y, Z = Z };

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not ALU other) return false;

            return W == other.W && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode() => (W, X, Y, Z).GetHashCode();

        public override string ToString() => $"W : {W}, X : {X}, Y : {Y}, Z :{Z}";

        private static void Execute((Operation operation, string a, string b) instruction, ref List<(ALU, ulong, ulong)> alus)
        {
            var (operation, a, b) = instruction;

            if (operation == Operation.Input)
            {
                var uniqueAlus = new Dictionary<ALU, (ulong min, ulong max)>();

                for(var i = 0; i < alus.Count; i++)
                {
                    var (alu, min, max) = alus[i];

                    for (ulong j = 1; j < 10; j++)
                    {
                        var newAlu = alu.Clone();

                        newAlu.Execute(operation, a, (long)j, 0);

                        var uMin = (min * 10) + j;
                        var uMax = (max * 10) + j;

                        if (!uniqueAlus.ContainsKey(newAlu))
                        {
                            uniqueAlus[newAlu] = (uMin, uMax);
                        }

                        var prev = uniqueAlus[newAlu];

                        uniqueAlus[newAlu] = (Math.Min(prev.min, uMin), Math.Min(prev.max, uMax));
                    }
                }

                alus = uniqueAlus.Select(kvp => (kvp.Key, kvp.Value.min, kvp.Value.max)).ToList();

                if (Debugger.IsAttached)
                {
                    Console.WriteLine($"Processing {alus.Count} alu states");
                }
            }
            else
            {
                Parallel.ForEach(alus, (v) =>
                {
                    var (alu, _, _) = v;

                    var aValue = alu.GetRegister(a);

                    if (!long.TryParse(b, out long bValue)) bValue = alu.GetRegister(b);

                    alu.Execute(operation, a, aValue, bValue);
                });

                //for (var i = 0; i < alus.Count; i++)
                //{
                //    var (alu, _, _) = alus[i];

                //    var aValue = alu.GetRegister(a);

                //    if (!long.TryParse(b, out long bValue)) bValue = alu.GetRegister(b);

                //    alu.Execute(operation, a, aValue, bValue);
                //}
            }
        }

        public static IEnumerable<(ALU alu, ulong min, ulong max)> Execute(List<(Operation operation, string a, string b)> instructions)
        {
            var alus = new List<(ALU, ulong, ulong)>() { (new ALU(), 0, 0) };

            for(var i = 0; i < instructions.Count; i++)
            {
                Execute(instructions[i], ref alus);
            }

            return alus;
        }
    }

    [DayPart]
    public string ValidateSerials()
    {
        var alus = ALU.Execute(instructions);

        var validSerials = alus.Where(v => v.alu.Z == 0).ToList();

        var max = validSerials.Select(v => v.max).Max();
        var min = validSerials.Select(v => v.min).Min();

        return $"{min}, {max}";
    }
}
