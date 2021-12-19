using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    foreach of 3 axis for base matrix:
        foreach of 3 axis for test matrix:
            foreach of 4 rotations of test matrix:
                // TODO: try to match at least 12 points and add to candidate group
                // this will be 2D translation step
    
    foreach candidate in candidates:
        // TODO: unflatten and slide along axis until 12 points match or out of range
        // TODO: combine matrices if match and restart until only 1 matrix left
 */

namespace AdventOfCode._2021;

internal class Day_19 : BaseDay
{
    public Day_19(string inputFile) { }
}
