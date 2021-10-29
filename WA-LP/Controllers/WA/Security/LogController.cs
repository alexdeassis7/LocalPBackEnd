using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web.Http.Cors;
using System.Configuration;
using SharedModel.Models.Security;
using SharedBusiness.Security;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.Security
{
    [Authorize]
    [RoutePrefix("v2/log")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class LogController : BaseApiController
    {

        [HttpGet]
        [Route("logout_session")]

        public HttpResponseMessage Logout()
        {

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0 && countryCode != null && countryCode.Length >= 3)
                {
                    bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                    BlLog blLog = new BlLog();
                    blLog.Logout(customer, TransactionMechanism, countryCode);

                    //TransactionError.List.Request lRequest = JsonConvert.DeserializeObject<TransactionError.List.Request>(data);
                    //TransactionError.List.Response Response = new TransactionError.List.Response();

                    //BlLog BlLog = new BlLog();

                    //Response = BlLog.ListTransactionsError(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, "OK");

                }
                else
                {

                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }



    }
}