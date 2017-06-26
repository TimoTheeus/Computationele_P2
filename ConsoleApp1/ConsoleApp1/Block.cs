using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    //Location class for a row and column
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

    //a Sqrt(N)xSqrt(N) block
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

        //Returns true or false whether or not a number is in the block
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
            for (int i = 0; i < Nb; i++)
            {
                for (int j = 0; j < Nb; j++)
                {
                    if (!unchangable[i, j])
                    {
                        swappable[counter] = new Location(i, j);
                        counter++;
                    };
                }
            }
            //Swap all swappables with eachother
            for (int i = 0; i < swappable.Length; i++)
            {
                for (int j = i + 1; j < swappable.Length; j++)
                {
                    if (swappable[i] != null && swappable[j] != null)
                        Swap(swappable[i].row, swappable[i].col, swappable[j].row, swappable[j].col);
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
                int temp = Program.sudoku[row1, col1];
                Program.sudoku[row1, col1] = Program.sudoku[row2, col2];
                Program.sudoku[row2, col2] = temp;
            }

        }
        public void Swap(int row1, int col1, int row2, int col2)
        {
            //Copy the original sudoku
            int[,] tempSud = new int[Program.N, Program.N];
            for (int i = 0; i < Program.N; i++)
                for (int j = 0; j < Program.N; j++)
                    tempSud[i, j] = Program.sudoku[i, j];


            //Get the global rows and columns
            int globalRow1 = get_global_row(row1);
            int globalCol1 = get_global_col(col1);
            int globalRow2 = get_global_row(row2);
            int globalCol2 = get_global_col(col2);
            int temp = tempSud[globalRow1, globalCol1];
            tempSud[globalRow1, globalCol1] = tempSud[globalRow2, globalCol2];
            tempSud[globalRow2, globalCol2] = temp;
            //Get the new evaluation of rows and columns changed in the swap
            int row1_eval = Program.EvalRow(tempSud, globalRow1); int col1_eval = Program.EvalCol(tempSud, globalCol1);
            int row2_eval = Program.EvalRow(tempSud, globalRow2); int col2_eval = Program.EvalCol(tempSud, globalCol2);

            int eval_difference = (Program.evalRow[globalRow1] - row1_eval) + (Program.evalCol[globalCol1] - col1_eval)
                + (Program.evalRow[globalRow2] - row2_eval) + (Program.evalCol[globalCol2] - col2_eval);

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

        //Random swap of two values
        public void RandomSwap()
        {
            //Get 2 random rows and columns
            int randomRow1 = Program.r.Next(0, Nb);
            int randomCol1 = Program.r.Next(0, Nb);
            int randomRow2 = Program.r.Next(0, Nb);
            int randomCol2 = Program.r.Next(0, Nb);
            //Check if they're not changeable
            if (!unchangable[randomRow1, randomCol1] && !unchangable[randomRow2, randomCol2])
            {
                //Get the global rows
                int globalRow1 = get_global_row(randomRow1); int globalCol1 = get_global_col(randomCol1);
                int globalRow2 = get_global_row(randomRow2); int globalCol2 = get_global_col(randomCol2);
                //Swap the two around
                int temp = Program.sudoku[globalRow1, globalCol1];
                Program.sudoku[globalRow1, globalCol1] = Program.sudoku[globalRow2, globalCol2];
                Program.sudoku[globalRow2, globalCol2] = temp;
            }
        }

        //Get global row of a row in the block
        int get_global_row(int localrow)
        {
            return numberR * Nb + localrow;
        }

        //Get global column of a column in the block
        int get_global_col(int localcol)
        {
            return numberC * Nb + localcol;
        }

        //Initialise a block with the correct numbers in a random order
        public void Initialise()
        {
            //Make a new list
            List<int> domain = new List<int>();
            for (int i = 1; i <= Program.N; i++)
            {
                //If a number from 1 to N is not in the block add them to the list
                if (!InBlock(i)) domain.Add(i);
            }

            for (int i = 0; i < Nb; i++)
            {
                for (int j = 0; j < Nb; j++)
                {
                    if (!unchangable[i, j])
                    {
                        //Get a random number in the list and place them in a non unchangeable value
                        int number_index = Program.r.Next(0, domain.Count());
                        grid[i, j] = domain.ElementAt(number_index);
                        Program.sudoku[numberR * Nb + i, numberC * Nb + j] = domain.ElementAt(number_index);
                        domain.RemoveAt(number_index);
                    }
                }
            }
        }
    }
}
