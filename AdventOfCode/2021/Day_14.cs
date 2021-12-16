namespace AdventOfCode._2021;

internal class Day_14 : BaseDay
{
    private readonly Polymer input;
    private readonly Polymer testInput;

    public Day_14(string inputFile)
    {
        var polymer = string.Empty;
        var rules = new List<string>();

        var parseRules = false;

        static Polymer ParseInput(string polymer, List<string> rules)
        {
            var parsedRules = new Dictionary<string, char>();

            foreach (var rule in rules)
            {
                var parts = rule.Split(" -> ");

                parsedRules.Add(parts[0], parts[1][0]);
            }

            rules.Clear();

            return new Polymer(polymer, parsedRules);
        }

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                testInput = ParseInput(polymer, rules);
                parseRules = false;
                continue;
            }

            if (line.StartsWith("!"))
            {
                input = ParseInput(polymer, rules);
                parseRules = false;
                continue;
            }

            if (line.StartsWith("-"))
            {
                parseRules = true;
                continue;
            }

            if (parseRules)
            {
                rules.Add(line);
            }
            else
            {
                polymer = line;
            }
        }
    }

    private class Polymer
    {
        public readonly Dictionary<char, ulong> Elements = new();

        private readonly Dictionary<string, ulong> pairs = new();
        private readonly Dictionary<string, char> rules;

        public Polymer(string polymer, Dictionary<string, char> rules) : this(rules)
        {
            for (var i = 0; i < polymer.Length - 1; i++)
            {
                Elements.AddOrCreate(polymer[i]);

                pairs.AddOrCreate(polymer.Substring(i, 2));
            }

            Elements.AddOrCreate(polymer.Last());
        }

        private Polymer(Dictionary<string, char> rules) { this.rules = rules; }

        private Polymer(Dictionary<string, char> rules, Dictionary<char, ulong> elements) : this(rules)
        {
            foreach (var element in elements)
            {
                Elements[element.Key] = element.Value;
            }
        }

        public ulong Length => Elements.Select(kvp => kvp.Value).Sum();

        public Polymer Update()
        {
            var updatedPolymer = new Polymer(rules, Elements);

            foreach (var pair in pairs)
            {
                var newElement = rules[pair.Key];

                updatedPolymer.Elements.AddOrCreate(newElement, pair.Value);

                updatedPolymer.pairs.AddOrCreate($"{pair.Key[0]}{newElement}", pair.Value);
                updatedPolymer.pairs.AddOrCreate($"{newElement}{pair.Key[1]}", pair.Value);
            }

            return updatedPolymer;
        }
    }

    private static ulong Simulate(Polymer polymer, int steps)
    {
        for (var i = 0; i < steps; i++)
        {
            polymer = polymer.Update();
        }

        var min = ulong.MaxValue;
        var max = ulong.MinValue;

        foreach (var kvp in polymer.Elements)
        {
            if (kvp.Value > max) max = kvp.Value;
            if (kvp.Value < min) min = kvp.Value;
        }

        return max - min;
    }

    [Test]
    public TestResult Test1() => ExecuteTest((ulong)1588, () => Simulate(testInput, 10));

    [Test]
    public TestResult Test2() => ExecuteTest((ulong)2_188_189_693_529, () => Simulate(testInput, 40));

    [Part]
    public string Solve1() => $"{Simulate(input, 10)}";

    [Part]
    public string Solve2() => $"{Simulate(input, 40)}";
}
