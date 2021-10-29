using SharedBusiness.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WA_LP.Controllers.Health
{
    [RoutePrefix("v2")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HealtCheckApiController : ApiController
    {
        const string HealthCheckKey = "HealthCheck_Code";
        [Route("healthcheck")]
        public bool Get([FromUri] string code)
        {
            if (code == ConfigurationManager.AppSettings[HealthCheckKey])
            {
                return new MonitorService().HealtCheckDb();
            }
            else
                throw new Exception("Invalid Code");
        }
    }
}