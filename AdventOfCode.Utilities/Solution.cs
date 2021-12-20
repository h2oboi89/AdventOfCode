namespace AdventOfCode.Utilities;

/// <summary>
/// Result class from running a <see cref="BaseDay"/> extending classes <see cref="DayTest"/> and <see cref="DayPart"/> functions.
/// </summary>
public class Solution
{
    /// <summary>
    /// Year this <see cref="Solution"/> belongs to.
    /// </summary>
    public readonly int Year;

    /// <summary>
    /// Specific <see cref="BaseDay"/> extending day class this <see cref="Solution"/> belongs to.
    /// </summary>
    public readonly int Day;

    /// <summary>
    /// Exact name of the class that extends <see cref="BaseDay"/>
    /// </summary>
    public readonly string TypeName;

    /// <summary>
    /// How long it takes constructor to execute.
    /// </summary>
    public TimeSpan Construction = TimeSpan.Zero;

    /// <summary>
    /// Overall actual run time for this <see cref="Solution"/>
    /// </summary>
    public TimeSpan RunTime = TimeSpan.Zero;

    /// <summary>
    /// Results of running days <see cref="DayTest"/> functions.
    /// </summary>
    public List<(string name, TestResult result, TimeSpan duration)> Tests = new();

    /// <summary>
    /// Results of running days <see cref="DayPart"/> functions.
    /// </summary>
    public List<(string name, string result, TimeSpan duration)> Parts = new();

    public Solution(int year, int day, string typeName)
    {
        Year = year;
        Day = day;
        TypeName = typeName;
    }

    /// <summary>
    /// Printable name for this <see cref="Solution"/> that should match <see cref="BaseDay"/> extending classes name and namespace.
    /// </summary>
    public string Name => $"{Year}.{TypeName}";

    /// <summary>
    /// Overall CPU run time for this <see cref="Solution"/>
    /// </summary>
    public TimeSpan ExecutionTime => TimeSpan.FromMilliseconds(
        Construction.TotalMilliseconds +
        Tests.Sum(r => r.duration.TotalMilliseconds) +
        Parts.Sum(r => r.duration.TotalMilliseconds));
}
