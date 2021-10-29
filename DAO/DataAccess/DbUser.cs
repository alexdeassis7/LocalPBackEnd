using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace DAO.DataAccess
{
    public class DbUser : DAO.Interfaces.Security.IDbSecurity
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public DbUser() { }
        public SharedModel.Models.User.User GetUserInfo(SharedModel.Models.Security.AccountModel.Login Credential)
        {
            SharedModel.Models.User.User UserInf = new SharedModel.Models.User.User();
            SharedModel.Models.Database.General.Result.Validation ValidationResult = new SharedModel.Models.Database.General.Result.Validation();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[GetEntityInfo]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@ClientID", SqlDbType.VarChar, 50) { Value = Credential.ClientID };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@App", SqlDbType.Bit) { Value = Credential.WebAcces };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                UserInf = JsonConvert.DeserializeObject<SharedModel.Models.User.User>(_ds.Tables[0].Rows[0][0].ToString());

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return UserInf;
        }

        public List<SharedModel.Models.User.User> GetListUsers()
        {
            List<SharedModel.Models.User.User> ListUsers = new List<SharedModel.Models.User.User>();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[GetListUsers]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                ListUsers = JsonConvert.DeserializeObject<List<SharedModel.Models.User.User>>(_ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception)
            {

                throw;
            }
            return ListUsers;


        }

        public List<SharedModel.Models.User.User> GetListKeyUsers()
        {
            List<SharedModel.Models.User.User> ListUsers = new List<SharedModel.Models.User.User>();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);


                if (_conn.State == ConnectionState.Open)
                    _conn.Close();

                _cmd = new SqlCommand("[LP_Entity].[GetListKeyUsers]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_conn.State == ConnectionState.Open)
                    _conn.Close();
                ListUsers = JsonConvert.DeserializeObject<List<SharedModel.Models.User.User>>(_ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception)
            {

                throw;
            }
            return ListUsers;


        }
    }
}
