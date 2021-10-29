using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using SharedModel.Models.General;
using SharedBusiness.Log;
using System.Web;
using SharedModelDTO.Models.Transaction;
using SharedModelDTO.File;
using System.Web.Http.Description;
using SharedModel.Models.View;
using System.Net.Http.Headers;

namespace WA_LP.Controllers.WA.Services
{
    [Authorize]
    [RoutePrefix("v2/retention")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class RetentionController : BaseApiController
    {

        [HttpPost]
        [Route("upload")]
        public HttpResponseMessage Upload()//string TransactionType, string File
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                string datetime = DateTime.Now.ToString("yyyyMMdd");

                var re = Request;

                var headers = re.Headers;

                string customer = headers.GetValues("customer_id").First();

                string retentionType = headers.GetValues("retention_type").First();

                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                //SharedModel.Models.Tools.AfipModel.Upload Upload = JsonConvert.DeserializeObject<SharedModel.Models.Tools.AfipModel.Upload>(data);
                SharedModel.Models.Tools.RetentionModel.ReadFile ReadFile = new SharedModel.Models.Tools.RetentionModel.ReadFile();

                SharedBusiness.Tools.BIRetention BIAfip = new SharedBusiness.Tools.BIRetention();

                switch (retentionType)
                {

                    case "RET-AFIP":

                        ReadFile.ini = DateTime.Now.ToString("yyyyMMddhhmmss");
                        ReadFile = BIAfip.ReadAfipFile("UPDATE_DB_AFIP_ARG_" + ReadFile.ini, "UPDATE_DB_AFIP_ARG_" + ReadFile.ini, customer, TransactionMechanism, datetime);
                        ReadFile.fin = DateTime.Now.ToString("yyyyMMddhhmmss");

                        break;
                    case "RET-ARBA":
                        ReadFile.ini = DateTime.Now.ToString("yyyyMMddhhmmss");
                        ReadFile = BIAfip.ReadArbaFile("UPDATE_DB_ARBA_ARG_" + ReadFile.ini, "UPDATE_DB_ARBA_ARG_" + ReadFile.ini, customer, TransactionMechanism, datetime);
                        ReadFile.fin = DateTime.Now.ToString("yyyyMMddhhmmss");
                        break;
                    default:
                        throw new Exception("Parameter :: retention_type :: invalid value - valid value [RET-AFIP | RET-ARBA]");
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

        [HttpPost]
        [Route("downloadRetentionForaDate")]
        public FileModel DownloadRetentionsByDate([FromBody]RetentionsByDateDownloadModel retentionsByDate)
        {
            SharedBusiness.Tools.BIRetention biRetention = new SharedBusiness.Tools.BIRetention();
            var fileToDownload = biRetention.DownloadRetentionCertificates(retentionsByDate);
            return fileToDownload;
        }

        [HttpPost]
        [Route("downloadRetentionByFilter")]
        public HttpResponseMessage DownloadRetentionsByFilter([FromBody]RetentionsByFilterDownloadModel retentionsByFilter)
        {
            SharedBusiness.Tools.BIRetention biRetention = new SharedBusiness.Tools.BIRetention();

            var fileToDownload = biRetention.DownloadRetentionCertificates(retentionsByFilter, retentionsByFilter.CertificateNumber, retentionsByFilter.MerchantID, retentionsByFilter.PayoutID);

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            if(fileToDownload.file_bytes_compressed != null)
            {
                response.Content = new ByteArrayContent(fileToDownload.file_bytes_compressed);
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = fileToDownload.file_name_zip;

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            
            return response;
        }

        [HttpGet]
        [Route("downloadRetentions")]
        public HttpResponseMessage DownloadRetentions()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();

                string retentionType = Request.Headers.GetValues("retention_type").First();

                if (customer != null && customer.Length > 0)
                {
                    SharedBusiness.Tools.BIRetention retention = new SharedBusiness.Tools.BIRetention();
                    SharedModelDTO.File.FileModel ReadFile = retention.DownloadRetentionCertificates(customer, retentionType);
                    if (!ReadFile.HasFile) {
                        return Request.CreateResponse(HttpStatusCode.OK, "NOTPENDINGFILES");
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, ReadFile);
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

        [HttpPost]
        [Route("downloadTxtRetentions")]
        public HttpResponseMessage DownloadTxtAfipMonthly() {

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedBusiness.Tools.BIRetention retention = new SharedBusiness.Tools.BIRetention();
                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request>(data);
                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt lResponse = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt();

                    lResponse = retention.DownloadTxtAfipMonthly(customer, lRequest.year, lRequest.month, lRequest.typeFile);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost]
        [Route("downloadExcelRetentions")]
        public HttpResponseMessage DownloadExcelAfipMonthly()
        {

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedBusiness.Tools.BIRetention retention = new SharedBusiness.Tools.BIRetention();

                    WA_LP.Controllers.Export.ExportController exportController = new WA_LP.Controllers.Export.ExportController();

                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request>(data);
                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel lResponse = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel();

                    lResponse = retention.DownloadExcelAfipMonthly(customer, lRequest.year, lRequest.month, lRequest.typeFile);

                    string excelBase64 = null;

                    if (JsonConvert.DeserializeObject<List<SharedModel.Models.Tools.RetentionModel.RetentionARBAMonthly>>(lResponse.ListRetentions) != null)
                    {
                        excelBase64 = exportController.ExcelAfipRetentionMonthly(JsonConvert.DeserializeObject<List<SharedModel.Models.Tools.RetentionModel.RetentionMonthly>>(lResponse.ListRetentions), "RETENTIONMONTHLY");
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, excelBase64);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost]
        [Route("downloadTxtARBARetentions")]
        public HttpResponseMessage DownloadTxtArba()
        {

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedBusiness.Tools.BIRetention retention = new SharedBusiness.Tools.BIRetention();
                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request>(data);
                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt lResponse = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt();

                    lResponse = retention.DownloadTxtArbaMonthly(customer, lRequest.year, lRequest.month, lRequest.typeFile);

                    return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpPost]
        [Route("downloadExcelArbaRetentions")]
        public HttpResponseMessage DownloadExcelArba()
        {

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {

                    SharedBusiness.Tools.BIRetention retention = new SharedBusiness.Tools.BIRetention();

                    WA_LP.Controllers.Export.ExportController exportController = new WA_LP.Controllers.Export.ExportController();

                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.RetentionFiles.Request>(data);
                    SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel lResponse = new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel();

                    lResponse = retention.DownloadExcelArbaMonthly(customer, lRequest.year, lRequest.month, lRequest.typeFile);

                    string excelBase64 = null;

                    if (JsonConvert.DeserializeObject<List<SharedModel.Models.Tools.RetentionModel.RetentionARBAMonthly>>(lResponse.ListRetentions) != null)
                    {
                        excelBase64 = exportController.ExcelArbaRetentionMonthly(JsonConvert.DeserializeObject<List<SharedModel.Models.Tools.RetentionModel.RetentionARBAMonthly>>(lResponse.ListRetentions), "RETENTIONMONTHLY");
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, excelBase64);
                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost]
        [Route("ListWhitelist")]
        public HttpResponseMessage ListWhiteList()
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


                    SharedModel.Models.Tools.RetentionModel.Whitelist.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.Whitelist.Request>(data);
                    SharedModel.Models.Tools.RetentionModel.Whitelist.Response lResponse = new SharedModel.Models.Tools.RetentionModel.Whitelist.Response();



                    SharedBusiness.Tools.BIRetention BiRetention = new SharedBusiness.Tools.BIRetention();

                    lResponse = BiRetention.ListWhitelist(customer, lRequest, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(lResponse.ListWhitelist));

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
        [Route("whiteListInsert")]
        public HttpResponseMessage InsertCuitToWhiteList()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string countryCode = headers.GetValues("countryCode").First();
                //var re = Request;
                //var headers = re.Headers;

                if (data != null && data.Length > 0)
                {

                    SharedModel.Models.Tools.RetentionModel.WhiteList whiteList = new SharedModel.Models.Tools.RetentionModel.WhiteList();
                    SharedModel.Models.Tools.RetentionModel.Whitelist.Response lResponse = new SharedModel.Models.Tools.RetentionModel.Whitelist.Response();

                    whiteList = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.WhiteList>(data);


                    SharedBusiness.Tools.BIRetention BI = new SharedBusiness.Tools.BIRetention();
                    lResponse = BI.CreateWhiteListMember(whiteList, customer);



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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("TransactionsForRetentions")]
        public List<TransAgForRetentionsModel> GetTransactionsAgroupedForRetentions([FromBody] TransAgForRetentionsFilterModel filter)
        {
            var BI = new SharedBusiness.Tools.BIRetention();
            return BI.GetTransactionsAgroupedForRetentions(filter);
        }
        [HttpPost]
        [Route("whiteListUpdate")]
        public HttpResponseMessage UpdateCuitToWhiteList()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string updateType = headers.GetValues("updateType").First();

                if (data != null && data.Length > 0)
                {

                    SharedModel.Models.Tools.RetentionModel.WhiteList whiteList = new SharedModel.Models.Tools.RetentionModel.WhiteList();
                    whiteList = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.WhiteList>(data);

                    //string CUIT = whiteList.Numberdoc;
                   

                    SharedBusiness.Tools.BIRetention BI = new SharedBusiness.Tools.BIRetention();
                    whiteList = BI.UpdateWhiteListMember(customer,whiteList, updateType);


                    if (whiteList.Status != "OK")
                        throw new Exception(whiteList.StatusMessage);

                    return Request.CreateResponse(HttpStatusCode.OK, whiteList);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteListWhite")]
        public HttpResponseMessage DeleteCuitToWhiteList()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                if (data != null && data.Length > 0)
                {

                    SharedModel.Models.Tools.RetentionModel.WhiteList whiteList = new SharedModel.Models.Tools.RetentionModel.WhiteList();
                    whiteList = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.WhiteList>(data);

                    string Numberdoc = whiteList.Numberdoc;


                    SharedBusiness.Tools.BIRetention BI = new SharedBusiness.Tools.BIRetention();
                    whiteList = BI.DeleteWhiteListMember(Numberdoc,customer);


                    if (whiteList.Status != "OK")
                        throw new Exception(whiteList.StatusMessage);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(whiteList));
                }

                else
                {
                    GeneralErrorModel Error = new GeneralErrorModel()
                    {
                        Code = "400",
                        CodeDescription = "BadRequest",
                        Message = "Bad Parameter Value or Parameters Values."
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(Error));
                }

            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        [Route("getCertificatesStatus")]
        public HttpResponseMessage GetCertificatesStatus()
        {
            try
            {
                string customer = Request.Headers.GetValues("customer_id").First();

                List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus> response = new List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus>();

                if (customer != null && customer.Length > 0)
                {
                    SharedBusiness.Tools.BIRetention certificatesProcess = new SharedBusiness.Tools.BIRetention();
                    response = certificatesProcess.ListCertificatesProcessInfo();

                    return Request.CreateResponse(HttpStatusCode.OK, response);
                }
                else
                {
                    GeneralErrorModel Error = new GeneralErrorModel()
                    {
                        Code = "400",
                        CodeDescription = "BadRequest",
                        Message = "Bad Parameter Value or Parameters Values."
                    };

                    return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(Error));
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("list_retentions")]
        public HttpResponseMessage List_Retentions()
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
                    Report.List.Request lRequest = JsonConvert.DeserializeObject<Report.List.Request>(data);
                    Report.List.Response lResponse = new Report.List.Response();

                    SharedBusiness.Tools.BIRetention BiRetention = new SharedBusiness.Tools.BIRetention();

                    lResponse = BiRetention.ListRetentions(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(lResponse.TransactionList));
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
        [Route("refund_retentions")]
        public HttpResponseMessage Refund_Retention() {

            HttpContent requestContent = Request.Content;
            string data = requestContent.ReadAsStringAsync().Result;
            if (data != null && data.Length > 0)
            {

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0)
                {
                    SharedModel.Models.Tools.RetentionModel.Refund.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Tools.RetentionModel.Refund.Request>(data);
                    Report.List.Response lResponse = new Report.List.Response();

                    SharedBusiness.Tools.BIRetention BiRetention = new SharedBusiness.Tools.BIRetention();

                    lResponse = BiRetention.RefundRetentions(lRequest, customer);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(lResponse.TransactionList));
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
        [Route("downloadCertificates")]
        public FileModel DownloadCertificatesZip()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result.Substring(1);
                var numbers = data.Substring(0, data.Length - 1).Split(',');
                List<long> idTransactions = new List<long>();

                if (numbers.Length == 1 && string.IsNullOrEmpty(numbers[0]))
                    throw new Exception("Bad Parameter Value or Parameters Values.");

                foreach (var number in numbers)
                {
                    idTransactions.Add(long.Parse(number));
                }

                string customer = Request.Headers.GetValues("customer_id").First();

                if (customer != null && customer.Length > 0 && idTransactions.Count > 0)
                {
                    SharedBusiness.Tools.BIRetention BiRetention = new SharedBusiness.Tools.BIRetention();
                    var fileDTO = BiRetention.DownloadCertificates(idTransactions);
                    return fileDTO;
                }
                else
                {
                    throw new Exception("Bad Parameter Value or Parameters Values.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
