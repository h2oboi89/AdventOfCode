namespace AdventOfCode._2021;

internal class Day_12 : BaseDay
{
    private readonly Dictionary<string, Node> input = new();
    private readonly List<Dictionary<string, Node>> testInputs = new();

    private const string START = "start", END = "end";
    private const int UNLIMITED = -1;

    public Day_12(string inputFile)
    {
        static Dictionary<string, Node> ProcessInput(List<(string a, string b)> connections)
        {
            var nodes = new Dictionary<string, Node>();

            var unique = new HashSet<string>();

            foreach (var (a, b) in connections)
            {
                unique.Add(a);
                unique.Add(b);
            }

            foreach (var id in unique)
            {
                nodes.Add(id, new Node(id));
            }

            foreach (var (a, b) in connections)
            {
                nodes[a].Connections.Add(nodes[b]);
                nodes[b].Connections.Add(nodes[a]);
            }

            return nodes;
        }

        var values = new List<(string, string)>();

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                testInputs.Add(ProcessInput(values));
                values.Clear();
                continue;
            }

            if (line.StartsWith("!"))
            {
                input = ProcessInput(values);
                values.Clear();
                continue;
            }

            var parts = line.Split('-');

            values.Add((parts[0], parts[1]));
        }
    }

    private enum Size { Small, Large }

    private class Node
    {
        public readonly string Id;

        public List<Node> Connections = new();

        public readonly Size Size;

        public Node(string id)
        {
            Id = id;

            Size = Id.Any(char.IsLower) ? Size.Small : Size.Large;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Node other) return false;

            if (other.Id != Id) return false;

            if (other.Connections.Count != Connections.Count) return false;

            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            var connectionHash = 13;

            foreach (var node in Connections)
            {
                connectionHash *= node.Id.GetHashCode();
            }

            return connectionHash;
        }

        public override string ToString() => $"{Id}";
    }

    private static List<Node> Clone(List<Node> nodes)
    {
        var clone = new List<Node>();

        clone.AddRange(nodes);

        return clone;
    }

    private static List<Dictionary<string, int>> GenerateVisitLimits(Dictionary<string, Node> nodes)
    {
        var visitLimits = new List<Dictionary<string, int>>();

        var baseLimits = new Dictionary<string, int>();

        foreach (var kvp in nodes)
        {
            baseLimits.Add(kvp.Key, 1);

            if (nodes[kvp.Key].Size == Size.Large)
            {
                baseLimits[kvp.Key] = UNLIMITED;
            }
        }

        foreach (var kvp in nodes.Where(kvp => kvp.Value.Size == Size.Small
             && kvp.Value.Id != START && kvp.Value.Id != END))
        {
            var clone = new Dictionary<string, int>(baseLimits);

            clone[kvp.Key]++;

            visitLimits.Add(clone);
        }

        return visitLimits;
    }

    private static List<List<Node>> GeneratePaths(Dictionary<string, Node> nodes, IEnumerable<Dictionary<string, int>> visitLimits)
    {
        var paths = new Dictionary<string, List<Node>>();

        foreach (var visitLimit in visitLimits)
        {
            var p = GeneratePaths(nodes, visitLimit);

            foreach (var path in p)
            {
                paths[string.Join("->", path.Select(n => n.Id))] = path;
            }
        }

        return paths.Select(kvp => kvp.Value).ToList();

    }

    private static List<List<Node>> GeneratePaths(Dictionary<string, Node> nodes, Dictionary<string, int> visitLimits)
    {
        var paths = new List<List<Node>>();

        var processing = new Queue<(List<Node>, Node, Dictionary<string, int>)>();

        processing.Enqueue((new List<Node>(), nodes[START], visitLimits));

        while (processing.Any())
        {
            var (path, current, limits) = processing.Dequeue();

            if (path.Contains(current) && limits[current.Id] == 0)
            {
                continue;
            }

            path.Add(current);
            if (limits[current.Id] != UNLIMITED)
            {
                limits[current.Id]--;
            }

            if (current.Id == END)
            {
                paths.Add(path);
                continue;
            }

            foreach (var node in current.Connections)
            {
                processing.Enqueue((Clone(path), node, new Dictionary<string, int>(limits)));
            }
        }

        return paths;
    }

    private static List<List<Node>> GeneratePaths(Dictionary<string, Node> nodes)
    {
        var paths = new List<List<Node>>();

        var processing = new Queue<(List<Node> path, Node node)>();

        processing.Enqueue((new List<Node>(), nodes[START]));

        while (processing.Any())
        {
            var (path, current) = processing.Dequeue();

            if (path.Contains(current) && current.Size == Size.Small)
            {
                continue;
            }

            path.Add(current);

            if (current.Id == END)
            {
                paths.Add(path);
                continue;
            }

            foreach (var node in current.Connections)
            {
                processing.Enqueue((Clone(path), node));
            }
        }

        return paths;
    }

    [Test]
    public TestResult Test1()
    {
        var testInputAndResults = testInputs.Zip(new List<object> { 10, 19, 226 }, (input, expected) => (input, expected));

        return ExecuteTests(testInputAndResults, (nodes) => GeneratePaths(nodes).Count);
    }

    [Test]
    public TestResult Test2()
    {
        var testInputAndResults = testInputs.Zip(new List<object> { 36, 103, 3509 }, (input, expected) => (input, expected));

        return ExecuteTests(testInputAndResults, (nodes) =>
            GeneratePaths(nodes, GenerateVisitLimits(nodes)).Count);
    }

    [Part]
    public string Solve1() => $"{GeneratePaths(input).Count}";

    [Part]
    public string Solve2() => $"{GeneratePaths(input, GenerateVisitLimits(input)).Count}";
}
