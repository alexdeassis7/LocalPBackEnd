using Hangfire;
using Hangfire.SqlServer;
using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WA_LP.HangFireCustomLogger;

namespace WA_LP.App_Start
{
    public class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        public const string PayoneerArgReportQueue = "PayoneerArgQueue";
        public const string PayoneerColReportQueue = "PayoneerColQueue";
        public const string PayoneerBraReportQueue = "PayoneerBraQueue";
        public const string PayoneerMexReportQueue = "PayoneerMexQueue";
        public const string PayoneerUryReportQueue = "PayoneerUryQueue";
        public const string PayoneerChlReportQueue = "PayoneerChlQueue";
        public const string NiumMerchantReportsQueue = "NiumReportsQueue";
        public const string AfipQueue = "AfipQueue";
        public const string GeneralBlceQueue = "GeneralBlceQueue";
        public const string TranLotQueue = "TranLotQueue";
        public const string CurrencyCleanQueue = "CurrencyCleanQueue";
        public const string AmlQueue = "AmlQueue";
        public const string NotificationPushRetryQueue = "NotificationPushRetryQueue";
        public const string PayinTaskQueue = "PayinTaskQueue";
        public const string OnholdTaskQueue = "OnholdTaskQueue";

        private HangfireBootstrapper()
        {
        }

        public void Start()
        {
            LogService.LogInfo("HangFire instance Start");

            lock (_lockObject)
            {
                if (_started) return;
                _started = true;

                HostingEnvironment.RegisterObject(this);

                Hangfire.GlobalConfiguration.Configuration
                .UseLogProvider(new CustomLogProvider())
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                });
                // Specify other options here

                var options = new BackgroundJobServerOptions
                {
                    Queues = new[] { PayoneerArgReportQueue.ToLower(), PayoneerColReportQueue.ToLower(), PayoneerBraReportQueue.ToLower(), PayoneerMexReportQueue.ToLower(), AfipQueue.ToLower(), GeneralBlceQueue.ToLower(), TranLotQueue.ToLower(), CurrencyCleanQueue.ToLower(), AmlQueue.ToLower(), PayinTaskQueue.ToLower(), NotificationPushRetryQueue.ToLower(), NiumMerchantReportsQueue.ToLower(), PayoneerChlReportQueue.ToLower(),PayoneerUryReportQueue.ToLower(), OnholdTaskQueue.ToLower() }
                };

                _backgroundJobServer = new BackgroundJobServer(options);
            }
        }

        public void Stop()
        {
            LogService.LogInfo("HangFire instance Stop");

            lock (_lockObject)
            {
                if (_backgroundJobServer != null)
                {
                    _backgroundJobServer.Dispose();
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            Stop();
        }
    }
}