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
            long bValue = 0;

            if (operation == Operation.Input)
            {
                aValue = GetInput();
            }
            else
            {
                if (!long.TryParse(b, out bValue)) bValue = GetRegister(b);
            }

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

    [DayTest]
    public static TestResult NegationTest()
    {
        var input = new List<long>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var expected = input.Select(v => -v).ToList();

        var testValues = input.Zip(expected);

        var instructions = new List<(Operation, string, string)>()
        {
            (Operation.Input, "x", string.Empty),
            (Operation.Multiply, "x", "-1")
        };

        return ExecuteTests(testValues, (i) =>
        {
            var alu = new ALU(() => i);

            alu.Execute(instructions);

            return alu.X;
        });
    }

    [DayTest]
    public static TestResult ThreeTimesTest()
    {
        var testValues = new List<(Queue<long>, long)>() {
            (new Queue<long>(new long[] { 1, 1}), 0),
            (new Queue<long>(new long[] { 1, 3}), 1),
            (new Queue<long>(new long[] { 3, 1}), 0),
            (new Queue<long>(new long[] { 3, 9}), 1),
        };

        var instructions = new List<(Operation, string, string)>()
        {
            (Operation.Input, "z", string.Empty),
            (Operation.Input, "x", string.Empty),
            (Operation.Multiply, "z", "3"),
            (Operation.Equal, "z", "x")
        };

        return ExecuteTests(testValues, (i) =>
        {
            var alu = new ALU(() => i.Dequeue());

            alu.Execute(instructions);

            return alu.Z;
        });
    }

    [DayTest]
    public static TestResult BinaryTest()
    {
        var testValues = new List<(long, (long, long, long, long))>() {
            (0b0000, (0, 0, 0, 0)),
            (0b0001, (0, 0, 0, 1)),
            (0b0010, (0, 0, 1, 0)),
            (0b0100, (0, 1, 0, 0)),
            (0b1000, (1, 0, 0, 0)),
            (0b1111, (1, 1, 1, 1)),
        };

        var instructions = new List<(Operation, string, string)>()
        {
            (Operation.Input, "w", string.Empty),

            (Operation.Add, "z", "w"),
            (Operation.Modulo, "z", "2"),
            (Operation.Divide, "w", "2"),

            (Operation.Add, "y", "w"),
            (Operation.Modulo, "y", "2"),
            (Operation.Divide, "w", "2"),

            (Operation.Add, "x", "w"),
            (Operation.Modulo, "x", "2"),
            (Operation.Divide, "w", "2"),

            (Operation.Modulo, "w", "2"),
        };

        return ExecuteTests(testValues, (i) =>
        {
            var alu = new ALU(() => i);

            alu.Execute(instructions);

            return (alu.W, alu.X, alu.Y, alu.Z);
        });
    }
}
