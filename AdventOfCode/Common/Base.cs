using System.Text;

namespace AdventOfCode.Common;

internal static class Base
{
    public static string ToHexString(IEnumerable<byte> bytes) => string.Join("", bytes.Select(b => b.ToString("X2")));

    public static IEnumerable<byte> HexToBytes(string hexString)
    {
        var bytes = new List<byte>();

        for(var i = 0; i < hexString.Length; i+= 2)
        {
            bytes.Add(Convert.ToByte(hexString.Substring(i, 2), 16));
        }

        return bytes;
    }

    public static string ToBinaryString(IEnumerable<byte> bits)
    {
        var bitString = new StringBuilder();

        foreach(var bit in bits)
        {
            bitString.Append(bit);
        }

        return bitString.ToString();
    }
}
