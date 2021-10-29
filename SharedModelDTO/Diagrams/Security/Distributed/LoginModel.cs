using SharedModelDTO.Models.Security.Distributed.Types;

namespace SharedModelDTO.Models.Security.Distributed
{
    public class LoginModel
    {
        #region Properties::Public
        public string LoginMode
        {
            get
            {
                return ExternalLogin != null && WebLogin == null ? "External" : "Web";
            }
        }
        public ExternalLoginModel ExternalLogin { get; set; }
        public WebLoginModel WebLogin { get; set; }
        public TokenModel Token { get; set; }
        #endregion

        #region Methods::Public
        #endregion
    }
}
