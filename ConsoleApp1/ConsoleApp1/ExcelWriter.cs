using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ConsoleApp1
{
    class ExcelWriter
    {
        public ExcelWriter()
        {

        }
        public void WriteStuff()
        {
            Application oXL;
            _Workbook oWB;
            _Worksheet oSheet;
            Range oRng;
            object misvalue = System.Reflection.Missing.Value;

           // try
           /// {
                //Start Excel and get Application object.
                oXL = new Application();
                oXL.Visible = true;

                //Get a new workbook.
                oWB = (_Workbook)(oXL.Workbooks.Add(""));
                oSheet = (_Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "Sudoku Size";
                oSheet.Cells[1, 2] = "Runtime (milliseconds)";
                oSheet.Cells[1, 3] = "S value";
                oSheet.Cells[1, 4] = "Random Walk Threshhold";
                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "D1").Font.Bold = true;
                oSheet.get_Range("A1", "D1").VerticalAlignment =
                     XlVAlign.xlVAlignCenter;

                // Create an array to multiple values at once.
                double[] results = Program.results.ToArray();

                for (int i = 0; i < results.Length; i++)
                {
                    oSheet.Cells[i + 2, 1] = Program.N + "x" + Program.N;
                    oSheet.Cells[i + 2, 2] = results[i];
                    oSheet.Cells[i + 2, 3] = Program.S;
                    oSheet.Cells[i + 2, 4] = Program.threshold;
                }

                int meanValueIndex = Program.amountOfSudokus + 3;
                int standarddeviationIndex = Program.amountOfSudokus + 4;
                oSheet.Cells[meanValueIndex, 1] = "Mean Runtime:";
                oSheet.Cells[standarddeviationIndex, 1] = "Standard Deviation:";

                //Set mean and standarddeviation to bold
                oSheet.get_Range("A"+ meanValueIndex, "A"+ standarddeviationIndex).Font.Bold = true;
                Range rng = oSheet.Range["B"+meanValueIndex];
                rng.Formula = String.Format("=AVERAGE(B2:B{0}", Program.amountOfSudokus + 1);
                rng.FormulaHidden = false;
               // oSheet.Unprotect();

                Range rng2 = oSheet.Range["B" + standarddeviationIndex];
                rng2.Formula = String.Format("=STDEVP(B2:B{0})", Program.amountOfSudokus + 1);
                rng2.FormulaHidden = false;

                //AutoFit columns A:D.
                oRng = oSheet.get_Range("A1", "D1");
                oRng.EntireColumn.AutoFit();

                oXL.Visible = false;
                oXL.UserControl = false;
                string location = "D:\\Github\\Computationele_P2\\ConsoleApp1\\ConsoleApp1\\";
                string name = String.Format("{0}x{0}__S={1}__threshhold={2}.xls", Program.N, Program.S,Program.threshold);
                oWB.SaveAs(location+name, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                oWB.Close();
          //  }
          //  catch
           // {
           //     Console.WriteLine("error writing to excel file");
           // }
        }
    }
}
