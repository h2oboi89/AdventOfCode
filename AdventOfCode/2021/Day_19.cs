using AdventOfCode.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_19 : BaseDay
{
    private readonly List<Scanner> partScanners = new();
    private readonly List<Scanner> testScanners = new();

    public Day_19(string inputFile)
    {
        var scanners = new List<Scanner>();
        var id = string.Empty;
        var points = new List<Point3D>();

        var idRegex = new Regex(@"--- scanner (?<id>\d+) ---");

        foreach (var line in File.ReadLines(inputFile))
        {
            if (line.StartsWith("###"))
            {
                testScanners.AddRange(scanners);
                scanners.Clear();
                continue;
            }

            if (line.StartsWith("!!!"))
            {
                partScanners.AddRange(scanners);
                scanners.Clear();
                continue;
            }

            if (line.StartsWith("---"))
            {
                id = idRegex.Match(line).Groups["id"].Value;
                continue;
            }

            if (line.Length == 0)
            {
                scanners.Add(new Scanner(id, points));
                points.Clear();
                continue;
            }

            // default line is coordinate triplet
            var coords = line.Split(',').Select(int.Parse).ToArray();
            points.Add(new Point3D(coords[0], coords[1], coords[2]));
        }
    }

    private class Scanner
    {
        private readonly List<Point3D> _points = new();

        public IEnumerable<Point3D> Points
        {
            get { foreach (var point in _points) yield return point; }
        }

        public readonly string Id;

        public Scanner(string id, IEnumerable<Point3D> points)
        {
            Id = id;
            _points.AddRange(points);
        }

        private static Exception InvalidAxis(Axis axis) => new($"Invalid axis {axis}");

        private IEnumerable<Point3D> ForEachPoint(Func<Point3D, Point3D> transformFunc)
        {
            foreach (var point in Points)
            {
                yield return transformFunc(point);
            }
        }

        /// <summary>
        /// Rotates <see cref="Scanner.Points"/> clockwise by 90 degrees around <paramref name="axis"/>.
        /// </summary>
        /// <param name="axis"><see cref="Axis"/> to rotate around.</param>
        /// <returns>New <see cref="Scanner"/> with <see cref="Scanner.Points"/> rotated by 90 degrees around <paramref name="axis"/></returns>
        public Scanner Rotate(Axis axis)
        {
            var points = ForEachPoint(point => axis switch
            {
                Axis.X => new Point3D(point.X, point.Z, -point.Y),
                Axis.Y => new Point3D(-point.Z, point.Y, point.X),
                Axis.Z => new Point3D(point.Y, -point.X, point.Z),
                _ => throw InvalidAxis(axis)
            });

            return new Scanner(Id, points);
        }

        /// <summary>
        /// Translates <see cref="Scanner.Points"/> by specified distances.
        /// </summary>
        /// <param name="dx"><see cref="Axis.X"/> adjustment.</param>
        /// <param name="dy"><see cref="Axis.Y"/> adjustment.</param>
        /// <param name="dz"><see cref="Axis.Z"/> adjustment.</param>
        /// <returns>New <see cref="Scanner"/> with <see cref="Scanner.Points"/> adjusted by the specified amounts.</returns>
        public Scanner Translate(int dx, int dy, int dz) =>
            new(Id, ForEachPoint(point => new Point3D(point.X + dx, point.Y + dy, point.Z + dz)));

        public (int min, int max) Range(Axis axis)
        {
            var min = int.MaxValue; var max = int.MinValue;

            static (int, int) Check(int a, int min, int max)
            {
                if (a < min) min = a;
                if (a > max) max = a;

                return (min, max);
            }

            foreach (var point in Points)
            {
                (min, max) = axis switch
                {
                    Axis.X => Check(point.X, min, max),
                    Axis.Y => Check(point.Y, min, max),
                    Axis.Z => Check(point.Z, min, max),
                    _ => throw InvalidAxis(axis)
                };
            }

            return (min, max);
        }

        public Scanner Merge(Scanner other)
        {
            var points = new List<Point3D>();

            points.AddRange(Points);

            foreach (var point in other.Points)
            {
                if (!points.Contains(point))
                {
                    points.Add(point);
                }
            }

            return new Scanner($"{Id}-{other.Id}", points);
        }

        public int Compare(Scanner other)
        {
            var matches = 0;

            foreach (var point in Points)
            {
                if (other.Points.Contains(point))
                {
                    matches++;
                }
            }

            return matches;
        }

        public override string ToString() => $"{Id} ({string.Join(", ", Points)})";

        public static Scanner? Match(Scanner a, Scanner b)
        {
            Scanner? result = null;

            /*
             * 1. Create all permutations
             * 2. Eliminate duplicates
             * 3. for (n * m) scanners set origin and check matches
             *      if (matches >= 12) calculate offset and merge
             */

            return result;
        }
    }

    [DayTest]
    public TestResult ParseTest()
    {
        var testValues = new List<(int, int)>()
        {
            (testScanners.Count, 5),
            (partScanners.Count, 40)
        };

        return ExecuteTests(testValues, i => i);
    }
}
