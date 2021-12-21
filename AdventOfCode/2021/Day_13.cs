using AdventOfCode.Common;
using System.Text;

namespace AdventOfCode._2021;

internal class Day_13 : BaseDay
{
    private readonly (Page page, List<(Axis axis, int value)> folds) input = new();
    private readonly (Page page, List<(Axis axis, int value)> folds) testInput = new();

    public Day_13(string inputFile)
    {
        var parseDots = true;

        var points = new List<string>();
        var folds = new List<string>();

        static (Page, List<(Axis, int)>) ParseInput(List<string> points, List<string> folds)
        {
            var parsedPoints = new List<Point.D2>();
            var parsedFolds = new List<(Axis, int)>();

            foreach (var p in points)
            {
                var parts = p.Split(',').Select(p => int.Parse(p));
                parsedPoints.Add(new Point.D2(parts.First(), parts.Last()));
            }

            points.Clear();

            foreach (var f in folds)
            {
                var parts = f.Split(' ')[2].Split('=');
                var axis = parts[0][0] switch
                {
                    'x' => Axis.X,
                    'y' => Axis.Y,
                    _ => throw new Exception($"Invalid axis {parts[0][0]}")
                };

                parsedFolds.Add((axis, int.Parse(parts[1])));
            }

            folds.Clear();

            return (new Page(parsedPoints), parsedFolds);
        }

        foreach (var line in File.ReadAllLines(inputFile))
        {
            if (line.StartsWith("#"))
            {
                testInput = ParseInput(points, folds);
                parseDots = true;
                continue;
            }

            if (line.StartsWith("!"))
            {
                input = ParseInput(points, folds);
                parseDots = true;
                continue;
            }

            if (line.StartsWith("-"))
            {
                parseDots = false;
                continue;
            }

            if (parseDots)
            {
                points.Add(line);
            }
            else
            {
                folds.Add(line);
            }
        }
    }

    private class Page
    {
        private readonly int[,] _points;

        private const int SET = 1;

        public Page(List<Point.D2> points)
        {
            var maxX = 0;
            var maxY = 0;

            foreach (var p in points)
            {
                if (p.X > maxX) maxX = p.X;

                if (p.Y > maxY) maxY = p.Y;
            }

            _points = new int[maxY + 1, maxX + 1];

            SetPoints(points);
        }

        private void SetPoints(IEnumerable<Point.D2> points)
        {
            foreach (var p in points)
            {
                _points[p.Y, p.X] = SET;
            }
        }

        public int AreSet
        {
            get
            {
                var count = 0;

                _points.All((x, y) => { if (_points[y, x] == SET) count++; });

                return count;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var y = 0; y < _points.GetLength(0); y++)
            {
                for (var x = 0; x < _points.GetLength(1); x++)
                {
                    sb.Append(_points[y, x] == 0 ? " " : "#");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public Page Fold((Axis axis, int value) fold)
        {
            return fold.axis switch
            {
                Axis.Y => FoldAlongY(fold.value),
                Axis.X => FoldAlongX(fold.value),
                _ => throw new Exception($"Invalid fold axis {fold.axis}"),
            };
        }

        private Page FoldAlongY(int value)
        {
            var points = new List<Point.D2>();

            var yH = value * 2;

            _points.All((x, y) =>
            {
                if (_points[y, x] == SET)
                {
                    if (y > value)
                    {
                        points.Add(new Point.D2(x, yH - y));
                    }
                    else
                    {
                        points.Add(new Point.D2(x, y));
                    }
                }
            });

            return new Page(points);
        }

        private Page FoldAlongX(int value)
        {
            var points = new List<Point.D2>();

            var xH = value * 2;

            _points.All((x, y) =>
            {
                if (_points[y, x] == SET)
                {
                    if (x > value)
                    {
                        points.Add(new Point.D2(xH - x, y));
                    }
                    else
                    {
                        points.Add(new Point.D2(x, y));
                    }
                }
            });

            return new Page(points);
        }
    }

    [DayTest]
    public TestResult Test1() => ExecuteTest(17, () => testInput.page.Fold(testInput.folds[0]).AreSet);

    [DayTest]
    public TestResult Test2() => ExecuteTest(16, () =>
    {
        var page = testInput.page;

        foreach (var fold in testInput.folds)
        {
            page = page.Fold(fold);
        }

        //Console.WriteLine(page);

        return page.AreSet;
    });


    [DayPart]
    public string Solve1() => $"{input.page.Fold(input.folds[0]).AreSet}";

    [DayPart]
    public string Solve2()
    {
        var page = input.page;

        foreach (var fold in input.folds)
        {
            page = page.Fold(fold);
        }

        return $"{Environment.NewLine}{page}";
    }
}
