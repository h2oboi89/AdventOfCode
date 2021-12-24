using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021;

internal class Day_24 : BaseDay
{
    public Day_24(string inputFile)
    {

    }

    private class ALU
    {
        public int W { get; private set; } = 0;
        public int X { get; private set; } = 0;
        public int Y { get; private set; } = 0;
        public int Z { get; private set; } = 0;

        private void SetRegister(string c, int value)
        {
            switch (c)
            {
                case "w": W = value; break;
                case "x": X = value; break;
                case "y": Y = value; break;
                case "z": Z = value; break;
                default: throw new Exception($"Invalid register: '{c}'");
            }
        }

        private int GetRegister(string c)
        {
            switch (c)
            {
                case "w": return W;
                case "x": return X;
                case "y": return Y;
                case "z": return Z;
                default: throw new Exception($"Invalid register: '{c}'");
            }
        }

        private void Input(string a, int b) => SetRegister(a, b);

        private void Add(string r, int a, int b) => SetRegister(r, a + b);

        private void Multiply(string r, int a, int b) => SetRegister(r, a * b);

        private void Divide(string r, int a, int b) => SetRegister(r, a / b);

        private void Modulo(string r, int a, int b) => SetRegister(r, a < 0 ? ((a + 1) % b + b - 1) : a % b);

        private void Equal(string r, int a, int b) => SetRegister(r, a == b ? 1 : 0);

        private void Execute((string operation, string a, string b) instruction)
        {
            var (operation, a, b) = instruction;

            int aValue = GetRegister(a);

            if (!int.TryParse(b, out var bValue)) bValue = GetRegister(b);

            switch (operation)
            {
                case "inp": Input(a, bValue); break;
                case "add": Add(a, aValue, bValue); break;
                case "mul": Multiply(a, aValue, bValue); break;
                case "div": Divide(a, aValue, bValue); break;
                case "mod": Modulo(a, aValue, bValue); break;
                case "eql": Equal(a, aValue, bValue); break;
            }
        }

        public void Execute(IEnumerable<(string operation, string a, string b)> instructions)
        {
            foreach(var instruction in instructions)
            {
                Execute(instruction);
            }
        }
    }
}
