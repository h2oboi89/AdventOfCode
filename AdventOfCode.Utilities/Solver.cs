using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode.Utilities;

public static class Solver
{
    private const string ClassPrefix = "Day";
    private static readonly Assembly dll = Assembly.GetEntryAssembly();
    private static readonly string dllDir = Path.GetDirectoryName(dll.Location);

    private class DayInfo
    {
        public readonly int Year;
        public readonly int Day;
        public readonly Type Type;

        public DayInfo(int year, int day, Type type)
        {
            Year = year;
            Day = day;
            Type = type;
        }
    }

    private static readonly List<DayInfo> days = new();

    static Solver()
    {
        foreach (var type in GetDays())
        {
            days.Add(new DayInfo(GetYear(type), GetDay(type), type));
        }
    }

    #region Helper Methods
    private static IEnumerable<Type> GetDays()
    {
        return dll.GetTypes().Where(type => typeof(BaseDay).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
    }

    private static int GetYear(Type type)
    {
        return int.Parse(type.Namespace.Split(".").Last().TrimStart('_'));
    }

    private static int GetHighestYear()
    {
        return days.OrderBy(x => x.Year).Last().Year;
    }

    private static int GetDay(Type type)
    {
        var name = type.Name;

        return int.Parse(name[(name.IndexOf(ClassPrefix) + ClassPrefix.Length)..].TrimStart('_'));
    }

    private static string GetInputFile(int year, int index)
    {
        return Path.Combine(dllDir, year.ToString(), "Inputs", $"{index:D2}.txt");
    }

    private static IEnumerable<MethodInfo> GetTests(Type type)
    {
        return type.GetMethods().Where(x => x.GetCustomAttributes(typeof(Test), false).FirstOrDefault() != null);
    }

    private static IEnumerable<MethodInfo> GetParts(Type type)
    {
        return type.GetMethods().Where(x => x.GetCustomAttributes(typeof(Part), false).FirstOrDefault() != null);
    }

    private static bool RunTests(Solution solution, Type type, BaseDay instance)
    {
        var sw = new Stopwatch();
        var testsPassed = true;

        foreach (var test in GetTests(type))
        {
            sw.Restart();
            var result = (bool)test.Invoke(instance, null);
            sw.Stop();

            solution.Tests.Add((test.Name, result, sw.Elapsed));

            if (!result)
            {
                testsPassed = false;
            }
        }

        return testsPassed;
    }

    private static void RunParts(Solution solution, Type type, BaseDay instance)
    {
        var sw = new Stopwatch();
        foreach (var part in GetParts(type))
        {
            sw.Restart();
            var result = (string)part.Invoke(instance, null);
            sw.Stop();

            solution.Parts.Add((part.Name, result, sw.Elapsed));
        }
    }

    private static void ConsoleColorMethod(ConsoleColor color, string text, Action<string> method)
    {
        Console.ForegroundColor = color;
        method(text);
        Console.ResetColor();
    }

    private static void ConsoleWriteWithColor(ConsoleColor color, string text)
    {
        ConsoleColorMethod(color, text, Console.Write);
    }

    private static void ConsoleWriteLineWithColor(ConsoleColor color, string text)
    {
        ConsoleColorMethod(color, text, Console.WriteLine);
    }
    #endregion

    private static Solution Solve(DayInfo day)
    {
        var solution = new Solution(day.Year, day.Day, day.Type.Name);

        var sw = new Stopwatch();
        var constructor = day.Type.GetConstructor(new Type[] { typeof(string) });
        var inputFile = GetInputFile(day.Year, day.Day);

        sw.Start();
        var instance = (BaseDay)constructor.Invoke(new object[] { inputFile });
        solution.Construction = sw.Elapsed;

        var testsPassed = RunTests(solution, day.Type, instance);

        if (testsPassed)
        {
            RunParts(solution, day.Type, instance);
        }
        sw.Stop();

        solution.RunTime = sw.Elapsed;

        return solution;
    }

    private static (IEnumerable<Solution> solutions, TimeSpan duration) Solve(IEnumerable<DayInfo> days)
    {
        if (!days.Any())
        {
            return (new List<Solution>(), TimeSpan.Zero);
        }
        
        var sw = new Stopwatch();
        var dayTasks = new List<Task<Solution>>();

        sw.Start();
        foreach (var day in days)
        {
            dayTasks.Add(Task.Run(() => Solve(day)));
        }

        Task.WhenAll(dayTasks).Wait();
        sw.Stop();

        var totalRunTime = sw.Elapsed;

        var solutions = dayTasks.Select(t => t.Result).OrderBy(s => s.Year).ThenBy(s => s.Day);

        return (solutions, totalRunTime);
    }

    public static (IEnumerable<Solution> solutions, TimeSpan duration) SolveAll(int year)
    {
        if (year == -1)
        {
            return Solve(days);
        }
        else
        {
            return Solve(days.Where(d => d.Year == year));
        }
    }

    public static (IEnumerable<Solution> solutions, TimeSpan duration) SolveLast(int year)
    {
        if (year == -1) year = GetHighestYear();

        var lastDay = days.Where(d => d.Year == year).LastOrDefault();

        if (lastDay != default)
        {
            return Solve(new List<DayInfo> { lastDay });
        }

        return (new List<Solution>(), TimeSpan.Zero);
    }

    public static (IEnumerable<Solution> solutions, TimeSpan duration) Solve(int year, IEnumerable<int> selectedDays)
    {
        if (year == -1) year = GetHighestYear();

        return Solve(days.Where(d => d.Year == year && selectedDays.Contains(d.Day)));
    }
}
