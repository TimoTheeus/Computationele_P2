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

            //Start Excel and get Application object.
            oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = true;

            //Get a new workbook.
            oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
            oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            //Add table headers going cell by cell.
            oSheet.Cells[1, 1] = "Sudoku Size";
            oSheet.Cells[1, 2] = "Runtime (seconds)";
            oSheet.Cells[1, 3] = "S value";

            //Format A1:D1 as bold, vertical alignment = center.
            oSheet.get_Range("A1", "C1").Font.Bold = true;
            oSheet.get_Range("A1", "D1").VerticalAlignment =
                Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            // Create an array to multiple values at once.
            double[] results = Program.results.ToArray();

            for (int i = 0; i < results.Length; i++)
            {
                oSheet.Cells[i + 2, 1] = Program.N + "x" + Program.N;
                oSheet.Cells[i + 2, 2] = results[i];
                oSheet.Cells[i + 2, 3] = Program.S;
            }
            



            //AutoFit columns A:D.
            oRng = oSheet.get_Range("A1", "C1");
            oRng.EntireColumn.AutoFit();

            oXL.Visible = false;
            oXL.UserControl = false;
            oWB.SaveAs("D:\\Github\\Computationele_P2\\ConsoleApp1\\ConsoleApp1\\test501.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();

            //...


        }
    }
}
