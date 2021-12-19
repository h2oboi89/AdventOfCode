using AdventOfCode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
