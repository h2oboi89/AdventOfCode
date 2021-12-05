using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021;

internal class Day_05 : BaseDay
{
    private class Point
    {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y) { X = x; Y = y; }

        public override string ToString() => $"( {X}, {Y} )";
    }

    private class PointPair
    {
        public readonly Point Start;
        public readonly Point End;

        public PointPair(Point start, Point end) { Start = start; End = end; }

        public bool IsDiagonal => Start.X != End.X && Start.Y != End.Y;

        public override string ToString() => $"[ {Start}, {End} ] {IsDiagonal}";
    }

    private class DangerZone
    {
        public readonly int[,] Points;

        public DangerZone(IEnumerable<int> values, int dimension)
        {
            Points = new int[dimension, dimension];

            // TODO: process values
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var dimension = Points.GetLength(0);

            for (var x = 0; x < dimension; x++)
            {
                var points = new List<int>();

                for (var y = 0; y < dimension; y++)
                {
                    points.Add(Points[x, y]);
                }

                sb.AppendLine(string.Join(" ", points.Select(p => p.ToString("D" + dimension))));
            }

            return sb.ToString();
        }
    }

    private readonly (int dimension, IEnumerable<PointPair> values) PartValues;
    private readonly (int dimension, IEnumerable<PointPair> values) TestValues;

    public Day_05(string intputFile)
    {
        var values = new List<PointPair>();
        var isTest = true;
        var max = 0;


        void CheckMax(int x, int y)
        {
            if (x > max) max = x;
            if (y > max) max = y;
        }

        foreach (var line in File.ReadAllLines(intputFile))
        {
            if (line.StartsWith("#")) // marks end of input section (test and input)
            {
                if (isTest)
                {
                    TestValues = (max.NearestPowerOfTen(), values);
                }
                else
                {
                    PartValues = (max.NearestPowerOfTen(), values);
                }

                max = 0;

                values = new List<PointPair>();
                isTest = false;
                continue;
            }

            // parse pair of points
            var parts = line.Split("->").Select(p => p.Trim());

            var points = new List<Point>();
            foreach(var part in parts)
            {
                var coords = part.Split(",").Select(c => int.Parse(c));
                var x = coords.First(); var y = coords.Last();

                CheckMax(x, y);

                points.Add(new Point(x, y));
            }

            values.Add(new PointPair(points.First(), points.Last()));
        }


        Console.WriteLine($"Test Values {TestValues.dimension}");
        foreach (var pp in TestValues.values)
        {
            Console.WriteLine(pp);
        }

        Console.WriteLine();

        Console.WriteLine($"Part Values {PartValues.dimension}");
        foreach (var pp in PartValues.values)
        {
            Console.WriteLine(pp);
        }
    }
}
