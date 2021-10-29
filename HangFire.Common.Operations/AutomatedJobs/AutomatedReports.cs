using Newtonsoft.Json;
using SharedModel.Models.View;
using System;
using System.Collections.Generic;
using Tools;
using System.Diagnostics;
using System.Configuration;
using SharedBusiness.Log;
using System.IO;
using Hangfire;
using System.Data;
using System.ComponentModel;
using System.Text;
using System.Web;
using Tools.Dto;

namespace HangFire.Common.Operations.AutomatedJobs
{
    public class AutomatedReports
    {
        [Queue("payoneercolqueue")]
        public static void GenerateAndUploadMerchantReportCSVcol(string configParamsName)
        {
            GenerateAndUploadMerchantReportCSV(configParamsName);
        }
        [Queue("payoneerargqueue")]
        public static void GenerateAndUploadMerchantReportCSVarg(string configParamsName)
        {
            GenerateAndUploadMerchantReportCSV(configParamsName);
        }

        [Queue("payoneerbraqueue")]
        public static void GenerateAndUploadMerchantReportCSVbra(string configParamsName)
        {
            GenerateAndUploadMerchantReportCSV(configParamsName);
        }

        [Queue("payoneermexqueue")]
        public static void GenerateAndUploadMerchantReportCSVmex(string configParamsName)
        {
            GenerateAndUploadMerchantReportCSV(configParamsName);
        }

        [Queue("payoneerchlqueue")]
        public static void GenerateAndUploadMerchantReportCSVchl(string configParamsName)
        {
            GenerateAndUploadMerchantReportCSV(configParamsName);
        }

        [Queue("payoneeruryqueue")]
        public static void GenerateAndUploadMerchantReportCSVury(string configParamsName)
        {
            GenerateAndUploadMerchantReportCSV(configParamsName);
        }

        [Queue("niumreportsqueue")]
        public static void GenerateAndUploadNiumMerchantReportsJob()
        {
            GenerateAndUploadNiumReports();
        }
        public static void GenerateAndUploadMerchantReportCSV(string configParamsName)
        {
            string localBackupFolder = @"C:\MERCHANT_CSV_BACKUP";
            try
            {
                LogService.LogInfo("Executing Merchant Automated CSV Report with Params: " + configParamsName);

                // Checking/Creating local backup folder
                if (!System.IO.Directory.Exists(localBackupFolder))
                {
                    LogService.LogInfo("Creating Local Backup Folder: " + localBackupFolder);
                    System.IO.Directory.CreateDirectory(localBackupFolder);
                }


                SharedBusiness.View.BIReport BiReport = new SharedBusiness.View.BIReport();

                string resultBase64 = "";

                // setting report vars and config
                string customer = "robot@localpayment.com";
                SharedModel.Models.Export.Export.Request exportData = new SharedModel.Models.Export.Export.Request();
                exportData.TypeReport = "MERCHANT_REPORT_CSV";

                string idEntityAccount = ConfigurationManager.AppSettings[configParamsName + "ENTITY_ACCOUNT_ID"].ToString();
                exportData.requestReport = new Report.List.Request();
                exportData.requestReport.idEntityAccount = idEntityAccount;
                exportData.requestReport.offset = "0";
                exportData.requestReport.dateFrom = DateTime.Today.AddDays(-1).ToString("yyyyMMdd");
                exportData.requestReport.dateTo = DateTime.Today.ToString("yyyyMMdd");

                Report.List.Response lResponse = new Report.List.Response();
                lResponse = BiReport.List_Merchant_Report(exportData.requestReport, customer);
                List<Report.List.MerchantReport_CSV_Format> dataExportMerchantRep_Csv = new List<Report.List.MerchantReport_CSV_Format>();
                dataExportMerchantRep_Csv = JsonConvert.DeserializeObject<List<Report.List.MerchantReport_CSV_Format>>(lResponse.TransactionList);

                resultBase64 = ExportToCsvMerchantReport(dataExportMerchantRep_Csv, exportData.TypeReport);

                // Creating TEMP File

                string tmpFileName = FileUtils.CreateTmpFile();
                LogService.LogInfo("Created Merchant temp csv file in " + tmpFileName);
                FileUtils.WriteTmpFile(tmpFileName, FileUtils.Base64Decode(resultBase64));

                // Set filename to upload to client sftp
                string todayDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                string uploadFileName = ConfigurationManager.AppSettings[configParamsName + "FILENAME"].ToString();
                uploadFileName = uploadFileName.Replace("YYYYMMDD", todayDate).ToString();

                // getting s/ftp parameters from config file
                string host = ConfigurationManager.AppSettings[configParamsName + "HOST"].ToString();
                string username = ConfigurationManager.AppSettings[configParamsName + "USERNAME"].ToString();
                string password = ConfigurationManager.AppSettings[configParamsName + "PASSWORD"].ToString();
                string workingDir = ConfigurationManager.AppSettings[configParamsName + "UPLOAD_DIR"].ToString();
                bool isSftp = bool.Parse(ConfigurationManager.AppSettings[configParamsName + "IS_SFTP"].ToString());

                if (isSftp)
                {
                    SftpHelper.UploadFile(host, username, password, workingDir, tmpFileName, uploadFileName);
                }
                else
                {
                    FtpHelper.UploadFile(host, username, password, workingDir, tmpFileName, uploadFileName);
                }
                LogService.LogInfo("Merchant automated CSV Report Successfully uploaded to " + workingDir + "/" + uploadFileName);

                File.Copy(tmpFileName, localBackupFolder + "\\" + uploadFileName);
                LogService.LogInfo("Merchant CSV Report saved locally. path: " + localBackupFolder + "\\" + uploadFileName);
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error in Payoneer Automated CSV Report. Exception message: " + ex.Message.ToString());
                throw ex;
            }
        }
        private static DataTable ConvertToDataTable<T>(IList<T> data)
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
         static string ExportToCsvMerchantReport(List<SharedModel.Models.View.Report.List.MerchantReport_CSV_Format> dataCsv, string TypeReport)
        {

            try
            {
                string csvBase64 = "";
                DataTable dt = ConvertToDataTable(dataCsv);
                csvBase64 = GenerateCsv(dt);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(csvBase64);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

         static string GenerateCsv(DataTable dataTable)
        {
            StringBuilder sbData = new StringBuilder();

            // Only return Null if there is no structure.
            if (dataTable.Columns.Count == 0)
                return null;

            foreach (var col in dataTable.Columns)
            {
                if (col == null)
                    sbData.Append(",");
                else
                    //sbData.Append("\"" + col.ToString().Replace("\"", "\"\"") + "\",");
                    sbData.Append(col.ToString().Replace("\"", "\"\"") + ",");
            }

            sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    if (column == null)
                        sbData.Append(",");
                    else
                        //sbData.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\",");
                        sbData.Append(column.ToString().Replace("\"", "\"\"") + ",");
                }
                sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);
            }

            return sbData.ToString();
        }


        public static void GenerateAndUploadNiumReports()
        {
            try
            {
                var exportUtilsService = new ExportUtilsService();

                string configParamsName = "NIUM_AUTOMATED_REPORT_";

                string niumEntitiesText = ConfigurationManager.AppSettings[configParamsName + "COUNTRY_ENTITIES"].ToString();
                Dictionary<string, int> niumEntities = JsonConvert.DeserializeObject<Dictionary<string, int>>(niumEntitiesText);

                string yersterdayDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

                string localBackupFolder = @"C:\MERCHANT_CSV_BACKUP\NIUM";
                LogService.LogInfo("Executing NIUM Merchant Automated Reports");

                // Checking/Creating local backup folder
                if (!System.IO.Directory.Exists(localBackupFolder))
                {
                    LogService.LogInfo("Creating Local Backup Folder: " + localBackupFolder);
                    System.IO.Directory.CreateDirectory(localBackupFolder);
                }


                foreach (KeyValuePair<string, int> entity in niumEntities)
                {
                    string countryCode = entity.Key;
                    int idEntityUser = entity.Value;

                    LogService.LogInfo("Executing NIUM Merchant Automated Reports for COUNTRY CODE " + countryCode);

                    List<ParameterInExport> inExport = new List<ParameterInExport>();

                    inExport.Add(new ParameterInExport { Key = "json", Val = "{\"idEntityAccount\": " + idEntityUser + ", \"dateFrom\": \"" + yersterdayDate + "\", \"dateTo\": \"" + yersterdayDate + "\"}" });
                    inExport.Add(new ParameterInExport { Key = "country_code", Val = countryCode });
                    inExport.Add(new ParameterInExport { Key = "returnAsJson", Val = "0" });

                    string FileName = "MerchantReport_" + countryCode + "_YYYYMMDD".Replace("YYYYMMDD", yersterdayDate).ToString();

                    var expListBytes = exportUtilsService.ExportarPath(localBackupFolder, new Tools.Dto.Export
                    {
                        FileName = FileName,
                        IsSP = true,
                        Name = "[LP_Operation].[MerchantReport]",
                        Parameters = inExport
                    });
                    LogService.LogInfo("NIUM Merchant Report Generated path : " + expListBytes);

                    if (expListBytes != "0")
                    {
                        // Set filename to upload to client sftp
                        string uploadFileName = FileName + ".xls";

                        // getting s/ftp parameters from config file
                        string host = ConfigurationManager.AppSettings[configParamsName + "HOST"].ToString();
                        string username = ConfigurationManager.AppSettings[configParamsName + "USERNAME"].ToString();
                        string password = ConfigurationManager.AppSettings[configParamsName + "PASSWORD"].ToString();
                        string workingDir = ConfigurationManager.AppSettings[configParamsName + "UPLOAD_DIR"].ToString();
                        bool isSftp = bool.Parse(ConfigurationManager.AppSettings[configParamsName + "IS_SFTP"].ToString());

                        if (isSftp)
                        {
                            SftpHelper.UploadFile(host, username, password, workingDir, expListBytes, uploadFileName);
                            LogService.LogInfo("NIUM Merchant automated Report Successfully uploaded to " + workingDir + "/" + uploadFileName);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error in NIUM Automated Merchant Report. Exception message: " + ex.Message.ToString() + " " + ex.StackTrace.ToString());
                throw ex;
            }

        }
    }
}