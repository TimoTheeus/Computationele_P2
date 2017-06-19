using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Block
    {
        public int[,] grid;
        public bool[,] unchangable;
        int Nb;
        public int numberR;
        public int numberC;

        public Block()
        {
            Nb = Program.sqrN;
            grid = new int[Nb, Nb];
            unchangable = new bool[Nb, Nb];
        }

        public bool InBlock(int number)
        {
            for (int i = 0; i < Nb; i++)
                for (int j = 0; j < Nb; j++)
                    if (number == grid[i, j]) return true;
         return false;
        }

        public void Initialise()
        {
            List<int> domain = new List<int>();
            for (int i = 1; i <= Program.N; i++)
            {
                if(!InBlock(i)) domain.Add(i);
            }

            for (int i = 0; i < Nb; i++)
            {
                for (int j = 0; j < Nb; j++)
                {
                    if (!unchangable[i, j])
                    {
                        int number_index = Program.r.Next(0, domain.Count());
                        grid[i, j] = domain.ElementAt(number_index);
                        Program.sudoku[numberR * Nb + i, numberC * Nb + j] = domain.ElementAt(number_index);
                        domain.RemoveAt(number_index);
                        
                    }
                }
            }

        }
    }
    class Program
    {
        public static int N;
        public static int sqrN;
        public static Random r = new Random();
        public static Block[,] blocks;
        public static int[,] sudoku;
        public static int[] evalRow;
        public static int[] evalCol;
        
        static int EvalRow(int row)
        {
            List<int> numbers = new List<int>();
            for(int i = 0; i < N; i++)
            {
                int number = sudoku[row, i];
                if (!numbers.Contains(number))
                {
                    numbers.Add(number);
                }
            }
            return N - numbers.Count();
        }

        static int EvalCol(int col)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < N; i++)
            {
                int number = sudoku[i, col];
                if (!numbers.Contains(number))
                {
                    numbers.Add(number);
                }
            }
            return N - numbers.Count();
        }

        static void Main(string[] args)
        {
            while (true)
            {
                //Get a line of numbers
                string readline = Console.ReadLine();
                string[] line = readline.Split(' ');

                //Determine sudoku puzzle size
                N = line.Length;
                sqrN = (int)Math.Sqrt(N);

                blocks = new Block[sqrN,sqrN];
                sudoku = new int[N, N];

                for (int i = 0; i < sqrN; i++)
                    for (int j = 0; j < sqrN; j++)
                        blocks[i, j] = new Block();

                //Store lines
                for (int i = 0; i < N; i++)
                {
                    //Get the numbers in a line
                    if (i != 0)
                    {
                        readline = Console.ReadLine();
                        line = readline.Split(' ');
                    }

                    // Store numbers in sudoku array
                    for (int j = 0; j < N; j++)
                    {
                        int number = int.Parse(line[j]);
                        int numberrow = i % sqrN;
                        int numbercol = j % sqrN;
                        int blockrow = i / sqrN;
                        int blockcol = j / sqrN;
                        blocks[blockrow, blockcol].grid[numberrow, numbercol] = number;
                        blocks[blockrow, blockcol].numberR = blockrow;
                        blocks[blockrow, blockcol].numberC = blockcol;
                        if (number != 0)
                        {
                            blocks[blockrow, blockcol].unchangable[numberrow, numbercol] = true;
                            sudoku[i, j] = number;
                        }
                    }
                }

                for (int i = 0; i < sqrN; i++)
                    for (int j = 0; j < sqrN; j++)
                        blocks[i, j].Initialise();

                Printer p = new Printer();
                p.PrintSudoku2();
               
            }
        }
    }

    class Printer
    {
        public void PrintSudoku()
        {
            Console.WriteLine("----------------------------------------------------");
            int sqrN = Program.sqrN;
            int blockrow = 0;
            int numberrow = 0;

            while (blockrow < sqrN)
            {
                string line = "";
                for (int i = 0; i < sqrN; i++) //index of blockcolumn
                {
                    for (int j = 0; j < sqrN; j++) //index of number column
                    {
                        line += Program.blocks[blockrow, i].grid[numberrow, j] + " ";
                    }
                }

                Console.WriteLine(line);
                numberrow++;
                if (numberrow >= sqrN)
                {
                    numberrow = 0;
                    blockrow++;
                }
            }
        }

        public void PrintSudoku2()
        {
            int N = Program.N;
            Console.WriteLine(" "); Console.WriteLine(" ");
            Console.WriteLine("{0}x{1} sudoku", N, N);
            Console.WriteLine("found solution:");
            Console.WriteLine(" ");

            // Lines in the middle of the sudoku
            string longLine = "";
            int numberL = N + (int)Math.Sqrt(N);
            // For a 9x9 sudoku
            if (N < 10)
            {
                for (int i = 0; i < numberL - 1; i++)
                {
                    longLine += "- ";
                }
            }
            // For double digits sudoku
            else
            {
                int counter1 = 0;
                for (int i = 0; i < numberL - 1; i++)
                {
                    if (counter1 == 4)
                    {
                        longLine += "- ";
                        counter1 = 0;
                    }
                    else
                    {
                        longLine += "-- ";
                        counter1++;
                    }
                }
            }

            int number = 0;

            // Writing the solution
            for (int i = 0; i < N; i++)
            {
                if (i % Math.Sqrt(N) == 0 && i != 0)
                {
                    Console.WriteLine(longLine);
                }
                string line = "";
                int counter2 = 0;
                for (int j = 0; j < N; j++)
                {
                    number = Program.sudoku[i, j];

                    if (counter2 == Math.Sqrt(N) - 1 && j != N - 1)
                    {
                        if (number < 10 && N > 10)
                        {
                            line += number + "  | ";
                        }
                        else
                        {
                            line += number + " | ";
                        }
                        counter2 = 0;
                    }
                    else
                    {
                        if (number < 10 && N > 10)
                        {
                            line += number + "  ";
                        }
                        else
                        {
                            line += number + " ";
                        }
                        counter2++;
                    }
                }
                Console.WriteLine(line);
            }
        }
    }
}
