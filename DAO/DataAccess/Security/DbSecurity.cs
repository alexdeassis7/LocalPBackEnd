using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DAO.DataAccess.Security
{
    public class DbSecurity : DAO.Interfaces.Security.IDbSecurity
    {
        SqlConnection _conn;
        SqlCommand _cmd;
        SqlParameter _prm;
        SqlDataAdapter _da;
        DataSet _ds;
        DataTable _dt;

        public DbSecurity() { }

        public bool GetValidationCBU(string CBU)
        {
            bool isValid = false;
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Open();

                _cmd = new SqlCommand("[LP_Security].[GetValidationCBU]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@CBU", SqlDbType.VarChar) { Value = CBU };
                _cmd.Parameters.Add(_prm);

                _dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_dt);

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Close();

                if (_dt.Rows.Count > 0)
                {
                    if (Convert.ToBoolean(_dt.Rows[0][0].ToString()))
                        isValid = true;
                    else
                        isValid = false;
                }
                else
                {
                    isValid = false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }
            return isValid;
        }

        public SharedModel.Models.Database.Security.Authentication.Account GetAuthentication(SharedModel.Models.Security.AccountModel.Login Credential)
        {
            SharedModel.Models.Database.Security.Authentication.Account Account = new SharedModel.Models.Database.Security.Authentication.Account();
            SharedModel.Models.Database.General.Result.Validation ValidationResult = new SharedModel.Models.Database.General.Result.Validation();
            SharedModel.Models.Security.AccountModel.Login login = new SharedModel.Models.Security.AccountModel.Login();
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Open();

                _cmd = new SqlCommand("[LP_Security].[Login]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@ClientID", SqlDbType.VarChar, 50) { Value = Credential.ClientID };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@ClientPassword", SqlDbType.VarChar, 50) { Value = Credential.Password };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@App", SqlDbType.Bit) { Value = Credential.WebAcces };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_ds.Tables.Count > 1)
                {
                    Account.SecretKey = _ds.Tables[1].Rows[0][0].ToString();
                    login.ClientID = _ds.Tables[1].Rows[0][1].ToString();
                    Account.Login = login;
                }

                ValidationResult.ValidationStatus = Convert.ToBoolean(_ds.Tables[0].Rows[0][0].ToString());
                ValidationResult.ValidationMessage = _ds.Tables[0].Rows[0][1].ToString();

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Close();
            }
            catch (Exception ex)
            {
                ValidationResult.ValidationStatus = false;
                ValidationResult.ValidationMessage = ex.Message;
            }

            Account.ValidationResult = ValidationResult;

            return Account;
        }

        public SharedModel.Models.Database.Security.Authentication.Account GetAuthentication(string ClientID)
        {
            SharedModel.Models.Database.Security.Authentication.Account Account = new SharedModel.Models.Database.Security.Authentication.Account();
            SharedModel.Models.Database.General.Result.Validation ValidationResult = new SharedModel.Models.Database.General.Result.Validation();

            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Open();

                _cmd = new SqlCommand("[LP_Security].[GetSecretKeyByClientID]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@ClientID", SqlDbType.VarChar, 50) { Value = ClientID };
                _cmd.Parameters.Add(_prm);

                _ds = new DataSet();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(_ds);

                if (_ds.Tables.Count > 1)
                {
                    Account.SecretKey = _ds.Tables[1].Rows[0][0].ToString();
                }

                ValidationResult.ValidationStatus = Convert.ToBoolean(_ds.Tables[0].Rows[0][0].ToString());
                ValidationResult.ValidationMessage = _ds.Tables[0].Rows[0][1].ToString();

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Close();
            }
            catch (Exception ex)
            {
                ValidationResult.ValidationStatus = false;
                ValidationResult.ValidationMessage = ex.ToString();
            }

            Account.ValidationResult = ValidationResult;

            return Account;
        }

        public void PutLoginToken(string Identification, bool Mechanism, string Token)
        {
            try
            {
                _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString);
                
                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Open();

                _cmd = new SqlCommand("[LP_Security].[Login_TokenUpdate]", _conn);
                _cmd.CommandType = CommandType.StoredProcedure;
                _cmd.CommandTimeout = 0;
                _prm = new SqlParameter("@Identification", SqlDbType.VarChar, 100) { Value = Identification };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@App", SqlDbType.Bit) { Value = Mechanism };
                _cmd.Parameters.Add(_prm);
                _prm = new SqlParameter("@JWT_Token", SqlDbType.VarChar, Int32.MaxValue) { Value = Token };
                _cmd.Parameters.Add(_prm);

                //_conn.Open();
                _cmd.ExecuteNonQuery();
                //_conn.Close();

                /*if (_conn.State == ConnectionState.Open)*/
                _conn.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<KeyPair> GetKeyPairs(string identification)
        {
            IEnumerable<KeyPair> keyPairs = null;

            string sql = "SELECT [publicKey] ,[privateKey] FROM [LP_Security].[EntityAccount] WHERE Identification =  @identification";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                keyPairs = connection.Query<KeyPair>(sql,new { identification = identification }, commandType: System.Data.CommandType.Text);
            }

            return keyPairs;
        }

        public void SetKeyPairs(string identification, string privateKey, string publicKey)
        {
            string sql = "UPDATE [LP_Security].[EntityAccount] SET [publicKey] = @publicKey, [privateKey] = @privateKey WHERE [Identification] = @identification";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
               var affectedRows = connection.Execute(sql, new { publicKey = publicKey, privateKey = privateKey, identification = identification });
            }
        }

        public void SetHMACKey(string identification, string key)
        {
            string sql = "UPDATE [LP_Security].[EntityAccount] SET [HMACKey] = @key WHERE [Identification] = @identification";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                var affectedRows = connection.Execute(sql, new { key = key, identification = identification });
            }
        }

        public IEnumerable<string> GetHMACKey(string identification)
        {
            IEnumerable<string> key = null;

            string sql = "SELECT [HMACKey] FROM [LP_Security].[EntityAccount] WHERE Identification =  @identification";

            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                key = connection.Query<string>(sql, new { identification = identification }, commandType: System.Data.CommandType.Text);
            }

            return key;
        }
    }
}
