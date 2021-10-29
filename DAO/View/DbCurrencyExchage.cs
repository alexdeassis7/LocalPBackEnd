using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DataAccess.View
{
    public class DbCurrencyExchage
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public SharedModel.Models.View.CurrencyExchange.Single.Response GetCurrencyExchange (string baseCurrency, string quoteCurrency, string transactionType, string date, string withSource, string customerId, SharedModel.Models.View.CurrencyExchange.Single.Response data)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[GetCurrencyExchange]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("currencyBase", SqlDbType.VarChar) { Value = baseCurrency };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("currencyTo", SqlDbType.VarChar) { Value = quoteCurrency };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("customerId", SqlDbType.VarChar) { Value = customerId };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("transactionType", SqlDbType.VarChar) { Value = transactionType };
                _cmd.Parameters.Add(_prm);

                if (date != null)
                {
                    _prm = new SqlParameter("date", SqlDbType.VarChar) { Value = date };
                    _cmd.Parameters.Add(_prm);
                }

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                decimal quote = _ds.Tables[0].Rows[0][2] == DBNull.Value ? 0 : (decimal)_ds.Tables[0].Rows[0][2];
                quote *= 1000000;
                data.quote = long.Parse(Decimal.Round(quote, 0).ToString());

                data.quote_date = _ds.Tables[0].Rows[0][3].ToString();

                if(_ds.Tables[0].Rows[0][5] != DBNull.Value)
                {
                    data.valid_from_datetime = Convert.ToDateTime(_ds.Tables[0].Rows[0][5].ToString()).ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff zzz");
                }
                if(_ds.Tables[0].Rows[0][6] != DBNull.Value)
                {
                    data.expiration_datetime = Convert.ToDateTime(_ds.Tables[0].Rows[0][6].ToString()).ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fffffff zzz");
                }

                if (withSource != null)
                {
                    decimal sourceQuote = _ds.Tables[0].Rows[0][2] == DBNull.Value ? 0 : (decimal)_ds.Tables[0].Rows[0][4];
                    sourceQuote *= 1000000;
                    //data.source_quote = int.Parse(Decimal.Round(sourceQuote, 0).ToString());
                }

                bool status = Boolean.Parse(_ds.Tables[0].Rows[0][0].ToString());

                if (!status)
                {
                    SharedModel.Models.General.ErrorModel.ValidationErrorGroup msg = new SharedModel.Models.General.ErrorModel.ValidationErrorGroup();
                    msg.Key = "CurrencyExchange";
                    msg.Messages = new List<string>();
                    msg.Messages.Add(_ds.Tables[0].Rows[0][1].ToString());
                    data.ErrorRow.Errors.Add(msg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return data;
        }

        public List<SharedModel.Models.View.CurrencyExchange.List.Response> GetListCurrencyExchange(string baseCurrency, string entityUser, string customerId)
        {
            try
            {
                List<SharedModel.Models.View.CurrencyExchange.List.Response> data = new List<SharedModel.Models.View.CurrencyExchange.List.Response>();
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[GetListCurrencyExchange]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("currencyBase", SqlDbType.VarChar) { Value = baseCurrency };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("entityUser", SqlDbType.VarChar) { Value = entityUser };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("customerId", SqlDbType.VarChar) { Value = customerId };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                //data.quote = _ds.Tables[0].Rows[0][2] == DBNull.Value ? 0 : (decimal)_ds.Tables[0].Rows[0][2];
                //data.Status = _ds.Tables[0].Rows[0][0].ToString();
                //data.StatusMessage = _ds.Tables[0].Rows[0][1].ToString();
                return data;
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
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Configuration].[CleanCurrencyExchange]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
