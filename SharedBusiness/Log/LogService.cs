
//alex
using Serilog.Core;
using SharedBusiness.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace SharedBusiness.Log
{
    public static class LogService
      {
    //    public static void Main(string[] args)
    //    {

    //    }
        //private readonly
        //alex
        private static Logger log;
        private static List<string> supportEmails;
        public static void Configure()
        {
            
                //alex
                // var loggerConfiguration= new LoggerConfiguration().WriteTo.MSSqlServer(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString, schemaName: "LP_Log", tableName: "AuditLog", autoCreateSqlTable: true);
                //var loggerConfiguration = new LoggerConfiguration().ReadFrom.AppSettings();
                //loggerConfiguration.WriteTo.MSSqlServer(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString, schemaName: "LP_Log", tableName: "AuditLog", autoCreateSqlTable: true).Async(x => x.MSSqlServer(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString, "AudittLog",autoCreateSqlTable: true));
                //alex
                //log = loggerConfiguration.CreateLogger();

                var Log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(@"C:\Users\alex\Desktop\git repo 2\localpayment\BackgroundSide\log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
            Console.WriteLine();
            
            //int a = 10, b = 0;
            //try
            //{
            //    Log.Debug("Dividing {A} by {B}", a, b);
            //    Log.Error("Dividing {A} by {B}", a, b);
            //    Log.Information("Dividing {A} by {B}", a, b);
            //   // Log.Write("Dividing {A} by {B}", a, b);
            //    Console.WriteLine(a / b);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex, "Something went wrong");
            //}

            supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
        }
        public static void LogError(Exception e, string customerId = "", string request = "")
        {
            try
            {
                //alex  log.Error(string.Format("CLIENT: {0}, REQUEST: {1}, ERROR:{2}", customerId, request, e.Message));
                MailService.SendMail(string.Format("Error - Cliente:{0}", customerId), string.Format("REQUEST: {0} ,ERROR: {1}", request, e.Message), supportEmails);
            }
            catch { }
        }
        public static void LogInfo(string val)
        {
            try
            {
                //alex
               // log.Information(val);
            }
            catch { }
        }
        public static void LogError(string val, string customerId = "")
        {
            try
            {
                //alex
                log.Error(val);
                MailService.SendMail(string.Format("Error - Cliente:{0}", customerId), string.Format("Error: {0}", val), supportEmails);
            }
            catch { }
        }

        public static async Task LogErrorAsync(Exception e, string customerId, string request = "")
        {
            //alex
            log.Error(string.Format("CLIENT: {0}, REQUEST: {1}, ERROR:{2}", customerId, request, e.Message));

            await Task.Run(() => {
                try
                {
                    MailService.SendMail(string.Format("Error - Cliente:{0}", customerId), string.Format("REQUEST: {0} ,ERROR: {1}", request, e.Message), supportEmails);
                }
                catch
                { }
            });

        }

  
    }
}
