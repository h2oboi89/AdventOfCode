using AdventOfCode.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace AdventOfCode.Tests
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var solver = new Solver(Assembly.GetAssembly(typeof(Program)));
#pragma warning restore CS8604 // Possible null reference argument.

            var solutions = solver.RunTests().OrderBy(s => s.Year).ThenBy(s => s.Day);

            Console.WriteLine($"Found {solutions.Count()} solutions");

            var allPass = true;
            foreach (var solution in solutions)
            {
                foreach (var (name, result, _) in solution.Tests)
                {
                    if (result.Pass == false)
                    {
                        Console.WriteLine($"{solution.Name}.{name} : ");

                        foreach (var (pass, output) in result.Results)
                        {
                            Console.Write($" - Pass : {pass}");

                            if (pass)
                            {
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine($" : {output}");
                            }
                        }

                        allPass = false;
                    }
                }
            }

            Assert.IsTrue(allPass);
        }
    }
}