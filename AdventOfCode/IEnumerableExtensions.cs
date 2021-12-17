namespace AdventOfCode;

static class IEnumerableExtensions
{
    public static ulong Sum(this IEnumerable<ulong> enumerable)
    {
        ulong sum = 0;

        foreach (var value in enumerable)
        {
            sum += value;
        }

        return sum;
    }
    public static int Product(this IEnumerable<int> enumerable) => enumerable.Aggregate(1, (acc, val) => acc * val);

    public static ulong Product(this IEnumerable<ulong> enumerable) => enumerable.Aggregate((ulong)1, (acc, val) => acc * val);
}
