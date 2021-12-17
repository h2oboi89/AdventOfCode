namespace AdventOfCode.Utilities;

/// <summary>
/// Result of running a <see cref="BaseDay"/> <see cref="Test"/> function.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Overall result for <see cref="TestResult"/>
    /// </summary>
    public bool Pass => Results.All(r => r.pass);

    /// <summary>
    /// Individual results of each test case in the <see cref="Test"/> function.
    /// </summary>
    public IEnumerable<(bool pass, string output)> Results;

    /// <summary>
    /// Dummy constructor for empty <see cref="Test"/> functions.
    /// </summary>
    public TestResult()
    {
        Results = new List<(bool, string)>();
    }

    public TestResult(IEnumerable<(bool pass, string output)> results)
    {
        Results = results;
    }

    public TestResult((bool pass, string output) result)
    {
        Results = new List<(bool, string)>() { result };
    }
}
