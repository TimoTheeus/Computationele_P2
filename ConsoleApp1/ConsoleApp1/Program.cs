using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Location
    {
        public int row;
        public int col;
        public Location(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    class Block
    {
        public int[,] grid;
        public bool[,] unchangable;
        int Nb;
        public int numberR;
        public int numberC;
        int[,] bestswap_rowsandcols;
        int[,] bestswap_rowandcol_evaluations;
        int bestswap_evalvalue;
        bool swap_was_found;
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

        public void PickBestSwap()
        {
            bestswap_evalvalue = 0;
            bestswap_rowsandcols = new int[2, 2];
            bestswap_rowandcol_evaluations = new int[2, 2];
            swap_was_found = false;
            //Get swappable locations
            Location[] swappable = new Location[Program.N];
            int counter = 0;
            for(int i = 0; i < Nb; i++)
            {
                for(int j = 0; j < Nb; j++)
                {
                    if (!unchangable[i, j])
                    {
                        swappable[counter] = new Location(i, j);
                        counter++;
                    };
                }
            }
            //Swap all swappables with eachother
            for(int i = 0; i < swappable.Length; i++)
            {
                for( int j = i + 1; j < swappable.Length; j++)
                {
                    if(swappable[i]!=null&&swappable[j]!=null)
                         Swap(swappable[i].row,swappable[i].col, swappable[j].row, swappable[j].col);
                }
            }
            //If a better or equally good swap wasnt found
            if (!swap_was_found)
            {
                Program.stagnation_counter++;
                return;
            }
            else
            {
                //If no improvement was found
                if (bestswap_evalvalue == 0) Program.stagnation_counter++;
                else Program.stagnation_counter = 0;

                //the to swap rows and columns
                int row1 = bestswap_rowsandcols[0, 0]; int col1 = bestswap_rowsandcols[0, 1];
                int row2 = bestswap_rowsandcols[1, 0]; int col2 = bestswap_rowsandcols[1, 1];
                //Adjust the evaluation value of the affected rows and columns (max 2 rows and 2 columns)
                Program.evalRow[row1] = bestswap_rowandcol_evaluations[0, 0];
                Program.evalCol[col1] = bestswap_rowandcol_evaluations[0, 1];
                Program.evalRow[row2] = bestswap_rowandcol_evaluations[1, 0];
                Program.evalCol[col2] = bestswap_rowandcol_evaluations[1, 1];
                //Adjust global evaluation value
                Program.evaluation_value -= bestswap_evalvalue;

                //Swap the values in the sudoku
                int temp = Program.sudoku[row1,col1];
                Program.sudoku[row1, col1] = Program.sudoku[row2,col2];
                Program.sudoku[row2, col2] = temp;
            }
            
        }
        public void Swap(int row1,int col1,int row2,int col2)
        {
            //Copy the original sudoku
            int[,] tempSud = new int[Program.N, Program.N];
            for(int i = 0; i < Program.N; i++)
                for (int j = 0; j < Program.N; j++)
                    tempSud[i, j] = Program.sudoku[i, j];

            
            //Get the global rows and columns
            int globalRow1 = get_global_row(row1);
            int globalCol1 = get_global_col(col1);
            int globalRow2 = get_global_row(row2);
            int globalCol2 = get_global_col(col2);
            int temp = tempSud[globalRow1,globalCol1];
            tempSud[globalRow1, globalCol1] = tempSud[globalRow2, globalCol2];
            tempSud[globalRow2, globalCol2] = temp;
            //Get the new evaluation of rows and columns changed in the swap
            int row1_eval = Program.EvalRow(tempSud, globalRow1); int col1_eval = Program.EvalCol(tempSud, globalCol1);
            int row2_eval = Program.EvalRow(tempSud, globalRow2); int col2_eval = Program.EvalCol(tempSud, globalCol2);

            int eval_difference = (Program.evalRow[globalRow1] - row1_eval) + (Program.evalCol[globalCol1] - col1_eval)
                + (Program.evalRow[globalRow2]-row2_eval) + (Program.evalCol[globalCol2]-col2_eval);

            //if the evaluation is better than the current best swap, store the swap values
            if (eval_difference >= bestswap_evalvalue)
            {
                swap_was_found = true;
                bestswap_evalvalue = eval_difference;
                bestswap_rowandcol_evaluations[0, 0] = row1_eval;
                bestswap_rowandcol_evaluations[0, 1] = col1_eval;
                bestswap_rowandcol_evaluations[1, 0] = row2_eval;
                bestswap_rowandcol_evaluations[1, 1] = col2_eval;
                bestswap_rowsandcols[0, 0] = globalRow1; bestswap_rowsandcols[0, 1] = globalCol1;
                bestswap_rowsandcols[1, 0] = globalRow2; bestswap_rowsandcols[1, 1] = globalCol2;
            }
        }

        public void RandomSwap()
        {
            int randomRow1 = Program.r.Next(0, Nb);
            int randomCol1 = Program.r.Next(0, Nb);
            int randomRow2 = Program.r.Next(0, Nb);
            int randomCol2 = Program.r.Next(0, Nb);
            if (!unchangable[randomRow1, randomCol1]&&!unchangable[randomRow2,randomCol2])
            {
                int globalRow1 = get_global_row(randomRow1); int globalCol1 = get_global_col(randomCol1);
                int globalRow2 = get_global_row(randomRow2); int globalCol2 = get_global_col(randomCol2);
                int temp = Program.sudoku[globalRow1, globalCol1];
                Program.sudoku[globalRow1, globalCol1] = Program.sudoku[globalRow2, globalCol2];
                Program.sudoku[globalRow2, globalCol2] = temp;
            }
        }

        int get_global_row(int localrow)
        {
            return numberR * Nb + localrow;
        }
        int get_global_col(int localcol)
        {
            return numberC * Nb + localcol;
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
        public const int amountOfSudokus = 50;
        public static int evaluation_value;
        public static Printer printer = new Printer();
        public static ExcelWriter excel_writer = new ExcelWriter();
        public static int stagnation_counter;
        static int threshold = 1000;
        public static int S = 100;
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
