namespace AdventOfCode._2021;

internal class Day_18 : BaseDay
{
    private readonly List<string> testInput = new();
    private readonly List<string> input = new();

    public Day_18(string inputFile)
    {
        var test = true;
        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                test = false;
                continue;
            }

            if (test) testInput.Add(line);
            else input.Add(line);
        }
    }

    private abstract class Node
    {
        public Internal? Parent = null;

        public class Internal : Node
        {
            internal Node Left = new External();
            internal Node Right = new External();

            internal Internal() { }

            internal Internal(Node left, Node right)
            {
                Left = left; left.Parent = this;
                Right = right; right.Parent = this;
            }

            public static IEnumerable<Internal> Parse(IEnumerable<string> inputs) => inputs.Select(i => Parse(i));

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

            public static Internal Add(Internal a, Internal b) => new Internal(a.Clone(), b.Clone()).Reduce();

            public static Internal Add(IEnumerable<Internal> numbers)
            {
                var stack = new Stack<Internal>(numbers.Reverse());

                while (stack.Count > 1)
                {
                    stack.Push(Add(stack.Pop(), stack.Pop()));
                }

                return stack.Pop();
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

            private Internal Reduce()
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
                if (node is External eNode)
                {
                    yield return eNode;
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

            private static int CalculateMagnitude(Node node)
            {
                if (node is External eNode)
                {
                    return eNode.Value;
                }

                if (node is Internal iNode)
                {
                    var left = 3 * CalculateMagnitude(iNode.Left);
                    var right = 2 * CalculateMagnitude(iNode.Right);

                    return left + right;
                }

                return 0;
            }

            public int Magnitude => CalculateMagnitude(this);

            private static Node Clone(Node node)
            {
                if (node is External eNode)
                {
                    return new External(eNode.Value);
                }

                if (node is Internal iNode)
                {
                    return new Internal(Clone(iNode.Left), Clone(iNode.Right));
                }

                return new Internal();
            }

            private Internal Clone() => (Internal)Clone(this);

            protected override int Hash() => Left.Hash() ^ Right.Hash();

            public override string ToString() => $"[{Left},{Right}]";
        }

        public class External : Node
        {
            internal int Value = -1;

            internal External() { }

            internal External(int value) { Value = value; }

            internal External(int value, Internal parent) : this(value) { Parent = parent; }

            public Internal Split()
            {
                var left = (int)Math.Floor(Value / 2.0);
                var right = (int)Math.Ceiling(Value / 2.0);

                return new Internal(new External(left), new External(right));
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

    private static (Node.Internal a, Node.Internal b, Node.Internal c, int max) FindLargestMagnitude(IEnumerable<string> input)
    {
        var (ma, mb, mc, maxMagnitude) = (new Node.Internal(), new Node.Internal(), new Node.Internal(), 0);

        void Add(Node.Internal a, Node.Internal b)
        {
            var c = Node.Internal.Add(a, b);
            var magnitude = c.Magnitude;

            if (magnitude > maxMagnitude)
            {
                ma = a; mb = b; mc = c; maxMagnitude = magnitude;
            }
        }

        foreach (var (a, b) in Node.Internal.Parse(input).ToList().CartesianProduct())
        {
            if (a.Equals(b)) continue;

            Add(a, b);
            Add(b, a);
        }

        return (ma, mb, mc, maxMagnitude);
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

        var c = Node.Internal.Add(a, b);

        var expected = Node.Internal.Parse("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");

        return ExecuteTest(expected.ToString(), () => c.ToString());
    }

    [Test]
    public static TestResult MultipleAddTest()
    {
        var testValues = new List<(string, string)>()
        {
            ("[1,1];[2,2];[3,3];[4,4]", "[[[[1,1],[2,2]],[3,3]],[4,4]]"),
            ("[1,1];[2,2];[3,3];[4,4];[5,5]", "[[[[3,0],[5,3]],[4,4]],[5,5]]"),
            ("[1,1];[2,2];[3,3];[4,4];[5,5];[6,6]", "[[[[5,0],[7,4]],[5,5]],[6,6]]"),
        };

        return ExecuteTests(testValues, (line) => Node.Internal.Add(Node.Internal.Parse(line.Split(';'))).ToString());
    }

    [Test]
    public static TestResult BigAddTest()
    {
        var inputs = new List<string>()
        {
            "[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]",
            "[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]",
            "[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]",
            "[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]",
            "[7,[5,[[3,8],[1,4]]]]",
            "[[2,[2,2]],[8,[8,1]]]",
            "[2,9]",
            "[1,[[[9,3],9],[[9,0],[0,7]]]]",
            "[[[5,[7,4]],7],1]",
            "[[[[4,2],2],6],[8,7]]"
        };

        var expected = "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]";

        return ExecuteTest(expected, () => Node.Internal.Add(Node.Internal.Parse(inputs)).ToString());
    }

    [Test]
    public static TestResult MagnitudeTest()
    {
        var testValues = new List<(string, int)>
        {
            ("[[1,2],[[3,4],5]]", 143),
            ("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384),
            ("[[[[1,1],[2,2]],[3,3]],[4,4]]",  445),
            ("[[[[3,0],[5,3]],[4,4]],[5,5]]", 791),
            ("[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137),
            ("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488),
        };

        return ExecuteTests(testValues, (i) => Node.Internal.Parse(i).Magnitude);
    }

    [Test]
    public TestResult Test1()
    {
        var expected = ("[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]", 4140);

        return ExecuteTest(expected, () =>
        {
            var result = Node.Internal.Add(Node.Internal.Parse(testInput));

            return (result.ToString(), result.Magnitude);
        });
    }

    [Test]
    public TestResult Test2()
    {
        var expected = (
            "[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]",
            "[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]",
            "[[[[7,8],[6,6]],[[6,0],[7,7]]],[[[7,8],[8,8]],[[7,9],[0,6]]]]",
            3993
        );

        return ExecuteTest(expected, () =>
        {
            var (a, b, c, max) = FindLargestMagnitude(testInput);

            return (a.ToString(), b.ToString(), c.ToString(), max);
        });
    }

    [Part]
    public string Solve1() => $"{Node.Internal.Add(Node.Internal.Parse(input)).Magnitude}";

    [Part]
    public string Solve2() => $"{FindLargestMagnitude(input).max}";
}
