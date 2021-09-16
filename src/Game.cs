using System;
using System.Text;

namespace Compdog.TicTacToe
{
    public enum Cell
    {
        A1 = 0,
        A2 = 1,
        A3 = 2,
        B1 = 3,
        B2 = 4,
        B3 = 5,
        C1 = 6,
        C2 = 7,
        C3 = 8
    }

    public class BoardUpdateEventArgs : EventArgs
    {
        public int Board { get; }
        public int Cell { get; }
        public bool OldValue { get; }
        public bool NewValue { get; }

        public BoardUpdateEventArgs(int board, Cell cell, bool oldValue, bool newValue)
        {
            Board = board;
            Cell = (int)cell;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public BoardUpdateEventArgs(int board, int cell, bool oldValue, bool newValue)
        {
            Board = board;
            Cell = cell;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class Game
    {
        public static int CELL_COUNT => CELL_LOOKUP_TABLE.Length;

        public Random Random { get; }
        public int RandomSeed { get; }

        public int BoardCount => boards.Length;
        public event EventHandler<BoardUpdateEventArgs> BoardUpdate;

        private static readonly int MIN_BOARD_COUNT = 0;
        private static readonly UInt64 DE_BRUIJN_SEQUENCE = 0x37E84A99DAE458F;
        private static readonly UInt32[] CELL_LOOKUP_TABLE = new UInt32[9] { 0x80080080, 0x40008000, 0x20000808, 0x08040000, 0x04004044, 0x02000400, 0x00820002, 0x00402000, 0x00200220 };

        private static readonly int[] MULTIPLY_DE_BRUIJN_BIT_POSITION = {
            0, 1, 17, 2, 18, 50, 3, 57,
            47, 19, 22, 51, 29, 4, 33, 58,
            15, 48, 20, 27, 25, 23, 52, 41,
            54, 30, 38, 5, 43, 34, 59, 8,
            63, 16, 49, 56, 46, 21, 28, 32,
            14, 26, 24, 40, 53, 37, 42, 7,
            62, 55, 45, 31, 13, 39, 36, 6,
            61, 44, 12, 35, 60, 11, 10, 9,
        };

        private UInt32[] boards;

        public Game(int boardCount, int seed)
        {
            boards = new UInt32[MIN_BOARD_COUNT + boardCount];
            RandomSeed = seed;
            Random = new Random(RandomSeed);
        }

        public UInt32 GetBoardRaw(int board)
        {
            if (board < MIN_BOARD_COUNT) throw new ArgumentOutOfRangeException("board", board, "Argument was below MIN_BOARD_COUNT.");
            if (board >= BoardCount) throw new ArgumentOutOfRangeException("board", board, "Argument was above BoardCount.");

            return boards[board];
        }

        public void Clear(int board)
        {
            if (board < MIN_BOARD_COUNT) throw new ArgumentOutOfRangeException("board", board, "Argument was below MIN_BOARD_COUNT.");
            if (board >= BoardCount) throw new ArgumentOutOfRangeException("board", board, "Argument was above BoardCount.");

            boards[board] = 0x00000000;
            m_boardUpdate(board, -1, false, false);
        }

        public void Clear(int board, Cell cell)
        {
            Clear(board, (int)cell);
        }

        public void Clear(int board, int cell)
        {
            if (board < MIN_BOARD_COUNT) throw new ArgumentOutOfRangeException("board", board, "Argument was below MIN_BOARD_COUNT.");
            if (board >= BoardCount) throw new ArgumentOutOfRangeException("board", board, "Argument was above BoardCount.");

            if (cell < 0 || cell >= CELL_COUNT) throw new ArgumentOutOfRangeException("cell", cell, "Argument was outside the bounds of the board.");

            bool before = Taken(board, cell);
            boards[board] &= ~CELL_LOOKUP_TABLE[cell];
            m_boardUpdate(board, cell, before, false);
        }

        public bool Taken(int board, Cell cell)
        {
            return Taken(board, (int)cell);
        }

        public bool Taken(int board, int cell)
        {
            if (board < MIN_BOARD_COUNT) throw new ArgumentOutOfRangeException("board", board, "Argument was below MIN_BOARD_COUNT.");
            if (board >= BoardCount) throw new ArgumentOutOfRangeException("board", board, "Argument was above BoardCount.");

            if (cell < 0 || cell >= CELL_COUNT) throw new ArgumentOutOfRangeException("cell", cell, "Argument was outside the bounds of the board.");

            return (boards[board] & CELL_LOOKUP_TABLE[cell]) != 0;
        }

        public void Move(int board, Cell cell)
        {
            Move(board, (int)cell);
        }

        public void Move(int board, int cell)
        {
            if (!TryMove(board, cell, out string error)) throw new InvalidOperationException(error);
        }

        public bool TryMove(int board, Cell cell, out string errorMessage)
        {
            return TryMove(board, (int)cell, out errorMessage);
        }

        public bool TryMove(int board, int cell, out string errorMessage)
        {
            if (board < MIN_BOARD_COUNT) throw new ArgumentOutOfRangeException("board", board, "Argument was below MIN_BOARD_COUNT.");
            if (board >= BoardCount) throw new ArgumentOutOfRangeException("board", board, "Argument was above BoardCount.");

            if (cell < 0 || cell >= CELL_COUNT) throw new ArgumentOutOfRangeException("cell", cell, "Argument was outside the bounds of the board.");

            for (int i = 0; i < BoardCount; ++i)
            {
                if (Taken(i, cell))
                {
                    errorMessage = $"Cell already taken by 'player_{i}'.";
                    return false;
                }
            }

            boards[board] |= CELL_LOOKUP_TABLE[cell];
            m_boardUpdate(board, cell, false, true);

            errorMessage = "OK";
            return true;
        }

        public int GetWin(int board)
        {
            if (board < MIN_BOARD_COUNT) throw new ArgumentOutOfRangeException("board", board, "Argument was below MIN_BOARD_COUNT.");
            if (board >= BoardCount) throw new ArgumentOutOfRangeException("board", board, "Argument was above BoardCount.");

            UInt32 check = boards[board] & (boards[board] << 1) & (boards[board] >> 1);
            if (check == 0) return -1;

            int bsf = (m_bitScanForward(check) - 2) >> 2;
            return bsf;
        }

        public override string ToString()
        {
            return ToString(new[] { 'X', 'O' });
        }

        public string ToString(char[] boardChars)
        {
            return ToString(boardChars, "\n-|-|-\n");
        }

        public string ToString(char[] boardChars, string rowBreak)
        {
            return ToString(boardChars, rowBreak, "|");
        }

        public string ToString(char[] boardChars, string rowBreak, string cellSpacer)
        {
            return ToString(boardChars, rowBreak, cellSpacer, ' ');
        }

        public string ToString(char[] boardChars, string rowBreak, string cellSpacer, char empty)
        {
            return ToString(boardChars, rowBreak, cellSpacer, empty, '?');
        }

        public string ToString(char[] boardChars, string rowBreak, string cellSpacer, char empty, char unknown)
        {
            int[] b = new int[BoardCount];
            for (int i = 0; i < BoardCount; ++i) b[i] = i;

            return m_prettyPrintBoards(b, boardChars, rowBreak, cellSpacer, empty, unknown);
        }

        public string ToString(int[] boards)
        {
            return ToString(boards, new[] { 'X', 'O' });
        }

        public string ToString(int[] boards, char[] boardChars)
        {
            return ToString(boards, boardChars, "\n-|-|-\n");
        }

        public string ToString(int[] boards, char[] boardChars, string rowBreak)
        {
            return ToString(boards, boardChars, rowBreak, "|");
        }

        public string ToString(int[] boards, char[] boardChars, string rowBreak, string cellSpacer)
        {
            return ToString(boards, boardChars, rowBreak, cellSpacer, ' ');
        }

        public string ToString(int[] boards, char[] boardChars, string rowBreak, string cellSpacer, char empty)
        {
            return ToString(boards, boardChars, rowBreak, cellSpacer, empty, '?');
        }

        public string ToString(int[] boards, char[] boardChars, string rowBreak, string cellSpacer, char empty, char unknown)
        {
            return m_prettyPrintBoards(boards, boardChars, rowBreak, cellSpacer, empty, unknown);
        }

        public string ToString(int board)
        {
            return ToString(board, '+');
        }

        public string ToString(int board, char boardChar)
        {
            return ToString(board, boardChar, "\n-|-|-\n");
        }

        public string ToString(int board, char boardChar, string rowBreak)
        {
            return ToString(board, boardChar, rowBreak, "|");
        }

        public string ToString(int board, char boardChar, string rowBreak, string cellSpacer)
        {
            return ToString(board, boardChar, rowBreak, cellSpacer, ' ');
        }

        public string ToString(int board, char boardChar, string rowBreak, string cellSpacer, char empty)
        {
            return m_prettyPrintBoards(new[] { board }, new[] { boardChar }, rowBreak, cellSpacer, empty, '?');
        }

        private int m_bitScanForward(ulong b)
        {
            if (b <= 0) throw new ArgumentOutOfRangeException("Target number should not be zero");
            return MULTIPLY_DE_BRUIJN_BIT_POSITION[((ulong)((long)b & -(long)b) * DE_BRUIJN_SEQUENCE) >> 58];
        }

        private void m_boardUpdate(int board, int cell, bool oldValue, bool newValue)
        {
            BoardUpdate?.Invoke(this, new BoardUpdateEventArgs(board, cell, oldValue, newValue));
        }

        private string m_prettyPrintBoards(int[] boards, char[] boardChars, string rowBreak, string cellSpacer, char empty, char unknown)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < CELL_COUNT; ++i)
            {
                if (i > 0 && i % 3 != 0) sb.Append(cellSpacer);
                if (i > 0 && i % 3 == 0) sb.Append(rowBreak);

                bool prev = false;
                int tN = -2;
                for (int j = 0; j < boards.Length; j++)
                {
                    if (Taken(boards[j], i))
                    {
                        if (prev)
                        {
                            tN = -1;
                            break;
                        }

                        prev = true;
                        tN = j;
                    }
                }

                if (tN == -2) sb.Append(empty);
                else if (tN == -1) sb.Append(unknown);
                else if (tN < boardChars.Length) sb.Append(boardChars[tN]);
                else sb.Append(tN);
            }

            return sb.ToString();
        }
    }
}