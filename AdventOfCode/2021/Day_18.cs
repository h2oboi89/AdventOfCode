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
        public Internal? Parent = null;

        public class Internal : Node
        {
            public Node Left = new External();
            public Node Right = new External();

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
                            node.Left.Parent = node;
                            break;
                        case ']':
                            return node;
                        case ',':
                            node.Right = Parse(input, ref index);
                            node.Right.Parent = node;
                            break;
                        default:
                            return new External(c - '0');
                    }
                }

                throw new ArgumentException("Invalid input", nameof(input));
            }

            public static Internal Add(Internal a, Internal b)
            {
                var c = new Internal
                {
                    Left = a,
                    Right = b
                };

                a.Parent = c;
                b.Parent = c;

                return c;
            }

            private static bool Reduce(Internal number)
            {
                // Check for explode
                var (exploder, left, right) = number.FindExploder(number, 0);

                if (exploder != null)
                {
                    var parent = exploder.Parent ?? new Internal();

                    if (left != null) left.Value += ((External)exploder.Left).Value;

                    if (right != null) right.Value += ((External)exploder.Right).Value;

                    var newNode = new External(0, parent);

                    if (parent.Left.Equals(exploder)) parent.Left = newNode;
                    if (parent.Right.Equals(exploder)) parent.Right = newNode;

                    return true;
                }

                // Check for split
                foreach (var node in Values(number))
                {
                    if (node.Value > 9)
                    {
                        var parent = node.Parent ?? new Internal();

                        var newNode = node.Split();
                        newNode.Parent = parent;

                        if (parent.Left.Equals(node)) parent.Left = newNode;
                        if (parent.Right.Equals(node)) parent.Right = newNode;

                        return true;
                    }
                }

                // Nothing to reduce
                return false;
            }

            public Internal Reduce()
            {
                while (Reduce(this)) { }

                return this;
            }

            private (Internal? node, External? left, External? right) FindExploder(Internal node, int depth)
            {
                External? FindLeft(External node)
                {
                    External? prev = null;

                    foreach (var value in Values(this))
                    {
                        if (prev == null && value.Equals(node)) return null;

                        if (prev != null && value.Equals(node)) return prev;

                        prev = value;
                    }

                    return null;
                }

                External? FindRight(External node)
                {
                    External? prev = null;

                    foreach (var value in Values(this))
                    {
                        if (prev != null && prev.Equals(node)) return value;

                        prev = value;
                    }

                    return null;
                }

                if (depth >= 4 && node.Left is External lNode && node.Right is External rNode)
                {
                    return (node, FindLeft(lNode), FindRight(rNode));
                }

                if (node.Left is Internal leftChild)
                {
                    var (n, l, r) = FindExploder(leftChild, depth + 1);

                    if (n != null) return (n, l, r);
                }

                if (node.Right is Internal rightChild)
                {
                    var (n, l, r) = FindExploder(rightChild, depth + 1);

                    if (n != null) return (n, l, r);
                }

                return (null, null, null);
            }

            private static IEnumerable<External> Values(Node node)
            {
                if (node is External external)
                {
                    yield return external;
                }

                if (node is Internal iNode)
                {
                    foreach (var child in Values(iNode.Left))
                    {
                        yield return child;
                    }

                    foreach (var child in Values(iNode.Right))
                    {
                        yield return child;
                    }
                }
            }

            protected override int Hash() => Left.Hash() ^ Right.Hash();

            public override string ToString() => $"[{Left},{Right}]";
        }

        public class External : Node
        {
            public int Value = -1;

            internal External() { }

            internal External(int value) { Value = value; }

            internal External(int value, Internal parent) : this(value) { Parent = parent; }

            public Internal Split()
            {
                var left = (int)Math.Floor(Value / 2.0);
                var right = (int)Math.Ceiling(Value / 2.0);

                var parent = new Internal();
                parent.Left = new External(left, parent);
                parent.Right = new External(right, parent);

                return parent;
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

    [Test]
    public static TestResult RulesExample()
    {
        var a = Node.Internal.Parse("[[[[4,3],4],4],[7,[[8,4],9]]]");
        var b = Node.Internal.Parse("[1,1]");

        var c = Node.Internal.Add(a, b).Reduce();

        var expected = Node.Internal.Parse("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");

        return ExecuteTest(expected.ToString(), () => c.ToString());
    }
}
