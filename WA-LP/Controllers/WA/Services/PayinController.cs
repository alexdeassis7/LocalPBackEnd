using Newtonsoft.Json;
using SharedBusiness.Filters;
using SharedBusiness.Log;
using SharedBusiness.Payin.DTO;
using SharedBusiness.Security;
using SharedModel.Models.General;
using SharedModel.Models.Services.Payins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WA_LP.Cache;

namespace WA_LP.Controllers.WA.Services
{
    [Authorize]
    [RoutePrefix("v2/payins")]
    public class PayInController : BaseApiController
    {
        [HttpPost]
        [Route("payin")]
        public HttpResponseMessage CreatePayin()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string countryCode = headers.GetValues("countryCode").First();

                if (data != null && data.Length > 0 && customer != null && customer.Length > 0 && countryCode != null && countryCode.Length > 0)
                {
                    try
                    {
                        LogService.LogInfo(SerializeRequest());
                    }
                    catch (Exception e)
                    {
                        LogService.LogError(e.ToString());
                    }

                    var subMerchantsUsers = new BlFilters().GetListSubMerchantUser();

                    PayinModel.Create.Request lRequest = JsonConvert.DeserializeObject<PayinModel.Create.Request>(data);

                    PayinModel.Create.Response lResponse = new PayinModel.Create.Response();

                    List<string> payinMethodCodes = PayinPaymentMethodValidateCacheService.Get();

                    var validatorData = new Dictionary<object, object>();
                    validatorData.Add("paymentMethods", payinMethodCodes);
                    validatorData.Add("countryCode", countryCode);
                    lRequest.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest, validatorData);

                    if (!lRequest.ErrorRow.HasError)
                    {
                        SharedBusiness.Services.Payins.BIPayIn BiPI = new SharedBusiness.Services.Payins.BIPayIn();
                        lResponse = BiPI.PayinTransaction(lRequest, customer, countryCode);
                    }
                    else
                    {
                        BlLog BlLog = new BlLog();
                        var jsonRequest = JsonConvert.SerializeObject(lRequest);
                        PayinModel.RejectedPayins.TransactionError errors = JsonConvert.DeserializeObject<PayinModel.RejectedPayins.TransactionError>(jsonRequest);
                        List<ErrorCode> _errors = new List<ErrorCode>();
                        foreach (var error in lRequest.ErrorRow.Errors)
                        {
                            foreach (var code in error.CodeTypeError)
                            {
                                if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                {
                                    _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                }
                            }
                        }
                        errors.errors = JsonConvert.SerializeObject(_errors);
                        BlLog.InsertPayinErrors(errors, customer, countryCode);
                        lResponse = JsonConvert.DeserializeObject<PayinModel.Create.Response>(jsonRequest);
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                }
                else
                {
                    GeneralErrorModel Error = new GeneralErrorModel()
                    {
                        Code = "400",
                        CodeDescription = "BadRequest",
                        Message = "Bad Parameter Value or Parameters Values."
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error);
            }

        }

        [HttpPost]
        [Route("list")]
        public HttpResponseMessage List()
        {
            HttpContent requestContent = Request.Content;
            string data = requestContent.ReadAsStringAsync().Result;
            if (data != null && data.Length > 0)
            {
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string countryCode = headers.GetValues("countryCode").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedModel.Models.Services.Payins.PayinModel.List.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Services.Payins.PayinModel.List.Request>(data);
                    var errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest);

                    List<SharedModel.Models.Services.Payins.PayinModel.List.Response> lResponse = new List<SharedModel.Models.Services.Payins.PayinModel.List.Response>();
                    SharedBusiness.Services.Payins.BIPayIn BiPI = new SharedBusiness.Services.Payins.BIPayIn();

                    if (lRequest.payin_id == 0 && lRequest.merchant_id == null && (lRequest.date_from == null || lRequest.date_to == null))
                    {
                        GeneralErrorModel Error = new GeneralErrorModel()
                        {
                            Code = "400",
                            CodeDescription = "BadRequest",
                            Message = "Error - Filter is required."
                        };
                        return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                    }

                    if (errors.Count() > 0)
                    {
                        GeneralErrorModel Error = new GeneralErrorModel()
                        {
                            Code = "400",
                            CodeDescription = "BadRequest",
                            Message = "Bad Parameters Values in "
                        };
                        foreach (var error in errors)
                        {
                            Error.Message += String.Format("{0} ", error.Key);
                        }
                        return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                    }

                    var listLotBachModel = BiPI.ListTransactions(lRequest, customer, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, listLotBachModel);
                }
                else
                {
                    GeneralErrorModel Error = new GeneralErrorModel()
                    {
                        Code = "400",
                        CodeDescription = "BadRequest",
                        Message = "Bad Parameter Value or Parameters Values."
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                }
            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        [HttpPost]
        [Route("manage_payins")]
        public HttpResponseMessage ManagePayins()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data))
                {
                    var re = Request;
                    var headers = re.Headers;
                    string customer = headers.GetValues("customer_id").First();
                    string countryCode = headers.GetValues("countryCode").First();

                    SharedModel.Models.Services.Payins.PayinModel.Manage.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Services.Payins.PayinModel.Manage.Request>(data);

                    List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response> lResponse = new List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response>();

                    SharedBusiness.Services.Payins.BIPayIn BiPI = new SharedBusiness.Services.Payins.BIPayIn();
                    lResponse = BiPI.ManagePayins(lRequest, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                }
                else
                {
                    GeneralErrorModel Error = new GeneralErrorModel()
                    {
                        Code = "400",
                        CodeDescription = "BadRequest",
                        Message = "Bad Parameter Value or Parameters Values."
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error);
            }
        }

        [HttpPost]
        [Route("update_payins")]
        public HttpResponseMessage UpdatePayins()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data))
                {
                    var re = Request;
                    var headers = re.Headers;
                    string customer = headers.GetValues("customer_id").First();
                    string countryCode = headers.GetValues("countryCode").First();

                    TicketsToChangeStatusDto lRequest = new TicketsToChangeStatusDto(); 
                    lRequest.Tickets = JsonConvert.DeserializeObject<List<string>>(data);

                    SharedBusiness.Payin.PayinService BiPI = new SharedBusiness.Payin.PayinService();
                    BiPI.SetExecuted(lRequest);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    GeneralErrorModel Error = new GeneralErrorModel()
                    {
                        Code = "400",
                        CodeDescription = "BadRequest",
                        Message = "Bad Parameter Value or Parameters Values."
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error);
            }
        }
    }
}