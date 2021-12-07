﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common;

internal class Point
{
    public readonly int X, Y;

    public Point(int x, int y) { X = x; Y = y; }

    public override string ToString() => $"( {X}, {Y} )";
}
