using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Security;
using SharedModelDTO.Models.Transaction.PayWay.PayIn;

namespace SharedBusiness.Security
{
    public class BlLog
    {
        public string InsertTransactionErrors(List<TransactionError.ModelDB> ListTxError, string customer_id, string countryCode, bool TransactionMechanism) {
            var result = "";
            try
            {          
                DAO.DataAccess.Security.DbLog dbLog = new DAO.DataAccess.Security.DbLog();
                dbLog.InsertTransactionErrors(ListTxError, customer_id, countryCode, TransactionMechanism);
            }
            catch (Exception ex)
            {

                throw;
            }

            return result;
        }
        public void Logout(string customer, bool TransactionMechanism, string countryCode) {

            try
            {
                DAO.DataAccess.Security.DbLog dbLog = new DAO.DataAccess.Security.DbLog();
                dbLog.Logout(customer, TransactionMechanism, countryCode);

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void InsertPayinErrors(SharedModel.Models.Services.Payins.PayinModel.RejectedPayins.TransactionError ListTxError, string customer_id, string countryCode)
        {
            try
            {
                DAO.DataAccess.Security.DbLog dbLog = new DAO.DataAccess.Security.DbLog();
                dbLog.InsertPayinErrors(ListTxError, customer_id, countryCode);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
