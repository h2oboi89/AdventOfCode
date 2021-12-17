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

    public static IEnumerable<(T, T)> CartesianProduct<T>(this IEnumerable<T> enumerable) => enumerable.CartesianProduct(enumerable);

    public static IEnumerable<(TA, TB)> CartesianProduct<TA, TB>(this IEnumerable<TA> enumerableA, IEnumerable<TB> enumerableB)
    {
        foreach (var a in enumerableA)
        {
            foreach(var b in enumerableB)
            {
                yield return (a, b);
            }
        }
    }
}
