using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode.Utilities;

public class Solver
{
    private const string ClassPrefix = "Day";
    private readonly Assembly dll;
    private readonly string dllDir;

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

    private readonly List<DayInfo> days = new();

    public IEnumerable<TaskStatus> TaskStates => tasks.Select(t => t.Status);

    private readonly List<Task<Solution>> tasks = new();

    public Solver(Assembly assembly)
    {
        dll = assembly;
        dllDir = Path.GetDirectoryName(dll.Location) ?? string.Empty;

        foreach (var type in GetDays())
        {
            days.Add(new DayInfo(GetYear(type), GetDay(type), type));
        }
    }

    #region Helper Methods
    private IEnumerable<Type> GetDays()
    {
        return dll.GetTypes().Where(type => typeof(BaseDay).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
    }

    private static int GetYear(Type type)
    {
        return int.Parse((type.Namespace ?? string.Empty).Split(".").Last().TrimStart('_'));
    }

    private int GetHighestYear()
    {
        return days.OrderBy(x => x.Year).Last().Year;
    }

    private static int GetDay(Type type)
    {
        var name = type.Name;

        return int.Parse(name[(name.IndexOf(ClassPrefix) + ClassPrefix.Length)..].TrimStart('_'));
    }

    private string GetInputFile(int year, int index)
    {
        return Path.Combine(dllDir, year.ToString(), "Inputs", $"{index:D2}.txt");
    }

    private static IEnumerable<MethodInfo> GetTests(Type type)
    {
        return type.GetMethods().Where(x => x.GetCustomAttributes(typeof(DayTest), false).FirstOrDefault() != null);
    }

    private static IEnumerable<MethodInfo> GetParts(Type type)
    {
        return type.GetMethods().Where(x => x.GetCustomAttributes(typeof(DayPart), false).FirstOrDefault() != null);
    }

    private static bool RunTests(Solution solution, Type type, BaseDay instance)
    {
        var sw = new Stopwatch();

        foreach (var test in GetTests(type))
        {
            sw.Restart();
            var result = test.Invoke(instance, null);
            sw.Stop();

            if (result is TestResult testResult)
            {
                solution.Tests.Add((test.Name, testResult, sw.Elapsed));
            }
            else
            {
                if (result == null)
                {
                    throw new InvalidCastException($"Test result should be of type {typeof(TestResult)} but was null");
                }
                else
                {
                    throw new InvalidCastException($"Test result should be of type {typeof(TestResult)} but was {result.GetType()}");
                }
            }
        }

        return solution.Tests.All(t => t.result.Pass == true);
    }

    private static void RunParts(Solution solution, Type type, BaseDay instance)
    {
        var sw = new Stopwatch();
        foreach (var part in GetParts(type))
        {
            sw.Restart();
            var result = part.Invoke(instance, null);
            sw.Stop();

            if (result is string output)
            {
                solution.Parts.Add((part.Name, output, sw.Elapsed));
            }
            else
            {
                if (result == null)
                {
                    throw new InvalidCastException($"Test result should be of type {typeof(TestResult)} but was null");
                }
                else
                {
                    throw new InvalidCastException($"Test result should be of type {typeof(string)} but was {result.GetType()}");
                }
            }
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

    private Solution Solve(DayInfo day, bool testMode)
    {
        var solution = new Solution(day.Year, day.Day, day.Type.Name);

        var sw = new Stopwatch();
        var constructor = day.Type.GetConstructor(new Type[] { typeof(string) });

        if (constructor == null)
        {
            throw new SolvingException("Expected constructor of type Constructor(string) but found none.");
        }

        var inputFile = GetInputFile(day.Year, day.Day);

        sw.Start();
        var instance = (BaseDay)constructor.Invoke(new object[] { inputFile });
        solution.Construction = sw.Elapsed;

        var testsPassed = RunTests(solution, day.Type, instance);

        if (testsPassed && !testMode)
        {
            RunParts(solution, day.Type, instance);
        }
        sw.Stop();

        solution.RunTime = sw.Elapsed;

        return solution;
    }

    private (IEnumerable<Solution> solutions, TimeSpan duration) Solve(IEnumerable<DayInfo> days, bool testMode = false)
    {
        if (!days.Any())
        {
            return (new List<Solution>(), TimeSpan.Zero);
        }

        var sw = new Stopwatch();

        tasks.Clear();

        sw.Start();
        foreach (var day in days)
        {
            tasks.Add(Task.Run(() => Solve(day, testMode)));
        }

        Task.WhenAll(tasks).Wait();

        sw.Stop();

        var totalRunTime = sw.Elapsed;

        var solutions = tasks.Select(t => t.Result).OrderBy(s => s.Year).ThenBy(s => s.Day);

        return (solutions, totalRunTime);
    }

    public (IEnumerable<Solution> solutions, TimeSpan duration) SolveAll(int year)
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

    public (IEnumerable<Solution> solutions, TimeSpan duration) SolveLast(int year)
    {
        if (year == -1) year = GetHighestYear();

        var lastDay = days.Where(d => d.Year == year).LastOrDefault();

        if (lastDay != default)
        {
            return Solve(new List<DayInfo> { lastDay });
        }

        return (new List<Solution>(), TimeSpan.Zero);
    }

    public (IEnumerable<Solution> solutions, TimeSpan duration) Solve(int year, IEnumerable<int> selectedDays)
    {
        if (year == -1) year = GetHighestYear();

        return Solve(days.Where(d => d.Year == year && selectedDays.Contains(d.Day)));
    }

    public IEnumerable<Solution> RunTests()
    {
        var (solutions, _) = Solve(days, true);

        return solutions;
    }
}
