using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharedModel.Models.General;
namespace SharedModel.Models.Security
{
    public class TransactionError
    {
        public class ModelDB
        {

            public string idTransactionType { get; set; }
            public string idEntityUser { get; set; }
            public string beneficiaryName { get; set; }

            public string typeOfId { get; set; }
            public string beneficiaryId { get; set; }
            public string cbu { get; set; }
            public string bankCode { get; set; }
            public string bankBranch { get; set; }
            public string accountType { get; set; }
            public string accountNumber { get; set; }
            public string amount { get; set; }
            public string paymentDate { get; set; }
            public string submerchantCode { get; set; }
            public string currency { get; set; }
            public string merchantId { get; set; }

            public string beneficiaryAddress { get; set; }
            public string beneficiaryCity { get; set; }
            public string beneficiaryCountry { get; set; }
            public string beneficiaryEmail { get; set; }
            public string beneficiaryBirthdate { get; set; }
            public string beneficiarySenderName { get; set; }
            public string beneficiarySenderAddress { get; set; }
            public string beneficiarySenderState { get; set; }
            public string beneficiarySenderCountry { get; set; }
            public string beneficiarySenderTaxid { get; set; }
            public string beneficiarySenderBirthDate { get; set; }
            public string beneficiarySenderEmail { get; set; }
            public string errors { get; set; }
            public string status { get; set; }

        }

        public class List
        {
            public class Model
            {

                public string ProcessDate { get; set; }
                public string TransactionType { get; set; }
                public string PaymentDate { get; set; }
                public string Merchant { get; set; }
                public string MerchantId { get; set; }
                public string SubMerchant { get; set; }
                public string BeneficiaryName { get; set; }
                public string Country { get; set; }
                public string City { get; set; }
                public string Address { get; set; }
                public string Email { get; set; }
                public string Birthdate { get; set; }
                public string BeneficiaryID { get; set; }
                public string CBU { get; set; }
                public string Amount { get; set; }
                public List<General.ErrorModel.ValidationError> ListErrors { get; set; }
                public List<General.ErrorModel.ValidationErrorGroup> ListErrorsCliente { get { return FormatErrorsCliente(ListErrors); } set { } }
                public string ListErrorsFormattedExport { get { return FormatErrors(ListErrorsCliente); } set {  } }

                public List<General.ErrorModel.ValidationErrorGroup> FormatErrorsCliente(List<General.ErrorModel.ValidationError> _listErrors)
                {

                    List<General.ErrorModel.ValidationErrorGroup> errores = new List<General.ErrorModel.ValidationErrorGroup>();

                    errores =
                                    (
                                    from pe in _listErrors
                                    group pe.Message by pe.Key into KeyGroup
                                    select new General.ErrorModel.ValidationErrorGroup { Key = KeyGroup.Key, Messages = KeyGroup.ToList() }
                                    ).ToList();

                    return errores;
                }
                public string FormatErrors(List<General.ErrorModel.ValidationErrorGroup> listErrors)
                {

                    string LineErrors = "";
                    //List<ErrorModel.ValidationErrorGroup> objErrors = JsonConvert.DeserializeObject<List<ErrorModel.ValidationErrorGroup>>(jsonError);



                    foreach (var error in listErrors)
                    {
                        LineErrors = LineErrors + "  **Key:  " + error.Key + "  *Messages:  ";

                        foreach (var message in error.Messages)
                        {
                            LineErrors = LineErrors + "  " + message;
                        }
                    }
                    return LineErrors;
                }
            }
       
            public class Request
            {

                public string dateFrom { get; set; }
                public string dateTo { get; set; }
                public string idEntityUser { get; set; }
                public string idEntitySubmerchant { get; set; }
                public string idTransactionType { get; set; }
                public string amount { get; set; }
                public string merchantId { get; set; }
                public string pageSize { get; set; }
                public string offset { get; set; }

            }

            public class Response
            {

                public string TransactionList { get; set; }
            }

        }
    }
}
