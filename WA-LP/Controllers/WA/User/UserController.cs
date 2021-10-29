using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;
using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;
using SharedSecurity.Signature;
using Newtonsoft.Json;

namespace WA_LP.Controllers.WA.User
{
    //[Authorize]
    [RoutePrefix("v2/users")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class UserController : BaseApiController
    {


        [HttpGet]
        [Route("user")]
        public HttpResponseMessage UserInfo()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;


                var re = Request;
                var headers = re.Headers;
                string user = headers.GetValues("customer_id").First();
                // var user = ((string[])Request.Content.Headers.FirstOrDefault(x => x.Key == "user").Value)[0];

                if (user == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }


                if (!string.IsNullOrEmpty(user) && user.Length > 0)
                {
                    SharedModel.Models.Security.AccountModel.Login Credential = new SharedModel.Models.Security.AccountModel.Login() { ClientID = user, WebAcces = true };

                    SharedBusiness.User.BlUser BL = new SharedBusiness.User.BlUser();
                    SharedModel.Models.User.User userinfo = BL.GetUserInfo(Credential);

                    return Request.CreateResponse(HttpStatusCode.OK, userinfo);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Unauthorized");
                }


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.Unauthorized, ex);
            }


        }

        [HttpGet]
        [Route("ListUsers")]
        public HttpResponseMessage GetList()
        {

            try
            {
                List<SharedModel.Models.User.User> ListUsers = new List<SharedModel.Models.User.User>();
                SharedBusiness.User.BlUser Bl = new SharedBusiness.User.BlUser();

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListUsers = Bl.GetListUsers();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    //return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListUsers);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.Unauthorized, ex);
            }
        }

        [HttpGet]
        [Route("ListUsersKeys")]
        public HttpResponseMessage GetListKeys()
        {

            try
            {
                List<SharedModel.Models.User.User> ListUsers = new List<SharedModel.Models.User.User>();
                SharedBusiness.User.BlUser Bl = new SharedBusiness.User.BlUser();

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListUsers = Bl.GetListKeyUsers();
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListUsers);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.Unauthorized, ex);
            }
        }

        [HttpPost]
        [Route("CreateUserKeys")]
        public HttpResponseMessage CreateUserKeys()
        {

            try
            {
                HttpContent requestContent = Request.Content;
                string identification = JsonConvert.DeserializeObject<string>(requestContent.ReadAsStringAsync().Result);
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                var signature = new DigitalHMACSignature();

                if (customer != null && customer.Length > 0)
                {
                    signature.AssignNewKey(identification);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.Unauthorized, ex);
            }
        }
    }
}

