namespace AdventOfCode;

static class IEnumerableExtensions
{
    public static uint Sum(this IEnumerable<uint> enumerable)
    {
        uint sum = 0;

        foreach (var value in enumerable)
        {
            sum += value;
        }

        return sum;
    }

    public static ulong Sum(this IEnumerable<ulong> enumerable)
    {
        ulong sum = 0;

        foreach (var value in enumerable)
        {
            sum += value;
        }

        return sum;
    }
}
