using AdventOfCode.Common;

namespace AdventOfCode;

static class MultidimensionalArrayExtensions
{
    public static IEnumerable<Point> GetNeighbors<T>(this T[,] array, Point p, bool includeDiagonal)
    {
        // top left
        if (includeDiagonal)
        {
            if (p.Y != 0 && p.X != 0) yield return new Point(p.X - 1, p.Y - 1);
        }

        // top
        if (p.Y != 0) yield return new Point(p.X, p.Y - 1);

        // top right
        if (includeDiagonal)
        {
            if (p.Y != 0 && p.X != array.GetLength(1) - 1) yield return new Point(p.X + 1, p.Y - 1);
        }

        // right
        if (p.X != array.GetLength(1) - 1) yield return new Point(p.X + 1, p.Y);

        // bottom right
        if (includeDiagonal)
        {
            if (p.Y != array.GetLength(0) - 1 && p.X != array.GetLength(1) - 1) yield return new Point(p.X + 1, p.Y + 1);
        }

        // bottom
        if (p.Y != array.GetLength(0) - 1) yield return new Point(p.X, p.Y + 1);

        // bottom left
        if (includeDiagonal)
        {
            if (p.Y != array.GetLength(0) - 1 && p.X != 0) yield return new Point(p.X - 1, p.Y + 1);
        }

        // left
        if (p.X != 0) yield return new Point(p.X - 1, p.Y);

    }

    public static T GetValue<T>(this T[,] array, Point p) => array[p.Y, p.X];

    public static void All<T>(this T[,] array, Action<Point> action)
    {
        for (var y = 0; y < array.GetLength(0); y++)
        {
            for (var x = 0; x < array.GetLength(1); x++)
            {
                action(new Point(x, y));
            }
        }
    }

    public static void All<T>(this T[,] array, Action<int, int> action)
    {
        for (var y = 0; y < array.GetLength(0); y++)
        {
            for (var x = 0; x < array.GetLength(1); x++)
            {
                action(x, y);
            }
        }
    }

    public static T[,] Clone<T>(this T[,] array, Func<T, T> tClone)
    {
        var clone = new T[array.GetLength(0), array.GetLength(1)];

        clone.All((x, y) => clone[y, x] = tClone(array[y,x]));

        return clone;
    }
}
