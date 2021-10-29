using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using WA_LP.Filters;
using WA_LP.Serializer;
using WA_LP.Serializer.Extensions;
namespace WA_LP.Controllers
{
    [CustomExceptionFilterAttribute]
    public class BaseApiController : ApiController
    {
        public string currentTimeZone;
        public string SerializeRequest()
        {
            return Request.SerializeRequest();
        }

        public IList ExcelBase64ToList(string file, string provider)
        {
            try
            {
                DataTable dt = new DataTable();
                Stream stream = null;

                byte[] bytes = Convert.FromBase64String(file);

                stream = new MemoryStream(bytes, 0, bytes.Length);

                using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(stream, false))
                {

                    WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                    IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                    string relationshipId = sheets.First().Id.Value;
                    WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                    Worksheet workSheet = worksheetPart.Worksheet;
                    SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                    IEnumerable<Row> rows = sheetData.Descendants<Row>();
                    if (provider == "ITAU")
                    {
                        dt = SetItauColumnNames<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.ExcelItauResponse>(dt);
                        dt = SetItauDataRowValues(spreadSheetDocument, dt, rows, 22);
                    }
                    else
                    {
                        foreach (Cell cell in rows.ElementAt(0))
                        {
                            dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                        }

                        foreach (Row row in rows) //this will also include your header row...
                        {
                            DataRow tempRow = dt.NewRow();

                            for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                            {
                                tempRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                            }

                            dt.Rows.Add(tempRow);
                        }

                        dt.Rows.RemoveAt(0); //remove the header
                    }

                }

                if (provider == "FASTCASH")
                {
                    return ConvertDataTable<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelFastcashResponse>(dt);
                }
                else if (provider == "ITAU")
                {
                    return ConvertDataTable<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.ExcelItauResponse>(dt);
                }
                else
                {
                    return ConvertDataTable<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelPluralResponse>(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("The upload file does not have a valid format. Error:" + ex.Message);
            }
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue != null ? cell.CellValue.InnerXml : "";

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        private static DataTable SetItauColumnNames<T>(DataTable dt)
        {
            Type temp = typeof(T);
            foreach (PropertyInfo pro in temp.GetProperties())
            {
                dt.Columns.Add(pro.Name);
            }
            return dt;
        }

        private static DataTable SetItauDataRowValues(SpreadsheetDocument shd, DataTable dt,  IEnumerable<Row> rows, int startRow) 
        {
            int x = 0;
            int y = 0;
            int cantOfRecords = 0;
            try
            {
                foreach (Row row in rows) //this will also include your header row...
                {
                    // get cantOfRecords
                    if (x == 10)
                    {
                        cantOfRecords = Int32.Parse(GetCellValue(shd, row.Descendants<Cell>().ElementAt(1)));
                    }

                    // get record
                    if (x >= startRow && y < cantOfRecords)
                    {
                        DataRow tempRow = dt.NewRow();
                        int cantOfColumns = dt.Columns.Count;
                        for (int z = 0; z < cantOfColumns; z++)
                        {
                            //int rowRecord = x + (i + 1);
                            tempRow[z] = GetCellValue(shd, row.Descendants<Cell>().ElementAt(z));
                        }
                        dt.Rows.Add(tempRow);
                        y++;
                    }

                    if (x >= startRow && y == cantOfRecords)
                    {
                        break;
                    }

                    x++;
                }
            }catch (Exception ex) 
            {
                throw new Exception("The upload file does not have a valid format. Error:" + ex.Message);
            }

            return dt;
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName.Replace(" ", string.Empty))
                    {
                        var cellValue = dr[column.ColumnName] == DBNull.Value ? string.Empty : dr[column.ColumnName];
                        pro.SetValue(obj, cellValue, null);
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

        public string ExcelFastCash(List<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelFastcash> excelData, string TypeReport)
        {
            string excelBase64 = "";
            List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "TID", FieldFromSP = "TID" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Amount", FieldFromSP = "Amount" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Payment Date", FieldFromSP = "Payment Date" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Bank Number", FieldFromSP = "Bank Number" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Bank Branch", FieldFromSP = "Bank Branch" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Bank Account Number", FieldFromSP = "Bank Account Number" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Bank Account Type", FieldFromSP = "Bank Account Type" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Beneficiary Name", FieldFromSP = "Beneficiary Name" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "Beneficiary Document", FieldFromSP = "Beneficiary Document" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Customer Name", FieldFromSP = "Customer Name" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Customer Email", FieldFromSP = "Customer Email" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "Customer Document", FieldFromSP = "Customer Document" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "Customer MobilePhone", FieldFromSP = "Customer MobilePhone" });
            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "Description", FieldFromSP = "Description" });

            lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

            DataTable dt = ConvertToDataTable(excelData);
            excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

            return excelBase64;
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(string.IsNullOrEmpty(prop.Description) ? prop.Name : prop.Description, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[string.IsNullOrEmpty(prop.Description) ? prop.Name : prop.Description] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public string GenerateExcel(List<SharedModel.Models.Export.Export.Excel> lExcelColumns, DataTable dt, string TypeReport)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            string excelBase64 = "";
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook, true))
            {
                List<OpenXmlAttribute> oxa;
                OpenXmlWriter oxw;
                Stylesheet stl = new Stylesheet();
                spreadsheetDocument.AddWorkbookPart();
                WorksheetPart wsp = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();



                oxw = OpenXmlWriter.Create(wsp);


                oxw.WriteStartElement(new Worksheet());

                oxw.WriteStartElement(new SheetData());



                for (int i = 0; i < dt.Rows.Count; i++) /* Bucle Columns */
                {
                    if (i == 0 && TypeReport != "RETENTIONMONTHLY") /* HEADERS */
                    {
                        oxa = new List<OpenXmlAttribute>();
                        oxa.Add(new OpenXmlAttribute("r", null, "1"));
                        oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Row(), oxa);

                        for (int y = 0; y < lExcelColumns.Count; y++) /* Listado de Columnas */
                        {
                            oxa = new List<OpenXmlAttribute>();
                            oxa.Add(new OpenXmlAttribute("t", null, "str"));
                            //oxa.Add(new OpenXmlAttribute("s", null, "2"));
                            oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell(), oxa);
                            oxw.WriteElement(new CellValue(lExcelColumns[y].DisplayName.ToString())); /* Se recupera el item del header */
                            oxw.WriteEndElement();
                        }
                        oxw.WriteEndElement();
                    }

                    oxa = new List<OpenXmlAttribute>();
                    if (TypeReport != "RETENTIONMONTHLY")
                    {
                        oxa.Add(new OpenXmlAttribute("r", null, (i + 2).ToString())); /* EL i + 2, es para generar el INDEX correcto para los rows, ya que el 1 es para los HEADERS. */

                    }
                    else
                    {
                        oxa.Add(new OpenXmlAttribute("r", null, (i + 1).ToString())); /* EL i + 2, es para generar el INDEX correcto para los rows, ya que el 1 es para los HEADERS. */

                    }
                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Row(), oxa);
                    //int cantColumns = lExcelColumns.Count > 0 ? lExcelColumns.Count : 17;

                    for (int j = 0; j < lExcelColumns.Count; ++j)
                    //for (int j = 0; j < cantColumns; ++j)
                    {
                        oxa = new List<OpenXmlAttribute>();

                        //oxa.Add(new OpenXmlAttribute("t", null, "str"));

                        //oxa.Add(new OpenXmlAttribute("s", null, "1"));
                        //string valueName = lExcelColumns.Count > 0 ? dt.Columns[lExcelColumns[j].FieldFromSP].DataType.FullName : dt.Columns[j].DataType.FullName;
                        switch (dt.Columns[lExcelColumns[j].FieldFromSP].DataType.FullName)
                        //switch (valueName)
                        {
                            case "System.DateTime":
                                oxa.Add(new OpenXmlAttribute("t", null, "str"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Date }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Date }, oxa);
                                    if (TypeReport == "RETENTIONMONTHLY")
                                    {

                                        oxw.WriteElement(new CellValue(Convert.ToDateTime(dt.Rows[i][lExcelColumns[j].FieldFromSP]).ToString("dd/MM/yyyy")));

                                    }
                                    else if (TypeReport == "MERCHANT_REPORT_XLSX")
                                    {
                                        oxw.WriteElement(new CellValue(TimeZoneInfo.ConvertTime(Convert.ToDateTime(dt.Rows[i][lExcelColumns[j].FieldFromSP]), TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone)).ToString("yyyy-MM-dd")));
                                    }
                                    else
                                    {
                                        oxw.WriteElement(new CellValue(TimeZoneInfo.ConvertTime(Convert.ToDateTime(dt.Rows[i][lExcelColumns[j].FieldFromSP]), TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById(currentTimeZone)).ToString("yyyy-MM-dd HH:mm:ss")));
                                    }

                                }
                                break;
                            case "System.Int16":
                                oxa.Add(new OpenXmlAttribute("t", null, "n"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(Convert.ToInt16(dt.Rows[i][lExcelColumns[j].FieldFromSP]).ToString()));
                                }
                                break;
                            case "System.Int32":
                                oxa.Add(new OpenXmlAttribute("t", null, "n"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(Convert.ToInt32(dt.Rows[i][lExcelColumns[j].FieldFromSP]).ToString()));
                                }
                                break;
                            case "System.Int64":
                                oxa.Add(new OpenXmlAttribute("t", null, "n"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(Convert.ToInt64(dt.Rows[i][lExcelColumns[j].FieldFromSP]).ToString()));
                                }
                                break;
                            case "System.Decimal":
                                oxa.Add(new OpenXmlAttribute("t", null, "n"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    if (TypeReport == "MERCHANT")
                                    {
                                        oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                        oxw.WriteElement(new CellValue(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString().Replace(',', '-')));

                                    }
                                    else if (TypeReport == "MERCHANT_REPORT_XLSX")
                                    {
                                        if (dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString() == "0")
                                        {
                                            oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                            oxw.WriteElement(new CellValue(""));
                                        }
                                        else
                                        {
                                            oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                            oxw.WriteElement(new CellValue(Convert.ToDecimal(dt.Rows[i][lExcelColumns[j].FieldFromSP]).ToString()));
                                        }
                                    }
                                    else
                                    {
                                        oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.Number, StyleIndex = 4 }, oxa);
                                        oxw.WriteElement(new CellValue(Convert.ToDecimal(dt.Rows[i][lExcelColumns[j].FieldFromSP]).ToString()));
                                    }

                                }
                                break;
                            case "System.Boolean":
                                oxa.Add(new OpenXmlAttribute("t", null, "str"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.String }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.String }, oxa);

                                    if (!string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                    {
                                        if (Convert.ToBoolean(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()) == true)
                                        {
                                            oxw.WriteElement(new CellValue("YES"));
                                        }
                                        else
                                        {
                                            oxw.WriteElement(new CellValue("NO"));
                                        }
                                    }
                                    else
                                    {
                                        oxw.WriteElement(new CellValue("NO"));
                                    }
                                }
                                break;
                            case "System.String":
                                oxa.Add(new OpenXmlAttribute("t", null, "str"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.String }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.String }, oxa);

                                    if (TypeReport == "RETENTIONMONTHLY")
                                    {

                                        if (lExcelColumns[j].FieldFromSP == "ImporteRet" || lExcelColumns[j].FieldFromSP == "BaseCalculo" || lExcelColumns[j].FieldFromSP == "ImporteComprbante")
                                        {
                                            string importe = dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString();
                                            oxw.WriteElement(new CellValue(importe.Replace(".", ",")));


                                        }
                                        else
                                        {
                                            oxw.WriteElement(new CellValue(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()));
                                        }

                                    }

                                    else
                                    {
                                        if (lExcelColumns[j].DisplayName == "Automatic")
                                        {
                                            if (dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString() == "MEC_AUTO")
                                            {
                                                oxw.WriteElement(new CellValue("YES"));
                                            }
                                            else
                                            {
                                                oxw.WriteElement(new CellValue("NO"));
                                            }
                                        }
                                        else
                                        {
                                            oxw.WriteElement(new CellValue(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()));
                                        }
                                    }

                                }
                                break;
                            default:
                                oxa.Add(new OpenXmlAttribute("t", null, "str"));
                                if (string.IsNullOrEmpty(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()))
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.String }, oxa);
                                    oxw.WriteElement(new CellValue(""));
                                }
                                else
                                {
                                    oxw.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell() { DataType = CellValues.String }, oxa);
                                    oxw.WriteElement(new CellValue(dt.Rows[i][lExcelColumns[j].FieldFromSP].ToString()));
                                }
                                break;
                        }

                        oxw.WriteEndElement();
                    }

                    oxw.WriteEndElement();

                }

                //wsp.Worksheet.AppendChild(columns);
                oxw.WriteEndElement();
                oxw.WriteEndElement();
                oxw.Close();
                //wsp.Worksheet = ws;
                oxw = OpenXmlWriter.Create(spreadsheetDocument.WorkbookPart);
                oxw.WriteStartElement(new Workbook());
                //oxw.WriteEndElement();

                oxw.WriteStartElement(new Sheets());
                //wsp.Worksheet.AppendChild(columns);
                string nameSheet = "";
                switch (TypeReport)
                {
                    case "ALLDATABASE":
                        nameSheet = "All Database Report";
                        break;
                    case "MERCHANT":
                        nameSheet = "Merchant Report";
                        break;
                    case "DETAILS_TRANSACTION_CLIENT":
                        nameSheet = "Txs Detail";
                        break;
                    case "HISTORICAL_FX":
                        nameSheet = "Historical FX";
                        break;
                    case "OPERATION_RETENTION":
                        nameSheet = "Merchant Withholding";
                        break;
                    case "MERCHANT_REPORT":
                        nameSheet = "Payoneer Merchant Report";
                        break;
                    case "TRANSACTION_REJECTED":
                        nameSheet = "Rejected Transactions Report";
                        break;
                    default:
                        ;
                        nameSheet = "-";
                        break;
                }

                oxw.WriteElement(new Sheet()
                {
                    Name = nameSheet,
                    SheetId = 1,
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(wsp)

                });

                oxw.WriteEndElement();
                oxw.WriteEndElement();
                oxw.Close();

                spreadsheetDocument.Close();
            }

            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);


            byte[] docBytes = memoryStream.ToArray();
            excelBase64 = Convert.ToBase64String(docBytes);
            return excelBase64;

        }
    }
}