using AdventOfCode.Common;
using System.Text;

namespace AdventOfCode._2021;

internal class Day_20 : BaseDay
{
    private readonly (Image image, byte[] algorithm) testValues = new();
    private readonly (Image image, byte[] algorithm) partValues = new();

    public Day_20(string inputFile)
    {
        var algo = Array.Empty<byte>();

        var parsingImage = false;
        var row = 0;
        var pixels = new List<Pixel>();

        static byte ParsePixelValue(char c) => c switch
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
                pixels.AddRange(line.Select((c, i) => new Pixel(new Point.D2(i, row), ParsePixelValue(c))));
                row++;
            }
        }
    }

    private struct Pixel
    {
        public readonly Point.D2 Point;
        public readonly byte Value;

        public Pixel(Point.D2 point, byte value) { Point = point; Value = value; }
    }

    private class Image
    {
        private readonly Dictionary<Point.D2, byte> _pixels = [];

        public IEnumerable<Pixel> Pixels => _pixels.Select(kvp => new Pixel(kvp.Key, kvp.Value));

        public Image(IEnumerable<Pixel> pixels)
        {
            foreach (var pixel in pixels)
            {
                _pixels.Add(pixel.Point, pixel.Value);
            }
        }

        private static (Point.D2 min, Point.D2 max) FindMinMax(IEnumerable<Pixel> pixels)
        {
            static (int min, int max) MinMax(int i, int min, int max) => (Math.Min(min, i), Math.Max(max, i));

            var minX = int.MaxValue; var maxX = int.MinValue;
            var minY = int.MaxValue; var maxY = int.MinValue;

            foreach (var pixel in pixels)
            {
                (minX, maxX) = MinMax(pixel.Point.X, minX, maxX);
                (minY, maxY) = MinMax(pixel.Point.Y, minY, maxY);
            }

            return (new Point.D2(minX, minY), new Point.D2(maxX, maxY));
        }

        public (Point.D2 min, Point.D2 max) Dimensions => FindMinMax(Pixels);

        public int Lit => _pixels.Sum(p => p.Value);

        private static byte GetDefaultValue(byte[] algorithm, int step)
        {
            if (algorithm.First() == 0) return 0;

            return step % 2 == 0 ? algorithm.Last() : algorithm.First();
        }

        private static IEnumerable<Pixel> Trim(IEnumerable<Pixel> pixels, byte defaultValue)
        {
            var (min, max) = FindMinMax(pixels.Where(p => p.Value != defaultValue));

            foreach (var pixel in pixels)
            {
                if (pixel.Point.X >= min.X && pixel.Point.X <= max.X &&
                    pixel.Point.Y >= min.Y && pixel.Point.Y <= max.Y)
                {
                    yield return pixel;
                }
            }
        }

        public Image Enhance(byte[] algorithm, int step)
        {
            var defaultValue = GetDefaultValue(algorithm, step);

            var updated = new List<Pixel>();

            int GetValue(Point.D2 point) => _pixels.ContainsKey(point) ? _pixels[point] : defaultValue;

            var (min, max) = Dimensions;

            for (var x = min.X - 1; x <= max.X + 1; x++)
            {
                for (var y = min.Y - 1; y <= max.Y + 1; y++)
                {
                    var point = new Point.D2(x, y);

                    var value = 0;

                    foreach (var neighbor in point.GetNeighbors())
                    {
                        value <<= 1;

                        value |= GetValue(neighbor);
                    }

                    updated.Add(new(point, algorithm[value]));
                }
            }

            return new Image(Trim(updated, defaultValue));
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var (min, max) = Dimensions;

            var image = new char[max.Y - min.Y + 1, max.X - min.X + 1];

            foreach (var pixel in Pixels)
            {
                image[pixel.Point.Y - min.Y, pixel.Point.X - min.X] = pixel.Value == 0 ? '.' : '#';
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

    private static Image EnhanceImage(Image image, byte[] algorithm, int steps)
    {
        var temp = image;

        for (var i = 0; i < steps; i++)
        {
            temp = temp.Enhance(algorithm, i);
        }

        return temp;
    }

    [DayTest]
    public TestResult ParseAlgoTest()
    {
        return ExecuteTests(new List<(byte[], int)>() { (testValues.algorithm, 512), (partValues.algorithm, 512) }, (i) => i.Length);
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

    [DayTest]
    public TestResult Test2() => ExecuteTest(3351, () => EnhanceImage(testValues.image, testValues.algorithm, 50).Lit);

    [DayPart]
    public string Solve1() => $"{EnhanceImage(partValues.image, partValues.algorithm, 2).Lit}";

    [DayPart]
    public string Solve2() => $"{EnhanceImage(partValues.image, partValues.algorithm, 50).Lit}";
}
