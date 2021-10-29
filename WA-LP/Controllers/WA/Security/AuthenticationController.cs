using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SharedSecurity.JWT.Token;
using SharedModel.Models.Security;
using SharedModel.Models.Database.Security;

using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using static WA_LP.SwaggerConfig.Consumes;
using WA_LP.Cache;
using static SharedModel.Models.Database.Security.Authentication;

namespace WA_LP.Controllers.WA.Security
{
    [AllowAnonymous]
    [RoutePrefix("v2")]
    [ApiExplorerSettings(IgnoreApi = false)]

    public class AuthenticationController : BaseApiController
    {   
        SharedBusiness.Security.BlSecurity BL = new SharedBusiness.Security.BlSecurity();

        /// <summary>
        /// Tokens
        /// </summary>
        [HttpPost]
        [Route("tokens")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Token))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Type = typeof(Authentication.Account))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [SwaggerConsumes("application/json")]
        //[SwaggerRequestExample(typeof(DeliveryOptionsSearchModel), typeof(DeliveryOptionsSearchModelExample))]
        public HttpResponseMessage Token()
        {
            string CustomerID = "";
            string ApiKey = "";
            try
            {
                CustomerID = ((string[])Request.Headers.FirstOrDefault(x => x.Key == "customer_id").Value)[0];
                ApiKey = ((string[])Request.Headers.FirstOrDefault(x => x.Key == "api_key").Value)[0];
                string App = "false";
                LogService.LogInfo(CustomerID + " trying to get a token with key " + ApiKey);

                if (!((Request.Headers.FirstOrDefault(x => x.Key == "app").Value) == null))
                {
                    App = ((string[])Request.Headers.FirstOrDefault(x => x.Key == "app").Value)[0];
                }

                if (CustomerID == null || ApiKey == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (!string.IsNullOrEmpty(CustomerID) && CustomerID.Length > 0 && !string.IsNullOrEmpty(ApiKey) && ApiKey.Length > 0)
                {
                    AccountModel.Login Credential = new AccountModel.Login() { ClientID = CustomerID, Password = ApiKey, WebAcces = Convert.ToBoolean(App) };

                    var accountCached = LogInCacheService.Get(CustomerID);

                    Authentication.Account Account = null;

                    if (accountCached == null)
                    { 
                        Account = BL.GetAuthentication(Credential);

                        if (Account.ValidationResult.ValidationStatus == true)
                        {
                            if (Convert.ToBoolean(@App))
                            {
                                SharedBusiness.User.BlUser BLUser = new SharedBusiness.User.BlUser();
                                SharedModel.Models.User.User userinfo = BLUser.GetUserInfo(Credential);
                                TokenWeb Token = new TokenWeb();
                                Token.customer_id = Account.Login.ClientID;
                                Token.token_id = TokenGenerator.GenerateTokenJwt(Account.Login.ClientID, Account.SecretKey, userinfo);
                                //BL.PutLoginToken(Account.Login.ClientID, Account.Login.WebAcces, Token.token_id);
                                LogInCacheService.AddToCache(new TokenAccount
                                {
                                    Login = Account.Login,
                                    SecretKey = Account.SecretKey,
                                    TokenWeb = Token
                                });
                                LogService.LogInfo(string.Format("Client {0} the token {1} was obtained from DB", CustomerID, Token.token_id));
                                return Request.CreateResponse(HttpStatusCode.OK, Token);
                            }
                            else
                            {
                                Token Token = new Token();
                                Token.token_id = TokenGenerator.GenerateTokenJwt(Credential.ClientID, Account.SecretKey);
                                //BL.PutLoginToken(Account.Login.ClientID, Account.Login.WebAcces, Token.token_id);
                                LogInCacheService.AddToCache(new TokenAccount
                                {
                                    Login = Account.Login,
                                    SecretKey = Account.SecretKey,
                                    Token = Token
                                });
                                LogService.LogInfo(string.Format("Client {0} the token {1} was obtained from DB", CustomerID, Token.token_id));
                                return Request.CreateResponse(HttpStatusCode.OK, Token);
                            }
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.Unauthorized, Account);
                        }
                    }
                    else
                    {
                        var tokenAccount = (TokenAccount)accountCached;
                        if (Convert.ToBoolean(@App))
                        {
                            LogService.LogInfo(string.Format("Client {0} the token {1} was obtained from Cache", CustomerID, tokenAccount.TokenWeb));
                            return Request.CreateResponse(HttpStatusCode.OK, tokenAccount.TokenWeb);
                        }
                        else
                        {
                            LogService.LogInfo(string.Format("Client {0} the token {1} was obtained from Cache", CustomerID, tokenAccount.Token));
                            return Request.CreateResponse(HttpStatusCode.OK, tokenAccount.Token);
                        }
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.Message, CustomerID);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        [HttpPost]
        [Route("login")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AccountModel.Login))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [ApiExplorerSettings(IgnoreApi = true)]

        public HttpResponseMessage Login()
        {
            try
            {
                var user = ((string[])Request.Content.Headers.FirstOrDefault(x => x.Key == "userNameLP").Value)[0];
                var pass = ((string[])Request.Content.Headers.FirstOrDefault(x => x.Key == "userPassLP").Value)[0];

                if (user == null || pass == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (!string.IsNullOrEmpty(user) && user.Length > 0 && !string.IsNullOrEmpty(pass) && pass.Length > 0)
                {
                    AccountModel.Login login = new AccountModel.Login() { ClientID = user, Password = pass };

                    var Login = "";
                    return Request.CreateResponse(HttpStatusCode.OK, login);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.Message);
                return Request.CreateResponse(HttpStatusCode.Unauthorized,ex.Message);
            }            
        }

        [HttpPost]
        [Route("RefreshToken")]
        public HttpResponseMessage RefreshToken()
        {
            try
            {
                string CustomerID = ((string[])Request.Headers.FirstOrDefault(x => x.Key == "customer_id").Value)[0];

                if (CustomerID == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (!string.IsNullOrEmpty(CustomerID) && CustomerID.Length > 0)
                {
                    Authentication.Account Account = BL.GetAuthentication(CustomerID);

                    if (Account.ValidationResult.ValidationStatus == true)
                    {
                        SharedModel.Models.Security.AccountModel.Login Credential = new SharedModel.Models.Security.AccountModel.Login() { ClientID = CustomerID, WebAcces = true };
                        SharedBusiness.User.BlUser BLUser = new SharedBusiness.User.BlUser();
                        SharedModel.Models.User.User userinfo = BLUser.GetUserInfo(Credential);
                        TokenWeb Token = new TokenWeb();
                        Token.customer_id = CustomerID;
                        Token.token_id = TokenGenerator.GenerateTokenJwt(CustomerID, Account.SecretKey, userinfo);
                        BL.PutLoginToken(CustomerID, true, Token.token_id);
                        return Request.CreateResponse(HttpStatusCode.OK, Token);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, Account);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
