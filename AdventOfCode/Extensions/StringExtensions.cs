namespace AdventOfCode.Extensions;

static class StringExtensions
{
    public static IEnumerable<string> SplitInParts(this string s, int partLength)
    {
        ArgumentNullException.ThrowIfNull(s);

        if (partLength <= 0) throw new ArgumentException("Part length has to be positive.", nameof(partLength));

        for (var i = 0; i < s.Length; i += partLength)
        {
            yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }

    public static string Repeat(this string s, int count) => string.Join("", Enumerable.Repeat(s, count));

    public static IEnumerable<int> ParseCommaSeparatedInt32s(this string input) =>
        input.Split(",").Select(v => int.Parse(v));
}