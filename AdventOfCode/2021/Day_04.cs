using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode._2021;

internal class Day_04 : BaseDay
{
    private class BingoBoard
    {
        public readonly Space[,] Spaces = new Space[5, 5];

        public BingoBoard(IEnumerable<int> values)
        {
            int x = 0, y = 0;
            foreach (var value in values)
            {
                Spaces[y, x] = new Space(value);

                x += 1;
                x %= Spaces.GetLength(0);

                if (x == 0)
                {
                    y++;
                }
            }
        }

        public class Space
        {
            public readonly int Value;

            public bool Marked = false;
            public Space(int value)
            {
                Value = value;
            }

            public override string ToString() => $"{(Marked ? 'x' : ' ')}{Value:D2}";
        }

        private IEnumerable<List<Space>> GetRows()
        {
            var row = new List<Space>();

            foreach (var space in Spaces)
            {
                row.Add(space);

                if (row.Count == Spaces.GetLength(0))
                {
                    yield return row;
                    row = new List<Space>();
                }
            }
        }

        private IEnumerable<List<Space>> GetColumns()
        {
            var column = new List<Space>();

            for (var y = 0; y < Spaces.GetLength(0); y++)
            {
                for (var x = 0; x < Spaces.GetLength(1); x++)
                {
                    var space = Spaces[x, y];

                    column.Add(space);
                }

                yield return column;
                column = new List<Space>();
            }
        }

        public bool Mark(int value)
        {
            var result = false;
            foreach (var space in Spaces)
            {
                if (space.Value == value)
                {
                    space.Marked = true;
                    result = true;
                }
            }

            return result;
        }

        public IEnumerable<int> GetUnmarked()
        {
            foreach (var space in Spaces)
            {
                if (space.Marked) continue;

                yield return space.Value;
            }
        }

        public bool HasWon()
        {
            foreach (var row in GetRows())
            {
                if (row.All(s => s.Marked))
                {
                    return true;
                }
            }

            foreach (var column in GetColumns())
            {
                if (column.All(s => s.Marked))
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var rows = GetRows();

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(" ", row.Select(s => s.ToString())));
            }

            return sb.ToString();
        }
    }

    private readonly IEnumerable<int> PartValues;
    private readonly IEnumerable<int> TestValues;

    private readonly IEnumerable<BingoBoard> PartBoards;
    private readonly IEnumerable<BingoBoard> TestBoards;

    public Day_04(string intputFile)
    {
        string? values = null;
        var cards = new StringBuilder();
        var isTest = true;

        foreach (var line in File.ReadAllLines(intputFile))
        {
            if (string.IsNullOrEmpty(line)) continue; // ignore empty lines

            if (values == null) // first line in each section is values
            {
                values = line;
                continue;
            }

            if (line.StartsWith("#")) // marks end of input section (test and part)
            {
                var (intValues, boards) = ParseInput(values, cards.ToString());
                
                if (isTest)
                {
                    TestValues = intValues;
                    TestBoards = boards;
                }
                else
                {
                    PartValues = intValues;
                    PartBoards = boards;
                }
                
                values = null;
                cards.Clear();
                isTest = false;
                continue;
            }

            // lines are usually card values except for cases above
            cards.AppendLine(line);
        }
    }

    private (IEnumerable<int>, IEnumerable<BingoBoard>) ParseInput(string values, string cards)
    {
        var intValues = values.Split(',').Select(v => int.Parse(v));

        var boards = new List<BingoBoard>();
        var boardValues = new List<int>();

        foreach (var row in cards.Split(Environment.NewLine))
        {
            boardValues.AddRange(row.SplitInParts(3).Select(v => int.Parse(v)));

            if (boardValues.Count == 25)
            {
                boards.Add(new BingoBoard(boardValues));
                boardValues.Clear();
            }
        }

        return (intValues, boards);
    }

    private static int PlayToWin(IEnumerable<int> values, IEnumerable<BingoBoard> boards)
    {
        foreach (var value in values)
        {
            foreach (var board in boards)
            {
                if (board.Mark(value))
                {
                    if (board.HasWon())
                    {
                        return board.GetUnmarked().Sum() * value;
                    }
                }
            }
        }

        return 0;
    }

    private static int PlayToLose(IEnumerable<int> values, IEnumerable<BingoBoard> boards)
    {
        var boardsInPlay = boards.ToList();

        foreach (var value in values)
        {
            var boardsToRemove = new List<BingoBoard>();

            foreach (var board in boardsInPlay)
            {
                if (board.Mark(value))
                {
                    if (board.HasWon())
                    {
                        boardsToRemove.Add(board);
                    }
                }
            }

            foreach(var board in boardsToRemove)
            {
                boardsInPlay.Remove(board);

                if (boardsInPlay.Count == 0)
                {
                    return board.GetUnmarked().Sum() * value;
                }
            }
        }

        return 0;
    }

    [Test]
    public bool Test1() => ExecuteTest(string.Empty, 4512, (_) => PlayToWin(TestValues, TestBoards));

    [Test]
    public bool Test2() => ExecuteTest(string.Empty, 1924, (_) => PlayToLose(TestValues, TestBoards));

    [Part]
    public string Solve1() => $"{PlayToWin(PartValues, PartBoards)}";

    [Part]
    public string Solve2() => $"{PlayToLose(PartValues, PartBoards)}";
}
