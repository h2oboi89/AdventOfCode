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

        private static Exception InvalidAxis(Axis axis) => new Exception($"Invalid axis {axis}");

        public Scanner Flatten(Axis axis)
        {
            var points = new List<Point3D>();

            foreach (var point in Points)
            {
                points.Add(axis switch
                {
                    Axis.X => new Point3D(0, point.Y, point.Z),
                    Axis.Y => new Point3D(point.X, 0, point.Z),
                    Axis.Z => new Point3D(point.X, point.Y, 0),
                    _ => throw InvalidAxis(axis)
                });
            }

            return new Scanner(Id, points);
        }

        public Scanner Rotate(Axis axis)
        {
            var points = new List<Point3D>();

            foreach (var point in Points)
            {
                points.Add(axis switch
                {
                    Axis.X => new Point3D(point.X, point.Z, -point.Y),
                    Axis.Y => new Point3D(-point.Z, point.Y, point.X),
                    Axis.Z => new Point3D(point.Y, -point.X, point.Z),
                    _ => throw InvalidAxis(axis)
                });
            }

            return new Scanner(Id, points);
        }

        public Scanner Translate(Axis axis, int distance)
        {
            var points = new List<Point3D>();

            foreach (var point in Points)
            {
                points.Add(axis switch
                {
                    Axis.X => new Point3D(point.X + distance, point.Y, point.Z),
                    Axis.Y => new Point3D(point.X, point.Y + distance, point.Z),
                    Axis.Z => new Point3D(point.X, point.Y, point.Z + distance),
                    _ => throw InvalidAxis(axis)
                });
            }

            return new Scanner(Id, points);
        }

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
    }

    /*
        foreach of 3 axis for base matrix:
            foreach of 3 axis for test matrix:
                foreach of 4 rotations of test matrix:
                    // TODO: try to match at least 12 points and add to candidate group
                    // this will be 2D translation step
    
        foreach candidate in candidates:
            // TODO: unflatten and slide along axis until 12 points match or out of range
            // TODO: combine matrices if match and restart until only 1 matrix left
    */

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
