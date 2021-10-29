using Newtonsoft.Json;
using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.View
{
    [Authorize]
    [RoutePrefix("v2/dashboard")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class DashboardController : BaseApiController
    {
        [HttpPost]
        [Route("list")]
        public HttpResponseMessage List_lot()
        {
            HttpContent requestContent = Request.Content;
            string data = requestContent.ReadAsStringAsync().Result;
            if (data != null && data.Length > 0)
            {
              
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {


                    SharedModel.Models.View.Dashboard.List.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.Dashboard.List.Request>(data);
                    SharedModel.Models.View.Dashboard.List.Response lResponse = new SharedModel.Models.View.Dashboard.List.Response();

                    SharedModel.Models.View.Dashboard.List.Response response = new SharedModel.Models.View.Dashboard.List.Response();

                    //SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch LotBatch = new SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch();

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    lResponse = BiPO.LotTransaction(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);

                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        [HttpPost]
        [Route("list/transactions")]
        public HttpResponseMessage transactions()
        {
            HttpContent requestContent = Request.Content;
            string data = requestContent.ReadAsStringAsync().Result;
            if (data != null && data.Length > 0)
            {

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {


                    SharedModel.Models.View.Dashboard.ListTransaction.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.Dashboard.ListTransaction.Request>(data);
                    SharedModel.Models.View.Dashboard.ListTransaction.Response lResponse = new SharedModel.Models.View.Dashboard.ListTransaction.Response();

                    //SharedModel.Models.View.Dashboard.ListTransaction.Response response = new SharedModel.Models.View.Dashboard.ListTransaction.Response();

                    //SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch LotBatch = new SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch();

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    lResponse = BiPO.ListTransaction(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);

                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        [HttpPost]
        [Route("indicators")]
        public HttpResponseMessage indicators()
        {
            HttpContent requestContent = Request.Content;
            string data = requestContent.ReadAsStringAsync().Result;
            if (data != null && data.Length > 0)
            {

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {


                    SharedModel.Models.View.Dashboard.Indicators.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.Dashboard.Indicators.Request>(data);
                    SharedModel.Models.View.Dashboard.Indicators.Response lResponse = new SharedModel.Models.View.Dashboard.Indicators.Response();

                    //SharedModel.Models.View.Dashboard.ListTransaction.Response response = new SharedModel.Models.View.Dashboard.ListTransaction.Response();

                    //SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch LotBatch = new SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch();

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    lResponse = BiPO.DashboardIndicators(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);

                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }


        [HttpGet]
        [Route("dollarToday")]
        public HttpResponseMessage dollarToday()
        {
            string customer = Request.Headers.GetValues("customer_id").First();

            if (customer != null && customer.Length > 0)
            {

                SharedModel.Models.View.Dashboard.DollarToday.Response lResponse = new SharedModel.Models.View.Dashboard.DollarToday.Response();

                SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                lResponse = BiPO.DashboardDollarToday(customer);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(lResponse.dollarToday));
            }
            else
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("Code", "400");
                result.Add("CodeDescription", "BadRequest");
                result.Add("Message", "Bad Parameter Value or Parameters Values.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
        }

        [HttpGet]
        [Route("providerCycle")]
        public HttpResponseMessage providerCycle()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    SharedModel.Models.View.Dashboard.ProviderCycle.Response.ResponseModel lResponse = BiPO.DashboardProviderCycle(customer);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse.Transactions);
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("Code", "400");
                    result.Add("CodeDescription", "BadRequest");
                    result.Add("Message", "Bad Parameter Value or Parameters Values.");
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
        [Route("merchantCycle")]
        public HttpResponseMessage merchantCycle()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    SharedModel.Models.View.Dashboard.MerchantCycle.Response.ResponseModel lResponse = BiPO.DashboardMerchantCycle(customer);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse.Transactions);
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("Code", "400");
                    result.Add("CodeDescription", "BadRequest");
                    result.Add("Message", "Bad Parameter Value or Parameters Values.");
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
        [Route("mainReport")]
        public HttpResponseMessage mainReport()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();
                Dictionary<string, string> parameters = Request.GetQueryNameValuePairs().ToDictionary(q => q.Key, q => q.Value);

                if ((parameters != null && parameters.Count > 0) && (customer != null && customer.Length > 0))
                {
                    SharedModel.Models.View.Dashboard.MainReport.Request request = (SharedModel.Models.View.Dashboard.MainReport.Request)parameters;
                    SharedModel.Models.View.Dashboard.MainReport.Response lResponse = new SharedModel.Models.View.Dashboard.MainReport.Response();

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                    lResponse = BiPO.DashboardMainReport(request, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(lResponse.mainReportData));
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("Code", "400");
                    result.Add("CodeDescription", "BadRequest");
                    result.Add("Message", "Bad Parameter Value or Parameters Values.");
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
        [Route("clientReport")]
        public HttpResponseMessage clientReport()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();

                SharedModel.Models.View.Dashboard.ClientReport.Response lResponse = new SharedModel.Models.View.Dashboard.ClientReport.Response();

                SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();

                lResponse = BiPO.DashboardClientReport(customer);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(lResponse.clientReportData));
            }
            catch (Exception ex)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result.Add("Status", "Error");
                result.Add("StatusMessage", ex.ToString());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
            }
        }

        [HttpPost]
        [Route("cashProvider")]
        public HttpResponseMessage cashProvider()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();
                string data = Request.Content.ReadAsStringAsync().Result;

                if ((data != null && data.Length > 0) && (customer != null && customer.Length > 0))
                {
                    SharedModel.Models.View.Dashboard.CashProviderCycle.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.Dashboard.CashProviderCycle.Request>(data);
                    SharedModel.Models.View.Dashboard.CashProviderCycle.Response lResponse = new SharedModel.Models.View.Dashboard.CashProviderCycle.Response();
                    
                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();
                    lResponse = BiPO.DashboardCashProviderCycle(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(lResponse));
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("Code", "400");
                    result.Add("CodeDescription", "BadRequest");
                    result.Add("Message", "Bad Parameter Value or Parameters Values.");
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

        [HttpPost]
        [Route("cashMerchant")]
        public HttpResponseMessage cashMerchant()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();
                string data = Request.Content.ReadAsStringAsync().Result;

                if ((data != null && data.Length > 0) && (customer != null && customer.Length > 0))
                {
                    SharedModel.Models.View.Dashboard.CashMerchantCycle.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.View.Dashboard.CashMerchantCycle.Request>(data);
                    SharedModel.Models.View.Dashboard.CashMerchantCycle.Response lResponse = new SharedModel.Models.View.Dashboard.CashMerchantCycle.Response();

                    SharedBusiness.View.BIDashboard BiPO = new SharedBusiness.View.BIDashboard();
                    lResponse = BiPO.DashboardCashMerchantCycle(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(lResponse));
                }
                else
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("Code", "400");
                    result.Add("CodeDescription", "BadRequest");
                    result.Add("Message", "Bad Parameter Value or Parameters Values.");
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
