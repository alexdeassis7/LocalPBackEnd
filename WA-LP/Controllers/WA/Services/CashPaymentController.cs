using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;
using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web.Http.Description;
using WA_LP.Infrastructure;

namespace WA_LP.Controllers.WA.Services
{
    [Authorize]
    [RoutePrefix("v2/payin")]
    [ApiExplorerSettings(IgnoreApi = true)]

    public class CashPaymentController : BaseApiController
    {
        [HttpPost]
        [Route("cash_payment")]
        public HttpResponseMessage cash_payment()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Request RequestCashPayment = JsonConvert.DeserializeObject<SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Request>(data);
                    SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response ResponseCashPayment = new SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response();
                    SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response resulte = new SharedModel.Models.Services.CashPayments.CashPaymentModel.Create.Response();
                    RequestCashPayment.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(RequestCashPayment);

                    if (RequestCashPayment.ErrorRow.HasError == false)
                    {
                        ResponseCashPayment.additional_info = RequestCashPayment.additional_info;
                        ResponseCashPayment.currency = RequestCashPayment.currency;
                        ResponseCashPayment.days_to_second_exp_date = "0"; //RequestCashPayment.days_to_second_exp_date;
                        ResponseCashPayment.first_expiration_amount = RequestCashPayment.first_expiration_amount;
                        ResponseCashPayment.first_expirtation_date = RequestCashPayment.first_expirtation_date;
                        ResponseCashPayment.invoice = RequestCashPayment.invoice;
                        ResponseCashPayment.payment_method = RequestCashPayment.payment_method;
                        ResponseCashPayment.surcharge = "00000000"; //RequestCashPayment.surcharge;
                        ResponseCashPayment.identification = RequestCashPayment.identification;
                        ResponseCashPayment.mail = RequestCashPayment.mail;

                        ResponseCashPayment.name = RequestCashPayment.name;

                        //ResponseCashPayment.first_name = RequestCashPayment.first_name;
                        //ResponseCashPayment.last_name = RequestCashPayment.last_name;
                        ResponseCashPayment.address = RequestCashPayment.address;
                        ResponseCashPayment.birth_date = RequestCashPayment.birth_date;
                        ResponseCashPayment.country = RequestCashPayment.country;
                        ResponseCashPayment.city = RequestCashPayment.city;
                        ResponseCashPayment.annotation = RequestCashPayment.annotation;

                        bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                        string Customer = Request.Headers.Contains("customer_id_to") == true ? ((string[])Request.Headers.GetValues("customer_id_to"))[0] : ((string[])Request.Headers.GetValues("customer_id"))[0];
                        string CustomerFrom = ((string[])Request.Headers.GetValues("customer_id"))[0];

                        //string countryCode = ((string[])Request.Headers.GetValues("countryCode"))[0];

                        string countryCode = Request.Headers.Contains("countryCode") == false ? "ARG" : ((string[])Request.Headers.GetValues("countryCode"))[0];

                        SharedBusiness.Services.CashPayment.BICashPayment BICashPayment = new SharedBusiness.Services.CashPayment.BICashPayment();

                        ConcurrentService.ExecuteSafe(() =>
                        {
                            resulte = BICashPayment.BarCodeGenerator(Customer, ResponseCashPayment, TransactionMechanism, CustomerFrom, countryCode);               
                        });
                        ResponseCashPayment.transaction_id = resulte.transaction_id;
                        ResponseCashPayment.bar_code_number = resulte.bar_code_number;
                        ResponseCashPayment.bar_code = resulte.bar_code;
                        ResponseCashPayment.bar_code_url = System.Configuration.ConfigurationManager.AppSettings["URL_BARCODE"].ToString() + resulte.bar_code;
                    }
                    else
                    {
                        ResponseCashPayment.ErrorRow.Errors = RequestCashPayment.ErrorRow.Errors;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseCashPayment);
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
        [Route("upload")]
        public HttpResponseMessage Upload()//string TransactionType, string File
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                SharedModel.Models.Services.CashPayments.CashPaymentModel.Upload Upload = JsonConvert.DeserializeObject<SharedModel.Models.Services.CashPayments.CashPaymentModel.Upload>(data);

                SharedBusiness.Services.CashPayment.BICashPayment BICashPayment = new SharedBusiness.Services.CashPayment.BICashPayment();

                if (string.IsNullOrEmpty(Upload.TransactionType))
                    throw new Exception("TransactionType Code is null or empty!");

                if (string.IsNullOrEmpty(Upload.File))
                    throw new Exception("File is null or empty!");

                if(string.IsNullOrEmpty(Upload.FileName))
                    throw new Exception("Original file name is null or empty!");

                if(!BICashPayment.ValidFile(Upload.FileName))
                    throw new Exception("the file can not be uploaded because it has already been uploaded!");

                string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");

                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                
                SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile ReadFile  = new SharedModel.Models.Services.CashPayments.CashPaymentModel.ReadFile();                

                switch (Upload.TransactionType)
                {
                    case "RAPA":
                        ReadFile = BICashPayment.ReadFileRAPA(Upload.File, Upload.TransactionType, datetime, TransactionMechanism, Upload.FileName);
                        break;
                    case "PAFA":
                        ReadFile = BICashPayment.ReadFilePAFA(Upload.File, Upload.TransactionType, datetime, TransactionMechanism, Upload.FileName);
                        break;
                    case "BAPR":
                        ReadFile = BICashPayment.ReadFileBAPR(Upload.File, Upload.TransactionType, datetime, TransactionMechanism, Upload.FileName);
                        break;
                    case "COEX":
                        ReadFile = BICashPayment.ReadFileCOEX(Upload.File, Upload.TransactionType, datetime, TransactionMechanism, Upload.FileName);
                        break;
                    default:
                        throw new Exception("Bad TransactionType Code!");
                }

                if (ReadFile.Status != "OK")
                    throw new Exception(ReadFile.StatusMessage);

                return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ReadFile));
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }            
        }

        [HttpGet]
        [Route("status_cash_payments")]
        public HttpResponseMessage list_cash_payment()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                string Customer = ((string[])Request.Headers.GetValues("customer_id"))[0];

                if (string.IsNullOrEmpty(Customer))
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Header Value.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                    //throw new Exception("customer_id is null!.");
                }
                else
                {
                    SharedBusiness.Services.CashPayment.BICashPayment BICashPayment = new SharedBusiness.Services.CashPayment.BICashPayment();
                    string result = BICashPayment.ListStatusCashPayments(Customer);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(result));
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
