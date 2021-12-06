namespace AdventOfCode.Utilities;

public class TestResult
{
    public readonly bool Pass;
    public IEnumerable<(bool pass, string output)> Results;

    public TestResult(IEnumerable<(bool pass, string output)> results)
    {
        Results = results;
        Pass = results.All(r => r.pass == true);
    }
}
