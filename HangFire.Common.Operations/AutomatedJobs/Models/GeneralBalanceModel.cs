using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HangFire.Common.Operations.AutomatedJobs.Models
{
    public class GeneralBalanceModel
    {
       public class ClientDashboardData {
            
            public string LastName { get; set; }
            public string CurrencyCode { get; set; }
            public decimal SaldoActual { get; set; }
            public decimal AmtInProgress { get; set; }
            public int cntInProgress { get; set; }
            public decimal AmtReceived { get; set; }
            public int cntReceived { get; set; }
        }
    }
}