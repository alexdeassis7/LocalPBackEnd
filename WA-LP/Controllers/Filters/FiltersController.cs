using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SharedModel.Models.Filters;

using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;
using SharedModel.Models.View;
using Newtonsoft.Json;

namespace WA_LP.Controllers.Filters
{
    [Authorize]
    [RoutePrefix("v2/filters")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class FiltersController : BaseApiController
    {
        [HttpGet]
        [Route("getClients")]
        public HttpResponseMessage GetClients()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.EntityUser> ListClients;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string idEntityUser = headers.GetValues("idEntityUser").First();

                if (customer != null && customer.Length > 0)
                {
                    ListClients = Bl.GetClients(idEntityUser);

                    
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListClients);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("getTransactionTypes")]
        public HttpResponseMessage GetTransactionTypes()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.TransactionType> ListTransactionTypes = new List<Filter.TransactionType>();
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListTransactionTypes = Bl.GetTransactionTypes();
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListTransactionTypes);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("getTransactionTypesProvider")]
        public HttpResponseMessage GetTransactionTypesProvider()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.TransactionTypeProvider> ListTransactionTypes = new List<Filter.TransactionTypeProvider>();
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListTransactionTypes = Bl.GetTransactionTypesProvider();
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListTransactionTypes);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("getProviderPayWayServices")]
        public HttpResponseMessage GetProviderPayWayServices()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.ProviderPayWayServices> ListTransactionTypes = new List<Filter.ProviderPayWayServices>();
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListTransactionTypes = Bl.GetProviderPayWayServices();
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListTransactionTypes);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("ListCurrency")]
        public HttpResponseMessage GetListCurrency()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.Currency> ListCurrency;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListCurrency = Bl.GetListCurrency();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListCurrency);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        [Route("ListCountries")]
        public HttpResponseMessage GetListCountries()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.Country> ListCountries;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListCountries = Bl.GetListCountries();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListCountries);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //----FrancoRivero----Api, que trae los datos del merchant y el country----- 
        [HttpGet]
        [Route("ListCountriesMerchant")]
        public HttpResponseMessage GetListCountriesMerchant()
        {
            try
            {
                
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.CountryOfMerchant> ListCountriesMerchant;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                Int64 idMerchant = Convert.ToInt64(Request.Headers.GetValues("idMerchant").First());

                if (customer != null && customer.Length > 0)
                {
                    ListCountriesMerchant = Bl.GetListCountriesMerchant(idMerchant);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListCountriesMerchant);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //---------------Franco Rivero, findelinea.

        [HttpGet]
        [Route("ListStatus")]
        public HttpResponseMessage GetListStatus()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.Status> ListStatus;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListStatus = Bl.GetListStatus();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListStatus);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("ListSubMerchantUser")]
        public HttpResponseMessage GetListSubMerchantUser()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.EntitySubMerchant> ListSubMerchant;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string idEntityUser = headers.GetValues("idEntityUser").First();

                if (customer != null && customer.Length > 0)
                {
                    ListSubMerchant = Bl.GetListSubMerchantUserForSelect(idEntityUser);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListSubMerchant);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("ListRetentionsReg")]
        public HttpResponseMessage GetListRetentionsReg()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.RetentionReg> ListRetentions;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListRetentions = Bl.GetListRetentionsReg();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListRetentions);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("ListFieldsValidation")]
        public HttpResponseMessage GetListFieldsValidation()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.FieldValidation> ListFields;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListFields = Bl.GetListFieldsValidation();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListFields);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("ListErrorTypes")]
        public HttpResponseMessage GetListErrorTypes()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.ErrorType> ListErrorTypes;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    ListErrorTypes = Bl.GetListErrorTypes();


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }

                return Request.CreateResponse(HttpStatusCode.OK, ListErrorTypes);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("getListSettlements")]
        public HttpResponseMessage GetSettlements()
        {
            HttpContent requestContent = Request.Content;
            List<Filter.SettlementInformation> ListSettlement;
            string data = requestContent.ReadAsStringAsync().Result;
            if (data != null && data.Length > 0)
            {
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    Report.List.Request lRequest = JsonConvert.DeserializeObject<Report.List.Request>(data);

                    SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                    ListSettlement = Bl.GetSettlements(lRequest.merchantId, lRequest.dateFrom, lRequest.dateTo);

                    return Request.CreateResponse(HttpStatusCode.OK, ListSettlement);
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
        [Route("getProviders")]
        public HttpResponseMessage GetProviders()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.Providers> providers;
                HttpContent requestContent = Request.Content;
                var re = Request;
                var headers = re.Headers;
                string providerCode = headers.GetValues("provider").First();

                providers = Bl.GetProviders(providerCode);

                return Request.CreateResponse(HttpStatusCode.OK, providers);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("getInternalStatus")]
        public HttpResponseMessage GetInternalStatus()
        {

            try
            {
                SharedBusiness.Filters.BlFilters Bl = new SharedBusiness.Filters.BlFilters();
                List<Filter.InternalStatus> internalStatuses;
                HttpContent requestContent = Request.Content;
                var re = Request;

                internalStatuses = Bl.GetInternalStatuses();

                return Request.CreateResponse(HttpStatusCode.OK, internalStatuses);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}