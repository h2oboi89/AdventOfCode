using System.Text.RegularExpressions;

namespace AdventOfCode._2015;

internal class Day_07 : BaseDay
{
    private enum Operation
    {
        Unknown,
        Label,
        Value,
        Not,
        And,
        Or,
        LeftShift,
        RightShift,
    }

    private class Node
    {
        public readonly string Name;

        public Operation Operation = Operation.Unknown;

        public ushort Value { get; private set; }

        public bool IsSet { get; private set; }

        private readonly List<Node> Children = new();

        public Node(string name) { Name = name; }

        public void AddChild(Node node)
        {
            var exists = false;
            foreach (var child in Children)
            {
                if (child.Name == node.Name)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                Children.Add(node);
            }
        }

        public void Execute()
        {
            if (IsSet) return;

            if (Children.All(c => c.IsSet))
            {
                switch (Operation)
                {
                    case Operation.Label:
                        Value = Children.First().Value;
                        break;
                    case Operation.Not:
                        Value = (ushort)~Children.First().Value;
                        break;
                    case Operation.And:
                        Value = (ushort)(Children.First().Value & Children.Last().Value);
                        break;
                    case Operation.Or:
                        Value = (ushort)(Children.First().Value | Children.Last().Value);
                        break;
                    case Operation.LeftShift:
                        Value = (ushort)(Children.First().Value << Children.Last().Value);
                        break;
                    case Operation.RightShift:
                        Value = (ushort)(Children.First().Value >> Children.Last().Value);
                        break;
                }

                IsSet = true;
            }
        }

        public void Reset() => IsSet = false;

        public void Set(ushort value) { IsSet = true; Value = value; }

        private static Node GetNode(Dictionary<string, Node> circuit, string name, Operation op)
        {
            if (!circuit.ContainsKey(name))
            {
                circuit[name] = new Node(name);
            }

            var node = circuit[name];
            node.Operation = op;

            return node;
        }

        public static Node Create(Dictionary<string, Node> circuit, string name, Node? child = null)
        {
            var node = GetNode(circuit, name, Operation.Label);

            if (ushort.TryParse(node.Name, out var sourceVal))
            {
                node.Value = sourceVal;
                node.Operation = Operation.Value;
                node.IsSet = true;
            }

            if (child != null)
            {
                node.AddChild(child);
            }

            return node;
        }

        private static int opId = 0;

        public static Node CreateOp(Dictionary<string, Node> circuit, string name, IEnumerable<Node> children)
        {
            static Operation Op(string op) => op switch
            {
                "NOT" => Operation.Not,
                "AND" => Operation.And,
                "OR" => Operation.Or,
                "LSHIFT" => Operation.LeftShift,
                "RSHIFT" => Operation.RightShift,
                _ => Operation.Unknown,
            };

            var node = GetNode(circuit, $"{name}-{opId++}", Op(name));

            foreach (var child in children)
            {
                node.AddChild(child);
            }

            return node;
        }
    }

    private readonly IEnumerable<string> instructions;
    private readonly IEnumerable<string> testInstructions = new List<string>()
    {
        "123 -> x",
        "456 -> y",
        "x AND y -> d",
        "x OR y -> e",
        "x LSHIFT 2 -> f",
        "y RSHIFT 2 -> g",
        "NOT x -> h",
        "NOT y -> i",
    };

    public Day_07(string inputFile)
    {
        var values = new List<string>();

        foreach (var line in File.ReadAllLines(inputFile))
        {
            values.Add(line);
        }

        instructions = values;
    }

    private static readonly Regex nopRegex = new(@"^(?<source>\w+)\s->\s(?<target>\w+)$");
    private static readonly Regex binaryNodeRegex = new(@"^(?<left>\w+)\s(?<op>AND|OR|LSHIFT|RSHIFT)\s(?<right>\w+)\s->\s(?<target>\w+)$");
    private static readonly Regex unaryNodeRegex = new(@"^(?<op>NOT)\s(?<source>\w+)\s->\s(?<target>\w+)$");

    private static Dictionary<string, Node> Assemble(IEnumerable<string> instructions)
    {
        var circuit = new Dictionary<string, Node>();

        foreach (var line in instructions)
        {
            var match = nopRegex.Match(line);

            if (match.Success)
            {
                Node.Create(circuit, match.Groups["target"].Value,
                    Node.Create(circuit, match.Groups["source"].Value));

                continue;
            }

            match = binaryNodeRegex.Match(line);

            if (match.Success)
            {
                Node.Create(circuit, match.Groups["target"].Value,
                    Node.CreateOp(circuit, match.Groups["op"].Value, new Node[] {
                        Node.Create(circuit,match.Groups["left"].Value),
                        Node.Create(circuit, match.Groups["right"].Value)
                    }));

                continue;
            }

            match = unaryNodeRegex.Match(line);

            if (match.Success)
            {
                Node.Create(circuit, match.Groups["target"].Value,
                    Node.CreateOp(circuit, match.Groups["op"].Value, new Node[] {
                        Node.Create(circuit, match.Groups["source"].Value)
                    }));

                continue;
            }
        }

        return circuit;
    }

    private static Dictionary<string, Node> Simulate(Dictionary<string, Node> circuit)
    {
        while (!circuit.All(kvp => kvp.Value.IsSet))
        {
            foreach (var kvp in circuit)
            {
                kvp.Value.Execute();
            }
        }

        return circuit;
    }

    [Test]
    public TestResult Test1()
    {
        var expected = new List<(string, object)>
        {
            ("d", (ushort)72),
            ("e", (ushort)507),
            ("f", (ushort)492),
            ("g", (ushort)114),
            ("h", (ushort)65412),
            ("i", (ushort)65079),
            ("x", (ushort)123),
            ("y", (ushort)456),
        };

        var circuit = Simulate(Assemble(testInstructions));

        return ExecuteTests(expected, (name) => circuit[name].Value);
    }

    [Part]
    public string Solve1()
    {
        return $"{Simulate(Assemble(instructions))["a"].Value}";
    }

    [Part]
    public string Solve2()
    {
        var circuit = Simulate(Assemble(instructions));

        var a = circuit["a"].Value;

        foreach (var kvp in circuit)
        {
            kvp.Value.Reset();

            if (kvp.Value.Name == "b")
            {
                kvp.Value.Set(a);
            }
        }

        circuit = Simulate(circuit);

        return $"{circuit["a"].Value}";
    }
}

