using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Newtonsoft.Json;
using SharedBusiness.Log;

namespace WA_LP.Controllers.WA.View
{
    [Authorize]
    [RoutePrefix("v2/currency")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class CurrencyExchangeController : BaseApiController
    {
        [HttpPost]
        [Route("currency_exchange")]
        public HttpResponseMessage Get()
        {
            try
            {
                HttpStatusCode httpStatusCode = HttpStatusCode.OK;
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string customer = Request.Headers.GetValues("customer_id").First();

                if ((data != null && data.Length > 0) && (customer != null && customer.Length > 0))
                {                    
                    SharedModel.Models.View.CurrencyExchange.Single.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.CurrencyExchange.Single.Request>(data);
                    SharedModel.Models.View.CurrencyExchange.Single.Response lResponse = new SharedModel.Models.View.CurrencyExchange.Single.Response();

                    lRequest.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest);
                    lResponse.ErrorRow = lRequest.ErrorRow;
                    if (!lRequest.ErrorRow.HasError)
                    {
                        SharedBusiness.View.BICurrencyExchange BiCurrencyExchange = new SharedBusiness.View.BICurrencyExchange();
                        BiCurrencyExchange.GetCurrencyExchange(lRequest.base_currency, lRequest.quote_currency, lRequest.transaction_type, lRequest.date, lRequest.with_source, customer, lResponse);
                    }

                    return Request.CreateResponse(
                        httpStatusCode,
                        JsonConvert.DeserializeObject(
                            JsonConvert.SerializeObject(lResponse, Formatting.None, new JsonSerializerSettings {
                                NullValueHandling = NullValueHandling.Ignore
                            })
                        )
                    );
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

        [HttpGet]
        [Route("currency_exchange/list")]
        public HttpResponseMessage List()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();
                Dictionary<string, string> parameters = Request.GetQueryNameValuePairs().ToDictionary(q => q.Key, q => q.Value);

                if ((parameters != null && parameters.Count > 0) && (customer != null && customer.Length > 0))
                {
                    SharedModel.Models.View.CurrencyExchange.List.Request lRequest = (SharedModel.Models.View.CurrencyExchange.List.Request)parameters;

                    lRequest.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest);

                    if (!lRequest.ErrorRow.HasError)
                    {
                        List<SharedModel.Models.View.CurrencyExchange.List.Response> lResponse = new List<SharedModel.Models.View.CurrencyExchange.List.Response>();
                        SharedBusiness.View.BICurrencyExchange BiCurrencyExchange = new SharedBusiness.View.BICurrencyExchange();

                        lResponse = BiCurrencyExchange.GetListCurrencyExchange(lRequest.base_currency, lRequest.entity_user, customer);

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