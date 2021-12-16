namespace AdventOfCode.Utilities;

public class TestResult
{
    public bool Pass => Results.All(r => r.pass);
    public IEnumerable<(bool pass, string output)> Results;

    public TestResult(IEnumerable<(bool pass, string output)> results)
    {
        Results = results;
    }

    public TestResult((bool pass, string output) result)
    {
        Results = new List<(bool pass, string output)>() { result };
    }
}
