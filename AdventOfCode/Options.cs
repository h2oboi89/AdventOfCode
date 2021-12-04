using CommandLine;

namespace AdventOfCode;

public class Options
{
    [Option('y', "year", Required = false, HelpText = "Year selection (default is highest year)")]
    public int Year { get; set; } = -1;

    [Option("all", Required = false, HelpText = "Enables running all days for year or all years if no year is specified")]
    public bool All { get; set; } = false;

    [Value(0, Required = false, HelpText = "Specify days to run (ie: 1 2 3)")]
    public IEnumerable<int> Days { get; set; } = new List<int>();
}
