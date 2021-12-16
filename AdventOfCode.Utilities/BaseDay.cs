namespace AdventOfCode.Utilities;

public abstract class BaseDay
{
    protected static IEnumerable<int> ParseCommaSeparatedInt32s(string input) =>
        input.Split(",").Select(v => int.Parse(v));

    protected static TestResult ExecuteTests<T>(IEnumerable<(T input, object expected)> testValues, Func<T, object> testFunc)
    {
        var output = new List<(bool, string)>();

        foreach (var (input, expected) in testValues)
        {
            output.Add(Check(expected, testFunc(input)));
        }

        return new TestResult(output);
    }

    protected static TestResult ExecuteTest(object expected, Func<object> testFunc) =>
        new(new List<(bool, string)> { Check(expected, testFunc()) });

    protected static (bool, string) Check(object expected, object actual)
    {
        if (!actual.Equals(expected))
        {
            return (false, $"Failure: expected '{expected}', but was '{actual}'");
        }
        else
        {
            return (true, string.Empty);
        }
    }
}
