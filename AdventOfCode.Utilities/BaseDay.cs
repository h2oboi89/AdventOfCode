namespace AdventOfCode.Utilities;

public abstract class BaseDay { 
    private static void PrintTestFailure(string input, object expected, object actual)
    {
        Console.WriteLine($"Failure: '{input}' -> '{expected}', but was '{actual}'");
    }

    protected static bool ExecuteTests(List<(string input, object expected)> testValues, Func<string, object> testFunc)
    {
        var pass = true;

        foreach (var (input, expected) in testValues)
        {
            if (!ExecuteTest(input, expected, testFunc))
            {
                pass = false;
            }
        }

        return pass;
    }

    protected static bool ExecuteTest(string input, object expected, Func<string, object> testFunc)
    {
        var actual = testFunc(input);

        if (!actual.Equals(expected))
        {
            PrintTestFailure(input, expected, actual);
            return false;
        }

        return true;
    }
}
