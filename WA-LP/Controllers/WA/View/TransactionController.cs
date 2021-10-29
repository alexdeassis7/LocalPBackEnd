using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

using System.Web.Http.Cors;
using System.Collections.Generic;
using System.Linq;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.View
{
    [Authorize]
    [RoutePrefix("v2/transactions")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class TransactionController : BaseApiController
    {
        [HttpPost]
        [Route("close")]
        public HttpResponseMessage TransactionExchangeClosed()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    SharedModel.Models.View.TransactionModel.Close.Request RequestTransactionsExchangeClosed = JsonConvert.DeserializeObject<SharedModel.Models.View.TransactionModel.Close.Request>(data);
                    SharedModel.Models.View.TransactionModel.Close.Response ResponseTransactionsExchangeClosed = new SharedModel.Models.View.TransactionModel.Close.Response();

                    RequestTransactionsExchangeClosed.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(RequestTransactionsExchangeClosed);

                    if (RequestTransactionsExchangeClosed.ErrorRow.HasError == false)
                    {
                        SharedBusiness.View.BlTransaction BlTransaction = new SharedBusiness.View.BlTransaction();
                        ResponseTransactionsExchangeClosed = BlTransaction.TransactionsExchangeClosed(RequestTransactionsExchangeClosed);
                        ResponseTransactionsExchangeClosed.value = RequestTransactionsExchangeClosed.value;
                        ResponseTransactionsExchangeClosed.transactions = RequestTransactionsExchangeClosed.transactions;
                    }
                    else
                    {
                        ResponseTransactionsExchangeClosed.ErrorRow.Errors = RequestTransactionsExchangeClosed.ErrorRow.Errors;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseTransactionsExchangeClosed);
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

        [HttpPost]
        [Route("create")]
        public HttpResponseMessage TransactionCreate()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                string customer = Request.Headers.GetValues("customer_id").First();

                if ((data != null && data.Length > 0) && (customer != null && customer.Length > 0))
                {
                    SharedModel.Models.View.TransactionModel.Transaction.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.TransactionModel.Transaction.Request>(data);

                    lRequest.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest);

                    if (!lRequest.ErrorRow.HasError)
                    {
                        SharedModel.Models.View.TransactionModel.Transaction.Response lResponse = new SharedModel.Models.View.TransactionModel.Transaction.Response();
                        SharedBusiness.View.BlTransaction BiTransaction = new SharedBusiness.View.BlTransaction();

                        lResponse.Amount = lRequest.Amount;
                        lResponse.Description = lRequest.Description;
                        lResponse.idCurrencyType = lRequest.idCurrencyType;
                        lResponse.idEntityUser = lRequest.idEntityUser;
                        lResponse.idTransactionType = lRequest.idTransactionType;
                        lResponse.TransactionTypeCode = lRequest.TransactionTypeCode;
                        lResponse.ValueFX = lRequest.ValueFX;

                        lResponse = BiTransaction.TransactionCreate(customer, lResponse, true);

                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
                    else
                    {
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        result.Add("Status", "Error");
                        result.Add("StatusMessage", JsonConvert.SerializeObject(lRequest.ErrorRow.Errors));
                        return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                    }
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("Status", "Error");
                    result.Add("StatusMessage", "Bad Parameter Value or Parameters Values.");
                    return Request.CreateResponse(HttpStatusCode.BadRequest, result);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("Status", "Error");
                result.Add("StatusMessage", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }
        }
    }
}
