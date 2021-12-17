using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021;

internal class Day_15 : BaseDay
{
    private readonly Node[,] input = new Node[0, 0];
    private readonly Node[,] testInput = new Node[0, 0];

    public Day_15(string inputFile)
    {
        var values = new List<int[]>();
        var gridSize = 0;

        static Node[,] ProcessInput(List<int[]> input)
        {
            var processedInput = new Node[input.Count, input[0].Length];

            processedInput.All((x, y) => processedInput[y, x] = new Node(new Point(x, y), input[y][x]));

            input.Clear();

            return processedInput;
        }

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                testInput = ProcessInput(values);
                gridSize = 0;
                continue;
            }

            if (line.StartsWith("!"))
            {
                input = ProcessInput(values);
                gridSize = 0;
                continue;
            }

            if (gridSize == 0)
            {
                gridSize = line.Length;
            }

            values.Add(line.Select(c => c - 48).ToArray());
        }
    }

    private class Node
    {
        public readonly Point Point;
        public readonly int Risk;
        public int TotalRisk = int.MaxValue;
        public Node? Previous = null;

        public Node(Point point, int risk)
        {
            Point = point;
            Risk = risk;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Node other) return false;

            if (other.Risk != Risk) return false;

            return other.Point.Equals(Point);
        }

        public override int GetHashCode() => Point.GetHashCode() * Risk.GetHashCode();

        public override string ToString() => $"[ {Point} {Risk} {TotalRisk} ]";
    }

    private static Node[,] Expand(Node[,] nodes)
    {
        const int expansionFactor = 5;

        var expandedNodes = new Node[nodes.GetLength(0) * expansionFactor, nodes.GetLength(1) * expansionFactor];

        static int UpdateRisk(int risk, int increase)
        {
            var newRisk = risk + increase;

            if (newRisk < 10) return newRisk;

            return newRisk % 9;
        }

        for (var y_expand = 0; y_expand < expansionFactor; y_expand++)
        {
            for (var x_expand = 0; x_expand < expansionFactor; x_expand++)
            {
                var valueIncrease = x_expand + y_expand;
                var y_increase = y_expand * nodes.GetLength(0);
                var x_increase = x_expand * nodes.GetLength(1);

                foreach (var node in nodes.Each())
                {
                    var x_new = x_increase + node.Point.X;
                    var y_new = y_increase + node.Point.Y;
                    var point_new = new Point(x_new, y_new);
                    var risk_new = UpdateRisk(node.Risk, valueIncrease);

                    expandedNodes[y_new, x_new] = new Node(point_new, risk_new);
                }
            }
        }

        return expandedNodes;
    }

    private static Node[,] Clone(Node[,] nodes) => nodes.Clone((n) => new Node(n.Point, n.Risk));

    private static IEnumerable<Node> DijkstraShortestPath(Node[,] nodes)
    {
        var start = nodes.First();
        start.TotalRisk = 0;

        var queue = new PriorityQueue<Node, int>();

        queue.Enqueue(start, start.Risk);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var neighbor in nodes.GetNeighbors(node.Point, false))
            {
                var risk = node.TotalRisk + neighbor.Risk;

                if (risk < neighbor.TotalRisk)
                {
                    neighbor.TotalRisk = risk;
                    neighbor.Previous = node;

                    queue.Enqueue(neighbor, risk);
                }
            }
        }

        var current = nodes.Last();

        var shortestPath = new List<Node>() { current };

        while (current.Previous != null)
        {
            current = current.Previous;

            shortestPath.Add(current);
        }

        shortestPath.Reverse();

        return shortestPath;
    }

    [Test]
    public TestResult Test1() => ExecuteTest(40, () => DijkstraShortestPath(Clone(testInput)).Last().TotalRisk);

    [Test]
    public TestResult Test2() => ExecuteTest(315, () => DijkstraShortestPath(Expand(testInput)).Last().TotalRisk);

    [Part]
    public string Solve1() => $"{DijkstraShortestPath(Clone(input)).Last().TotalRisk}";

    [Part]
    public string Solve2() => $"{DijkstraShortestPath(Expand(input)).Last().TotalRisk}";
}
