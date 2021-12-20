using AdventOfCode.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode._2021;

internal class Day_19 : BaseDay
{
    private readonly List<Field> partsFields = new();
    private readonly List<Field> testFields = new();

    public Day_19(string inputFile)
    {
        var fields = new List<Field>();
        var id = string.Empty;
        var points = new List<Point3D>();

        var idRegex = new Regex(@"--- scanner (?<id>\d+) ---");

        foreach (var line in File.ReadLines(inputFile))
        {
            if (line.StartsWith("###"))
            {
                testFields.AddRange(fields);
                fields.Clear();
                continue;
            }

            if (line.StartsWith("!!!"))
            {
                partsFields.AddRange(fields);
                fields.Clear();
                continue;
            }

            if (line.StartsWith("---"))
            {
                id = idRegex.Match(line).Groups["id"].Value;
                continue;
            }

            if (line.Length == 0)
            {
                fields.Add(new Field(id, points));
                points.Clear();
                continue;
            }

            // default line is coordinate triplet
            var coords = line.Split(',').Select(int.Parse).ToArray();
            points.Add(new Point3D(coords[0], coords[1], coords[2]));
        }
    }

    private class Field
    {
        private readonly List<Point3D> _scanners = new() { new Point3D(0, 0, 0) };
        private readonly List<Point3D> _beacons = new();

        public IEnumerable<Point3D> Beacons
        {
            get { foreach (var beacon in _beacons) yield return beacon; }
        }

        public IEnumerable<Point3D> Scanners
        {
            get { foreach (var scanner in _scanners) yield return scanner; }
        }

        public readonly string Id;

        public Field(string id, IEnumerable<Point3D> beacons)
        {
            Id = id;
            _beacons.AddRange(beacons);
        }

        private Field(string id, IEnumerable<Point3D> beacons, IEnumerable<Point3D> scanners) : this(id, beacons)
        {
            _scanners = new(scanners);
        }

        private static Exception InvalidAxis(Axis axis) => new($"Invalid axis {axis}");

        private static IEnumerable<Point3D> ForEach(IEnumerable<Point3D> points, Func<Point3D, Point3D> transformFunc)
        {
            foreach (var point in points)
            {
                yield return transformFunc(point);
            }
        }

        public Field Rotate(Axis axis)
        {
            IEnumerable<Point3D> Rotate(IEnumerable<Point3D> points) => ForEach(points, point => axis switch
            {
                Axis.X => new Point3D(point.X, point.Z, -point.Y),
                Axis.Y => new Point3D(-point.Z, point.Y, point.X),
                Axis.Z => new Point3D(point.Y, -point.X, point.Z),
                _ => throw InvalidAxis(axis)
            });

            return new Field(Id, Rotate(Beacons), Rotate(Scanners));
        }

        public Field Translate(Point3D translation)
        {
            IEnumerable<Point3D> Translate(IEnumerable<Point3D> points, Point3D translation) =>
                ForEach(points, point => new Point3D(point.X + translation.X, point.Y + translation.Y, point.Z + translation.Z));

            return new Field(Id, Translate(Beacons, translation), Translate(Scanners, translation));
        }

        public Field Merge(Field other) =>
            new($"{Id}-{other.Id}", Beacons.Concat(other.Beacons).Distinct(), Scanners.Concat(other.Scanners).Distinct());

        public Field Clone() => new(Id, Beacons, Scanners);

        public int Compare(Field other)
        {
            var matches = 0;

            foreach (var beacon in Beacons)
            {
                if (other.Beacons.Contains(beacon))
                {
                    matches++;
                }
            }

            return matches;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Field other) return false;

            if (Id != other.Id) return false;

            return Scanners.SequenceEqual(other.Scanners) && Beacons.SequenceEqual(other.Beacons);
        }

        public override int GetHashCode()
        {
            var hash = _beacons.Count;

            foreach (var scanner in Scanners)
            {
                hash ^= scanner.GetHashCode();
            }

            foreach (var beacon in Beacons)
            {
                hash ^= beacon.GetHashCode();
            }

            return hash;
        }

        public override string ToString() => $"{Id} [{Scanners.Count()}] ({string.Join(", ", Beacons)})";

        public Field? Align(Field other)
        {
            static IEnumerable<Field> GenerateRotations(Field field)
            {
                var permutations = new List<Field>();

                var temp = field.Clone();

                for (var x = 0; x < 4; x++)
                {
                    for (var y = 0; y < 4; y++)
                    {
                        for (var z = 0; z < 4; z++)
                        {
                            if (!permutations.Contains(temp)) permutations.Add(temp);

                            temp = temp.Rotate(Axis.Z);
                        }

                        temp = temp.Rotate(Axis.Y);
                    }

                    temp = temp.Rotate(Axis.X);
                }

                return permutations;
            }

            static IEnumerable<Point3D> GenerateTranslations(Field field)
            {
                foreach (var beacon in field.Beacons)
                {
                    yield return -beacon;
                }
            }

            var translations = GenerateTranslations(this);

            foreach (var rotation in GenerateRotations(other))
            {
                var otherTranslations = GenerateTranslations(rotation);

                foreach (var (aT, bT) in translations.CartesianProduct(otherTranslations))
                {
                    var temp = rotation.Translate(bT);

                    var count = Translate(aT).Compare(temp);

                    if (count >= 12)
                    {
                        return temp.Translate(-aT);
                    }
                }
            }

            return null;
        }

        public static IEnumerable<Field> Align(IEnumerable<Field> fields)
        {
            var tasks = new List<Task<List<Field>>>();

            foreach (var (a, b) in fields.UniqueProduct())
            {
                tasks.Add(Task.Run(() =>
                {
                    var c = a.Align(b);

                    if (c != null)
                    {
                        return new List<Field>() { a, c };
                    }

                    return new();
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var alignedFields = tasks.Where(t => t.Result.Any()).Select(t => t.Result).ToList();

            static (Field a, int groupIndex, int scannerIndex)? FindMatch(Field a, List<List<Field>> fields)
            {
                for (var i = 0; i < fields.Count; i++)
                {
                    for (var j = 0; j < fields[i].Count; j++)
                    {
                        if (a.Id == fields[i][j].Id)
                        {
                            return (a, i, j);
                        }
                    }
                }

                return null;
            }

            while (alignedFields.Count > 1)
            {
                var rootGroup = alignedFields.First();

                alignedFields.RemoveAt(0);

                var success = false;
                foreach (var aligned in rootGroup)
                {
                    var match = FindMatch(aligned, alignedFields);

                    if (match != null)
                    {
                        var (a, groupIndex, scannerIndex) = match.Value;

                        var nextGroup = alignedFields[groupIndex];

                        alignedFields.RemoveAt(groupIndex);
                        nextGroup.RemoveAt(scannerIndex);

                        foreach (var b in nextGroup)
                        {
                            var c = a.Align(b);

                            if (c != null) rootGroup.Add(c);
                        }

                        success = true;
                        break;
                    }
                }

                if (success)
                {
                    alignedFields.Insert(0, rootGroup);
                }
                else
                {
                    alignedFields.Add(rootGroup);
                }
            }

            return alignedFields.First();
        }

        public static Field Merge(IEnumerable<Field> fields)
        {
            var root = fields.First();

            foreach (var field in fields.Skip(1))
            {
                root = root.Merge(field);
            }

            return root;
        }
    }

    private static int LargestDistance(Field field)
    {
        var max = int.MinValue;

        foreach (var (a, b) in field.Scanners.UniqueProduct())
        {
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            var dz = Math.Abs(a.Z - b.Z);

            var manhattanDistance = dx + dy + dz;

            if (manhattanDistance > max) max = manhattanDistance;
        }

        return max;
    }

    private static (int beacons, int maxDistance) Merge(IEnumerable<Field> fields)
    {
        var merged = Field.Merge(Field.Align(fields));

        return (merged.Beacons.Count(), LargestDistance(merged));
    }

    [DayTest]
    public TestResult ParseTest()
    {
        var testValues = new List<(int, int)>()
        {
            (testFields.Count, 5),
            (partsFields.Count, 40)
        };

        return ExecuteTests(testValues, i => i);
    }

    [DayTest]
    public TestResult Test() => ExecuteTest((79, 3621), () => Merge(testFields));

    [DayPart]
    public string Solve()
    {
        var (beacons, largestDistance) = Merge(partsFields);

        return $"{beacons} {largestDistance}";
    }
}
