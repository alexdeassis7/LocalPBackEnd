using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.View
{
    [Authorize]
    [RoutePrefix("v2/cycle")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class CyclesController : BaseApiController
    {
        [HttpPost]
        [Route("entity")]
        public HttpResponseMessage EntityInsert()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    string customer = ((string[])Request.Headers.GetValues("customer_id"))[0];

                    SharedModel.Models.View.CycleModel.Create.Request CycleRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.CycleModel.Create.Request>(data);
                    SharedModel.Models.View.CycleModel.Create.Response CycleResponse = new SharedModel.Models.View.CycleModel.Create.Response();

                    CycleRequest.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(CycleRequest);

                    if (!CycleRequest.ErrorRow.HasError)
                    {
                        var Medium = JsonConvert.SerializeObject(CycleRequest);
                        CycleResponse = JsonConvert.DeserializeObject<SharedModel.Models.View.CycleModel.Create.Response>(Medium);
                        SharedBusiness.View.BlCycles BICycle = new SharedBusiness.View.BlCycles();
                        CycleResponse = BICycle.InsertEntity(customer, CycleResponse);
                    }
                    else
                    {
                        var Medium = JsonConvert.SerializeObject(CycleRequest);
                        CycleResponse = JsonConvert.DeserializeObject<SharedModel.Models.View.CycleModel.Create.Response>(Medium);
                        CycleResponse.Status = "Error";
                        CycleResponse.StatusMessage = "Validation Error!";
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, CycleResponse);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"InternalServerError\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }
    }
}
