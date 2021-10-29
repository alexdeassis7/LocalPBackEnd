using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;


namespace DAO.DataAccess.Services
{
    public class DbDebin
    {

        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        //public string CrearTicket(string ticket)
        //{
        //    try
        //    {
        //        _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

        //        if (_conn.State == ConnectionState.Open)
        //            _conn.Close();

        //        _cmd = new SqlCommand("[LP_Operation].[CreateTicket]", _conn);
        //        _cmd.CommandType = CommandType.StoredProcedure;

        //        _ds = new DataSet();
        //        _da = new SqlDataAdapter(_cmd);
        //        _da.Fill(_ds);

        //        ticket=Convert.ToString(_ds.Tables[0].Rows[0][0]);


        //        if (_conn.State == ConnectionState.Open)
        //            _conn.Close();
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }


        //    return ticket;

        //}

        public SharedModelDTO.Models.Transaction.Debin.InternalDebinModel CreateDebinTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel data, string customer, bool TransactionMechanism)
        {
            SharedModelDTO.Models.Transaction.Debin.InternalDebinModel result = new SharedModelDTO.Models.Transaction.Debin.InternalDebinModel();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CreateDebinTransaction]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
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

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                result = JsonConvert.DeserializeObject<SharedModelDTO.Models.Transaction.Debin.InternalDebinModel>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        //public SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines ActualizarDebines(List<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response> data, string customer, bool TransactionMechanism)
        public SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines ActualizarDebines(SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response data, string customer, bool TransactionMechanism)
        {
            SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines result = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[UpdateDebins]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
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

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                if (_ds.Tables[0].Rows.Count is 0)
                {
                    result = null;
                }
                else
                {
                    result = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines>(_ds.Tables[0].Rows[0][0].ToString());
                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines DeleteDebines(SharedModel.Models.Services.Banks.Bind.Debin.RequestDeleteDebines data, string customer, bool TransactionMechanism)
        {
            SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines result = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[DeleteDebins]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@json", SqlDbType.NVarChar, 5000) { Value = JsonConvert.SerializeObject(data) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@customer", SqlDbType.NVarChar, 12) { Value = customer };
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

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);
                if (_ds.Tables[0].Rows.Count is 0)
                {
                    result = null;
                }
                else
                {
                    result = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines>(_ds.Tables[0].Rows[0][0].ToString());

                }

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
