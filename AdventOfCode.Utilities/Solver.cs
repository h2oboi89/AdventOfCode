using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

    public static void Solve(string year, int index)
    {
        var d = days.Where(x => x.year == year && x.index == index).FirstOrDefault();

        if (d != default)
        {
            var totalTime = default(TimeSpan);
            var sw = new Stopwatch();
            var constructor = d.type.GetConstructor(new Type[] { typeof(string) });
            var inputFile = GetInputFile(year, index);
            var originalColor = Console.ForegroundColor;

            sw.Start();
            var day = (BaseDay)constructor.Invoke(new object[] { inputFile });
            sw.Stop();

            totalTime += sw.Elapsed;

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{year}.{d.type.Name}");
            Console.ForegroundColor = originalColor;
            Console.WriteLine($" {FormatTime(sw.Elapsed)}");

            var testsPassed = true;

            var testResults = new List<(string, bool)>();

            sw.Restart();
            foreach (var test in GetTests(d.type))
            {
                var result = (bool)test.Invoke(day, null);

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

                foreach (var part in GetParts(d.type))
                {
                    sw.Restart();
                    var result = (string)part.Invoke(day, null);
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
    }

    // TODO: solve specified

    // TODO: solve all

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
