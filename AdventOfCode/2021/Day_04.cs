﻿using System.Text;
using AdventOfCode.Extensions;

namespace AdventOfCode._2021;

internal class Day_04 : BaseDay
{
    private class BingoBoard
    {
        private readonly Space[,] Spaces = new Space[5, 5];

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

        private class Space
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

    private readonly (IEnumerable<int> values, IEnumerable<BingoBoard> boards) input, testInput = (new List<int>(), new List<BingoBoard>());

    public Day_04(string intputFile)
    {
        string? values = null;
        var cards = new List<string>();
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
                if (isTest)
                {
                    testInput = ParseInput(values, cards);
                }
                else
                {
                    input = ParseInput(values, cards);
                }
                
                values = null;
                cards.Clear();
                isTest = false;
                continue;
            }

            // lines are usually card values except for cases above
            cards.Add(line);
        }
    }

    private static (IEnumerable<int>, IEnumerable<BingoBoard>) ParseInput(string values, IEnumerable<string> cards)
    {
        var intValues = values.Split(',').Select(v => int.Parse(v));

        var boards = new List<BingoBoard>();
        var boardValues = new List<int>();

        foreach (var row in cards)
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

    [DayTest]
    public TestResult Test1() => ExecuteTest(4512, () => PlayToWin(testInput.values, testInput.boards));

    [DayTest]
    public TestResult Test2() => ExecuteTest(1924, () => PlayToLose(testInput.values, testInput.boards));

    [DayPart]
    public string Solve1() => $"{PlayToWin(input.values, input.boards)}";

    [DayPart]
    public string Solve2() => $"{PlayToLose(input.values, input.boards)}";
}
