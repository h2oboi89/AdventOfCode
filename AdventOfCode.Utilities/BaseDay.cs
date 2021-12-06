namespace AdventOfCode.Utilities;

public abstract class BaseDay
{
    protected static TestResult ExecuteTests(List<(string input, object expected)> testValues, Func<string, object> testFunc)
    {
        var output = new List<(bool, string)>();

        foreach (var (input, expected) in testValues)
        {
            var testResult = ExecuteTest(input, expected, testFunc);

            foreach(var r in testResult.Results)
            {
                output.Add((r.pass, r.output));
            }
        }

        return new TestResult(output);
    }

    protected static TestResult ExecuteTest(string input, object expected, Func<string, object> testFunc)
    {
        var actual = testFunc(input);
        var result = new List<(bool, string)>();

        if (!actual.Equals(expected))
        {
            result.Add(new(false, $"Failure: '{input}' -> '{expected}', but was '{actual}'"));
        }
        else
        {
            result.Add(new(true, string.Empty));
        }

        return new TestResult(result);
    }
}
