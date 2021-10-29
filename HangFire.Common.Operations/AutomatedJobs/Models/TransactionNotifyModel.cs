using SharedModel.Models.Services.Colombia.Banks.Bancolombia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace HangFire.Common.Operations.AutomatedJobs.Models
{
    public class TransactionNotifyModel
    {
        public IEnumerable<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail> GetColombiaTransactionDetails(int idEntityUserParam)
        {
            string sp = "[LP_Operation].[COL_Get_Transactions_To_Notify]";

            IEnumerable<PayOutColombia.UploadTxtFromBank.TransactionDetail> transactions = null;

            using (var _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                _conn.Open();
                transactions = _conn.Query<PayOutColombia.UploadTxtFromBank.TransactionDetail>(sp, new { idEntityUser = idEntityUserParam }, commandType: CommandType.StoredProcedure);
            }

            return transactions;
        }
    }
}
