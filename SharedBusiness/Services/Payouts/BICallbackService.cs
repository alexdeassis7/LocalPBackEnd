using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO.DataAccess.Services;

namespace SharedBusiness.Services.Payouts
{
    public class BICallbackService
    {
        public string GetCallbackConfiguration(string idEntityUser)
        {
            DbCallbackService DBCs = new DbCallbackService();
            return DBCs.GetCallbackConfiguration(int.Parse(idEntityUser));
        }

        public string GetEntityUserByTxId(long? transaction_id)
        {
            DbCallbackService DBCs = new DbCallbackService();
            return DBCs.GetEntityUserByTxId(transaction_id);
        }
    }
}
