using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangFire;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Data.SqlClient;
using SharedBusiness.Mail;

namespace HangFire.Common.Operations.AutomatedJobs
{
    public static class ArbaUpdateJob
    {
        [Hangfire.Queue("arbaqueue")]
        public static void UpdateArbaRetentionEntities()
        {
            string arbaFolder = "";
            string processStatus = "";
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Initializing ARBA entities update");
                //LogService.LogInfo("Initializing ARBA entities update");

                ArbaPadronHelper arbaHelper = new ArbaPadronHelper();
                // get actual month and year
                string date = DateTime.Now.ToString("MMyyyy");
                // Get folder path with current month
                arbaFolder = Path.Combine(ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_ZIP_IMPORT_FOLDER"].ToString(), date).ToString();
                // Create Folder
                if (!Directory.Exists(arbaFolder))
                    Directory.CreateDirectory(arbaFolder);

                // get login url, username and password
                string loginUrl = ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_LOGIN_URL"].ToString();
                string loginUsername = ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_LOGIN_CUIT"].ToString();
                string loginPassword = ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_LOGIN_PASSWORD"].ToString();
                // Start selenium process to download file
                string downloadedZipFilename = arbaHelper.LoginAndDownloadFile(loginUrl, loginUsername, loginPassword, arbaFolder);
                LogService.LogInfo("File downloaded from ARBA site successfully.");
                // set full downloaded file path
                string downloadedZipFilePath = Path.Combine(arbaFolder, downloadedZipFilename);
                if (!File.Exists(downloadedZipFilePath))
                {
                    throw new Exception("Downloaded zip file does not exists: " + downloadedZipFilePath);
                }

                // extract downloaded zip
                ZipFile.ExtractToDirectory(downloadedZipFilePath, arbaFolder);
                // Check if downloaded ARBA file matches date
                string extractedFileName = downloadedZipFilename.Replace(".zip", ".txt");
                string extractedFilenamePath = Path.Combine(arbaFolder, extractedFileName);
                if (!File.Exists(extractedFilenamePath))
                {
                    throw new Exception("Extracted txt file does not exists: " + extractedFilenamePath);
                }

                DataTable padronRows = arbaHelper.GenerateRows(extractedFilenamePath);

                string tableName = "LP_Retentions_ARG.RegisteredArbaEntities";

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string truncateTable = string.Format("truncate table {0}", tableName);
                    SqlCommand command = new SqlCommand(truncateTable, connection);
                    command.ExecuteNonQuery();

                    // bulk insert
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.ColumnMappings.Add("ProcessDate", "ProcessDate");
                        bulkCopy.ColumnMappings.Add("Reg", "Reg");
                        bulkCopy.ColumnMappings.Add("ToDate", "ToDate");
                        bulkCopy.ColumnMappings.Add("Cuit", "Cuit");
                        bulkCopy.ColumnMappings.Add("BussinessName", "BussinessName");
                        bulkCopy.ColumnMappings.Add("Letter", "Letter");
                        bulkCopy.ColumnMappings.Add("Active", "Active");

                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.WriteToServer(padronRows);
                    }
                    connection.Close();
                }

                // deleting folder with downloaded and extracted files
                Directory.Delete(arbaFolder, true);
                processStatus = "ARBA entities updated successfully on date " + date + ". Inserted rows count: " + padronRows.Rows.Count.ToString();
                //LogService.LogInfo(processStatus);
                LogService.LogInfo(processStatus);
                MailService.SendMail("ARBA - Proceso de actualizacion realizado", processStatus, supportEmails);
            }
            catch (Exception e)
            {
                processStatus = "There has been an error when updating ARBA registered entities: " + e.Message.ToString();
                //LogService.LogError(processStatus);
                LogService.LogInfo(processStatus);
                MailService.SendMail("ARBA - Proceso de actualizacion ERROR", processStatus, supportEmails);
            }
        }
    }
}
