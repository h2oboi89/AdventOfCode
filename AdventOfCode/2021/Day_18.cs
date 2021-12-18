using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021;

internal class Day_18 : BaseDay
{
    private readonly List<string> input = new();
    public Day_18(string inputFile)
    {
        foreach (var line in File.ReadAllLines(inputFile))
        {
            input.Add(line);
        }
    }

    private abstract class Node
    {
        public class Internal : Node
        {
            public Node Left = new External(-1);
            public Node Right = new External(-1);

            public static Internal Parse(string input)
            {
                var index = 0;
                return (Internal)Parse(input, ref index);
            }

            private static Node Parse(string input, ref int index)
            {
                var node = new Internal();

                while (index < input.Length)
                {
                    var c = input[index++];

                    switch (c)
                    {
                        case '[':
                            node.Left = Parse(input, ref index);
                            break;
                        case ']':
                            return node;
                        case ',':
                            node.Right = Parse(input, ref index);
                            break;
                        default:
                            return new External(c - '0');
                    }
                }

                throw new ArgumentException("Invalid input", nameof(input));
            }

            public Internal Add(Internal other)
            {
                return new Internal
                {
                    Left = this,
                    Right = other
                };
            }

            public Internal Reduce()
            {
                throw new NotImplementedException();
            }

            private IEnumerable<(External, Internal, int)> Iterate(Node node, Internal? parent, int depth)
            {
                if (node is External external && parent != null)
                {
                    yield return (external, parent, depth);
                }

                if (node is Internal iNode)
                {
                    foreach (var child in Iterate(iNode.Left, iNode, depth + 1))
                    {
                        yield return child;
                    }

                    foreach (var child in Iterate(iNode.Right, iNode, depth + 1))
                    {
                        yield return child;
                    }
                }
            }

            private List<(External, Internal, int)> Iterate() => Iterate(this, null, 0).ToList();

            protected override int Hash() => Left.Hash() ^ Right.Hash();

            public override string ToString() => $"[{Left},{Right}]";
        }

        public class External : Node
        {
            public readonly int Value;

            internal External(int value) { Value = value; }

            public Internal Split()
            {
                var left = (int)Math.Floor(Value / 2.0);
                var right = (int)Math.Ceiling(Value / 2.0);

                return new Internal
                {
                    Left = new External(left),
                    Right = new External(right)
                };
            }

            protected override int Hash() => Value.GetHashCode();

            public override string ToString() => Value.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            return ReferenceEquals(this, obj);
        }

        protected abstract int Hash();

        public override int GetHashCode() => Hash();
    }

    [Test]
    public static TestResult ParseTest()
    {
        var input = new List<string>()
        {
            "[1,2]",
            "[[1,2],3]",
            "[9,[8,7]]",
            "[[1,9],[8,5]]",
            "[[[[1,2],[3,4]],[[5,6],[7,8]]],9]",
            "[[[9,[3,8]],[[0,9],6]],[[[3,7],[4,9]],3]]",
            "[[[[1,3],[5,3]],[[1,3],[8,7]]],[[[4,9],[6,9]],[[8,2],[7,3]]]]",
        };

        var testValues = input.Select(i => (i, i));

        return ExecuteTests(testValues, (i) => Node.Internal.Parse(i).ToString());
    }
}
