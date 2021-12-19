﻿using AdventOfCode.Common;

namespace AdventOfCode;

static class MultidimensionalArrayExtensions
{
    public static IEnumerable<Point2D> GetNeighborPoints<T>(this T[,] array, Point2D p, bool includeDiagonal)
    {
        // top left
        if (includeDiagonal)
        {
            if (p.Y != 0 && p.X != 0) yield return new Point2D(p.X - 1, p.Y - 1);
        }

        // top
        if (p.Y != 0) yield return new Point2D(p.X, p.Y - 1);

        // top right
        if (includeDiagonal)
        {
            if (p.Y != 0 && p.X != array.GetLength(1) - 1) yield return new Point2D(p.X + 1, p.Y - 1);
        }

        // right
        if (p.X != array.GetLength(1) - 1) yield return new Point2D(p.X + 1, p.Y);

        // bottom right
        if (includeDiagonal)
        {
            if (p.Y != array.GetLength(0) - 1 && p.X != array.GetLength(1) - 1) yield return new Point2D(p.X + 1, p.Y + 1);
        }

        // bottom
        if (p.Y != array.GetLength(0) - 1) yield return new Point2D(p.X, p.Y + 1);

        // bottom left
        if (includeDiagonal)
        {
            if (p.Y != array.GetLength(0) - 1 && p.X != 0) yield return new Point2D(p.X - 1, p.Y + 1);
        }

        // left
        if (p.X != 0) yield return new Point2D(p.X - 1, p.Y);
    }

    public static IEnumerable<T> GetNeighbors<T>(this T[,] array, Point2D p, bool includeDiagonals) =>
        array.GetNeighborPoints(p, includeDiagonals).Select(p => array.GetValue(p));

    public static T First<T>(this T[,] array) => array[0, 0];

    public static T Last<T>(this T[,] array) => array[array.GetLength(0) - 1, array.GetLength(1) - 1];

    public static T GetValue<T>(this T[,] array, Point2D p) => array[p.Y, p.X];

    public static void All<T>(this T[,] array, Action<Point2D> action)
    {
        for (var y = 0; y < array.GetLength(0); y++)
        {
            for (var x = 0; x < array.GetLength(1); x++)
            {
                action(new Point2D(x, y));
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

    public static IEnumerable<T> Each<T>(this T[,] array)
    {
        for (var y = 0; y < array.GetLength(0); y++)
        {
            for (var x = 0; x < array.GetLength(1); x++)
            {
                yield return array[y, x];
            }
        }
    }

    public static T[,] Clone<T>(this T[,] array, Func<T, T> tClone)
    {
        var clone = new T[array.GetLength(0), array.GetLength(1)];

        clone.All((x, y) => clone[y, x] = tClone(array[y, x]));

        return clone;
    }
}
