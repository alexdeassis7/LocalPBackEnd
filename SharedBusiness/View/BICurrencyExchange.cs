using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.View
{
    public class BICurrencyExchange
    {
        public SharedModel.Models.View.CurrencyExchange.Single.Response GetCurrencyExchange(string baseCurrency, string quoteCurrency, string transactionType, string date, string withSource, string customerId, SharedModel.Models.View.CurrencyExchange.Single.Response data)
        {
            try
            {
                DAO.DataAccess.View.DbCurrencyExchage dbCurrencyExchage = new DAO.DataAccess.View.DbCurrencyExchage();
                dbCurrencyExchage.GetCurrencyExchange(baseCurrency, quoteCurrency, transactionType, date, withSource, customerId, data);
                data.base_currency = baseCurrency;
                data.quote_currency = quoteCurrency;
                data.transaction_type = transactionType;
                //data.with_source = withSource;
                data.date = date;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return data;
        }

        public List<SharedModel.Models.View.CurrencyExchange.List.Response> GetListCurrencyExchange(string baseCurrency, string entityUser, string customerId)
        {
            try
            {
                DAO.DataAccess.View.DbCurrencyExchage dbCurrencyExchage = new DAO.DataAccess.View.DbCurrencyExchage();
                return dbCurrencyExchage.GetListCurrencyExchange(baseCurrency, entityUser, customerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CleanCurrencyExchange()
        {
            try
            {
                DAO.DataAccess.View.DbCurrencyExchage dbCurrencyExchage = new DAO.DataAccess.View.DbCurrencyExchage();
                dbCurrencyExchage.CleanCurrencyExchange();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
