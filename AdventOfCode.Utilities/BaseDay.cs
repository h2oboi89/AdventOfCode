namespace AdventOfCode.Utilities;

public abstract class BaseDay
{
    protected static TestResult ExecuteTests<TInput, TOutput>(IEnumerable<(TInput input, TOutput expected)> testValues, Func<TInput, TOutput> testFunc)
    {
        var output = new List<(bool, string)>();

        foreach (var (input, expected) in testValues)
        {
            output.Add(Check(expected, testFunc(input)));
        }

        return new TestResult(output);
    }

    protected static TestResult ExecuteTest<TInput, TOutput>(TInput input, TOutput expected, Func<TInput, TOutput> testFunc)
    {
        return new TestResult(Check(expected, testFunc(input)));
    }

    protected static TestResult ExecuteTest<TOutput>(TOutput expected, Func<TOutput> testFunc) =>
        new(new List<(bool, string)> { Check(expected, testFunc()) });

    protected static (bool, string) Check<TOutput>(TOutput expected, TOutput actual)
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
