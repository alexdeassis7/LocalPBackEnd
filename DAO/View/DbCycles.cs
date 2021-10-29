using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DataAccess.View
{
    public class DbCycles
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public SharedModel.Models.View.CycleModel.Create.Response InsertEntity(string customer, SharedModel.Models.View.CycleModel.Create.Response data)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                if (data.Type == "PROV")
                {
                    _cmd = new SqlCommand("[LP_Configuration].[InsertProviderCycles]", _conn);
                    _prm = new SqlParameter("@idProvider", SqlDbType.BigInt) { Value = data.idEntity };
                    _cmd.Parameters.Add(_prm);
                }
                else if (data.Type == "PWS")
                {
                    _cmd = new SqlCommand("[LP_Configuration].[InsertPayWayServicesCycles]", _conn);
                    _prm = new SqlParameter("@idPayWayService", SqlDbType.BigInt) { Value = data.idEntity };
                    _cmd.Parameters.Add(_prm);
                }
                else
                {
                    _cmd = new SqlCommand("[LP_Configuration].[InsertEntityCycles]", _conn);
                    _prm = new SqlParameter("@idEntityAccount", SqlDbType.BigInt) { Value = data.idEntity };
                    _cmd.Parameters.Add(_prm);
                }
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@idTransactionType", SqlDbType.Int) { Value = data.idTransactionType };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@isMonthly", SqlDbType.Bit) { Value = Convert.ToBoolean(data.isMonthly) };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@TransactionFromDay", SqlDbType.Int) { Value = data.TransactionFromDay };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@TransactionToDay", SqlDbType.Int) { Value = data.TransactionToDay };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@AddDaysToPay", SqlDbType.Int) { Value = data.AddDaysToPay };
                _cmd.Parameters.Add(_prm);

                _conn.Open();
                _cmd.ExecuteNonQuery();
                _conn.Close();

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                data.Status = "OK";
                data.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                data.Status = "Error";
                data.StatusMessage = ex.ToString();
            }

            return data;
        }
    }
}
