using System;

namespace SharedModel.Models.Security
{
    public class AccountModel
    {
        public class Login
        {
            public string ClientID { get; set; }
            public string Password { get; set; }

            public bool WebAcces { get; set; }
        }
    }
}
