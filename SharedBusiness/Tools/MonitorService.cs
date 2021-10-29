using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO.DataAccess.Tools;

namespace SharedBusiness.Tools
{
    public class MonitorService
    {
        public bool HealtCheckDb()
        {
            return new DbMonitor().HealthCheck();
        }
    }
}
