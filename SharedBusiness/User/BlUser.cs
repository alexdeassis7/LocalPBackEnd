using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.User
{
    public class BlUser
    {
        public SharedModel.Models.User.User GetUserInfo(SharedModel.Models.Security.AccountModel.Login Credential)
        {
            SharedModel.Models.User.User UserInfo = new SharedModel.Models.User.User();

            try
            {
                DAO.DataAccess.DbUser DbSecurity = new DAO.DataAccess.DbUser();
                UserInfo = DbSecurity.GetUserInfo(Credential);
            }
            catch (Exception ex)
            {
                throw ex;
               // Account.ValidationResult = new SharedModel.Models.Database.General.Result.Validation() { ValidationStatus = false, ValidationMessage = ex.ToString() };
            }
            return UserInfo;
        }

        public List<SharedModel.Models.User.User> GetListUsers() {

            List<SharedModel.Models.User.User> ListUsers = new List<SharedModel.Models.User.User>();
            
            try
            {
                DAO.DataAccess.DbUser DbUser= new DAO.DataAccess.DbUser();

                ListUsers = DbUser.GetListUsers();

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
                DAO.DataAccess.DbUser DbUser = new DAO.DataAccess.DbUser();

                ListUsers = DbUser.GetListKeyUsers();

            }
            catch (Exception)
            {

                throw;
            }

            return ListUsers;


        }
    }
}
