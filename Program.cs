using System;
using System.Collections.Generic;

namespace ConnectFourMinimax
{
    public class GameState
    {
        public const int Rows = 6, Cols = 7;
        public const char Empty = '.', AI = 'X', Human = 'O';
        public char[,] Board = new char[Rows, Cols];

        public GameState()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++) Board[r, c] = Empty;
        }

        public GameState(char[,] board)
        {
            Array.Copy(board, Board, Rows * Cols);
        }

        // Отрисовка цветного игрового поля
        public void PrintBoard()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" 0 1 2 3 4 5 6");
            Console.WriteLine("---------------");
            Console.ResetColor();

            for (int r = 0; r < Rows; r++)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("|");
                Console.ResetColor();

                for (int c = 0; c < Cols; c++)
                {
                    char cell = Board[r, c];
                    if (cell == Human)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; // Фишка человека — Жёлтая
                        Console.Write(cell);
                    }
                    else if (cell == AI)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;    // Фишка ИИ — Красная
                        Console.Write(cell);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray; // Пустая клетка — Серая
                        Console.Write(cell);
                    }
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("|");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("---------------\n");
            Console.ResetColor();
        }

        public List<int> GetValidMoves()
        {
            var moves = new List<int>();
            for (int c = 0; c < Cols; c++)
                if (Board[0, c] == Empty) moves.Add(c);
            return moves;
        }

        public GameState ApplyMove(int col, char player)
        {
            var newState = new GameState(Board);
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (newState.Board[r, col] == Empty)
                {
                    newState.Board[r, col] = player;
                    break;
                }
            }
            return newState;
        }

        public bool CheckWin(char p)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    if (Board[r, c] != p) continue;
                    if (c + 3 < Cols && Board[r, c + 1] == p && Board[r, c + 2] == p && Board[r, c + 3] == p) return true;
                    if (r + 3 < Rows && Board[r + 1, c] == p && Board[r + 2, c] == p && Board[r + 3, c] == p) return true;
                    if (r + 3 < Rows && c + 3 < Cols && Board[r + 1, c + 1] == p && Board[r + 2, c + 2] == p && Board[r + 3, c + 3] == p) return true;
                    if (r - 3 >= 0 && c + 3 < Cols && Board[r - 1, c + 1] == p && Board[r - 2, c + 2] == p && Board[r - 3, c + 3] == p) return true;
                }
            return false;
        }

        public bool IsTerminal() => CheckWin(AI) || CheckWin(Human) || GetValidMoves().Count == 0;

        public int Evaluate()
        {
            if (CheckWin(AI)) return 1000;
            if (CheckWin(Human)) return -1000;

            int score = 0;
            for (int r = 0; r < Rows; r++)
                if (Board[r, 3] == AI) score += 5;

            return score;
        }
    }

    public class MinimaxSolver
    {
        public static (int score, int bestMove) Minimax(GameState state, int depth, bool isMax)
        {
            var moves = state.GetValidMoves();
            if (depth == 0 || state.IsTerminal())
                return (state.Evaluate(), -1);

            int bestVal = isMax ? int.MinValue : int.MaxValue;
            int bestMove = moves[0];

            foreach (int col in moves)
            {
                var next = state.ApplyMove(col, isMax ? GameState.AI : GameState.Human);
                int val = Minimax(next, depth - 1, !isMax).score;

                if (isMax ? val > bestVal : val < bestVal)
                {
                    bestVal = val;
                    bestMove = col;
                }
            }
            return (bestVal, bestMove);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var state = new GameState();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== Четыре в ряд (Connect Four) ===");
            Console.ResetColor();

            Console.Write("Хотите ходить первым? (y/n): ");
            bool humanTurn = Console.ReadLine()?.ToLower() == "y";

            while (!state.IsTerminal())
            {
                state.PrintBoard();
                if (humanTurn)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Ваш ход (выберите колонку {string.Join(",", state.GetValidMoves())}): ");
                    Console.ResetColor();

                    if (int.TryParse(Console.ReadLine(), out int col) && state.GetValidMoves().Contains(col))
                    {
                        state = state.ApplyMove(col, GameState.Human);
                        humanTurn = false;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ИИ думает...");
                    Console.ResetColor();

                    var (_, bestMove) = MinimaxSolver.Minimax(state, depth: 4, isMax: true);
                    state = state.ApplyMove(bestMove, GameState.AI);
                    humanTurn = true;
                }
            }

            state.PrintBoard();

            if (state.CheckWin(GameState.AI))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Победил ИИ!");
            }
            else if (state.CheckWin(GameState.Human))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Вы победили!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Ничья!");
            }
            Console.ResetColor();
        }
    }
}