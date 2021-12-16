namespace AdventOfCode.Utilities;

public abstract class BaseDay
{
    protected static IEnumerable<int> ParseCommaSeparatedInt32s(string input) =>
        input.Split(",").Select(v => int.Parse(v));

    protected static TestResult ExecuteTests<I, O>(IEnumerable<(I input, O expected)> testValues, Func<I, O> testFunc)
    {
        var output = new List<(bool, string)>();

        foreach (var (input, expected) in testValues)
        {
            output.Add(Check(expected, testFunc(input)));
        }

        return new TestResult(output);
    }

    protected static TestResult ExecuteTest<I, O>(I input, O expected, Func<I, O> testFunc)
    {
        return new TestResult(Check(expected, testFunc(input)));
    }

    protected static TestResult ExecuteTest<T>(T expected, Func<T> testFunc) =>
        new(new List<(bool, string)> { Check(expected, testFunc()) });

    protected static (bool, string) Check<T>(T expected, T actual)
    {
        if (actual == null) throw new ArgumentNullException(nameof(actual));

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
