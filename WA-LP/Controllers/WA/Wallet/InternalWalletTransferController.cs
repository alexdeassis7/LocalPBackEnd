using Newtonsoft.Json;
using SharedBusiness.Log;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.Wallet
{
    [Authorize]
    [RoutePrefix("v2/wallet")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class InternalWalletTransferController : BaseApiController
    {
        [HttpPost]
        [Route("accreditation")]
        public HttpResponseMessage Accreditation()
        {

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    var re = Request;
                    var headers = re.Headers;
                    string customer = headers.GetValues("customer_id").First();
                    bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                    SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request>(data);
                    SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response Response = new SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response();


                    //SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request model = new SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Request();

                    lRequest.ErrorRow.Errors = (SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest));


                    /* var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                     var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();*/

                    if (lRequest.ErrorRow.HasError == false)
                    {
                        SharedBusiness.Services.Wallet.BIInternalWalletTransfer BiIWT = new SharedBusiness.Services.Wallet.BIInternalWalletTransfer();
                        Response = BiIWT.WalletTransfer(lRequest, customer, TransactionMechanism);
                    }
                    else
                    {
                        var a = JsonConvert.SerializeObject(lRequest);
                        Response = (JsonConvert.DeserializeObject<SharedModel.Models.Services.Wallet.InternalWalletTransfer.Create.Response>(a));
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, Response);
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
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }
    }
}
