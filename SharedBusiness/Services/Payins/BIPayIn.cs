using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.Services.Payins
{
    public class BIPayIn
    {
        public SharedModel.Models.Services.Payins.PayinModel.Create.Response PayinTransaction(SharedModel.Models.Services.Payins.PayinModel.Create.Request data, string customer, string countryCode)
        {
            try
            {
                DAO.DataAccess.Services.DbPayIn LPDAO = new DAO.DataAccess.Services.DbPayIn();

                SharedModel.Models.Services.Payins.PayinModel.Create.Response response = new SharedModel.Models.Services.Payins.PayinModel.Create.Response();
                DataTable payin = new DataTable();
                payin = LPDAO.CreatePayin(data, customer, countryCode);

                response.currency = data.currency;
                response.merchant_id = data.merchant_id;
                response.payer_account_number = data.payer_account_number;
                response.payer_document_number = data.payer_document_number;
                response.payer_name = data.payer_name;
                response.payment_method_code = data.payment_method_code;
                response.submerchant_code = data.submerchant_code;
                response.amount = data.amount;
                response.payer_phone_number = data.payer_phone_number;
                response.payer_email = data.payer_email;

                string statusObservation = payin.Rows[0].Field<string>("StatusObservation");

                if (statusObservation != "OK")
                {
                    if (statusObservation == "ERROR::NF")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The submerchant_code not found in database.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_NOTFOUND");
                        response.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "submerchant_code", Messages = messages, CodeTypeError = codeTypeError });
                    }
                    else if (statusObservation == "ERROR::CNF")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The customer_id or countryCode not found in database.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_INVALID");
                        response.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "credentials", Messages = messages, CodeTypeError = codeTypeError });
                    }
                    else if (statusObservation == "ERROR::AEXIST")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The merchant_id specified already exist in the system, must be unique.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_ALREADYEXIST");

                        response.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "merchant_id", Messages = messages, CodeTypeError = codeTypeError });
                    } else if ( statusObservation == "ERROR::DUPAMT")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("There's already another transaction in progress with the same amount specified.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_DUPAMOUNT");

                        response.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "amount", Messages = messages, CodeTypeError = codeTypeError });
                    }
                } else
                {
                    response.payin_id = payin.Rows[0].Field<long>("idTransaction");
                    response.status = payin.Rows[0].Field<string>("Status");
                    response.status_detail = payin.Rows[0].Field<string>("StatusDetail");
                    response.transaction_date = payin.Rows[0].Field<string>("TransactionDate");
                    response.reference_code = payin.Rows[0].Field<string>("ReferenceCode");
                }

                return response;
            }
            catch(Exception ex)
            {
                LogService.LogError(ex.ToString());
                throw ex;
            }
        }

        public List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response> ManagePayins(SharedModel.Models.Services.Payins.PayinModel.Manage.Request data, string countryCode)
        {
            try
            {
                DAO.DataAccess.Services.DbPayIn LPDAO = new DAO.DataAccess.Services.DbPayIn();

                List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response> response = new List<SharedModel.Models.Services.Payins.PayinModel.Manage.Response>();

                response = LPDAO.ManagePayin(data, countryCode);

                return response;

            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                throw ex;
            }
        }

        public List<SharedModel.Models.Services.Payins.PayinModel.List.Response> ListTransactions(SharedModel.Models.Services.Payins.PayinModel.List.Request request, string customer, string countryCode)
        {
            List<SharedModel.Models.Services.Payins.PayinModel.List.Response> list;

            DAO.DataAccess.Services.DbPayIn dbPI = new DAO.DataAccess.Services.DbPayIn();

            list = dbPI.ListPayinTransactions(request, customer, countryCode);

            return list;
        }
    }
}
