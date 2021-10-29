using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_LP.App_Start
{
    public class ApplicationPreload : System.Web.Hosting.IProcessHostPreloadClient
    {
        public void Preload(string[] parameters)
        {
            LogService.LogInfo("Application Preload");
            HangfireBootstrapper.Instance.Start();
        }
    }
}