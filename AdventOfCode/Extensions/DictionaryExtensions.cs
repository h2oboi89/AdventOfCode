namespace AdventOfCode.Extensions;

static class DictionaryExtensions
{
    public static void AddOrUpdate<TKey>(this Dictionary<TKey, ulong> dict, TKey key) where TKey : notnull
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, 0);
        }

        dict[key]++;
    }

    public static void AddOrUpdate<TKey>(this Dictionary<TKey, ulong> dict, TKey key, ulong value) where TKey : notnull
    {
        if (!dict.ContainsKey(key))
        {
            dict.Add(key, value);
        }
        else
        {
            dict[key] += value;
        }
    }
}
