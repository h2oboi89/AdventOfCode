namespace AdventOfCode.Extensions;

static class Int32Extensions
{
    public static int NearestPowerOfTen(this int i)
    {
        return (int)Math.Round(Math.Pow(10, Math.Ceiling(Math.Log10(i))));
    }

    public static int NumberOfDigits(this int i)
    {
        return (int)Math.Floor(Math.Log10(i) + 1);
    }
}
