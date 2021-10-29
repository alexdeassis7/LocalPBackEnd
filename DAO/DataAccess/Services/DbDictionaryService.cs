using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO.Services.PayOut;
using Dapper;

namespace DAO.DataAccess.Services
{
    public class DbDictionaryService
    {
        private const string ConnStringLocalPayment = "connDbLocalPayment";
        private const string SpPayoutsConcepts = "[LP_Entity].[Get_Payout_Concepts]";

        public IEnumerable<PayoutConcept> Get_Payouts_Concepts()
        {
            IEnumerable<PayoutConcept> payoutConcepts = null;

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnStringLocalPayment].ConnectionString))
            {
                payoutConcepts = connection.Query<PayoutConcept>(SpPayoutsConcepts, commandType: System.Data.CommandType.StoredProcedure);
            }

            return payoutConcepts;
        }
    }
}
