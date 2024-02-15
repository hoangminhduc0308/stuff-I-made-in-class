using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using static System.Random;
using static System.Environment;

namespace Bingo {
    internal class Program {
        static void shuffle(ref int[] arr, ref Random rng) {
            int n = arr.Length;
            while (n > 1) {
                int k = rng.Next(n--);
                int temp = arr[n];
                arr[n] = arr[k];
                arr[k] = temp;
            }
        }

        static int count_valid_lines(bool[,] valid_board) {
            int edge_length = valid_board.GetLength(0);
            int res = 0;
            int t = 0;

            // horizontal
            for (int i = 0; i < edge_length; i++) {
                t = 0;
                for (int j = 0; j < edge_length; j++) {
                    t += Convert.ToInt32(valid_board[i, j]);
                }
                if (t == edge_length)
                    res += 1;
            }

            // vertical
            for (int i = 0; i < edge_length; i++) {
                t = 0;
                for (int j = 0; j < edge_length; j++) {
                    t += Convert.ToInt32(valid_board[j, i]);
                }
                if (t == edge_length)
                    res += 1;
            }

            // diagonal
            t = 0;
            for (int i = 0; i < edge_length; i++) {
                t += Convert.ToInt32(valid_board[i, i]);
            }
            if (t == edge_length)
                res += 1;

            // reverse diagonal
            t = 0;
            for (int i = 0; i < edge_length; i++) {
                t += Convert.ToInt32(valid_board[i, edge_length - 1 - i]);
            }
            if (t == edge_length)
                res += 1;

            return res;
        }

        static string repeat(string s, int n) {
            string x = "";
            for (int i = 0; i < n; i++) {
                x += s;
            }
            return x;
        }

        static string bingo_shout(int n) {
            if (n >= 5)
                return "BING" + repeat("O", n - 4);
            else {
                const string res = "BING";
                string x = "";
                for (int i = 0; i < n; i++) {
                    x += res[i];
                }
                return x;
            }
        }

        static void display(int[,] game_board, bool[,] valid_board) {
            /*
             * +--+--+--+--+--+
             * |  |  |  |  |  |
             * +--+--+--+--+--+
             * |  |  |  |  |  |
             * +--+--+--+--+--+
             * |  |  |  |  |  |
             * +--+--+--+--+--+
             * |  |  |  |  |  |
             * +--+--+--+--+--+
             * |  |  |  |  |  |
             * +--+--+--+--+--+
             *
             */

            int edge_length = game_board.GetLength(0);
            int squared_digits =
                (edge_length * edge_length * 2).ToString().Length;

            for (int i = 0; i < edge_length; i++) {
                for (int j = 0; j < edge_length; j++) {
                    Console.Write("+" + repeat("-", squared_digits));
                }
                Console.WriteLine("+");
                for (int j = 0; j < edge_length; j++) {
                    Console.Write("|");
                    if (valid_board[i, j]) {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.Write(
                        game_board[i, j].ToString($"D{squared_digits}"));
                    if (valid_board[i, j]) {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Console.WriteLine("|");
            }
            for (int i = 0; i < edge_length; i++) {
                Console.Write("+" + repeat("-", squared_digits));
            }
            Console.WriteLine("+");
            string progress = bingo_shout(count_valid_lines(valid_board));
            Console.Write(" ");
            for (int i = 0; i < progress.Length; i++) {
                Console.Write(progress[i] + repeat(" ", squared_digits));
            }
            Console.WriteLine();
        }

        static void press_any_key() {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
        }
        static void Main(string[] args) {
            const int edge_length = 9;
            const int board_size = edge_length * edge_length;

            int[,] player_board = new int[edge_length, edge_length];
            int[,] computer_board = new int[edge_length, edge_length];

            bool[,] player_valid = new bool[edge_length, edge_length];
            bool[,] computer_valid = new bool[edge_length, edge_length];

            int[] num_list = new int[board_size * 2];
            for (int i = 0; i < num_list.Length; i++) {
                num_list[i] = i + 1;
            }

            for (int i = 0; i < edge_length; i++) {
                for (int j = 0; j < edge_length; j++) {
                    player_valid[i, j] = false;
                    computer_valid[i, j] = false;
                }
            }

            Random rng = new Random();
            shuffle(ref num_list, ref rng);

            for (int i = 0; i < board_size; i++) {
                computer_board[i / edge_length, i % edge_length] = num_list[i];
            }

            shuffle(ref num_list, ref rng);

            for (int i = 0; i < board_size; i++) {
                player_board[i / edge_length, i % edge_length] = num_list[i];
            }

            shuffle(ref num_list, ref rng);
            for (int i = 0; i < num_list.Length &&
                            count_valid_lines(player_valid) <= edge_length &&
                            count_valid_lines(computer_valid) <= edge_length;
                 i++) {
                bool end = false;
                Console.Clear();
                Console.WriteLine($"Current number: {num_list[i]}");
                for (int j = 0; j < edge_length; j++) {
                    for (int k = 0; k < edge_length; k++) {
                        if (player_board[j, k] == num_list[i])
                            player_valid[j, k] = true;
                        if (computer_board[j, k] == num_list[i])
                            computer_valid[j, k] = true;
                    }
                }
                display(player_board, player_valid);
                display(computer_board, computer_valid);
                if (count_valid_lines(player_valid) > edge_length &&
                    count_valid_lines(computer_valid) > edge_length) {
                    Console.WriteLine("Tie!");
                    end = true;

                } else if (count_valid_lines(computer_valid) > edge_length) {
                    Console.WriteLine("Computer wins!");
                    end = true;
                } else if (count_valid_lines(player_valid) > edge_length) {
                    Console.WriteLine("You win!");
                    end = true;
                }

                press_any_key();
                if (end) {
                    Exit(0);
                }
            }
        }
    }
}
