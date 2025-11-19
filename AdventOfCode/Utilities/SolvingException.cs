namespace AdventOfCode.Utilities;

public class SolvingException : Exception
{
    public SolvingException(string message) : base(message) { }

    public SolvingException(string message, Exception innerException) : base(message, innerException) { }
}
