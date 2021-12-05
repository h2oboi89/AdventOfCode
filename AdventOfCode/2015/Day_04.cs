using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2015;

internal class Day_04 : BaseDay
{
    private readonly string input;

    public Day_04(string inputFile)
    {
        input = File.ReadAllText(inputFile).Trim();
    }

    private static string Hex(IEnumerable<byte> hash) => string.Join("", hash.Select(b => b.ToString("X2")));

    private static int Mine(string input, int requiredLength)
    {
        using var md5 = MD5.Create();

        var requiredStart = "0".Repeat(requiredLength);
        var byteCount = (int)Math.Ceiling(requiredLength / 2.0);

        var seed = 0;

        while (true)
        {
            var value = $"{input}{seed}";

            var bytes = Encoding.ASCII.GetBytes(value);

            var hash = Hex(md5.ComputeHash(bytes).Take(byteCount));

            if (hash.StartsWith(requiredStart)) return seed;

            seed++;
        }
    }

    [Test]
    public static bool Test1()
    {
        var testValues = new List<(string, object)>
        {
            ("abcdef", 609043),
            ("pqrstuv", 1048970),
        };

        return ExecuteTests(testValues, (i) => Mine(i, 5));
    }

    [Part]
    public string Solve1() => $"{Mine(input, 5)}";

    [Part]
    public string Solve2() => $"{Mine(input, 6)}";
}
