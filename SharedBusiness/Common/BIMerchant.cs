using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace SharedBusiness.Common
{
    public class BIMerchant
    {
        public string GetCountryCodeByidMerchantSelected(string customer, Int64 idMerchantSelected)
        {
            try
            {
                DAO.DataAccess.Common.DbMerchant dbMerchant = new DAO.DataAccess.Common.DbMerchant();

                return dbMerchant.GetCountryCodeByMerchantSelected(customer, idMerchantSelected);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
