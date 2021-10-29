using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using SharedBusiness.Services.CrossCutting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using Hangfire;
using Hangfire.SqlServer;
using WA_LP.Cache;
using HangFire.Common.Operations.AutomatedJobs;
using SharedBusiness.Log;
using WA_LP.App_Start;

namespace WA_LP
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static readonly object appStartLock = new object();

        protected void Application_Start(object sender, EventArgs e)
        {
            lock (appStartLock)
            {
                //Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
                SharedBusiness.Common.Configuration.Init();
                DictionaryService.Init();
                LogInCacheService.Init();
                BankValidateCacheService.Init();
                PayinPaymentMethodValidateCacheService.Init();
                System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);

                // set up hangfire jobs

                HangfireBootstrapper.Instance.Start();

                SetupHangfireJobs();
            }
        }
        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }

        private void SetupHangfireJobs()
        {
            LogService.LogInfo("Setting up hangfire jobs");
            // Starting HangFire Server
            // HangfireAspNet.Use(GetHangfireServers);

            // Adding Payoneer CSV Report Generation job ARG
            // Get cron Expression from config file
            string configParamsNameArg = "PAYONEER_ARG_AUTOMATED_REPORT_";
            string cronExpressionArg = ConfigurationManager.AppSettings[configParamsNameArg + "CRON_EXPRESSION"].ToString();
            string jobNamePayoneerArg = "payoneer_automated_report_arg";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNamePayoneerArg);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNamePayoneerArg, () => HangFire.Common.Operations.AutomatedJobs.AutomatedReports.GenerateAndUploadMerchantReportCSVarg(configParamsNameArg), cronExpressionArg, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayoneerArgReportQueue.ToLower());

            // Adding Payoneer CSV Report Generation job COL
            // Get cron Expression from config file
            string configParamsNameCol = "PAYONEER_COL_AUTOMATED_REPORT_";
            string cronExpressionCol = ConfigurationManager.AppSettings[configParamsNameCol + "CRON_EXPRESSION"].ToString();
            string jobNamePayoneerCol = "payoneer_automated_report_col";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNamePayoneerCol);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNamePayoneerCol, () => AutomatedReports.GenerateAndUploadMerchantReportCSVcol(configParamsNameCol), cronExpressionCol, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayoneerColReportQueue.ToLower());

            // Adding Payoneer CSV Report Generation job BRA
            // Get cron Expression from config file
            string configParamsNameBra = "PAYONEER_BRA_AUTOMATED_REPORT_";
            string cronExpressionBra = ConfigurationManager.AppSettings[configParamsNameBra + "CRON_EXPRESSION"].ToString();
            string jobNamePayoneerBra = "payoneer_automated_report_bra";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNamePayoneerBra);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNamePayoneerBra, () => AutomatedReports.GenerateAndUploadMerchantReportCSVbra(configParamsNameBra), cronExpressionBra, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayoneerBraReportQueue.ToLower());

            // Adding Payoneer CSV Report Generation job MEX
            // Get cron Expression from config file
            string configParamsNameMex = "PAYONEER_MEX_AUTOMATED_REPORT_";
            string cronExpressionMex = ConfigurationManager.AppSettings[configParamsNameMex + "CRON_EXPRESSION"].ToString();
            string jobNamePayoneerMex = "payoneer_automated_report_mex";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNamePayoneerMex);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNamePayoneerMex, () => AutomatedReports.GenerateAndUploadMerchantReportCSVmex(configParamsNameMex), cronExpressionMex, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayoneerMexReportQueue.ToLower());

            // Adding Payoneer CSV Report Generation job URY
            // Get cron Expression from config file
            string configParamsNameUry = "PAYONEER_URY_AUTOMATED_REPORT_";
            string cronExpressionUry = ConfigurationManager.AppSettings[configParamsNameUry + "CRON_EXPRESSION"].ToString();
            string jobNamePayoneerUry = "payoneer_automated_report_ury";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNamePayoneerUry);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNamePayoneerUry, () => AutomatedReports.GenerateAndUploadMerchantReportCSVury(configParamsNameUry), cronExpressionUry, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayoneerUryReportQueue.ToLower());

            // Adding Payoneer CSV Report Generation job CHL
            // Get cron Expression from config file
            string configParamsNameChl = "PAYONEER_CHL_AUTOMATED_REPORT_";
            string cronExpressionChl = ConfigurationManager.AppSettings[configParamsNameChl + "CRON_EXPRESSION"].ToString();
            string jobNamePayoneerChl = "payoneer_automated_report_chl";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNamePayoneerChl);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNamePayoneerChl, () => AutomatedReports.GenerateAndUploadMerchantReportCSVchl(configParamsNameChl), cronExpressionChl, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayoneerChlReportQueue.ToLower());

            // Adding Nium Merchant Report Generation job
            // Get cron Expression from config file
            string niumCronExpression = ConfigurationManager.AppSettings["NIUM_AUTOMATED_REPORT_CRON_EXPRESSION"].ToString();
            string jobNameNium = "nium_automated_merchant_reports";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNameNium);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNameNium, () => AutomatedReports.GenerateAndUploadNiumMerchantReportsJob(), niumCronExpression, TimeZoneInfo.Local, queue: HangfireBootstrapper.NiumMerchantReportsQueue.ToLower());

            // AFIP Registered Entities UPDATE
            string afipJobName = ConfigurationManager.AppSettings["AFIP_UPDATE_JOB_HF_NAME"].ToString();
            string afipJobCronExp = ConfigurationManager.AppSettings["AFIP_UPDATE_JOB_CRON_EXPRESSION"].ToString(); // Runs every saturday at 23 PM
            RecurringJob.RemoveIfExists(afipJobName);
            RecurringJob.AddOrUpdate(afipJobName, () => AutomatedProcesses.UpdateAfipRetentionEntitiesAsync(), afipJobCronExp, TimeZoneInfo.Local, queue: HangfireBootstrapper.AfipQueue.ToLower());

            //NOTIFICATION EMAIL SECTION

            //GENERAL BALANCE
            // Get cron Expression from config file
            string cronExpressionGeneralBalance = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_GENERAL_BALANCE_CRON_EXPRESSION"].ToString();
            string jobNameGeneralBalance = "email_notificacion_general_balance";
            // To prevent Job being duplicated we first Remove it if it exists.
            RecurringJob.RemoveIfExists(jobNameGeneralBalance);
            // Set up job in HangFire
            RecurringJob.AddOrUpdate(jobNameGeneralBalance, () => AutomatedEmails.GenerateAndSendGeneralBalanceEmail(), cronExpressionGeneralBalance, TimeZoneInfo.Local, queue: HangfireBootstrapper.GeneralBlceQueue.ToLower());

            //TRANSACTION LOT
            string jobNameTransactionLot = "email_notificacion_transaction_lot";
            string cronExpressionTransactionLot = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_TRANSACTION_LOT_CRON_EXPRESSION"].ToString(); //Runs every 6 hours
            RecurringJob.RemoveIfExists(jobNameTransactionLot);
            RecurringJob.AddOrUpdate(jobNameTransactionLot, () => AutomatedEmails.SendTransactionLotInfo(), cronExpressionTransactionLot, TimeZoneInfo.Local, queue: HangfireBootstrapper.TranLotQueue.ToLower());

            //CLEANING CURRENCY EXCHANGE
            string jobNameCleanCurrency = "clean_job_currency_exchange";
            string cronExpressionCleanCurrency = ConfigurationManager.AppSettings["AUTOMATED_CLEANING_CURRENCY_EXCHANGE_CRON_EXPRESSION"].ToString(); 
            RecurringJob.RemoveIfExists(jobNameCleanCurrency);
            RecurringJob.AddOrUpdate(jobNameCleanCurrency, () => AutomatedProcesses.CleanCurrencyExchange(), cronExpressionCleanCurrency, TimeZoneInfo.Local, queue: HangfireBootstrapper.CurrencyCleanQueue.ToLower());

            //AML
            string jobNameAML = "email_notificacion_aml";
            string cronExpressionAML = ConfigurationManager.AppSettings["AUTOMATED_EMAIL_AML_CRON_EXPRESSION"].ToString();
            RecurringJob.RemoveIfExists(jobNameAML);
            RecurringJob.AddOrUpdate(jobNameAML, () => AutomatedEmails.SendAmlReport(HttpContext.Current.Server.MapPath("~/" + "AML")), cronExpressionAML, TimeZoneInfo.Local, queue: HangfireBootstrapper.AmlQueue.ToLower());

            //JOB NOTIFICATION PUSH RETRY
            string jobNameNotificationPushRetry = "notification_push_retry";
            string cronExpressionNotificationPushRetry = ConfigurationManager.AppSettings["NOTIFICATION_PUSH_RETRY_CRON_EXPRESSION"].ToString();
            RecurringJob.RemoveIfExists(jobNameNotificationPushRetry);
            RecurringJob.AddOrUpdate(jobNameNotificationPushRetry, () => AutomatedProcesses.NotificationPushRetry(), cronExpressionNotificationPushRetry, TimeZoneInfo.Local, queue: HangfireBootstrapper.NotificationPushRetryQueue.ToLower());
            
            //SET PAYIN EXPIRE
            string jobNameSetPayinExpire = "set_payin_expire";
            string cronExpressionSetPayinExpire = ConfigurationManager.AppSettings["PAYIN_JOB_SET_EXPIRATION_CRON_EXPRESSION"].ToString();
            RecurringJob.RemoveIfExists(jobNameSetPayinExpire);
            RecurringJob.AddOrUpdate(jobNameSetPayinExpire, () => AutomatedProcesses.SetExpirePayins() , cronExpressionSetPayinExpire, TimeZoneInfo.Local, queue: HangfireBootstrapper.PayinTaskQueue.ToLower());

            //REJECT ONHOLD
            string jobNameRejectOnHold = "reject_onhold";
            string cronExpressionRejectOnHold = ConfigurationManager.AppSettings["ONHOLD_JOB_SET_EXPIRATION_CRON_EXPRESSION"].ToString();
            RecurringJob.RemoveIfExists(jobNameRejectOnHold);
            RecurringJob.AddOrUpdate(jobNameRejectOnHold, () => AutomatedProcesses.RejectExpiredOnHold(), cronExpressionRejectOnHold, TimeZoneInfo.Local, queue: HangfireBootstrapper.OnholdTaskQueue.ToLower());

        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
              //  HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, customer_id, api_key, customer_id_to, countryCode");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.AddHeader("Access-Control-Request-Method", "*");
                //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*, http://localhost:4200");
            }
        }
    }

    public class AddCustomHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.Add("customHeader", "custom value date time");
        }
    }
}
