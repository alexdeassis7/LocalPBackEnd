using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace DAO.DataAccess.Services
{
    public class DbCashPayment
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public bool ValidateTicket(string Ticket, bool TransactionMechanism)
        {
            bool isValid = false;

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Cashpayment_Generic_EntityOperation_Ticket_Validate]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Ticket", SqlDbType.VarChar) { Value = Ticket };
                _cmd.Parameters.Add(_prm);
                if (TransactionMechanism == true)
                {
                    _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = TransactionMechanism };
                    _cmd.Parameters.Add(_prm);
                }
                else
                {
                    _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = false };
                    _cmd.Parameters.Add(_prm);
                }

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (Convert.ToBoolean(_dt.Rows[0][0].ToString()))
                    isValid = true;
                else
                    isValid = false;
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }

        public string ListStatusCashPayments(string customer_id)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[ARG_Cashpayment_Generic_EntityOperation_List]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Customer", SqlDbType.VarChar, 12) { Value = customer_id };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                return _ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class DbBarcode
        {
            SqlConnection _conn;
            SqlCommand _cmd;
            SqlParameter _prm;
            SqlDataAdapter _da;
            DataSet _ds;
            DataTable _dt;

            public string BarCodeTicketGenerator(string TransactionType, string Invoice, string AdditionalInfo)
            {
                try
                {
                    _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();

                    _cmd = new SqlCommand("[LP_Operation].[ARG_Cashpayment_Generic_EntityOperation_Ticket_Get]", _conn);
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandTimeout = 0;
                    _prm = new SqlParameter("@TransactionType", SqlDbType.VarChar, 4) { Value = TransactionType };
                    _cmd.Parameters.Add(_prm);
                    _prm = new SqlParameter("@Invoice", SqlDbType.VarChar) { Value = Invoice };
                    _cmd.Parameters.Add(_prm);
                    _prm = new SqlParameter("@AdditionalInfo", SqlDbType.VarChar) { Value = AdditionalInfo };
                    _cmd.Parameters.Add(_prm);
                    _prm = new SqlParameter("@Action", SqlDbType.VarChar) { Value = "GENERATE" };
                    _cmd.Parameters.Add(_prm);

                    _ds = new DataSet();
                    _da = new SqlDataAdapter(_cmd);
                    _da.Fill(_ds);

                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();

                    return _ds.Tables[0].Rows[0][0].ToString();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.Common CreateLotTransaction(SharedModelDTO.Models.Transaction.PayWay.PayIn.CashPayment.Distributed.Types.Common data, bool TransactionMechanism, string countryCode)
            {
                try
                {
                    _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();

                    _cmd = new SqlCommand("[LP_Operation].[ARG_Cashpayment_Generic_EntityOperation_Create]", _conn);
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.CommandTimeout = 0;
                    _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                    _cmd.Parameters.Add(_prm);
                    _prm = new SqlParameter("@country_code", SqlDbType.VarChar, 3) { Value = countryCode };
                    _cmd.Parameters.Add(_prm);
                    if (TransactionMechanism == true)
                    {
                        _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = TransactionMechanism };
                        _cmd.Parameters.Add(_prm);
                    }
                    else
                    {
                        _prm = new SqlParameter("@TransactionMechanism", SqlDbType.Bit) { Value = false };
                        _cmd.Parameters.Add(_prm);
                    }

                    _dt = new DataTable();
                    _da = new SqlDataAdapter(_cmd);
                    _da.Fill(_dt);

                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();

                    return data;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
