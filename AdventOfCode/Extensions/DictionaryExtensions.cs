namespace AdventOfCode.Extensions;

static class DictionaryExtensions
{
    public static void AddOrUpdate<TKey>(this Dictionary<TKey, ulong> dict, TKey key) where TKey : notnull
    {
        if (!dict.TryGetValue(key, out ulong value))
        {
            dict.Add(key, 0);
        }

        dict[key] = ++value;
    }

    public static void AddOrUpdate<TKey>(this Dictionary<TKey, ulong> dict, TKey key, ulong value) where TKey : notnull
    {
        if (!dict.TryAdd(key, value))
        {
            dict[key] += value;
        }
    }
}
