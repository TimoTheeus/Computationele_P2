﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{   
    class Program
    {
        public static int N;
        public static int sqrN;
        public static Random r = new Random();
        public static Block[,] blocks;
        public static int[,] sudoku;
        public static int[] evalRow;
        public static int[] evalCol;
        public const int amountOfSudokus = 50;
        public static int evaluation_value;
        public static Printer printer = new Printer();
        public static ExcelWriter excel_writer = new ExcelWriter();
        public static int stagnation_counter;
        public static int threshold = 500;
        public static int S = 50;
        static public List<double> results;
        static int sudoku_number;

        public static int EvalRow(int[,] sudoku, int row)
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

       public static int EvalCol(int[,] sudoku,int col)
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
            results = new List<double>();
            sudoku_number = 0;
            while (true)
            {
                stagnation_counter = 0;
                //Get a line of numbers
                string readline = Console.ReadLine();
                string split = string.Join(" ", readline.ToCharArray());
                string[] line = split.Split(' ');

                //Determine sudoku puzzle size
                N = line.Length;
                sqrN = (int)Math.Sqrt(N);

                blocks = new Block[sqrN, sqrN];
                sudoku = new int[N, N];
                evalCol = new int[N];
                evalRow = new int[N];

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
                        split = string.Join(" ", readline.ToCharArray());
                        line = split.Split(' ');
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

                InitialiseEvaluation();
                DateTime starttime = DateTime.Now;
                while (evaluation_value != 0)
                {
                    if (stagnation_counter > threshold)
                    {
                        //random walk
                        stagnation_counter = 0;
                        for (int i = 0; i < S; i++)
                        {
                            //Choose random blockrow and column
                            int stagRow = r.Next(0, sqrN);
                            int stagCol = r.Next(0, sqrN);

                            blocks[stagRow, stagCol].RandomSwap();
                        }
                        InitialiseEvaluation();
                    }
                    //Choose random blockrow and column
                    int blockRow = r.Next(0, sqrN);
                    int blockCol = r.Next(0, sqrN);

                    blocks[blockRow, blockCol].PickBestSwap();
                   //printer.PrintSudoku2();
                  // Console.WriteLine("row: {0}, col: {1}", blockRow, blockCol);
                  //Console.WriteLine("stag counter: {0}       eval: {1}", stagnation_counter, evaluation_value);
                }
                TimeSpan runtime = DateTime.Now- starttime;
                printer.PrintSudoku2();
                Console.WriteLine("found solution");
                string testResult = String.Format("Sudoku {1}: \nRuntime: {0} seconds, value of S = {2}", runtime.TotalMilliseconds, sudoku_number, S);
                results.Add(runtime.TotalMilliseconds);
                Console.WriteLine(runtime.TotalMilliseconds);
                sudoku_number++;
                if (sudoku_number == amountOfSudokus)
                {
                    excel_writer.WriteStuff();
                }
            }
        }
        
        static void InitialiseEvaluation()
        {
            evaluation_value = 0;
            for(int i = 0; i < N; i++)
            {
                int row_evaluation = EvalRow(sudoku,i);
                evalRow[i] = row_evaluation;
                evaluation_value += row_evaluation;
            }
            for(int j =0; j < N; j++)
            {
                int col_evaluation = EvalCol(sudoku,j);
                evalCol[j] = col_evaluation;
                evaluation_value += col_evaluation;
            }
        }
    }
}
