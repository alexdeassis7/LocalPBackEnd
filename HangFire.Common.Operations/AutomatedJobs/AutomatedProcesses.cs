using Hangfire;
using HangFire.Common.Operations.AutomatedJobs.Models;
using SharedBusiness.Log;
using SharedBusiness.Mail;
using SharedBusiness.Payin;
using SharedBusiness.Services.Payouts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Tools;

namespace HangFire.Common.Operations.AutomatedJobs
{
    public static class AutomatedProcesses
    {
        //public static async Task UpdateArbaRetentionEntities()
        //{
        //    string arbaFolder = "";
        //    string processStatus = "";
        //    List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
        //    try
        //    {
        //        LogService.LogInfo("Initializing ARBA entities update");

        //        ArbaPadronHelper arbaHelper = new ArbaPadronHelper();
        //        // get actual month and year
        //        string date = DateTime.Now.ToString("MMyyyy");
        //        // Get folder path with current month
        //        arbaFolder = Path.Combine(ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_ZIP_IMPORT_FOLDER"].ToString(), date).ToString();
        //        // Create Folder
        //        if (!Directory.Exists(arbaFolder))
        //            Directory.CreateDirectory(arbaFolder);

        //        // get login url, username and password
        //        string loginUrl = ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_LOGIN_URL"].ToString();
        //        string loginUsername = ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_LOGIN_CUIT"].ToString();
        //        string loginPassword = ConfigurationManager.AppSettings["ARBA_UPDATE_JOB_LOGIN_PASSWORD"].ToString();
        //        // Start selenium process to download file
        //        arbaHelper.LoginAndDownloadFile(loginUrl, loginUsername, loginPassword);
        //        LogService.LogInfo("File downloaded from ARBA site successfully.");
        //        // get windows user
        //        string userProfileFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //        // set downloaded file name
        //        string downloadedZipFilename = "PadronIntermediarios" + date + ".zip";
        //        // get user downloads folder
        //        string downloadsFolder = Path.Combine(userProfileFolder, "Downloads");
        //        // set full downloaded file path
        //        string downloadedZipFilePath = Path.Combine(downloadsFolder, downloadedZipFilename);
        //        // set working directory full path
        //        string workingDirectoryZipFilePath = Path.Combine(arbaFolder, downloadedZipFilename);
        //        // check if the downloaded zip is there
        //        if (File.Exists(downloadedZipFilePath))
        //        {
        //            // Move file to working directory
        //            File.Move(downloadedZipFilePath, workingDirectoryZipFilePath);
        //        } else
        //        {
        //            throw new Exception("Downloaded zip file does not exists: " + downloadedZipFilePath);
        //        }


        //        // extract downloaded zip
        //        ZipFile.ExtractToDirectory(workingDirectoryZipFilePath, arbaFolder);
        //        // Check if downloaded ARBA file matches date
        //        string extractedFilenamePath = Path.Combine(arbaFolder, "PadronIntermediarios" + date + ".txt");
        //        if (!File.Exists(extractedFilenamePath))
        //        {
        //            throw new Exception("Extracted txt file does not exists: " + extractedFilenamePath);
        //        }

        //        DataTable padronRows = arbaHelper.GenerateRows(extractedFilenamePath);

        //        string tableName = "LP_Retentions_ARG.RegisteredArbaEntities";

        //        using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
        //        {
        //            connection.Open();
        //            // truncate table
        //            string truncateTable = string.Format("truncate table {0}", tableName);
        //            SqlCommand command = new SqlCommand(truncateTable, connection);
        //            command.ExecuteNonQuery();

        //            // bulk insert
        //            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
        //            {
        //                bulkCopy.DestinationTableName = tableName;
        //                bulkCopy.ColumnMappings.Add("ProcessDate", "ProcessDate");
        //                bulkCopy.ColumnMappings.Add("Reg", "Reg");
        //                bulkCopy.ColumnMappings.Add("ToDate", "ToDate");
        //                bulkCopy.ColumnMappings.Add("Cuit", "Cuit");
        //                bulkCopy.ColumnMappings.Add("BussinessName", "BussinessName");
        //                bulkCopy.ColumnMappings.Add("Letter", "Letter");
        //                bulkCopy.ColumnMappings.Add("Active", "Active");

        //                bulkCopy.BulkCopyTimeout = 0;
        //                bulkCopy.WriteToServer(padronRows);
        //            }
        //            connection.Close();
        //        }

        //        // deleting folder with downloaded and extracted files
        //        Directory.Delete(arbaFolder, true);
        //        processStatus = "ARBA entities updated successfully on date " + date + ". Inserted rows count: " + padronRows.Rows.Count.ToString();
        //        LogService.LogInfo(processStatus);
        //        MailService.SendMail("ARBA - Proceso de actualizacion realizado", processStatus, supportEmails);
        //    }
        //    catch (Exception e)
        //    {
        //        processStatus = "There has been an error when updating ARBA registered entities: " + e.Message.ToString();
        //        LogService.LogError(processStatus);
        //        MailService.SendMail("ARBA - Proceso de actualizacion ERROR", processStatus, supportEmails);
        //    }
        //}
        [Queue("afipqueue")]
        public static async System.Threading.Tasks.Task UpdateAfipRetentionEntitiesAsync()
        {
            string afipFolder = "";
            string processStatus = "";
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Initializing AFIP entities update");
                string afipJobName = ConfigurationManager.AppSettings["AFIP_UPDATE_JOB_HF_NAME"].ToString();
                string afipJobCronExp = ConfigurationManager.AppSettings["AFIP_UPDATE_JOB_CRON_EXPRESSION"].ToString();
                string afipJobCronExpOnOutdatedFile = "0 */3 * * *";
                string afipUrl = ConfigurationManager.AppSettings["AFIP_UPDATE_JOB_DOWNLOAD_URL"].ToString();

                // creating instance of AFIP Helper
                AfipPadronHelper afipHelper = new AfipPadronHelper();

                // Get last saturday date
                DateTime lastSaturday = DateTime.Now;
                while(lastSaturday.DayOfWeek != DayOfWeek.Saturday)
                    lastSaturday = lastSaturday.AddDays(-1);
                // date format
                string date = lastSaturday.ToString("yyyyMMdd");
                afipFolder = Path.Combine(ConfigurationManager.AppSettings["AFIP_UPDATE_JOB_ZIP_IMPORT_FOLDER"].ToString(), date).ToString();
                var fileName = Path.Combine(afipFolder, date + ".zip");


                if (!Directory.Exists(afipFolder))
                    Directory.CreateDirectory(afipFolder);

                if (File.Exists(fileName))
                {
                    return;
                }

                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(afipUrl, fileName);
                };

                // folder to extract zip content
                string extractFolderPath = Path.Combine(afipFolder, "extracted");
                // extract downloaded zip
                ZipFile.ExtractToDirectory(fileName, extractFolderPath);
                //string extractedFilePath = afipHelper.FindDownloadedFile(extractFolderPath);
                string extractedFilePath = Directory.GetFiles(extractFolderPath, "*.*", SearchOption.AllDirectories)[0];
                // get extracted file modification date
                DateTime FileDate = new FileInfo(extractedFilePath).LastWriteTime;
                LogService.LogInfo("Downloaded File Creation Time: " + FileDate.ToString());

                DataTable padronRows = new DataTable();

                string tableName = "LP_Retentions_ARG.RegisteredEntities";

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // get last update date from Table
                    SqlCommand entityRecordCmd = new SqlCommand(string.Format("SELECT TOP 1 FORMAT(OP_InsDateTime, 'yyyyMMdd') AS insert_date FROM {0}", tableName), connection);
                    using (SqlDataReader reader = entityRecordCmd.ExecuteReader())
                    {
                        int compare = 0;
                        if (reader.Read())
                        {
                            if (reader["insert_date"] != System.DBNull.Value)
                            {
                                string lastInsertDate = reader["insert_date"].ToString();
                                DateTime lastInsertDateTime = DateTime.ParseExact(lastInsertDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                                compare = DateTime.Compare(FileDate, lastInsertDateTime);
                            }
                            else
                            {
                                compare = 1;
                            }
                        }
                        else
                        {
                            compare = 1;
                        }
                        if (compare > 0)
                        {
                            padronRows = afipHelper.GenerateRows(extractedFilePath);
                        }
                        else
                        {
                            processStatus = "Downloaded file was not updated. File date:" + FileDate.ToShortDateString();
                            LogService.LogInfo(processStatus);
                            MailService.SendMail("AFIP - Proceso de actualizacion realizado", processStatus, supportEmails);
                            Directory.Delete(afipFolder, true);

                            return;
                        }
                    }

                    if (padronRows.Rows.Count == 0) throw new Exception();

                    // truncate table
                    string truncateTable = string.Format("truncate table {0}", tableName);
                    SqlCommand command = new SqlCommand(truncateTable, connection);
                    command.ExecuteNonQuery();

                    // bulk insert
                    using(SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = tableName;
                        bulkCopy.ColumnMappings.Add("ProcessDate", "ProcessDate");
                        bulkCopy.ColumnMappings.Add("CUIT", "CUIT");
                        bulkCopy.ColumnMappings.Add("Alias", "Alias");
                        bulkCopy.ColumnMappings.Add("IncomeTax", "IncomeTax");
                        bulkCopy.ColumnMappings.Add("VatTax", "VatTax");
                        bulkCopy.ColumnMappings.Add("MonoTax", "MonoTax");
                        bulkCopy.ColumnMappings.Add("SocialMember", "SocialMember");
                        bulkCopy.ColumnMappings.Add("Employer", "Employer");
                        bulkCopy.ColumnMappings.Add("MonoTaxActivity", "MonoTaxActivity");
                        bulkCopy.ColumnMappings.Add("Active", "Active");

                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.WriteToServer(padronRows);
                    }
                    connection.Close();
                }

                // deleting folder with downloaded and extracted files
                Directory.Delete(afipFolder, true);

                processStatus = "AFIP entities updated successfully on date " + date + ". Inserted rows count: " + padronRows.Rows.Count.ToString();
                LogService.LogInfo(processStatus);
                MailService.SendMail("AFIP - Proceso de actualizacion realizado", processStatus, supportEmails);
            } catch (Exception e)
            {
                if (!string.IsNullOrWhiteSpace(afipFolder) && Directory.Exists(afipFolder))
                {
                    Directory.Delete(afipFolder, true);
                }
                processStatus = "There has been an error when updating AFIP registered entities: " + e.Message.ToString();
                LogService.LogError(processStatus);
                MailService.SendMail("AFIP - Proceso de actualizacion ERROR", processStatus, supportEmails);
            }
            
        }
        [Queue("currencycleanqueue")]
        [AutomaticRetry(Attempts = 0)]
        public static void CleanCurrencyExchange() 
        {
            try 
            {
                LogService.LogInfo("Starting Currency Exchange cleaning process");

                SharedBusiness.View.BICurrencyExchange BiCurrencyExchange = new SharedBusiness.View.BICurrencyExchange();
                BiCurrencyExchange.CleanCurrencyExchange();

                string process = "Currency Exchange cleaned successfully on date " + DateTime.Now.ToString("yyyyMMdd");
                LogService.LogInfo(process);

            }
            catch (Exception ex) 
            {
                LogService.LogError("There was an error in Automated Currency Exchange Clean. Exception message: " + ex.ToString());
            }
        }

        [Queue("notificationpushretryqueue")]
        [AutomaticRetry(Attempts = 0)]
        public static void NotificationPushRetry()
        {
            try
            {
                LogService.LogInfo("Starting NotificationPushRetry");

                var PayoneerTransactions = new TransactionNotifyModel().GetColombiaTransactionDetails(int.Parse(ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_COL_ENTITY_ID"].ToString()));

                Task.Run(async () =>
                {
                    if (PayoneerTransactions.Count() > 0)
                    {
                        await MerchantNotificationService.RetrieveAndSendTransactionsCOL(PayoneerTransactions.ToList());
                        await MerchantNotificationService.SetNotificationPushSent(PayoneerTransactions.Select(x => int.Parse(x.TransactionId)));
                    }

                    string process = "NotificationPushRetry successfully:(" + PayoneerTransactions.Count() + ") on date " + DateTime.Now.ToString("yyyyMMdd");
                    LogService.LogInfo(process);

                });
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error in Automated NotificationPushRetry. Exception message: " + ex.ToString());

            }
        }

        [Queue("payintaskqueue")]
        [AutomaticRetry(Attempts = 2)]
        public static void SetExpirePayins()
        {
            try
            {
                LogService.LogInfo("Job Started - Set Expire Payin process");

                new PayinService().SetExpires();

                string process = "Job Finished - Set Expire Payin process on date " + DateTime.Now.ToString("yyyyMMdd");

                LogService.LogInfo(process);
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error while expiring payin. Exception message: " + ex.ToString());
            }
        }

        [Queue("onholdtaskqueue")]
        [AutomaticRetry(Attempts = 2)]
        public static void RejectExpiredOnHold()
        {
            try
            {
                LogService.LogInfo("Job Started - Reject Expired Onhold process");

                new BIPayOut().RejectExpiredOnHold();

                string process = "Job Finished - Reject Expired Onhold process on date " + DateTime.Now.ToString("yyyyMMdd");

                LogService.LogInfo(process);
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error while rejecting onhold transactions. Exception message: " + ex.ToString());
            }
        }
    }
}