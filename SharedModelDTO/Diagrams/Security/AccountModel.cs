using SharedModelDTO.Models.Security.Distributed;

namespace SharedModelDTO.Models.Security
{
    public class AccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public LoginModel Login { get; set; }
    }
}
