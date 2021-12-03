using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode.Utilities;

public static class Solver
{
    private const string ClassPrefix = "Day";
    private static readonly Assembly dll = Assembly.GetEntryAssembly();
    private static readonly string dllDir = Path.GetDirectoryName(dll.Location);

    private static readonly List<(string year, int index, Type type)> days = new();

    static Solver()
    {
        foreach (var day in GetDays())
        {
            var year = GetYear(day);
            var index = GetIndex(day);

            days.Add((year, index, day));
        }
    }

    private static IEnumerable<Type> GetDays()
    {
        return dll.GetTypes().Where(type => typeof(BaseDay).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
    }

    private static string GetYear(Type type)
    {
        return type.Namespace.Split(".").Last().TrimStart('_');
    }

    private static string GetHighestYear()
    {
        return days.OrderBy(x => x.year).Last().year;
    }

    private static int GetIndex(Type type)
    {
        var name = type.Name;

        return int.Parse(name[(name.IndexOf(ClassPrefix) + ClassPrefix.Length)..].TrimStart('_'));
    }

    private static string GetInputFile(string year, int index)
    {
        return Path.Combine(dllDir, year, "Inputs", $"{index:D2}.txt");
    }

    private static IEnumerable<MethodInfo> GetTests(Type type)
    {
        return type.GetMethods().Where(x => x.GetCustomAttributes(typeof(Test), false).FirstOrDefault() != null);
    }

    private static IEnumerable<MethodInfo> GetParts(Type type)
    {
        return type.GetMethods().Where(x => x.GetCustomAttributes(typeof(Part), false).FirstOrDefault() != null);
    }

    private static void Solve((string year, int index, Type type) day)
    {
        if (day == default) return;

        var (year, index, type) = day;

        var totalTime = default(TimeSpan);
        var sw = new Stopwatch();
        var constructor = type.GetConstructor(new Type[] { typeof(string) });
        var inputFile = GetInputFile(year, index);
        var originalColor = Console.ForegroundColor;

        sw.Start();
        var instance = (BaseDay)constructor.Invoke(new object[] { inputFile });
        sw.Stop();

        totalTime += sw.Elapsed;

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write($"{year}.{type.Name}");
        Console.ForegroundColor = originalColor;
        Console.WriteLine($" {FormatTime(sw.Elapsed)}");

        var testsPassed = true;

        var testResults = new List<(string, bool)>();

        sw.Restart();
        foreach (var test in GetTests(type))
        {
            var result = (bool)test.Invoke(instance, null);

            testResults.Add((test.Name, result));

            if (!result)
            {
                testsPassed = false;
            }
        }
        sw.Stop();

        totalTime += sw.Elapsed;

        Console.WriteLine(" - Tests:");
        foreach (var (name, result) in testResults)
        {
            if (result)
            {
                Console.Write($"   - {name}: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("passed");
            }
            else
            {
                Console.Write($"   - {name}: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("failed");
            }

            Console.ForegroundColor = originalColor;
        }

        if (testsPassed)
        {
            var partResults = new List<(string, string, TimeSpan)>();

            foreach (var part in GetParts(type))
            {
                sw.Restart();
                var result = (string)part.Invoke(instance, null);
                sw.Stop();

                totalTime += sw.Elapsed;

                partResults.Add((part.Name, result, sw.Elapsed));
            }

            Console.WriteLine(" - Parts:");
            foreach (var (name, result, time) in partResults)
            {
                Console.Write($"   - {name}: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(result);
                Console.ForegroundColor = originalColor;
                Console.WriteLine($" [{FormatTime(time)}]");
            }
        }

        Console.WriteLine($" - Total Time: {FormatTime(totalTime)}");
    }

    public static void Solve(string year, int index)
    {
        Solve(days.Where(x => x.year == year && x.index == index).FirstOrDefault());
    }

    public static void SolveAll()
    {
        var yearGroups = days.OrderBy(x => x.year).GroupBy(x => x.year);

        var sw = new Stopwatch();

        sw.Start();
        foreach (var yearGroup in yearGroups)
        {
            var year = yearGroup.First().year;

            var sortedDays = yearGroup.OrderBy(x => x.index);

            foreach (var day in sortedDays)
            {
                Solve(day);
                Console.WriteLine();
            }
        }
        sw.Stop();

        Console.WriteLine($"Total Execution time: {FormatTime(sw.Elapsed)}");
    }

    public static void SolveAll(string year)
    {
        var filteredDays = days.Where(x => x.year == year).OrderBy(x => x.index);

        var sw = new Stopwatch();

        sw.Start();
        foreach (var day in filteredDays)
        {
            Solve(day);
            Console.WriteLine();
        }
        sw.Stop();

        Console.WriteLine($"Total Execution time: {FormatTime(sw.Elapsed)}");
    }

    public static void Solve(IEnumerable<int> indexes) => Solve(GetHighestYear(), indexes);

    public static void Solve(string year, IEnumerable<int> indexes) {
        var sw = new Stopwatch();

        sw.Start(); 
        foreach (var i in indexes)
        {
            Solve(year, i);
        }
        sw.Stop();

        Console.WriteLine($"Total Execution time: {FormatTime(sw.Elapsed)}");
    }

    public static void SolveLast() => SolveLast(GetHighestYear());

    public static void SolveLast(string year)
    {
        Solve(days.Where(x => x.year == year).OrderBy(x => x.index).LastOrDefault());
    }

    private static string FormatTime(TimeSpan time)
    {
        var elapsedMilliseconds = time.TotalMilliseconds;

        const int MILLISECOND = 1;
        const int SECOND = 1_000 * MILLISECOND;
        const int MINUTE = 60 * SECOND;

        var message = elapsedMilliseconds switch
        {
            < MILLISECOND => $"{elapsedMilliseconds:F} ms",
            < SECOND => $"{Math.Round(elapsedMilliseconds)} ms",
            < MINUTE => $"{0.001 * elapsedMilliseconds:F} s",
            _ => $"{Math.Floor(elapsedMilliseconds / MINUTE)} min {Math.Round(0.001 * (elapsedMilliseconds % MINUTE))} s",
        };

        return message;
    }
}
