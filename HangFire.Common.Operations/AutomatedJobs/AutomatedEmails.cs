using Newtonsoft.Json;
using SharedModel.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Configuration;
using SharedBusiness.Log;
using SharedBusiness.Mail;
using Hangfire;
using HangFire.Common.Operations.AutomatedJobs.Models;
using Tools;
using System.Web;
using System.IO;
using System.Net.Mail;

namespace HangFire.Common.Operations.AutomatedJobs
{
    public class AutomatedEmails
    {
        [Queue("generalblcequeue")]
        public static void GenerateAndSendGeneralBalanceEmail()
        {
            try
            {
                List<SharedModel.Models.User.User> ListUsers = new List<SharedModel.Models.User.User>();
                SharedBusiness.User.BlUser Bl = new SharedBusiness.User.BlUser();
                ListUsers = Bl.GetListUsers();
                ListUsers = ListUsers.Where(y => y.ReportEmail != null).GroupBy(x => x.Merchant).Select(group => group.First()).ToList();

                //Foreach user send an email
                foreach (var user in ListUsers)
                {
                    string customer = user.UserSiteIdentification;

                    Dashboard.ClientReport.Response lResponse = new Dashboard.ClientReport.Response();

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    lResponse = BiPO.DashboardClientReport(customer);
                    var response = JsonConvert.DeserializeObject<List<GeneralBalanceModel.ClientDashboardData>>(lResponse.clientReportData);

                    //CREATE EMAIL START
                    string messageBody = "<font>The following is your balance: </font><br><br>";
                    string htmlTableStart = "<table align ='center'  style=\"border-collapse:collapse; text-align:center;\"";
                    string htmlTableHead = "<thead background-color: #dee2e6;><tr style =\"background-color:#dee2e6;\"><th>Merchant</th><th>Current Balance</th><th>In Progress</th><th>Balance After Execution</th><th>Received</th></tr></thead><tbody>";
                    string htmlTableEnd = "</tbody></table>";
                    string htmlTrStart = "<tr align ='center' style =\"color:#555555;\">";
                    string htmlTrEnd = "</tr>";
                    string htmlTdStart = "<td style=\" border-color:#dee2e6; border-style:solid; border-width:thin; padding: 5px;\">";
                    string htmlTdEnd = "</td>";

                    messageBody += htmlTableStart;
                    messageBody += htmlTableHead;
                    foreach (var clientDashboardData in response)
                    {
                        messageBody += htmlTrStart;
                        messageBody += htmlTdStart + clientDashboardData.LastName + htmlTdEnd;
                        messageBody += htmlTdStart + String.Format("{0} {1}", clientDashboardData.CurrencyCode, Math.Round(clientDashboardData.SaldoActual, 2)) + htmlTdEnd;
                        messageBody += htmlTdStart + String.Format("{0} {1}", clientDashboardData.CurrencyCode, Math.Round(clientDashboardData.AmtInProgress, 2)) + "<br/> (" + clientDashboardData.cntInProgress + ") " + htmlTdEnd;
                        messageBody += htmlTdStart + String.Format("{0} {1}", clientDashboardData.CurrencyCode, Math.Round((clientDashboardData.SaldoActual - clientDashboardData.AmtInProgress), 2))  + htmlTdEnd;
                        messageBody += htmlTdStart + String.Format("{0} {1}", clientDashboardData.CurrencyCode, Math.Round(clientDashboardData.AmtReceived, 2)) + "<br/> (" + clientDashboardData.cntReceived + ") " + htmlTdEnd;
                        messageBody += htmlTrEnd;
                    }
                    messageBody += htmlTableEnd;
                    //CREATE EMAIL END

                    List<string> email = new List<string>();
                    var AdminEmails = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_ADMIN_EMAIL"].ToString().Split(';');
                    var MerchantEmails = user.ReportEmail.Split(';');

#if DEBUG
                    email.AddRange(AdminEmails);
#else
                         email.AddRange(AdminEmails.Concat(MerchantEmails));
#endif
                    email = email.Distinct().ToList();
                    var subject = String.Format(ConfigurationManager.AppSettings["AUTOMATED_EMAIL_GENERAL_BALANCE_SUBJECT"].ToString(), user.Merchant, DateTime.Now.ToString("dd/MM/yyyy"));
                    MailService.SendMail(subject, messageBody, email);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error in General Balance Automated Email. Exception message: " + ex.Message.ToString());
                throw ex;
            }

        }
        [Queue("tranlotqueue")]
        public static void SendTransactionLotInfo() 
        {
            try
            {
                //GET TRANSACTION LOTS
                List<TransactionModel.TransactionLot.Response> ListTransactionLot = new List<TransactionModel.TransactionLot.Response>();
                SharedBusiness.View.BlTransaction BiTransaction = new SharedBusiness.View.BlTransaction();
                ListTransactionLot = BiTransaction.GetTransactionLots();

                //FOREACH LOT SEND AN EMAIL
                foreach (var transactionLot in ListTransactionLot)
                {
                    //CREATE EMAIL
                    string messageBody = Tools.MailHelper.CreateTransactionLotMailBody(transactionLot);

                    //SEND EMAIL
                    var AdminEmails = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_ADMIN_EMAIL"].ToString().Split(';');
                    var subject = String.Format(ConfigurationManager.AppSettings["AUTOMATED_EMAIL_TRANSACTION_LOT_SUBJECT"].ToString(), transactionLot.Merchant, transactionLot.LotDate.ToString("dd/MM/yyyy HH:ss"), DateTime.Now.ToString("dd/MM/yyyy HH:ss"), transactionLot.TotalTransactions, String.Format("{0} {1}", transactionLot.CurrencyType, Math.Round(transactionLot.TotalAmount, 2)));
                    MailService.SendMail(subject, messageBody, AdminEmails);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error in Transaction Lot Automated Email. Exception message: " + ex.ToString());
                throw ex;
            }
        }

        [Queue("amlqueue")]
        public static void SendAmlReport(string path) 
        {
            try
            {
                //GET BENEFICIARIES
                var worksheetnames = new List<string>();
                worksheetnames.Add("YELLOW FLAG BENEFICIARY");
                worksheetnames.Add("RED FLAG BENEFICIARY");
                worksheetnames.Add("YELLOW FLAG SENDER");
                worksheetnames.Add("RED FLAG SENDER");
                worksheetnames.Add("BENEFICIARY TRAN");
                worksheetnames.Add("SENDER TRAN");
                var exportUtilsService = new ExportUtilsService();
                var expListBytes = exportUtilsService.ExportarPath(path, new Tools.Dto.Export
                {
                    FileName = "AMLReport",
                    IsSP = true,
                    Name = "[LP_Operation].[AMLData]",
                    Parameters = { }
                }, worksheetnames);

                //SEND EMAIL
                if (expListBytes != "0")
                {
                    var AdminEmails = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_AML_EMAIL"].ToString().Split(';');
                    var subject = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_AML_SUBJECT"].ToString();
                    var attachments = new List<string>();
                    attachments.Add(expListBytes);
                    MailService.SendMail(subject, "", AdminEmails, attachments);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError("There was an error in AML Automated Email. Exception message: " + ex.ToString());
                throw ex;
            }
        }
    }
}