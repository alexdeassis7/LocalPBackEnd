using DAO.DataAccess.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SharedBusiness.Security
{
    public class BlSecurity
    {
        public static void SetDbLog(string message)
        {
            DAO.DataAccess.Security.DbLogger.SetDbLog(message);
        }

        public SharedModel.Models.Database.Security.Authentication.Account GetAuthentication(SharedModel.Models.Security.AccountModel.Login Credential)
        {
            SharedModel.Models.Database.Security.Authentication.Account Account = new SharedModel.Models.Database.Security.Authentication.Account();

            try
            {
                DAO.DataAccess.Security.DbSecurity DbSecurity = new DAO.DataAccess.Security.DbSecurity();
                Account = DbSecurity.GetAuthentication(Credential);

                if (Account.Login != null)
                {
                    Account.Login.WebAcces = Credential.WebAcces;
                }

            }
            catch (Exception ex)
            {
                Account.ValidationResult = new SharedModel.Models.Database.General.Result.Validation() { ValidationStatus = false, ValidationMessage = ex.ToString() };
            }
            return Account;
        }

        public SharedModel.Models.Database.Security.Authentication.Account GetAuthentication(string ClientID)
        {
            SharedModel.Models.Database.Security.Authentication.Account Account = new SharedModel.Models.Database.Security.Authentication.Account();

            try
            {
                DAO.DataAccess.Security.DbSecurity DbSecurity = new DAO.DataAccess.Security.DbSecurity();
                Account = DbSecurity.GetAuthentication(ClientID);
            }
            catch (Exception ex)
            {
                Account.ValidationResult = new SharedModel.Models.Database.General.Result.Validation() { ValidationStatus = false, ValidationMessage = ex.ToString() };
            }
            return Account;
        }

        public void PutLoginToken(string Identification, bool Mechanism, string Token)
        {
            try
            {
                DAO.DataAccess.Security.DbSecurity DbSecurity = new DAO.DataAccess.Security.DbSecurity();
                DbSecurity.PutLoginToken(Identification, Mechanism, Token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetValidationCBU(string CBU)
        {
            try
            {
                DAO.DataAccess.Security.DbSecurity DbSecurity = new DAO.DataAccess.Security.DbSecurity();
                return DbSecurity.GetValidationCBU(CBU);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public KeyPair GetKeyPair(string identification) 
        {
            DbSecurity DbSecurity = new DbSecurity();

            return DbSecurity.GetKeyPairs(identification).ElementAtOrDefault(0);
        }

        public void SetKeyPair(string identification, string privateKey, string publicKey)
        {
            DbSecurity DbSecurity = new DbSecurity();

            DbSecurity.SetKeyPairs(identification, privateKey, publicKey);
        }

        public void SetHMACKey(string identification, string key)
        {
            DbSecurity DbSecurity = new DbSecurity();

            DbSecurity.SetHMACKey(identification, key);
        }

        public string GetHMACKey(string identification)
        {
            DbSecurity DbSecurity = new DbSecurity();

            return DbSecurity.GetHMACKey(identification).ElementAtOrDefault(0);
        }
    }
}
