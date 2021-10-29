using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Services.Decidir;
using SharedModelDTO.Models.Transaction.Decidir;

namespace SharedBusiness.Services.Decidir
{
    public class BIDecidir
    {
        public DecidirTransaction Payment(DecidirTransaction paymentData, bool transactionMechanism)
        {
            try
            {
                DAO.DataAccess.Services.DbDecidir LPDAO = new DAO.DataAccess.Services.DbDecidir();
                var pay = LPDAO.CreateDecidirTransaction(paymentData, transactionMechanism);

                return pay;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
