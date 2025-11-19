using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2022
{
    internal class Day_07 : BaseDay
    {
        private readonly List<string> input = [];

        private const string testInput = """
        $ cd /
        $ ls
        dir a
        14848514 b.txt
        8504156 c.dat
        dir d
        $ cd a
        $ ls
        dir e
        29116 f
        2557 g
        62596 h.lst
        $ cd e
        $ ls
        584 i
        $ cd ..
        $ cd ..
        $ cd d
        $ ls
        4060174 j
        8033020 d.log
        5626152 d.ext
        7214296 k
        """;

        public Day_07(string inputFile)
        {
            foreach (var line in File.ReadAllLines(inputFile))
            {
                input.Add(line);
            }
        }

        private class TerminalStatement
        {
            public readonly string Command;
            public readonly List<string> Output = [];

            public TerminalStatement(string command)
            {
                Command = command;
            }
        }

        private abstract class FileSystemNode
        {
            public FileSystemNode? Parent = null;
            public readonly List<FileSystemNode> Children = [];

            public string Name { get; }
            public abstract int Size { get; }

            protected FileSystemNode(FileSystemNode? parent, string name) { Parent = parent; Name = name; }
        }

        private class DirectoryNode : FileSystemNode
        {
            public override int Size => Children.Sum(node => node.Size);

            public DirectoryNode(FileSystemNode? parent, string name) : base(parent, name) { }
        }

        private class FileNode : FileSystemNode
        {
            public override int Size { get; }

            public FileNode(FileSystemNode? parent, string name, int size) : base(parent, name) { Size = size; }
        }

        private static int FindDirsUnderSize(IEnumerable<string> input, int sizeLimit) =>
            GetDirectorySizes(ConstructFileSystem(input)).Where(dir => dir.size < sizeLimit).Sum(dir => dir.size);

        private static int FindSmallestDirToDelete(IEnumerable<string> input)
        {
            var directorySizes = GetDirectorySizes(ConstructFileSystem(input));

            var max = 70_000_000;
            var maxForUpdate = max - 30_000_000;
            var current = directorySizes.Find(node => node.name == "/").size;
            var spaceRequired = current - maxForUpdate;

            return directorySizes.Where(node => spaceRequired < node.size).OrderBy(node => node.size).First().size;
        }

        private static List<(string name, int size)> GetDirectorySizes(DirectoryNode fileSystem)
        {
            var directorySizes = new List<(string name, int size)>();

            var root = fileSystem.Children.First();

            static IEnumerable<(string, int)> VisitNode(FileSystemNode node)
            {
                var directorySizes = new List<(string, int)>
                {
                    (node.Name, node.Size)
                };

                foreach (var child in node.Children)
                {
                    if (child is DirectoryNode)
                    {
                        directorySizes.AddRange(VisitNode(child));
                    }
                }

                return directorySizes;
            }

            directorySizes.AddRange(VisitNode(root));
            return directorySizes;
        }

        private static DirectoryNode ConstructFileSystem(IEnumerable<string> input)
        {
            var fileSystem = new DirectoryNode(null, string.Empty);
            var currentDirectory = fileSystem;

            foreach (var statement in ParseInput(new Queue<string>(input)))
            {
                if (statement.Command.StartsWith("cd"))
                {
                    var dir = statement.Command.Split(' ')[1];

                    switch (dir)
                    {
                        case "/":
                            var newDir = new DirectoryNode(fileSystem, dir);
                            fileSystem.Children.Add(newDir);
                            currentDirectory = newDir;
                            break;
                        case "..":
                            currentDirectory = (DirectoryNode)(currentDirectory.Parent ?? currentDirectory);
                            break;
                        default:
                            foreach (var child in currentDirectory.Children)
                            {
                                if (child.Name == dir)
                                {
                                    currentDirectory = (DirectoryNode)child;
                                    break;
                                }
                            }
                            break;
                    }
                }
                else if (statement.Command == "ls")
                {
                    foreach (var output in statement.Output)
                    {
                        if (output.StartsWith("dir"))
                        {
                            currentDirectory.Children.Add(new DirectoryNode(currentDirectory, output.Split(' ')[1]));
                        }
                        else
                        {
                            var parts = output.Split(' ');
                            var size = int.Parse(parts[0]);
                            var name = parts[1];

                            currentDirectory.Children.Add(new FileNode(currentDirectory, name, size));
                        }
                    }
                }
            }

            return fileSystem;
        }

        private static IEnumerable<TerminalStatement> ParseInput(Queue<string> input)
        {
            while (input.Any())
            {
                var current = input.Dequeue().Trim();

                if (current.StartsWith("$"))
                {
                    var command = new TerminalStatement(current[2..]);

                    if (command.Command == "ls")
                    {
                        while (input.Any() && !input.Peek().StartsWith("$"))
                        {
                            command.Output.Add(input.Dequeue());
                        }
                    }

                    yield return command;
                }
            }
        }

        [DayTest]
        public static TestResult Test_1() => ExecuteTest(95437, () => FindDirsUnderSize(testInput.Split(Environment.NewLine), 100_000));

        [DayTest]
        public static TestResult Test_2() => ExecuteTest(24933642, () => FindSmallestDirToDelete(testInput.Split(Environment.NewLine)));

        [DayPart]
        public string Solve_1() => $"{FindDirsUnderSize(input, 100_000)}";

        [DayPart]
        public string Solve_2() => $"{FindSmallestDirToDelete(input)}";
    }
}
