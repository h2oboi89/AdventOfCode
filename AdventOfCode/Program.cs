using CommandLine;

namespace AdventOfCode;

public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                var (solutions, totalRunTime) = Solve(options);

                PrintResults(solutions, totalRunTime);
            });
    }

    private static (IEnumerable<Solution> solutions, TimeSpan duration) Solve(Options options)
    {
        IEnumerable<Solution> solutions;
        TimeSpan totalRunTime;
        
        if (options.All)
        {
            (solutions, totalRunTime) = Solver.SolveAll(options.Year);
        }
        else if (options.Days.Any())
        {
            (solutions, totalRunTime) = Solver.Solve(options.Year, options.Days);
        }
        else
        {
            (solutions, totalRunTime) = Solver.SolveLast(options.Year);
        }

        return (solutions, totalRunTime);
    }

    private static void PrintResults(IEnumerable<Solution> solutions, TimeSpan totalRunTime)
    {
        if (!solutions.Any())
        {
            ConsoleWriteLineWithColor(ConsoleColor.Red, "No solutions found!");
            return;
        }

        var min = TimeSpan.MaxValue;
        var max = TimeSpan.Zero;
        var runTimes = new List<TimeSpan>();

        foreach (var solution in solutions)
        {
            ConsoleWriteWithColor(ConsoleColor.Blue, solution.Name);
            Console.WriteLine($" {FormatTime(solution.Construction)}");

            Console.WriteLine(" - Tests:");
            foreach (var (name, passed, duration) in solution.Tests)
            {
                Console.Write($"   - {name}: ");
                if (passed)
                {
                    ConsoleWriteWithColor(ConsoleColor.Green, "passed");
                }
                else
                {
                    ConsoleWriteWithColor(ConsoleColor.Red, "failed");
                }
                Console.WriteLine($" [{FormatTime(duration)}]");
            }

            Console.WriteLine(" - Parts:");
            foreach (var (name, result, duration) in solution.Parts)
            {
                Console.Write($"   - {name}: ");
                ConsoleWriteWithColor(ConsoleColor.Cyan, result);
                Console.WriteLine($" [{FormatTime(duration)}]");
            }
            Console.WriteLine($" - CPU Time: {FormatTime(solution.ExecutionTime)}");
            Console.WriteLine($" - Run Time: {FormatTime(solution.RunTime)}");
            Console.WriteLine();

            if (solution.RunTime > max) max = solution.RunTime;

            if (solution.RunTime < min) min = solution.RunTime;

            runTimes.Add(solution.RunTime);
        }

        var avg = TimeSpan.FromMilliseconds(runTimes.Select(t => t.TotalMilliseconds).Average());

        Console.WriteLine($"{solutions.Count()} problems solved");
        Console.WriteLine($"Total Run Time: {FormatTime(totalRunTime)}");
        Console.WriteLine($" - Minimum: {FormatTime(min)}");
        Console.WriteLine($" - Maximum: {FormatTime(max)}");
        Console.WriteLine($" - Average: {FormatTime(avg)}");
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
