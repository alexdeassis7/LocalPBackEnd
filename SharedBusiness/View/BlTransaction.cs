using SharedModel.Models.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.View
{
    public class BlTransaction
    {
        public SharedModel.Models.View.TransactionModel.Close.Response TransactionsExchangeClosed(SharedModel.Models.View.TransactionModel.Close.Request model)
        {
            SharedModel.Models.View.TransactionModel.Close.Response response = new SharedModel.Models.View.TransactionModel.Close.Response();
            try
            {                
                DAO.View.DbTransaction DbTransaction = new DAO.View.DbTransaction();
                response = DbTransaction.TransactionsExchangeClosed(model);
            }
            catch (Exception ex)
            {
                response.status = "ERROR";
                response.status_message = ex.ToString();
            }
            return response;
        }

        public SharedModel.Models.View.TransactionModel.Transaction.Response TransactionCreate(string customer, SharedModel.Models.View.TransactionModel.Transaction.Response model, bool transactionMechanism)
        {
            try
            {
                SharedModel.Models.View.TransactionModel.Transaction.Response response = new SharedModel.Models.View.TransactionModel.Transaction.Response();
                DAO.View.DbTransaction db = new DAO.View.DbTransaction();

                response = db.TransactionCreate(customer, model, transactionMechanism);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransactionModel.TransactionLot.Response> GetTransactionLots(string lotId = null)
        {
            try
            {
                List<TransactionModel.TransactionLot.Response> response = new List<TransactionModel.TransactionLot.Response>();
                DAO.View.DbTransaction db = new DAO.View.DbTransaction();

                response = db.GetTransactionLots(lotId);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
