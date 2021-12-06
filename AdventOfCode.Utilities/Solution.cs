namespace AdventOfCode.Utilities;

public class Solution
{
    public readonly int Year;
    public readonly int Day;
    public readonly string TypeName;
    public TimeSpan Construction = TimeSpan.Zero;
    public TimeSpan RunTime = TimeSpan.Zero;
    public List<(string name, bool passed, TimeSpan duration)> Tests = new();
    public List<(string name, string result, TimeSpan duration)> Parts = new();

    public Solution(int year, int day, string typeName)
    {
        Year = year;
        Day = day;
        TypeName = typeName;
    }

    public string Name => $"{Year}.{TypeName}";

    public TimeSpan ExecutionTime
    {
        get
        {
            var result = Construction;

            foreach (var (_, __, duration) in Tests)
            {
                result += duration;
            }

            foreach (var (_, __, duration) in Parts)
            {
                result += duration;
            }

            return result;
        }
    }
}
