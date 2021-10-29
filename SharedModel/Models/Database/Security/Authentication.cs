using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Security;
namespace SharedModel.Models.Database.Security
{
    public class Authentication
    {
        public class Account
        {
            public string SecretKey { get; set; }
            public General.Result.Validation ValidationResult { get; set; }

            public Models.Security.AccountModel.Login Login { get; set; }
        }

        public class TokenAccount : Account
        {
            public Token Token { get; set; }
            public TokenWeb TokenWeb { get; set; }
        }
    }
}
