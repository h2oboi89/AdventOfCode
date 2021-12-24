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

        private readonly Func<long> GetInput;

        public ALU(Func<long> getInput) { GetInput = getInput; }

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

        public ALU Clone() => new(GetInput) { W = W, X = X, Y = Y, Z = Z };

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not ALU other) return false;

            return W == other.W && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode() => (W, X, Y, Z).GetHashCode();

        public override string ToString() => $"W : {W}, X : {X}, Y : {Y}, Z :{Z}";

        private void Execute((Operation operation, string a, string b) instruction)
        {
            var (operation, a, b) = instruction;

            var aValue = GetRegister(a);

            if (!long.TryParse(b, out long bValue)) bValue = GetRegister(b);

            Execute(operation, a, aValue, bValue);
        }

        public void Execute(List<(Operation operation, string a, string b)> instructions)
        {
            for (var i = 0; i < instructions.Count; i++)
            {
                Execute(instructions[i]);
            }
        }
    }
}
