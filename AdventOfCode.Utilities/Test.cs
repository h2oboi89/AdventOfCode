namespace AdventOfCode.Utilities;

/// <summary>
/// Identifies a <see cref="BaseDay"/> test function.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class Test : Attribute
{
    public Test() { }
}
