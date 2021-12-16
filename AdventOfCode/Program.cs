﻿using CommandLine;
using System.Diagnostics;
using System.Reflection;

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
        var sw = new Stopwatch();
        sw.Start();

        Console.WriteLine("Running solution...");

        var solveTask = Task.Run(() =>
        {
            var solver = new Solver(Assembly.GetExecutingAssembly());

            if (options.All)
            {
                return solver.SolveAll(options.Year);
            }
            else if (options.Days.Any())
            {
                return solver.Solve(options.Year, options.Days);
            }
            else
            {
                return solver.SolveLast(options.Year);
            }
        });

        var clearString = new string(Enumerable.Repeat(' ', Console.BufferWidth).ToArray());

        while (!solveTask.IsCompleted)
        {
            Console.Write($"{clearString}\r{FormatTime(sw.Elapsed)}\r");

            Thread.Sleep(900);
        }

        Console.WriteLine($"{Environment.NewLine}Finished");

        sw.Stop();

        return solveTask.Result;
    }

    private static void PrintResults(IEnumerable<Solution> solutions, TimeSpan totalRunTime)
    {
        if (!solutions.Any())
        {
            ConsoleUtils.WriteLine(ConsoleColor.Red, "No solutions found!");
            return;
        }

        var runTimes = new List<TimeSpan>();

        foreach (var solution in solutions)
        {
            ConsoleUtils.Write(ConsoleColor.Blue, solution.Name);
            Console.WriteLine($" {FormatTime(solution.Construction)}");

            Console.WriteLine(" - Tests:");
            foreach (var (name, result, duration) in solution.Tests)
            {
                Console.Write($"   - {name}: ");
                if (result.Pass)
                {
                    ConsoleUtils.Write(ConsoleColor.Green, "passed");
                }
                else
                {
                    ConsoleUtils.Write(ConsoleColor.Red, "failed");
                }
                Console.WriteLine($" [{FormatTime(duration)}]");

                if (!result.Pass)
                {
                    foreach (var (pass, output) in result.Results)
                    {
                        Console.WriteLine($"      - {pass} : {output}");
                    }
                }
            }

            Console.WriteLine(" - Parts:");
            foreach (var (name, result, duration) in solution.Parts)
            {
                Console.Write($"   - {name}: ");
                ConsoleUtils.Write(ConsoleColor.Cyan, result);
                Console.WriteLine($" [{FormatTime(duration)}]");
            }
            Console.WriteLine($" - CPU Time: {FormatTime(solution.ExecutionTime)}");
            Console.WriteLine($" - Run Time: {FormatTime(solution.RunTime)}");
            Console.WriteLine();

            runTimes.Add(solution.RunTime);
        }

        var min = runTimes.Min();
        var max = runTimes.Max();
        var avg = TimeSpan.FromMilliseconds(runTimes.Select(t => t.TotalMilliseconds).Average());

        Console.WriteLine($"{solutions.Count()} problems solved");
        Console.WriteLine($"Total Run Time: {FormatTime(totalRunTime)}");
        Console.WriteLine($" - Minimum: {FormatTime(min)}");
        Console.WriteLine($" - Maximum: {FormatTime(max)}");
        Console.WriteLine($" - Average: {FormatTime(avg)}");
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
