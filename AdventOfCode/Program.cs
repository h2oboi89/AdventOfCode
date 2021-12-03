using CommandLine;

namespace AdventOfCode;

public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                //Console.WriteLine($"Year: '{options.Year}'");
                //Console.WriteLine($"All: '{options.All}'");
                //Console.WriteLine($"Days: '{string.Join(", ", options.Days)}'");

                if (options.Year == -1)
                {
                    if (options.All)
                    {
                        Solver.SolveAll();
                    }
                    else if (options.Days.Any())
                    {
                        Solver.Solve(options.Days);
                    }
                    else
                    {
                        Solver.SolveLast();
                    }
                }
                else
                {
                    var year = options.Year.ToString();

                    if (options.All)
                    {
                        Solver.SolveAll(year);
                    }
                    else if (options.Days.Any())
                    {
                        Solver.Solve(year, options.Days);
                    }
                    else
                    {
                        Solver.SolveLast(year);
                    }
                }
            });
    }
}
