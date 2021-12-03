using CommandLine;

namespace AdventOfCode;

public class Options
{
    [Option('y', "year", Required = false)]
    public int Year { get; set; } = -1;

    [Option("all", Required = false)]
    public bool All { get; set; } = false;

    [Value(0, Required = false)]
    public IEnumerable<int> Days { get; set; }
}
