using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Linq;

using System.Web.Http.Cors;

using System.Configuration;
using SharedModel.Models.General;
using SharedBusiness.Log;
using System.Web;
using SharedBusiness.Filters;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
using SharedBusiness.Services.CrossCutting;
using WA_LP.Infrastructure;
using SharedBusiness.Mail;
using WA_LP.Cache;
using static SharedModel.Models.Services.Payouts.Payouts;
using System.Threading.Tasks;
using SharedBusiness.Payin.DTO;
using SharedModel.Models.Services.Banks.Galicia;
using static SharedModel.Models.Services.Mexico.PayOutMexico;

namespace WA_LP.Controllers.WA.Services
{
    [Authorize]
    [RoutePrefix("v2/payouts")]
    [ApiExplorerSettings(IgnoreApi = false)]

    public class PayOutController : BaseApiController
    {

        /// <summary>
        /// Create
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [Route("payout")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request>))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response>))]

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Request>))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(GeneralErrorModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(GeneralErrorModel))]


        public HttpResponseMessage Payouts()
        {
            String data = "";
            string customer = "";
            string countryCode = "";

            try
            {
                HttpContent requestContent = Request.Content;
                data = requestContent.ReadAsStringAsync().Result;

                string customerByAdmin = null;
                string countryCodeByAdmin = null;

                if (data != null && data.Length > 0)
                {
                    var re = Request;
                    var headers = re.Headers;
                    string _customer = headers.GetValues("customer_id").First();
                    string _countryCode = headers.GetValues("countryCode").First();

                    customerByAdmin = headers.Contains("customerByAdmin") ? headers.GetValues("customerByAdmin").First() : null;
                    countryCodeByAdmin = headers.Contains("countryCodeByAdmin") ? headers.GetValues("countryCodeByAdmin").First() : null;

                    bool TransactionMechanism = headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                    customer = _customer;
                    countryCode = _countryCode;

                    if (customerByAdmin != null && countryCodeByAdmin != null)
                    {
                        customer = customerByAdmin;
                        countryCode = countryCodeByAdmin;
                    }

                    var subMerchantsUsers = new BlFilters().GetListSubMerchantUser();
                    var blackList = new BlFilters().GetBlackLists();

                    //var aml = new BlFilters().GetAML(countryCode);
                    List<SharedModel.Models.Database.General.BankCodesModel.BankCodesOrdered> bankCodes = BankValidateCacheService.Get();
                    var validatorData = new Dictionary<object, object>();
                    validatorData.Add("bankCodes", bankCodes);
                    validatorData.Add("countryCode", countryCode);

                    LogService.LogInfo(string.Format("REQUEST HEADERS: {0}, REQUEST BODY {1}", headers.ToString(), re.ToString()));

                    if (countryCode == "ARG")
                    {
                        List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request>>(data);
                        if (lRequest.Count > Convert.ToInt32(ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]))
                            throw new Exception(string.Format("The max limit of the item in the array transaction is: {0}", ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]));

                        List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> lResponse = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response>();

                        foreach (SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request model in lRequest)
                        {
                            model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model, validatorData);

                            var subMerchant = subMerchantsUsers.FirstOrDefault(x => (x.Identification == _customer || ((x.MailAccount == _customer || (x.MailAccount == customerByAdmin && customerByAdmin != null)) && x.CountryCode == countryCode)) && x.Description == model.submerchant_code);
                            var newSubmerchant = new SharedModel.Models.Filters.Filter.EntitySubMerchant();
                            newSubmerchant.Identification = model.submerchant_code;
                            newSubmerchant.IsCorporate = true;
                            subMerchant = subMerchant != null ? subMerchant : newSubmerchant;

                            if (subMerchant == null)
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "SubMerchantNotFound", Messages = new List<string>() { "SubMerchant not found" }, CodeTypeError = new List<string> { "SubMerchantNotFound" } });
                            }
                            else if (!subMerchant.IsCorporate && (string.IsNullOrEmpty(model.sender_name) || string.IsNullOrEmpty(model.sender_address) || string.IsNullOrEmpty(model.sender_country)))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "MissingSenderInformation", Messages = new List<string>() { "Missing sender information" }, CodeTypeError = new List<string> { "MissingSenderInformation" } });
                            }

                            if (!string.IsNullOrEmpty(model.concept_code) && !DictionaryService.PayoutConceptExist(model.concept_code))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ConceptCodeNotFound", Messages = new List<string>() { "Concept Code not found" }, CodeTypeError = new List<string> { "ConceptCodeNotFound" } });
                            }

                            if (blackList.Any(x => x.accountNumber.Replace(" ", "").ToLower() == model.bank_cbu.Replace(" ", "").ToLower() || x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || x.documentId.Replace(" ", "").ToLower() == model.beneficiary_cuit.Replace(" ", "").ToLower()))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_BENEFICIARY_BLACKLISTED", Messages = new List<string>() { "Beneficiary is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }
                            if (model.sender_name != null)
                            {
                                if (blackList.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_SENDER_BLACKLISTED", Messages = new List<string>() { "Sender is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                            }
                        }

                        var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                        var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                        if (ListWhitOutErrors.Count > 0)
                        {
                            SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();
                            lResponse.AddRange(BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism, countryCode));
                        }
                        if (ListWhitErrors.Count > 0)
                        {
                            var a = JsonConvert.SerializeObject(ListWhitErrors);
                            lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response>>(a));


                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"))) 
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_REJECTED_BENEFICIARY_BLACKLISTED", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }
                        }
                        List<SharedModel.Models.Security.TransactionError.ModelDB> ListTxError = new List<SharedModel.Models.Security.TransactionError.ModelDB>();
                        SharedBusiness.Security.BlLog BlLog = new SharedBusiness.Security.BlLog();

                        List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> ResponseWithErrors = lResponse.Where(e => e.ErrorRow.HasError == true).ToList();

                        lResponse.ForEach(x => x.authenticate = lRequest.Where(m => m.merchant_id == x.merchant_id).Select(s => s.authenticate).FirstOrDefault());

                        List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> ResponseToOnHold = lResponse.Where(e => e.ErrorRow.HasError == false && e.authenticate == true).ToList();

                        if (ResponseToOnHold.Count > 0) {
                            List<string> ticketsOnHold = ResponseToOnHold.Select(x => x.Ticket).ToList();
                            SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                            List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                            lResponse.Where(x => onHoldResponse.Any(a => a == x.Ticket)).ToList().ForEach(x => x.status = "OnHold");
                        }


                        if (ResponseWithErrors.Count > 0)
                        {
                            SharedModel.Models.Security.TransactionError.ModelDB errorModel;

                            foreach (var e in ResponseWithErrors)
                            {
                                errorModel = new SharedModel.Models.Security.TransactionError.ModelDB();

                                List<ErrorCode> _errors = new List<ErrorCode>();
                                errorModel.idTransactionType = "2";
                                errorModel.idEntityUser = null;
                                errorModel.beneficiaryName = e.beneficiary_name;
                                errorModel.beneficiaryId = e.beneficiary_cuit;
                                errorModel.paymentDate = e.payout_date;
                                errorModel.cbu = e.bank_cbu;
                                errorModel.amount = e.amount.ToString();
                                errorModel.accountType = e.bank_account_type;
                                errorModel.submerchantCode = e.submerchant_code;
                                errorModel.currency = e.currency;
                                errorModel.merchantId = e.merchant_id;
                                errorModel.beneficiaryAddress = e.beneficiary_address;
                                errorModel.beneficiaryCity = e.beneficiary_state;
                                errorModel.beneficiaryCountry = e.beneficiary_country;
                                errorModel.beneficiaryEmail = e.beneficiary_email;
                                errorModel.beneficiaryBirthdate = e.beneficiary_birth_date;
                                errorModel.beneficiarySenderName = e.sender_name;
                                errorModel.beneficiarySenderAddress = e.sender_address;
                                errorModel.beneficiarySenderCountry = e.sender_country;
                                errorModel.beneficiarySenderState = e.sender_state;
                                errorModel.beneficiarySenderTaxid = e.sender_taxid;
                                errorModel.beneficiarySenderBirthDate = e.sender_birthdate;
                                errorModel.beneficiarySenderEmail = e.sender_email;

                                foreach (var error in e.ErrorRow.Errors)
                                {
                                    foreach (var code in error.CodeTypeError)
                                    {
                                        if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                        {
                                            _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                        }
                                    }
                                }
                                errorModel.errors = JsonConvert.SerializeObject(_errors);

                                ListTxError.Add(errorModel);
                            }
                            BlLog.InsertTransactionErrors(ListTxError, customer, countryCode, TransactionMechanism);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
                    else if (countryCode == "COL")
                    {
                        var aml = new BlFilters().GetAML(countryCode);
                        List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Request>>(data);

                        if (lRequest.Count > 0)
                        {
                            lRequest.Select(c => { c.beneficiary_email = string.IsNullOrEmpty(c.beneficiary_email) ? null : c.beneficiary_email; return c; }).ToList();
                        }

                        if (lRequest.Count > Convert.ToInt32(ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]))
                            throw new Exception(string.Format("The max limit of the item in the array transaction is: {0}", ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]));

                        List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response> lResponse = new List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response>();

                        foreach (SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Request model in lRequest)
                        {
                            model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model, validatorData);

                            var subMerchant = subMerchantsUsers.FirstOrDefault(x => (x.Identification == _customer || ((x.MailAccount == _customer || (x.MailAccount == customerByAdmin && customerByAdmin != null)) && (x.CountryCode == countryCode || (x.CountryCode == countryCodeByAdmin && countryCodeByAdmin != null)))) && x.Description.ToLower() == model.submerchant_code.ToLower());
                            var newSubmerchant = new SharedModel.Models.Filters.Filter.EntitySubMerchant();
                            newSubmerchant.Identification = model.submerchant_code;
                            newSubmerchant.IsCorporate = true;
                            subMerchant = subMerchant != null ? subMerchant : newSubmerchant;

                            if (subMerchant == null)
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "SubMerchantNotFound", Messages = new List<string>() { "SubMerchant not found" }, CodeTypeError = new List<string> { "SubMerchantNotFound" } });
                            }
                            else if (!subMerchant.IsCorporate && (string.IsNullOrEmpty(model.sender_name) || string.IsNullOrEmpty(model.sender_address) || string.IsNullOrEmpty(model.sender_country)))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "MissingSenderInformation", Messages = new List<string>() { "Missing sender information" }, CodeTypeError = new List<string> { "MissingSenderInformation" } });
                            }
                            if (!string.IsNullOrEmpty(model.concept_code) && !DictionaryService.PayoutConceptExist(model.concept_code))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ConceptCodeNotFound", Messages = new List<string>() { "Concept Code not found" }, CodeTypeError = new List<string> { "ConceptCodeNotFound" } });
                            }
                            if (blackList.Any(x => x.accountNumber.Replace(" ", "").ToLower() == model.beneficiary_account_number.Replace(" ", "").ToLower() || x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || x.documentId.Replace(" ", "").ToLower() == model.id.Replace(" ", "").ToLower()))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_BENEFICIARY_BLACKLISTED", Messages = new List<string>() { "Beneficiary is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }
                            if (aml.Any(x => x.isSender == "0" && (x.accountNumber.Replace(" ", "").ToLower() == model.beneficiary_account_number.Replace(" ", "").ToLower() || x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || x.documentId.Replace(" ", "").ToLower() == model.id.Replace(" ", "").ToLower())))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_AMOUNT_EXCEEDS_BENEFICIARY_MAX_LIMIT", Messages = new List<string>() { "Beneficiary exceeds transaction limit" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }
                            if (model.sender_name != null)
                            {
                                if (blackList.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_SENDER_BLACKLISTED", Messages = new List<string>() { "Sender is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                                if (aml.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_AMOUNT_EXCEEDS_SENDER_MAX_LIMIT", Messages = new List<string>() { "Sender exceeds transaction limit" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                            }
                        }

                        var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                        var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                        if (ListWhitOutErrors.Count > 0)
                        {
                            SharedBusiness.Services.Colombia.Banks.Bancolombia.BIPayOutColombia BiPO = new SharedBusiness.Services.Colombia.Banks.Bancolombia.BIPayOutColombia();
                            lResponse.AddRange(BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism, countryCode));
                        }
                        if (ListWhitErrors.Count > 0)
                        {
                            var a = JsonConvert.SerializeObject(ListWhitErrors);
                            lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response>>(a));

                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"))) 
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_REJECTED_BENEFICIARY_BLACKLISTED", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }
                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_AMOUNT_EXCEEDS_BENEFICIARY_MAX_LIMIT" || y.Key == "ERROR_AMOUNT_EXCEEDS_SENDER_MAX_LIMIT")))
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_AMOUNT_EXCEEDS_BENEFICIARY_MAX_LIMIT" || y.Key == "ERROR_AMOUNT_EXCEEDS_SENDER_MAX_LIMIT"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_AMOUNT_EXCEEDS_BENEFICIARY_MAX_LIMIT/ERROR_AMOUNT_EXCEEDS_SENDER_MAX_LIMIT", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }


                        }
                        List<SharedModel.Models.Security.TransactionError.ModelDB> ListTxError = new List<SharedModel.Models.Security.TransactionError.ModelDB>();
                        SharedBusiness.Security.BlLog BlLog = new SharedBusiness.Security.BlLog();

                        List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response> ResponseWithErrors = lResponse.Where(e => e.ErrorRow.HasError == true).ToList();

                        lResponse.ForEach(x => x.authenticate = lRequest.Where(m => m.merchant_id == x.merchant_id).Select(s => s.authenticate).FirstOrDefault());

                        List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response> ResponseToOnHold = lResponse.Where(e => e.ErrorRow.HasError == false && e.authenticate == true).ToList();

                        if (ResponseToOnHold.Count > 0)
                        {
                            List<string> ticketsOnHold = ResponseToOnHold.Select(x => x.Ticket).ToList();
                            SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                            List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                            lResponse.Where(x => onHoldResponse.Any(a => a == x.Ticket)).ToList().ForEach(x => x.status = "OnHold");
                        }

                        if (ResponseWithErrors.Count > 0)
                        {

                            SharedModel.Models.Security.TransactionError.ModelDB errorModel;

                            foreach (var e in ResponseWithErrors)
                            {
                                errorModel = new SharedModel.Models.Security.TransactionError.ModelDB();

                                List<ErrorCode> _errors = new List<ErrorCode>();

                                errorModel.idTransactionType = "2";
                                errorModel.idEntityUser = null;
                                errorModel.beneficiaryName = e.beneficiary_name;
                                errorModel.beneficiaryId = e.id;
                                errorModel.paymentDate = e.payout_date;
                                errorModel.typeOfId = e.type_of_id;
                                errorModel.amount = e.amount.ToString();
                                errorModel.accountType = e.account_type;
                                errorModel.submerchantCode = e.submerchant_code;
                                errorModel.currency = e.currency;
                                errorModel.merchantId = e.merchant_id;
                                errorModel.beneficiaryAddress = e.beneficiary_address;
                                errorModel.beneficiaryCity = e.beneficiary_state;
                                errorModel.beneficiaryCountry = e.beneficiary_country;
                                errorModel.beneficiaryEmail = e.beneficiary_email;
                                errorModel.bankCode = e.bank_code;
                                errorModel.accountNumber = e.beneficiary_account_number;
                                errorModel.beneficiaryBirthdate = e.beneficiary_birth_date;
                                errorModel.beneficiarySenderName = e.sender_name;
                                errorModel.beneficiarySenderAddress = e.sender_address;
                                errorModel.beneficiarySenderCountry = e.sender_country;
                                errorModel.beneficiarySenderState = e.sender_state;
                                errorModel.beneficiarySenderTaxid = e.sender_taxid;
                                errorModel.beneficiarySenderBirthDate = e.sender_birthdate;
                                errorModel.beneficiarySenderEmail = e.sender_email;

                                foreach (var error in e.ErrorRow.Errors)
                                {
                                    foreach (var code in error.CodeTypeError)
                                    {
                                        if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                        {
                                            _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                        }
                                    }
                                }
                                errorModel.errors = JsonConvert.SerializeObject(_errors);

                                ListTxError.Add(errorModel);
                            }
                            BlLog.InsertTransactionErrors(ListTxError, customer, countryCode, TransactionMechanism);

                        }


                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
                    else if (countryCode == "URY")
                    {
                        List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Request>>(data);

                        if (lRequest.Count > Convert.ToInt32(ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]))
                            throw new Exception(string.Format("The max limit of the item in the array transaction is: {0}", ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]));

                        List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response> lResponse = new List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response>();

                        foreach (SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Request model in lRequest)
                        {
                            model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model, validatorData);

                            var subMerchant = subMerchantsUsers.FirstOrDefault(x => (x.Identification == _customer || ((x.MailAccount == _customer || (x.MailAccount == customerByAdmin && customerByAdmin != null)) && (x.CountryCode == countryCode || (x.CountryCode == countryCodeByAdmin && countryCodeByAdmin != null)))) && x.Description.ToLower() == model.submerchant_code.ToLower());
                            var newSubmerchant = new SharedModel.Models.Filters.Filter.EntitySubMerchant();
                            newSubmerchant.Identification = model.submerchant_code;
                            newSubmerchant.IsCorporate = true;
                            subMerchant = subMerchant != null ? subMerchant : newSubmerchant;

                            if (subMerchant == null)
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "SubMerchantNotFound", Messages = new List<string>() { "SubMerchant not found" }, CodeTypeError = new List<string> { "SubMerchantNotFound" } });
                            }
                            else if (!subMerchant.IsCorporate && (string.IsNullOrEmpty(model.sender_name) || string.IsNullOrEmpty(model.sender_address) || string.IsNullOrEmpty(model.sender_country)))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "MissingSenderInformation", Messages = new List<string>() { "Missing sender information" }, CodeTypeError = new List<string> { "MissingSenderInformation" } });
                            }
                            if (!string.IsNullOrEmpty(model.concept_code) && !DictionaryService.PayoutConceptExist(model.concept_code))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ConceptCodeNotFound", Messages = new List<string>() { "Concept Code not found" }, CodeTypeError = new List<string> { "ConceptCodeNotFound" } });
                            }
                            if (blackList.Any(x => x.accountNumber.Replace(" ", "").ToLower() == model.beneficiary_account_number.Replace(" ", "").ToLower() || x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || x.documentId.Replace(" ", "").ToLower() == model.beneficiary_document_id.Replace(" ", "").ToLower()))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_BENEFICIARY_BLACKLISTED", Messages = new List<string>() { "Beneficiary is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }
                            if (model.sender_name != null)
                            {
                                if (blackList.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_SENDER_BLACKLISTED", Messages = new List<string>() { "Sender is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                            }
                        }

                        var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                        var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                        if (ListWhitOutErrors.Count > 0)
                        {
                            SharedBusiness.Services.Uruguay.BIPayOutUruguay BiPO = new SharedBusiness.Services.Uruguay.BIPayOutUruguay();
                            lResponse.AddRange(BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism, countryCode));
                        }
                        if (ListWhitErrors.Count > 0)
                        {
                            var a = JsonConvert.SerializeObject(ListWhitErrors);
                            lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response>>(a));

                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"))) 
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_REJECTED_BENEFICIARY_BLACKLISTED", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }
                        }

                        List<SharedModel.Models.Security.TransactionError.ModelDB> ListTxError = new List<SharedModel.Models.Security.TransactionError.ModelDB>();
                        SharedBusiness.Security.BlLog BlLog = new SharedBusiness.Security.BlLog();

                        List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response> ResponseWithErrors = lResponse.Where(e => e.ErrorRow.HasError == true).ToList();

                        lResponse.ForEach(x => x.authenticate = lRequest.Where(m => m.merchant_id == x.merchant_id).Select(s => s.authenticate).FirstOrDefault());

                        List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response> ResponseToOnHold = lResponse.Where(e => e.ErrorRow.HasError == false && e.authenticate == true).ToList();

                        if (ResponseToOnHold.Count > 0)
                        {
                            List<string> ticketsOnHold = ResponseToOnHold.Select(x => x.Ticket).ToList();
                            SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                            List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                            lResponse.Where(x => onHoldResponse.Any(a => a == x.Ticket)).ToList().ForEach(x => x.status = "OnHold");
                        }

                        if (ResponseWithErrors.Count > 0)
                        {

                            SharedModel.Models.Security.TransactionError.ModelDB errorModel;

                            foreach (var e in ResponseWithErrors)
                            {
                                errorModel = new SharedModel.Models.Security.TransactionError.ModelDB();

                                List<ErrorCode> _errors = new List<ErrorCode>();

                                errorModel.idTransactionType = "2";
                                errorModel.idEntityUser = null;
                                errorModel.beneficiaryName = e.beneficiary_name;
                                errorModel.beneficiaryId = e.beneficiary_document_id;
                                errorModel.paymentDate = e.payout_date;
                                errorModel.typeOfId = e.beneficiary_document_type;
                                errorModel.amount = e.amount.ToString();
                                errorModel.accountType = e.bank_account_type;
                                errorModel.submerchantCode = e.submerchant_code;
                                errorModel.currency = e.currency;
                                errorModel.merchantId = e.merchant_id;
                                errorModel.beneficiaryAddress = e.beneficiary_address;
                                errorModel.beneficiaryCity = e.beneficiary_state;
                                errorModel.beneficiaryCountry = e.beneficiary_country;
                                errorModel.beneficiaryEmail = e.beneficiary_email;
                                errorModel.bankCode = e.bank_code;
                                errorModel.accountNumber = e.beneficiary_account_number;
                                errorModel.beneficiaryBirthdate = e.beneficiary_birth_date;
                                errorModel.beneficiarySenderName = e.sender_name;
                                errorModel.beneficiarySenderAddress = e.sender_address;
                                errorModel.beneficiarySenderCountry = e.sender_country;
                                errorModel.beneficiarySenderState = e.sender_state;
                                errorModel.beneficiarySenderTaxid = e.sender_taxid;
                                errorModel.beneficiarySenderBirthDate = e.sender_birthdate;
                                errorModel.beneficiarySenderEmail = e.sender_email;

                                foreach (var error in e.ErrorRow.Errors)
                                {
                                    foreach (var code in error.CodeTypeError)
                                    {
                                        if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                        {
                                            _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                        }
                                    }
                                }
                                errorModel.errors = JsonConvert.SerializeObject(_errors);

                                ListTxError.Add(errorModel);
                            }

                            BlLog.InsertTransactionErrors(ListTxError, customer, countryCode, TransactionMechanism);

                        }


                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
                    else if (countryCode == "BRA")
                    {
                        List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request>>(data);

                        if (lRequest.Count > 0)
                        {
                            lRequest.Select(c => { c.beneficiary_email = string.IsNullOrEmpty(c.beneficiary_email) ? null : c.beneficiary_email; return c; }).ToList();
                        }

                        if (lRequest.Count > Convert.ToInt32(ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]))
                            throw new Exception(string.Format("The max limit of the item in the array transaction is: {0}", ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]));

                        List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response> lResponse = new List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response>();

                        foreach (SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request model in lRequest)
                        {
                            model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model, validatorData);

                            var subMerchant = subMerchantsUsers.FirstOrDefault(x => (x.Identification == _customer || ((x.MailAccount == _customer || (x.MailAccount == customerByAdmin && customerByAdmin != null)) && (x.CountryCode == countryCode || (x.CountryCode == countryCodeByAdmin && countryCodeByAdmin != null)))) && x.Description.ToLower() == model.submerchant_code.ToLower());
                            var newSubmerchant = new SharedModel.Models.Filters.Filter.EntitySubMerchant();
                            newSubmerchant.Identification = model.submerchant_code;
                            newSubmerchant.IsCorporate = true;
                            subMerchant = subMerchant != null ? subMerchant : newSubmerchant;

                            if (subMerchant == null)
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "SubMerchantNotFound", Messages = new List<string>() { "SubMerchant not found" }, CodeTypeError = new List<string> { "SubMerchantNotFound" } });
                            }
                            else if (!subMerchant.IsCorporate && (string.IsNullOrEmpty(model.sender_name) || string.IsNullOrEmpty(model.sender_address) || string.IsNullOrEmpty(model.sender_country)))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "MissingSenderInformation", Messages = new List<string>() { "Missing sender information" }, CodeTypeError = new List<string> { "MissingSenderInformation" } });
                            }
                            if (!string.IsNullOrEmpty(model.concept_code) && !DictionaryService.PayoutConceptExist(model.concept_code))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ConceptCodeNotFound", Messages = new List<string>() { "Concept Code not found" }, CodeTypeError = new List<string> { "ConceptCodeNotFound" } });
                            }
                            if (_customer == "000003100001") //BL rule for  for Efx Capital customer
                            {
                                if (blackList.Any(x => (x.accountNumber.Replace(" ", "").Replace("-","").Replace("/","").ToLower() == model.beneficiary_account_number.Replace(" ", "").Replace("-", "").Replace("/", "").ToLower() && x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower()) || 
                                                       (x.accountNumber.Replace(" ", "").Replace("-","").Replace("/","").ToLower() == model.beneficiary_account_number.Replace(" ", "").Replace("-", "").Replace("/", "").ToLower() && x.documentId.Replace(" ", "").ToLower() == model.beneficiary_document_id.Replace(" ", "").ToLower()) ||
                                                       (x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() && x.documentId.Replace(" ", "").ToLower() == model.beneficiary_document_id.Replace(" ", "").ToLower())))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_BENEFICIARY_BLACKLISTED", Messages = new List<string>() { "Beneficiary is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                };
                            }
                            /*if (aml.Any(x => x.isSender == "0" && (x.accountNumber.Replace(" ", "").Replace("-", "").Replace("/", "").ToLower() == model.beneficiary_account_number.Replace(" ", "").Replace("-", "").Replace("/", "").ToLower() || 
                                                                   x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || 
                                                                   x.documentId.Replace(" ", "").ToLower() == model.beneficiary_document_id.Replace(" ", "").ToLower())))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_BRA_AMOUNT_EXCEEDS_BENEFICIARY_MAX_LIMIT", Messages = new List<string>() { "Beneficiary exceeds transaction limit" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }*/
                            if (model.sender_name != null)
                            {
                                if (blackList.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_SENDER_BLACKLISTED", Messages = new List<string>() { "Sender is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                            }
                        }

                        var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                        var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                        if (ListWhitOutErrors.Count > 0)
                        {
                            ListWhitOutErrors = Tools.ValidationHelper.ValidateRequestBrasil(ListWhitOutErrors);
                            SharedBusiness.Services.Brasil.BIPayOutBrasil BiPO = new SharedBusiness.Services.Brasil.BIPayOutBrasil();
                            lResponse.AddRange(BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism, countryCode));

                            //CALL AML BLACKLIST
                            var aml = new BlFilters().GetAML(countryCode);
                            List<string> ticketsOnHold = new List<string>();

                            foreach (var item in lResponse)
                            {
                                if (aml.Any(x => x.isSender == "0" && ( x.accountNumber.Replace(" ", "").Replace("-", "").Replace("/", "").ToLower() == item.beneficiary_account_number.Replace(" ", "").Replace("-", "").Replace("/", "").ToLower() ||
                                                                        x.beneficiaryName.Replace(" ", "").ToLower() == item.beneficiary_name.Replace(" ", "").ToLower() ||
                                                                        x.documentId.Replace(" ", "").ToLower() == item.beneficiary_document_id.Replace(" ", "").ToLower())))
                                {
                                    //PUT ON HOLD TRANSACTION
                                    ticketsOnHold.Add(item.Ticket);
                                    item.status = "OnHold";
                                }
                            }

                            if(ticketsOnHold.Count > 0)
                            {
                                SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                                List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                                
                                //SEND EMAIL
                                //var amllisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were put in OnHold: </font><br><br>";
                                body += "<div> " + string.Join(",", ticketsOnHold.ToArray()) + " </div>";
                                MailService.SendMail("ONHOLD: ERROR_AMOUNT_EXCEEDS_BENEFICIARY_MAX_LIMIT", body, ConfigurationManager.AppSettings["AmlEmails"].ToString().Split(';'));
                            }

                        }
                        if (ListWhitErrors.Count > 0)
                        {
                            var a = JsonConvert.SerializeObject(ListWhitErrors);
                            lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response>>(a));

                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"))) 
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_REJECTED_BENEFICIARY_BLACKLISTED", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }
                        }
                        List<SharedModel.Models.Security.TransactionError.ModelDB> ListTxError = new List<SharedModel.Models.Security.TransactionError.ModelDB>();
                        SharedBusiness.Security.BlLog BlLog = new SharedBusiness.Security.BlLog();

                        List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response> ResponseWithErrors = lResponse.Where(e => e.ErrorRow.HasError == true).ToList();

                        lResponse.ForEach(x => x.authenticate = lRequest.Where(m => m.merchant_id == x.merchant_id).Select(s => s.authenticate).FirstOrDefault());

                        List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response> ResponseToOnHold = lResponse.Where(e => e.ErrorRow.HasError == false && e.authenticate == true).ToList();

                        if (ResponseToOnHold.Count > 0)
                        {
                            List<string> ticketsOnHold = ResponseToOnHold.Select(x => x.Ticket).ToList();
                            SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                            List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                            lResponse.Where(x => onHoldResponse.Any(a => a == x.Ticket)).ToList().ForEach(x => x.status = "OnHold");
                        }

                        if (ResponseWithErrors.Count > 0)
                        {

                            SharedModel.Models.Security.TransactionError.ModelDB errorModel;

                            foreach (var e in ResponseWithErrors)
                            {
                                errorModel = new SharedModel.Models.Security.TransactionError.ModelDB();

                                List<ErrorCode> _errors = new List<ErrorCode>();

                                errorModel.idTransactionType = "2";
                                errorModel.idEntityUser = null;
                                errorModel.beneficiaryName = e.beneficiary_name;
                                errorModel.beneficiaryId = e.beneficiary_document_id;
                                errorModel.paymentDate = e.payout_date;
                                errorModel.amount = e.amount.ToString();
                                errorModel.accountType = e.bank_account_type;
                                errorModel.submerchantCode = e.submerchant_code;
                                errorModel.currency = e.currency;
                                errorModel.merchantId = e.merchant_id;
                                errorModel.beneficiaryAddress = e.beneficiary_address;
                                errorModel.beneficiaryCity = e.beneficiary_state;
                                errorModel.beneficiaryCountry = e.beneficiary_country;
                                errorModel.beneficiaryEmail = e.beneficiary_email;
                                errorModel.bankCode = e.bank_code;
                                errorModel.bankBranch = e.bank_branch;
                                errorModel.accountNumber = e.beneficiary_account_number;
                                errorModel.beneficiaryBirthdate = e.beneficiary_birth_date;
                                errorModel.beneficiarySenderName = e.sender_name;
                                errorModel.beneficiarySenderAddress = e.sender_address;
                                errorModel.beneficiarySenderCountry = e.sender_country;
                                errorModel.beneficiarySenderState = e.sender_state;
                                errorModel.beneficiarySenderTaxid = e.sender_taxid;
                                errorModel.beneficiarySenderBirthDate = e.sender_birthdate;
                                errorModel.beneficiarySenderEmail = e.sender_email;

                                foreach (var error in e.ErrorRow.Errors)
                                {
                                    foreach (var code in error.CodeTypeError)
                                    {
                                        if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                        {
                                            _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                        }
                                    }
                                }
                                errorModel.errors = JsonConvert.SerializeObject(_errors);

                                ListTxError.Add(errorModel);
                            }
                            BlLog.InsertTransactionErrors(ListTxError, customer, countryCode, TransactionMechanism);

                        }
                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
                    else if (countryCode == "MEX")
                    {
                        List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Request>>(data);

                        if (lRequest.Count > Convert.ToInt32(ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]))
                            throw new Exception(string.Format("The max limit of the item in the array transaction is: {0}", ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]));

                        List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response> lResponse = new List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response>();

                        foreach (SharedModel.Models.Services.Mexico.PayOutMexico.Create.Request model in lRequest)
                        {

                            if (model.bank_code == null || model.bank_code == "")
                                model.bank_code = model.beneficiary_account_number != null ? model.beneficiary_account_number.Substring(0, 3) : null;

                            model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model);

                            var subMerchant = subMerchantsUsers.FirstOrDefault(x => (x.Identification == _customer || ((x.MailAccount == _customer || (x.MailAccount == customerByAdmin && customerByAdmin != null)) && (x.CountryCode == countryCode || (x.CountryCode == countryCodeByAdmin && countryCodeByAdmin != null)))) && x.Description.ToLower() == model.submerchant_code.ToLower());
                            var newSubmerchant = new SharedModel.Models.Filters.Filter.EntitySubMerchant();
                            newSubmerchant.Identification = model.submerchant_code;
                            newSubmerchant.IsCorporate = true;
                            subMerchant = subMerchant != null ? subMerchant : newSubmerchant;

                            if (subMerchant == null)
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "SubMerchantNotFound", Messages = new List<string>() { "SubMerchant not found" }, CodeTypeError = new List<string> { "SubMerchantNotFound" } });
                            }
                            else if (!subMerchant.IsCorporate && (string.IsNullOrEmpty(model.sender_name)))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "MissingSenderInformation", Messages = new List<string>() { "Missing sender information" }, CodeTypeError = new List<string> { "MissingSenderInformation" } });
                            }
                            if (!string.IsNullOrEmpty(model.concept_code) && !DictionaryService.PayoutConceptExist(model.concept_code))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ConceptCodeNotFound", Messages = new List<string>() { "Concept Code not found" }, CodeTypeError = new List<string> { "ConceptCodeNotFound" } });
                            }
                            if (blackList.Any(x => x.accountNumber.Replace(" ", "").ToLower() == model.beneficiary_account_number.Replace(" ", "").ToLower() || x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || (x.documentId.Replace(" ", "").ToLower() == model.beneficiary_document_id.Replace(" ", "").ToLower() && x.documentId.Replace(" ", "").ToLower() != "")))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_BENEFICIARY_BLACKLISTED", Messages = new List<string>() { "Beneficiary is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }
                            if (model.sender_name != null)
                            {
                                if (blackList.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_SENDER_BLACKLISTED", Messages = new List<string>() { "Sender is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                            }
                        }

                        var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                        var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                        if (ListWhitOutErrors.Count > 0)
                        {
                            SharedBusiness.Services.Mexico.BIPayOutMexico BiPO = new SharedBusiness.Services.Mexico.BIPayOutMexico();
                            lResponse.AddRange(BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism, countryCode));
                        }
                        if (ListWhitErrors.Count > 0)
                        {
                            var a = JsonConvert.SerializeObject(ListWhitErrors);
                            lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response>>(a));

                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"))) 
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_REJECTED_BENEFICIARY_BLACKLISTED", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }
                        }

                        List<SharedModel.Models.Security.TransactionError.ModelDB> ListTxError = new List<SharedModel.Models.Security.TransactionError.ModelDB>();
                        SharedBusiness.Security.BlLog BlLog = new SharedBusiness.Security.BlLog();

                        List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response> ResponseWithErrors = lResponse.Where(e => e.ErrorRow.HasError == true).ToList();

                        lResponse.ForEach(x => x.authenticate = lRequest.Where(m => m.merchant_id == x.merchant_id).Select(s => s.authenticate).FirstOrDefault());

                        List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response> ResponseToOnHold = lResponse.Where(e => e.ErrorRow.HasError == false && e.authenticate == true).ToList();

                        if (ResponseToOnHold.Count > 0)
                        {
                            List<string> ticketsOnHold = ResponseToOnHold.Select(x => x.Ticket).ToList();
                            SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                            List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                            lResponse.Where(x => onHoldResponse.Any(a => a == x.Ticket)).ToList().ForEach(x => x.status = "OnHold");
                        }

                        if (ResponseWithErrors.Count > 0)
                        {

                            SharedModel.Models.Security.TransactionError.ModelDB errorModel;

                            foreach (var e in ResponseWithErrors)
                            {
                                errorModel = new SharedModel.Models.Security.TransactionError.ModelDB();

                                List<ErrorCode> _errors = new List<ErrorCode>();

                                errorModel.idTransactionType = "2";
                                errorModel.idEntityUser = null;
                                errorModel.beneficiaryName = e.beneficiary_name;
                                errorModel.beneficiaryId = e.beneficiary_document_id;
                                errorModel.paymentDate = e.payout_date;
                                errorModel.amount = e.amount.ToString();
                                errorModel.accountType = e.bank_account_type;
                                errorModel.submerchantCode = e.submerchant_code;
                                errorModel.currency = e.currency;
                                errorModel.merchantId = e.merchant_id;
                                errorModel.beneficiaryAddress = e.beneficiary_address;
                                errorModel.beneficiaryCity = e.beneficiary_state;
                                errorModel.beneficiaryCountry = e.beneficiary_country;
                                errorModel.beneficiaryEmail = e.beneficiary_email;
                                errorModel.bankCode = e.bank_code;
                                errorModel.accountNumber = e.beneficiary_account_number;
                                errorModel.beneficiaryBirthdate = e.beneficiary_birth_date;
                                errorModel.beneficiarySenderName = e.sender_name;
                                errorModel.beneficiarySenderAddress = e.sender_address;
                                errorModel.beneficiarySenderCountry = e.sender_country;
                                errorModel.beneficiarySenderState = e.sender_state;
                                errorModel.beneficiarySenderTaxid = e.sender_taxid;
                                errorModel.beneficiarySenderBirthDate = e.sender_birthdate;
                                errorModel.beneficiarySenderEmail = e.sender_email;

                                foreach (var error in e.ErrorRow.Errors)
                                {
                                    foreach (var code in error.CodeTypeError)
                                    {
                                        if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                        {
                                            _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                        }
                                    }
                                }
                                errorModel.errors = JsonConvert.SerializeObject(_errors);

                                ListTxError.Add(errorModel);
                            }

                            BlLog.InsertTransactionErrors(ListTxError, customer, countryCode, TransactionMechanism);

                        }
                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
                    else if(countryCode == "CHL")
                    {
                        List<SharedModel.Models.Services.Chile.PayOutChile.Create.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Chile.PayOutChile.Create.Request>>(data);

                        if (lRequest.Count > Convert.ToInt32(ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]))
                            throw new Exception(string.Format("The max limit of the item in the array transaction is: {0}", ConfigurationManager.AppSettings["LIMIT_PAYOUT_CREATE_API_COUNT"]));

                        List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response> lResponse = new List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response>();

                        foreach (SharedModel.Models.Services.Chile.PayOutChile.Create.Request model in lRequest)
                        {
                            model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model, validatorData);

                            var subMerchant = subMerchantsUsers.FirstOrDefault(x => (x.Identification == _customer || ((x.MailAccount == _customer || (x.MailAccount == customerByAdmin && customerByAdmin != null)) && (x.CountryCode == countryCode || (x.CountryCode == countryCodeByAdmin && countryCodeByAdmin != null)))) && x.Description.ToLower() == model.submerchant_code.ToLower());
                            var newSubmerchant = new SharedModel.Models.Filters.Filter.EntitySubMerchant();
                            newSubmerchant.Identification = model.submerchant_code;
                            newSubmerchant.IsCorporate = true;
                            subMerchant = subMerchant != null ? subMerchant : newSubmerchant;

                            if (subMerchant == null)
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "SubMerchantNotFound", Messages = new List<string>() { "SubMerchant not found" }, CodeTypeError = new List<string> { "SubMerchantNotFound" } });
                            }
                            else if (!subMerchant.IsCorporate && (string.IsNullOrEmpty(model.sender_name)))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "MissingSenderInformation", Messages = new List<string>() { "Missing sender information" }, CodeTypeError = new List<string> { "MissingSenderInformation" } });
                            }
                            if (!string.IsNullOrEmpty(model.concept_code) && !DictionaryService.PayoutConceptExist(model.concept_code))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ConceptCodeNotFound", Messages = new List<string>() { "Concept Code not found" }, CodeTypeError = new List<string> { "ConceptCodeNotFound" } });
                            }
                            if (blackList.Any(x => x.accountNumber.Replace(" ", "").ToLower() == model.beneficiary_account_number.Replace(" ", "").ToLower() || x.beneficiaryName.Replace(" ", "").ToLower() == model.beneficiary_name.Replace(" ", "").ToLower() || x.documentId.Replace(" ", "").ToLower() == model.beneficiary_document_id.Replace(" ", "").ToLower()))
                            {
                                model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_BENEFICIARY_BLACKLISTED", Messages = new List<string>() { "Beneficiary is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                            }
                            if (model.sender_name != null)
                            {
                                if (blackList.Any(x => x.isSender == "1" && x.beneficiaryName.Replace(" ", "").ToLower() == model.sender_name.Replace(" ", "").ToLower()))
                                {
                                    model.ErrorRow.Errors.Add(new ErrorModel.ValidationErrorGroup { Key = "ERROR_REJECTED_SENDER_BLACKLISTED", Messages = new List<string>() { "Sender is in the blacklist" }, CodeTypeError = new List<string> { "REJECTED" } });
                                }
                            }
                        }

                        var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                        var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                        if (ListWhitOutErrors.Count > 0)
                        {
                            SharedBusiness.Services.Chile.BIPayOutChile BiPO = new SharedBusiness.Services.Chile.BIPayOutChile();
                            lResponse.AddRange(BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism, countryCode));
                        }
                        if (ListWhitErrors.Count > 0)
                        {
                            var a = JsonConvert.SerializeObject(ListWhitErrors);
                            lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response>>(a));

                            if (ListWhitErrors.Any(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED")))
                            {
                                var blacklisted = ListWhitErrors.Where(x => x.ErrorRow.Errors.Any(y => y.Key == "ERROR_REJECTED_SENDER_BLACKLISTED" || y.Key == "ERROR_REJECTED_BENEFICIARY_BLACKLISTED"));
                                var body = "<font>The following transactions were rejected: </font><br><br>";
                                body += "<div> " + JsonConvert.SerializeObject(blacklisted) + " </div>";
                                MailService.SendMail("REJECTED: ERROR_REJECTED_BENEFICIARY_BLACKLISTED", body, ConfigurationManager.AppSettings["SupportEmails"].ToString().Split(';'));
                            }
                        }

                        List<SharedModel.Models.Security.TransactionError.ModelDB> ListTxError = new List<SharedModel.Models.Security.TransactionError.ModelDB>();
                        SharedBusiness.Security.BlLog BlLog = new SharedBusiness.Security.BlLog();

                        List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response> ResponseWithErrors = lResponse.Where(e => e.ErrorRow.HasError == true).ToList();

                        lResponse.ForEach(x => x.authenticate = lRequest.Where(m => m.merchant_id == x.merchant_id).Select(s => s.authenticate).FirstOrDefault());

                        List<SharedModel.Models.Services.Chile.PayOutChile.Create.Response> ResponseToOnHold = lResponse.Where(e => e.ErrorRow.HasError == false && e.authenticate == true).ToList();

                        if (ResponseToOnHold.Count > 0)
                        {
                            List<string> ticketsOnHold = ResponseToOnHold.Select(x => x.Ticket).ToList();
                            SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                            List<string> onHoldResponse = BiP.HoldTransactions(ticketsOnHold, countryCode);
                            lResponse.Where(x => onHoldResponse.Any(a => a == x.Ticket)).ToList().ForEach(x => x.status = "OnHold");
                        }

                        if (ResponseWithErrors.Count > 0)
                        {

                            SharedModel.Models.Security.TransactionError.ModelDB errorModel;

                            foreach (var e in ResponseWithErrors)
                            {
                                errorModel = new SharedModel.Models.Security.TransactionError.ModelDB();

                                List<ErrorCode> _errors = new List<ErrorCode>();

                                errorModel.idTransactionType = "2";
                                errorModel.idEntityUser = null;
                                errorModel.beneficiaryName = e.beneficiary_name;
                                errorModel.beneficiaryId = e.beneficiary_document_id;
                                errorModel.paymentDate = e.payout_date;
                                errorModel.typeOfId = e.beneficiary_document_type;
                                errorModel.amount = e.amount.ToString();
                                errorModel.accountType = e.bank_account_type;
                                errorModel.submerchantCode = e.submerchant_code;
                                errorModel.currency = e.currency;
                                errorModel.merchantId = e.merchant_id;
                                errorModel.beneficiaryAddress = e.beneficiary_address;
                                errorModel.beneficiaryCity = e.beneficiary_state;
                                errorModel.beneficiaryCountry = e.beneficiary_country;
                                errorModel.beneficiaryEmail = e.beneficiary_email;
                                errorModel.bankCode = e.bank_code;
                                errorModel.accountNumber = e.beneficiary_account_number;
                                errorModel.beneficiaryBirthdate = e.beneficiary_birth_date;
                                errorModel.beneficiarySenderName = e.sender_name;
                                errorModel.beneficiarySenderAddress = e.sender_address;
                                errorModel.beneficiarySenderCountry = e.sender_country;
                                errorModel.beneficiarySenderState = e.sender_state;
                                errorModel.beneficiarySenderTaxid = e.sender_taxid;
                                errorModel.beneficiarySenderBirthDate = e.sender_birthdate;
                                errorModel.beneficiarySenderEmail = e.sender_email;

                                foreach (var error in e.ErrorRow.Errors)
                                {
                                    foreach (var code in error.CodeTypeError)
                                    {
                                        if (_errors.Where(a => a.Code == code && a.Key == error.Key).ToList().Count == 0)
                                        {
                                            _errors.Add(new ErrorCode { Key = error.Key, Code = code });
                                        }
                                    }
                                }
                                errorModel.errors = JsonConvert.SerializeObject(_errors);

                                ListTxError.Add(errorModel);
                            }

                            BlLog.InsertTransactionErrors(ListTxError, customer, countryCode, TransactionMechanism);

                        }


                        return Request.CreateResponse(HttpStatusCode.OK, lResponse);
                    }
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
                LogService.LogError(ex, customer, string.Format("REQUEST {0}, COUNTRY {1}", data, countryCode));

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, Error);
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        [HttpDelete]
        [Route("cancel")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Request>))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(GeneralErrorModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(GeneralErrorModel))]
        public async System.Threading.Tasks.Task<HttpResponseMessage> Payout_cancel()
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

                    List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Request> lRequest = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Request>>(data);

                    List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> lResponse = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response>();

                    foreach (SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Request model in lRequest)
                    {
                        model.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(model);
                    }

                    var ListWhitOutErrors = lRequest.Where(x => x.ErrorRow.HasError == false).ToList();
                    var ListWhitErrors = lRequest.Where(x => x.ErrorRow.HasError == true).ToList();

                    if (ListWhitOutErrors.Count > 0)
                    {
                        SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();
                        List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs = BiPO.LotTransaction(ListWhitOutErrors, customer, TransactionMechanism);
                        lResponse.AddRange(cancelledTxs);

                        // CANCEL TXS CALLBACK NOTIFICATION
                        await ClientCallbackService.SendCancelCallback(cancelledTxs, customer);
                    }
                    if (ListWhitErrors.Count > 0)
                    {
                        var a = JsonConvert.SerializeObject(ListWhitErrors);
                        lResponse.AddRange(JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response>>(a));
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
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch>))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.List.Response>))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(GeneralErrorModel))]
        public HttpResponseMessage List_payouts()
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

                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request>(data);
                    var errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(lRequest);
                    List<SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch> lResponse = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.LotBatch>();
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Response response = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Response();
                    SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                    if (lRequest.payout_id == 0 && lRequest.transaction_id == 0 && lRequest.merchant_id == null && (lRequest.date_from == null || lRequest.date_to == null))
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

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);

                    if (countryCode == "ARG")
                    {
                        SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                    }
                    else if (countryCode == "COL")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                    }
                    else if (countryCode == "BRA")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                    }
                    else if (countryCode == "URY")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                    }
                    else if (countryCode == "CHL")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Chile.PayOutMapperChile();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                    }
                    else if (countryCode == "MEX")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Mexico.PayOutMapperMexico();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                    }
                    else
                    {
                        GeneralErrorModel Error = new GeneralErrorModel()
                        {
                            Code = "404",
                            CodeDescription = "Data not found",
                            Message = "Error - No data found."
                        };
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                    }


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
        /// <summary>
        /// Download
        /// </summary>
        [HttpPost]
        [Route("download")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(GeneralErrorModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(GeneralErrorModel))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage DownloadPayOutBankTxt()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                string providerName = Request.Headers.GetValues("providerName").First();
                var hourTo = Request.Headers.GetValues("hourTo").First().Split(':');
                int internalFiles = Int16.Parse(Request.Headers.GetValues("internalFiles").First().ToString());

                if (!string.IsNullOrEmpty(data))
                {
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(data);
                    List<string> lRequest = new List<string>();
                    lRequest = RequestModel.Select(x => x.Ticket).ToList();

                    SharedBusiness.Services.Payouts.BIPayOut BiPayOut = new SharedBusiness.Services.Payouts.BIPayOut();

                    if (countryCode == "ARG")
                    {
                        
                        SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();
                        SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();
                        ResponseModel = BiPO.DownloadLotBatchTransactionToBank(lRequest, TransactionMechanism, providerName);

                        ResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();

                        return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
                    }
                    else if (countryCode == "COL")
                    {
                        SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response ResponseModel = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();
                        SharedBusiness.Services.Colombia.Banks.Bancolombia.BIPayOutColombia BiPO = new SharedBusiness.Services.Colombia.Banks.Bancolombia.BIPayOutColombia();
                        ResponseModel = BiPO.DownloadLotBatchTransactionToBank(lRequest, TransactionMechanism, providerName);

                        ResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();

                        return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
                    }
                    else if (countryCode == "BRA")
                    {
                        SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse ExcelResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse();
                        SharedBusiness.Services.Brasil.BIPayOutBrasil BiPO = new SharedBusiness.Services.Brasil.BIPayOutBrasil();

                        if (providerName == "FASTCASH" || providerName == "PLURAL")
                        {
                            var reportesPath = HttpContext.Current.Server.MapPath("~/" + "Reports");
                            ExcelResponseModel = BiPO.DownloadExcelLotBatchTransactionToBank(lRequest, TransactionMechanism, providerName, reportesPath);

                            if (providerName == "FASTCASH")
                            {
                                if (ExcelResponseModel.transactions.Count > 0) { 
                                    ExcelResponseModel.FileBase64 = ExcelFastCash(ExcelResponseModel.transactions, "FastCash");
                                    ExcelResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();
                                }
                            }

                            return Request.CreateResponse(HttpStatusCode.OK, ExcelResponseModel);
                        }
                        else
                        {
                            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response();
                            ResponseModel = BiPO.DownloadLotBatchTransactionToBank(lRequest, TransactionMechanism, providerName);

                            ResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();

                            return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
                        }


                    }
                    else if (countryCode == "MEX")
                    {
                        SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response();
                        SharedBusiness.Services.Mexico.BIPayOutMexico BiPO = new SharedBusiness.Services.Mexico.BIPayOutMexico();
                        
                        ResponseModel = BiPO.DownloadLotBatchTransactionToBank(lRequest, TransactionMechanism, internalFiles, providerName);

                        ResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();

                        return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
                    }
                    else if (countryCode == "URY")
                    {
                        SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response ResponseModel = new SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response();
                        SharedBusiness.Services.Uruguay.BIPayOutUruguay BiPO = new SharedBusiness.Services.Uruguay.BIPayOutUruguay();
                        ResponseModel = BiPO.DownloadLotBatchTransactionToBank(lRequest, TransactionMechanism);

                        ResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();

                        return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
                    }
                    else if (countryCode == "CHL")
                    {
                        SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response ResponseModel = new SharedModel.Models.Services.Chile.PayOutChile.DownloadLotBatchTransactionToBank.Response();
                        SharedBusiness.Services.Chile.BIPayOutChile BiPO = new SharedBusiness.Services.Chile.BIPayOutChile();
                        ResponseModel = BiPO.DownloadLotBatchTransactionToBank(lRequest, TransactionMechanism, providerName);

                        ResponseModel.idLotOut = BiPayOut.GetLastLotOutNumber();

                        return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
                    }

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

                    return Request.CreateResponse(HttpStatusCode.BadRequest, JsonConvert.SerializeObject(Error));
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

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }
        /// <summary>
        /// Upload
        /// </summary>
        [HttpPost]
        [Route("upload")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response))]
        [SwaggerResponse(HttpStatusCode.BadRequest, Type = typeof(GeneralErrorModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof(GeneralErrorModel))]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async System.Threading.Tasks.Task<HttpResponseMessage> UploadPayOutBankTxtAsync()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;
                string datetime = DateTime.Now.ToString("yyyyMMddhhmmss");
                string providerName = Request.Headers.GetValues("providerName").First();
                string NiumEntityId = null;
                string ArfEntityId = null;
                string RemiteeEntityId = null;
                string ThunesEntityId = null;
                string InswitchEntityId = null;
                string EFXEntityId = null;

                if (countryCode == "ARG")
                {
                    SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request>(data);
                    if (string.IsNullOrEmpty(RequestModel.File) && providerName != "BSPVIELLE")
                        throw new Exception("File is null or empty!");

                    SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();
                    SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response();

                    if (providerName == "BGALICIA")
                    {
                        ResponseModel = BiPO.UploadTxtFromGALICIA(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "BSPVIELLE")
                    {
                        ResponseModel = BiPO.UploadTxtFromSUPERVIELLE(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "BBBVA")
                    {
                        ResponseModel = BiPO.UploadTxtFromBBVA(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    } 
                    else if (providerName == "BBBVATP")
                    {
                        ResponseModel = BiPO.UploadTxtFromBBVATuPago(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "SANTAR")
                    {
                        ResponseModel = BiPO.UploadTxtFromSantander(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }

                    // CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ResponseModel));
                }

                else if (countryCode == "COL")
                {
                    SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Request>(data);
                    if (string.IsNullOrEmpty(RequestModel.File))
                        throw new Exception("File is null or empty!");

                    SharedBusiness.Services.Colombia.Banks.Bancolombia.BIPayOutColombia BiPO = new SharedBusiness.Services.Colombia.Banks.Bancolombia.BIPayOutColombia();
                    SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response();

                    if (providerName == "BCOLOMBIA") 
                    {
                        ResponseModel = BiPO.UploadTxtFromBank(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "BCOLOMBIA2") 
                    {
                        ResponseModel = BiPO.UploadTxtFromBankBColombiaSas(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "OCCIDENTE")
                    {
                        ResponseModel = BiPO.UploadTxtFromBankOccidente(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }

                    // CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ResponseModel));
                }
                else if (countryCode == "BRA")
                {
                    SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Request>(data);
                    if (string.IsNullOrEmpty(RequestModel.File))
                        throw new Exception("File is null or empty!");

                    SharedBusiness.Services.Brasil.BIPayOutBrasil BiPO = new SharedBusiness.Services.Brasil.BIPayOutBrasil();
                    SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response();

                    if (providerName == "FASTCASH")
                    {
                        var ExcelData = ExcelBase64ToList(RequestModel.File, providerName);
                        var castData = ExcelData.Cast<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelFastcashResponse>().ToList();

                        ResponseModel = BiPO.UploadExcelFromBankFastcash(castData, RequestModel.CurrencyFxClose, datetime, TransactionMechanism, countryCode);
                    }
                    else if (providerName == "PLURAL") 
                    {
                        var ExcelData = ExcelBase64ToList(RequestModel.File, providerName);
                        var castData = ExcelData.Cast<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelPluralResponse>().ToList();

                        ResponseModel = BiPO.UploadExcelFromBankPlural(castData, RequestModel.CurrencyFxClose, datetime, TransactionMechanism, countryCode);
                    }
                    else if (providerName == "SAFRA")
                    {
                        ResponseModel = BiPO.UploadTxtFromBankSafra(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "BDOBR")
                    {
                        ResponseModel = BiPO.UploadTxtFromBankDoBrasil(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }
                    else if (providerName == "SANTBR")
                    {
                        ResponseModel = BiPO.UploadTxtFromBankSantander(RequestModel, datetime, TransactionMechanism, countryCode, providerName);
                    }

                    // CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ResponseModel));
                }
                else if (countryCode == "MEX")
                {
                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Request>(data);
                    if (string.IsNullOrEmpty(RequestModel.File))
                        throw new Exception("File is null or empty!");

                    SharedBusiness.Services.Mexico.BIPayOutMexico BiPO = new SharedBusiness.Services.Mexico.BIPayOutMexico();
                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();
                    string FileOperation = RequestModel.FileName.Substring(0, 2);

                    if(providerName == "MIFEL")
                    {
                        ResponseModel = BiPO.UploadTxtFromBank(RequestModel, datetime, TransactionMechanism, countryCode);
                    }
                    else if (providerName == "SRM")
                    {
                        ResponseModel = BiPO.UploadTxtFromBankSantander(RequestModel, datetime, TransactionMechanism, countryCode);
                    }
                        
                    //CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ResponseModel));

                }else if (countryCode == "CHL")
                {
                    SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.Request>(data);
                    if (string.IsNullOrEmpty(RequestModel.File))
                        throw new Exception("File is null or empty!");

                    SharedBusiness.Services.Chile.BIPayOutChile BiPO = new SharedBusiness.Services.Chile.BIPayOutChile();
                    SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.Response();

                    var ExcelData = ExcelBase64ToList(RequestModel.File, providerName);
                    var castData = ExcelData.Cast<SharedModel.Models.Services.Chile.PayOutChile.UploadTxtFromBank.ExcelItauResponse>().ToList();

                    ResponseModel = BiPO.UploadExcelFromBankItau(castData, RequestModel.CurrencyFxClose, datetime, TransactionMechanism, countryCode);

                    // CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(ResponseModel));
                }

                return Request.CreateResponse(HttpStatusCode.OK);


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
        [Route("list_payouts")]

        public HttpResponseMessage ListPayOuts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                var hourTo = Request.Headers.GetValues("hourTo").First().Split(':');

                if (!string.IsNullOrEmpty(data))
                {
                    SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request>(data);
                    RequestModel.dateTo = RequestModel.dateTo != null ? DateTime.Parse(RequestModel.dateTo).Date.ToString() : null;
                    if (hourTo.Length > 1)
                    {
                        RequestModel.dateTo = Tools.DateUtils.ParseDateHour(RequestModel.dateTo, hourTo);
                    }
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ResponseModel = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.ListTransaction(RequestModel, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("validate_payouts")]

        public HttpResponseMessage ValidatePayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request>(data);
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ResponseModel = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.ManageTransaction(RequestModel, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("received_payouts")]

        public HttpResponseMessage ReceivedPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request>(data);
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ResponseModel = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.ReceivedPayouts(RequestModel, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("update_payouts")]

        public async System.Threading.Tasks.Task<HttpResponseMessage> UpdatePayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                string providerName = Request.Headers.GetValues("providerName").First();
                string statusCode = Request.Headers.GetValues("statusCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(data);
                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.UpdateTransaction(RequestModel, countryCode, providerName, statusCode);

                    // CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("cancel_payouts")]

        public async Task<HttpResponseMessage> CancelPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(data);
                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.CancelTransaction(RequestModel, countryCode);

                    // CANCEL CALLBACK
                    await ClientCallbackService.SendCancelCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("get_onhold_payouts")]

        public async Task<HttpResponseMessage> GetOnHoldPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    ListPayoutsDownload.Request RequestModel = JsonConvert.DeserializeObject<ListPayoutsDownload.Request>(data);
                    List<ListPayoutsDownload.Response> ResponseModel = new List<ListPayoutsDownload.Response>();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.GetOnHoldPayouts(RequestModel, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("put_onhold_payouts")]
        public async Task<HttpResponseMessage> PutOnHoldPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<ListPayoutsDownload.Response>>(data);
                    UploadTxtFromBank.Response ResponseModel = new UploadTxtFromBank.Response();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut(); 

                    List<string> ticketsOnHold = RequestModel.Select(x => x.Ticket).ToList();

                    List<string> onHoldResponse = BiPO.HoldTransactions(ticketsOnHold, countryCode);
                    if(onHoldResponse.Count > 0)
                    {
                        foreach (var transaction in onHoldResponse)
                        {
                            UploadTxtFromBank.TransactionDetail detail = new UploadTxtFromBank.TransactionDetail();
                            detail.Ticket = transaction;
                            ResponseModel.TransactionDetail.Add(detail);
                        }
                        //TODO: Callback
                        ResponseModel.Status = "OK";
                        ResponseModel.StatusMessage = "OK";
                    }
                    else
                    {
                        ResponseModel.Status = "ERROR";
                        ResponseModel.StatusMessage = "An error ocurred processing a transaction";
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("put_received_payouts")]
        public async Task<HttpResponseMessage> PutReceivedPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(data);
                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.ReceivedTransaction(RequestModel, countryCode);

                    // TODO: Callback

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("revert_download")]
        public HttpResponseMessage RevertDownload()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(data);
                    List<string> lRequest = new List<string>();
                    lRequest = RequestModel.Select(x => x.Ticket).ToList();

                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.RevertDownload(lRequest, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("executed_payouts")]

        public HttpResponseMessage GetExecutedPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();

                if (!string.IsNullOrEmpty(data))
                {
                    SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request RequestModel = JsonConvert.DeserializeObject<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request>(data);
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ResponseModel = new List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.GetExecutedPayouts(RequestModel, countryCode);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("return_payouts")]
        public async System.Threading.Tasks.Task<HttpResponseMessage> ReturnPayouts()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                string providerName = Request.Headers.GetValues("providerName").First();
                string status = Request.Headers.GetValues("status").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> RequestModel = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response>>(data);
                    SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();
                    SharedBusiness.Services.Payouts.BIPayOut BiPO = new SharedBusiness.Services.Payouts.BIPayOut();
                    ResponseModel = BiPO.ReturnTransaction(RequestModel, countryCode, providerName, status);

                    // CALLBACK
                    await ClientCallbackService.CheckAndSendCallback(ResponseModel.TransactionDetail);

                    return Request.CreateResponse(HttpStatusCode.OK, ResponseModel);
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }

        [HttpPost]
        [Route("authenticate")]
        public HttpResponseMessage AuthorizePO()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                string countryCode = Request.Headers.GetValues("countryCode").First();
                string customer = Request.Headers.GetValues("customer_id").First();

                if (!string.IsNullOrEmpty(data))
                {
                    List<string> ticketList = new List<string>();
                    ticketList = JsonConvert.DeserializeObject<List<string>>(data);

                    SharedBusiness.Services.Payouts.BIPayOut BiP = new SharedBusiness.Services.Payouts.BIPayOut();
                    var listLotBachModel = BiP.ChangeTxtToRecieved(ticketList, countryCode, customer);

                    if (countryCode == "ARG")
                    {
                        SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else 
                        {
                            listResponse = listResponse.OrderBy(x => x.payout_id).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                        }
                    }
                    else if (countryCode == "COL")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                        {
                            listResponse = listResponse.OrderBy(x => x.payout_id).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                        }
                    }
                    else if (countryCode == "BRA")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                        {
                            listResponse = listResponse.OrderBy(x => x.payout_id).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                        }
                    }
                    else if (countryCode == "URY")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                        {
                            listResponse = listResponse.OrderBy(x => x.payout_id).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                        }
                    }
                    else if (countryCode == "CHL")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Chile.PayOutMapperChile();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                        {
                            listResponse = listResponse.OrderBy(x => x.payout_id).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                        }
                    }
                    else if (countryCode == "MEX")
                    {
                        var LPMapper = new SharedMaps.Converters.Services.Mexico.PayOutMapperMexico();

                        var listResponse = LPMapper.MapperModelsBatch(listLotBachModel);
                        listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                        if (listResponse == null || listResponse.Count == 0)
                        {
                            GeneralErrorModel Error = new GeneralErrorModel()
                            {
                                Code = "404",
                                CodeDescription = "Data not found",
                                Message = "Error - No data found."
                            };
                            return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                        }
                        else
                        {
                            listResponse = listResponse.OrderBy(x => x.payout_id).ToList();
                            return Request.CreateResponse(HttpStatusCode.OK, listResponse);
                        }
                    }
                    else
                    {
                        GeneralErrorModel Error = new GeneralErrorModel()
                        {
                            Code = "404",
                            CodeDescription = "Data not found",
                            Message = "Error - No data found."
                        };
                        return Request.CreateResponse(HttpStatusCode.NotFound, Error);
                    }
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
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name, SerializeRequest());

                GeneralErrorModel Error = new GeneralErrorModel()
                {
                    Code = "500",
                    CodeDescription = "InternalServerError",
                    Message = ex.Message.ToString()
                };

                return Request.CreateResponse(HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(Error));
            }
        }
    }
}
