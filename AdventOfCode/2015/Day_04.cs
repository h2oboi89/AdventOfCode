using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2015;

internal class Day_04 : BaseDay
{
    private readonly string input;

    public Day_04(string inputFile)
    {
        input = File.ReadAllText(inputFile).Trim();
    }

    private static string Hex(byte[] hash) => string.Join("", hash.Select(b => b.ToString("X2")));

    private static int Mine(string input)
    {
        using (var md5 = MD5.Create())
        {
            var seed = 0;
            while (true)
            {
                var value = $"{input}{seed}";

                var bytes = Encoding.ASCII.GetBytes(value);

                var hash = Hex(md5.ComputeHash(bytes));

                if (hash.Substring(0, 5).Equals("00000")) return seed;

                seed++;
            }
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

        return ExecuteTests(testValues, (i) => Mine(i));
    }

    [Test]
    public bool Test2() => true;

    [Part]
    public string Solve1() => $"{Mine(input)}";

    [Part]
    public string Solve2() => $"{string.Empty}";
}
