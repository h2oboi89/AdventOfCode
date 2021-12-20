using AdventOfCode.Common;
using System.Text;

namespace AdventOfCode._2021;

internal class Day_20 : BaseDay
{
    private readonly (Image image, int[] algorithm) testValues = new();
    private readonly (Image image, int[] algorithm) partValues = new();

    public Day_20(string inputFile)
    {
        var algo = Array.Empty<int>();

        var parsingImage = false;
        var row = 0;
        var pixels = new List<(Point2D point, int v)>();

        static int ParsePixelValue(char c) => c switch
        {
            '.' => 0,
            '#' => 1,
            _ => throw new Exception("borked")
        };

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("&&&"))
            {
                testValues = (new Image(pixels), algo);
                parsingImage = false;
                row = 0;
                pixels.Clear();
                continue;
            }

            if (line.StartsWith("!!!"))
            {
                partValues = (new Image(pixels), algo);
                parsingImage = false;
                row = 0;
                pixels.Clear();
                continue;
            }

            if (line.Length == 0)
            {
                parsingImage = true;
                continue;
            }

            if (!parsingImage)
            {
                algo = line.Select(ParsePixelValue).ToArray();
            }
            else
            {
                pixels.AddRange(line.Select((c, i) => (new Point2D(i, row), ParsePixelValue(c))));
                row++;
            }
        }
    }

    private class Image
    {
        private readonly Dictionary<Point2D, int> _pixels = new();

        public IEnumerable<(Point2D p, int v)> Pixels => _pixels.Select(kvp => (kvp.Key, kvp.Value));

        public Image(IEnumerable<(Point2D, int)> pixels)
        {
            foreach (var (point, v) in pixels)
            {
                _pixels.Add(point, v);
            }
        }

        public Point2D Dimensions => Pixels.Select(pixel => (pixel.p, pixel.p.X + pixel.p.Y)).OrderByDescending(p => p.Item2).First().p;

        public int Lit => Pixels.Sum(p => p.v);

        public Image Expand()
        {
            var dimensions = Dimensions;
            var newDimensions = new Point2D(dimensions.X + 3, dimensions.Y + 3);

            var newImagePixels = new Dictionary<Point2D, int>();

            for (var y = 0; y < newDimensions.Y; y++)
            {
                for (var x = 0; x < newDimensions.X; x++)
                {
                    newImagePixels.Add(new Point2D(x, y), 0); // FIXME: need to smart expand
                }
            }

            foreach (var (p, v) in Pixels.Select(pixel => (new Point2D(pixel.p.X + 1, pixel.p.Y + 1), pixel.v)))
            {
                newImagePixels[p] = v;
            }

            return new Image(newImagePixels.Select(kvp => (kvp.Key, kvp.Value)));
        }

        public Image Enhance(int[] algorithm)
        {
            var expanded = Expand();

            var updated = expanded.Clone();

            int GetValue(Point2D point)
            {
                if (expanded._pixels.ContainsKey(point)) return expanded._pixels[point];

                return 0; // TODO: need to smart get expanded value
            }

            foreach (var (point, _) in expanded.Pixels)
            {
                var neighbors = point.GetNeighbors();

                var value = 0;

                foreach (var neighbor in neighbors)
                {
                    value <<= 1;

                    value |= GetValue(neighbor);
                }

                updated._pixels[point] = algorithm[value];
            }

            return updated;
        }

        private Image Clone() => new(Pixels);

        public override string ToString()
        {
            var sb = new StringBuilder();

            var dimensions = Dimensions;

            var image = new char[dimensions.Y + 1, dimensions.X + 1];

            foreach (var (p, v) in Pixels)
            {
                image[p.Y, p.X] = v == 0 ? '.' : '#';
            }

            for (var y = 0; y < image.GetLength(0); y++)
            {
                for (var x = 0; x < image.GetLength(1); x++)
                {
                    sb.Append(image[y, x]);
                }

                sb.AppendLine();
            }

            return sb.ToString().Trim();
        }
    }

    private static Image EnhanceImage(Image image, int[] algorithm, int steps)
    {
        var temp = image;

        for (var i = 0; i < steps; i++)
        {
            temp = temp.Enhance(algorithm);
        }

        return temp;
    }

    [DayTest]
    public TestResult ParseTest()
    {
        var lines = new List<string>()
        {
            "#..#.",
            "#....",
            "##..#",
            "..#..",
            "..###",
        };

        var expected = string.Join(Environment.NewLine, lines);

        return ExecuteTest(expected, () => testValues.image.ToString());
    }

    [DayTest]
    public TestResult ExpandTest()
    {
        var lines = new List<string>()
        {
            ".......",
            ".#..#..",
            ".#.....",
            ".##..#.",
            "...#...",
            "...###.",
            ".......",
        };

        var expected = string.Join(Environment.NewLine, lines);

        return ExecuteTest(expected, () => testValues.image.Expand().ToString());
    }

    [DayTest]
    public TestResult EnhanceTest()
    {
        var lines = new List<string>()
        {
            ".##.##.",
            "#..#.#.",
            "##.#..#",
            "####..#",
            ".#..##.",
            "..##..#",
            "...#.#.",
        };

        var expected = string.Join(Environment.NewLine, lines);

        return ExecuteTest(expected, () => testValues.image.Enhance(testValues.algorithm).ToString());
    }

    [DayTest]
    public TestResult EnhanceImageTest()
    {
        var lines = new List<string>()
        {
            ".......#.",
            ".#..#.#..",
            "#.#...###",
            "#...##.#.",
            "#.....#.#",
            ".#.#####.",
            "..#.#####",
            "...##.##.",
            "....###..",
        };

        var expected = string.Join(Environment.NewLine, lines);

        return ExecuteTest(expected, () => EnhanceImage(testValues.image, testValues.algorithm, 2).ToString());
    }



    [DayTest]
    public TestResult Test1() => ExecuteTest(35, () => EnhanceImage(testValues.image, testValues.algorithm, 2).Lit);

    [DayPart]
    public string Solve1()
    {
        var enhanced = EnhanceImage(partValues.image, partValues.algorithm, 2);

        //Console.WriteLine(enhanced);
        //Console.WriteLine(enhanced.Dimensions);
        return $"{enhanced.Lit}";
    }
}
