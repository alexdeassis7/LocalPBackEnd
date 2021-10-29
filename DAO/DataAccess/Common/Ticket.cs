using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO.DataAccess.Common
{
    public class Ticket
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;
        public string CrearTicket(string ticket)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Operation].[CreateTicket]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                ticket = Convert.ToString(_ds.Tables[0].Rows[0][0]);


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }


            return ticket;

        }
    }
}
