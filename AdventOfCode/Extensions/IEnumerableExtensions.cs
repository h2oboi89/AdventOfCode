namespace AdventOfCode.Extensions;

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

    public static IEnumerable<(T a, T b)> CartesianProduct<T>(this IEnumerable<T> enumerable) => enumerable.CartesianProduct(enumerable);

    public static IEnumerable<(TA a, TB b)> CartesianProduct<TA, TB>(this IEnumerable<TA> enumerableA, IEnumerable<TB> enumerableB)
    {
        foreach (var a in enumerableA)
        {
            foreach (var b in enumerableB)
            {
                yield return (a, b);
            }
        }
    }

    public static IEnumerable<(T a, T b)> UniqueProduct<T>(this IEnumerable<T> enumerable)
    {
        var pairs = new List<(T a, T b)>();

        // get all combinations
        var possiblePairs = enumerable.CartesianProduct();

        // filter out pairs matching against themselves
        possiblePairs = possiblePairs.Where(pair => pair.a != null && !pair.a.Equals(pair.b));

        // filter out pairs that we already saw reverse of (a, b) == (b, a)
        foreach (var p in possiblePairs)
        {
            if (!pairs.Contains((p.b, p.a)))
            {
                pairs.Add(p);
            }
        }

        return pairs;
    }

    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        if (!source.Any())
        {
            yield return new List<T>();
        }

        using var enumerator = source.GetEnumerator();

        var batch = new List<T>();
        while (enumerator.MoveNext())
        {
            batch.Add(enumerator.Current);

            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = [];
            }
        }

        if (batch.Count > 0)
        {
            yield return batch;
        };
    }
}
