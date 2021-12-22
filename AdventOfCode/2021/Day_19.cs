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
        var points = new List<Point.D3>();

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
            points.Add(new Point.D3(coords[0], coords[1], coords[2]));
        }
    }

    private class Field
    {
        private readonly List<Point.D3> _scanners = new();
        private readonly List<Point.D3> _beacons = new();

        private static readonly IEnumerable<int[,]> _rotations = new List<int[,]>()
        {
            new int[3,3] { {  1,  0,  0 }, {  0,  1,  0 }, {  0,  0,  1 } },
            new int[3,3] { {  0,  0,  1 }, {  0,  1,  0 }, { -1,  0,  0 } },
            new int[3,3] { { -1,  0,  0 }, {  0,  1,  0 }, {  0,  0, -1 } },
            new int[3,3] { {  0,  0, -1 }, {  0,  1,  0 }, {  1,  0,  0 } },

            new int[3,3] { {  0, -1,  0 }, {  1,  0,  0 }, {  0,  0,  1 } },
            new int[3,3] { {  0,  0,  1 }, {  1,  0,  0 }, {  0,  1,  0 } },
            new int[3,3] { {  0,  1,  0 }, {  1,  0,  0 }, {  0,  0, -1 } },
            new int[3,3] { {  0,  0, -1 }, {  1,  0,  0 }, {  0, -1,  0 } },

            new int[3,3] { {  0,  1,  0 }, { -1,  0,  0 }, {  0,  0,  1 } },
            new int[3,3] { {  0,  0,  1 }, { -1,  0,  0 }, {  0, -1,  0 } },
            new int[3,3] { {  0, -1,  0 }, { -1,  0,  0 }, {  0,  0, -1 } },
            new int[3,3] { {  0,  0, -1 }, { -1,  0,  0 }, {  0,  1,  0 } },

            new int[3,3] { {  1,  0,  0 }, {  0,  0, -1 }, {  0,  1,  0 } },
            new int[3,3] { {  0,  1,  0 }, {  0,  0, -1 }, { -1,  0,  0 } },
            new int[3,3] { { -1,  0,  0 }, {  0,  0, -1 }, {  0, -1,  0 } },
            new int[3,3] { {  0, -1,  0 }, {  0,  0, -1 }, {  1,  0,  0 } },

            new int[3,3] { {  1,  0,  0 }, {  0, -1,  0 }, {  0,  0, -1 } },
            new int[3,3] { {  0,  0, -1 }, {  0, -1,  0 }, { -1,  0,  0 } },
            new int[3,3] { { -1,  0,  0 }, {  0, -1,  0 }, {  0,  0,  1 } },
            new int[3,3] { {  0,  0,  1 }, {  0, -1,  0 }, {  1,  0,  0 } },

            new int[3,3] { {  1,  0,  0 }, {  0,  0,  1 }, {  0, -1,  0 } },
            new int[3,3] { {  0, -1,  0 }, {  0,  0,  1 }, { -1,  0,  0 } },
            new int[3,3] { { -1,  0,  0 }, {  0,  0,  1 }, {  0,  1,  0 } },
            new int[3,3] { {  0,  1,  0 }, {  0,  0,  1 }, {  1,  0,  0 } },
        };

        public IEnumerable<Point.D3> Beacons
        {
            get { foreach (var beacon in _beacons) yield return beacon; }
        }

        public IEnumerable<Point.D3> Scanners
        {
            get { foreach (var scanner in _scanners) yield return scanner; }
        }

        public readonly string Id;

        public Field(string id, IEnumerable<Point.D3> beacons, IEnumerable<Point.D3>? scanners = null)
        {
            Id = id;
            _beacons.AddRange(beacons);

            if (scanners == null)
            {
                _scanners.Add(new Point.D3());
            }
            else
            {
                _scanners.AddRange(scanners);
            }
        }

        public Field Rotate(int[,] rotation)
        {
            static IEnumerable<Point.D3> Rotate(List<Point.D3> points, int[,] rotation)
            {
                foreach (var point in points)
                {
                    var x = rotation[0, 0] * point.X + rotation[0, 1] * point.Y + rotation[0, 2] * point.Z;
                    var y = rotation[1, 0] * point.X + rotation[1, 1] * point.Y + rotation[1, 2] * point.Z;
                    var z = rotation[2, 0] * point.X + rotation[2, 1] * point.Y + rotation[2, 2] * point.Z;

                    yield return new(x, y, z);
                }
            };

            return new Field(Id, Rotate(_beacons, rotation), Rotate(_scanners, rotation));
        }

        public Field Translate(Point.D3 translation)
        {
            static IEnumerable<Point.D3> Translate(IEnumerable<Point.D3> points, Point.D3 translation)
            {
                foreach (var point in points)
                {
                    yield return point + translation;
                }
            };

            return new Field(Id, Translate(_beacons, translation), Translate(_scanners, translation));
        }

        public Field Merge(Field other) =>
            new($"{Id}-{other.Id}", _beacons.Concat(other._beacons).Distinct(), _scanners.Concat(other._scanners).Distinct());

        public Field Clone() => new(Id, _beacons, _scanners);

        public int Compare(Field other) => new HashSet<Point.D3>(_beacons).Intersect(other._beacons).Count();

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (obj is not Field other) return false;

            if (Id != other.Id) return false;

            if (_scanners.Count != other._scanners.Count) return false;

            if (_beacons.Count != other._beacons.Count) return false;

            for (var i = 0; i < _scanners.Count; i++)
            {
                if (_scanners[i] != other._scanners[i]) return false;
            }

            for (var i = 0; i < _beacons.Count; i++)
            {
                if (_beacons[i] != other._beacons[i]) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = _beacons.Count;

            foreach (var scanner in _scanners)
            {
                hash ^= scanner.GetHashCode();
            }

            foreach (var beacon in _beacons)
            {
                hash ^= beacon.GetHashCode();
            }

            return hash;
        }

        public override string ToString() => $"{Id} [{_scanners.Count}] ({string.Join(", ", _beacons)})";

        public Field? Align(Field other)
        {
            static IEnumerable<Field> GenerateRotations(Field field) => _rotations.Select(r => field.Rotate(r));

            static IEnumerable<Point.D3> GenerateTranslations(Field field)
            {
                foreach (var beacon in field._beacons)
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
            var distance = a.Distance(b);

            var manhattanDistance = distance.X + distance.Y + distance.Z;

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
