using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    using Aspose.Cells;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace MileniumLed.Domain.Services.Utils
    {
        public class ExcelService
        {
            public ExcelService()
            {
                Aspose.Cells.License cellsLicense = new Aspose.Cells.License();
                //alex esta lib la levantamos de localhost  cellsLicense.SetLicense("Aspose.Total.lic");
                cellsLicense.SetLicense("C:\\Users\\alex\\Desktop\\git repo 2\\localpayment\\BackgroundSide\\Resources\\Libs\\Aspose.Total.lic");
            }

            public void Export(DataSet ds, string path, string name, bool formatCurrency = false)
            {
                var finalPath = path + "\\" + name;
                var wb = new Workbook();
                

                
                var intSheet = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    wb.Worksheets.Add();
                    var sheet = wb.Worksheets[intSheet];

                    Style styleDate = wb.CreateStyle();

                    //Number 14 means Date format
                    styleDate.Number = 14;

                    Style styleNumber = wb.CreateStyle();
                    styleNumber.Number = 1;

                    Style styleDecimal = wb.CreateStyle();
                    styleDecimal.Number = 2;
                    if (formatCurrency)
                    {
                        styleDecimal.Number = 7;
                    }

                    StyleFlag flagSpecial = new StyleFlag();
                    flagSpecial.NumberFormat = true;

                    var intRow = 0;
                    var intCol = 0;

                    #region Titles
                    foreach (DataColumn column in dt.Columns)
                    {
                        var subtitleStyle = sheet.Cells[intRow, intCol].GetStyle();
                        subtitleStyle.Font.Name = "Arial";
                        subtitleStyle.Font.Size = 10;
                        subtitleStyle.Font.IsBold = true;

                        subtitleStyle.HorizontalAlignment = TextAlignmentType.Center;
                        //subtitleStyle.Pattern = BackgroundType.Solid;
                        subtitleStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Medium;
                        subtitleStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Medium;

                        sheet.Cells[intRow, intCol].PutValue(column.ColumnName);
                        sheet.Cells[intRow, intCol].SetStyle(subtitleStyle);

                        if (column.DataType == typeof(DateTime))
                        {
                            sheet.Cells.Columns[intCol].ApplyStyle(styleDate, flagSpecial);
                        }
                        /* else if (column.DataType == typeof(Int32))
                         {
                             sheet.Cells.Columns[intCol].ApplyStyle(styleNumber, flagSpecial);
                         }*/
                        else if (column.DataType == typeof(decimal))
                        {
                            sheet.Cells.Columns[intCol].ApplyStyle(styleDecimal, flagSpecial);
                        }

                        intCol++;
                    }
                    #endregion
                    intRow++;

                    foreach (DataRow row in dt.Rows)
                    {
                        intCol = 0;
                        foreach (DataColumn column in dt.Columns)
                        {
                            sheet.Cells[intRow, intCol].PutValue(row[column]);

                            intCol++;
                        }
                        intRow++;
                    }

                    intCol = 0;
                    intRow++;

                    sheet.AutoFitColumns();
                    sheet.AutoFitRows();

                    intSheet++;
                }
                

                wb.Save(finalPath, SaveFormat.Xlsx);
            }

            public void ExportTabs(DataSet ds, string path, string name, List<string> sheetNames = null)
            {
                var finalPath = path + "\\" + name;
                var wb = new Workbook();
                wb.Worksheets.RemoveAt(0);

                Style styleDate = wb.CreateStyle();

                //Number 14 means Date format
                styleDate.Number = 14;

                Style styleNumber = wb.CreateStyle();
                styleNumber.Number = 1;

                Style styleDecimal = wb.CreateStyle();
                styleNumber.Number = 2;

                StyleFlag flagSpecial = new StyleFlag();
                flagSpecial.NumberFormat = true;


                var datatableCount = 0;
                foreach (DataTable dt in ds.Tables)
                {
                    var intRow = 0;
                    var intCol = 0;
                    var index = 0;
                    Worksheet sheet;
                    if (sheetNames != null)
                    {
                        sheet = wb.Worksheets.Add(sheetNames[datatableCount]);
                    }
                    else 
                    {
                        index = wb.Worksheets.Add();
                        sheet = wb.Worksheets[index];
                    }
                     
                    #region Titles
                    foreach (DataColumn column in dt.Columns)
                    {
                        var subtitleStyle = sheet.Cells[intRow, intCol].GetStyle();
                        subtitleStyle.Font.Name = "Arial";
                        subtitleStyle.Font.Size = 10;
                        subtitleStyle.Font.IsBold = true;

                        subtitleStyle.HorizontalAlignment = TextAlignmentType.Center;
                        //subtitleStyle.Pattern = BackgroundType.Solid;
                        subtitleStyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Medium;
                        subtitleStyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Medium;

                        sheet.Cells[intRow, intCol].PutValue(column.ColumnName);
                        sheet.Cells[intRow, intCol].SetStyle(subtitleStyle);

                        if (column.DataType == typeof(DateTime))
                        {
                            sheet.Cells.Columns[intCol].ApplyStyle(styleDate, flagSpecial);
                        }
                        /* else if (column.DataType == typeof(Int32))
                         {
                             sheet.Cells.Columns[intCol].ApplyStyle(styleNumber, flagSpecial);
                         }*/
                        else if (column.DataType == typeof(decimal))
                        {
                            sheet.Cells.Columns[intCol].ApplyStyle(styleDecimal, flagSpecial);
                        }

                        intCol++;
                    }
                    #endregion
                    intRow++;

                    foreach (DataRow row in dt.Rows)
                    {
                        intCol = 0;
                        foreach (DataColumn column in dt.Columns)
                        {
                            sheet.Cells[intRow, intCol].PutValue(row[column]);

                            intCol++;
                        }
                        intRow++;
                    }

                    intCol = 0;
                    intRow++;

                    sheet.AutoFitColumns();
                    sheet.AutoFitRows();
                    datatableCount++;
                }

                wb.Save(finalPath, SaveFormat.Xlsx);
            }
        }
    }

}
