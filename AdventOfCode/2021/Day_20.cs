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

        public (Point2D min, Point2D max) Dimensions
        {
            get
            {
                var minX = int.MaxValue; var maxX = int.MinValue;
                var minY = int.MaxValue; var maxY = int.MinValue;

                foreach (var (point, _) in Pixels)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.X > maxX) maxX = point.X;
                    if (point.Y < minY) minY = point.Y;
                    if (point.Y > maxY) maxY = point.Y;
                }

                return (new Point2D(minX, minY), new Point2D(maxX, maxY));
            }
        }

        public int Lit => Pixels.Sum(p => p.v);

        private static int GetDefaultValue(int[] algorithm, int step)
        {
            if (algorithm.First() == 0) return 0;

            return step % 2 == 0 ? algorithm.Last() : algorithm.First();
        }

        private Image Expand(int defaultValue)
        {
            var (min, max) = Dimensions;

            var points = new List<(Point2D p, int v)>();

            for (var x = min.X - 3; x <= max.X + 3; x++)
            {
                for (var y = min.Y - 3; y <= max.Y + 3; y++)
                {
                    points.Add((new Point2D(x, y), defaultValue));
                }
            }

            var image = new Image(points);

            foreach (var (p, v) in Pixels)
            {
                image._pixels[p] = v;
            }

            return image;
        }

        private Image Trim(int defaultValue)
        {
            var minX = int.MaxValue; var maxX = int.MinValue;
            var minY = int.MaxValue; var maxY = int.MinValue;

            foreach (var (point, v) in Pixels)
            {
                if (v != defaultValue)
                {
                    if (point.X < minX) minX = point.X;
                    if (point.X > maxX) maxX = point.X;
                    if (point.Y < minY) minY = point.Y;
                    if (point.Y > maxY) maxY = point.Y;
                }
            }

            var keep = new List<(Point2D, int)>();

            foreach (var (point, v) in Pixels)
            {
                if (point.X >= minX && point.X <= maxX &&
                    point.Y >= minY && point.Y <= maxY)
                {
                    keep.Add((point, v));
                }
            }

            return new Image(keep);
        }

        public Image Enhance(int[] algorithm, int step)
        {
            Console.WriteLine();
            Console.WriteLine(ToString());
            Console.WriteLine();

            var defaultValue = GetDefaultValue(algorithm, step);

            var expanded = Expand(defaultValue);

            Console.WriteLine();
            Console.WriteLine(expanded.ToString());
            Console.WriteLine();

            var updated = new List<(Point2D, int)>();

            int GetValue(Point2D point) => expanded._pixels.ContainsKey(point) ? expanded._pixels[point] : defaultValue;

            foreach (var (point, _) in expanded.Pixels)
            {
                var neighbors = point.GetNeighbors();

                var value = 0;

                foreach (var neighbor in neighbors)
                {
                    value <<= 1;

                    value |= GetValue(neighbor);
                }

                var v = algorithm[value];

                updated.Add((point, v));
            }

            var enhancedImage = new Image(updated).Trim(defaultValue);

            Console.WriteLine();
            Console.WriteLine(enhancedImage.ToString());
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine();

            return enhancedImage;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var (min, max) = Dimensions;

            var image = new char[max.Y - min.Y + 1, max.X - min.X + 1];

            foreach (var (p, v) in Pixels)
            {
                image[p.Y - min.Y, p.X - min.X] = v == 0 ? '.' : '#';
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
            var enhancedImage = temp.Enhance(algorithm, i);

            temp = enhancedImage;
        }

        return temp;
    }

    [DayTest]
    public TestResult ParseAlgoTest()
    {
        return ExecuteTests(new List<(int[], int)>() { (testValues.algorithm, 512), (partValues.algorithm, 512) }, (i) => i.Length);
    }

    [DayTest]
    public TestResult ParseImageTest()
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

        return ExecuteTest(expected, () => testValues.image.Enhance(testValues.algorithm, 0).ToString());
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
