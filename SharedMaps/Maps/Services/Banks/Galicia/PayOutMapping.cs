using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Services.Banks.Galicia;
using AutoMapper;

namespace SharedMaps.Maps.Services.Banks.Galicia
{
    public class PayOutMapping : Profile
    {
        public PayOutMapping()
        {
            #region POST API-DTO|DTO-API

            #region POST API-DTO
            CreateMap<PayOut.Create.Request, SharedModelDTO.Models.Transaction.TransactionModel>()
                .ForMember(to => to.Value, from => from.MapFrom(x => x.amount))
                .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")));

            CreateMap<PayOut.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel>()
                .ForMember(to => to.CBU, from => from.MapFrom(x => x.bank_cbu))
                .ForMember(to => to.TransactionAcreditationDate, from => from.MapFrom(x => x.payout_date))
                .ForMember(to => to.RecipientCUIT, from => from.MapFrom(x => x.beneficiary_cuit))
                .ForMember(to => to.CurrencyType, from => from.MapFrom(x => x.currency))
                .ForMember(to => to.Recipient, from => from.MapFrom(x => x.beneficiary_name))
                .ForMember(to => to.InternalDescription, from => from.MapFrom(x => x.merchant_id))
                .ForMember(to => to.BankAccountType, from => from.MapFrom(x => x.bank_account_type))
                .ForMember(to => to.RecipientPhoneNumber, from => from.MapFrom(x => x.beneficiary_phone_number))
                .ForMember(to => to.ConceptCode, from => from.MapFrom(x => x.concept_code));

            CreateMap<PayOut.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>()
                .ForMember(to => to.Address, from => from.MapFrom(x => x.beneficiary_address))
                .ForMember(to => to.BirthDate, from => from.MapFrom(x => x.beneficiary_birth_date))
                .ForMember(to => to.City, from => from.MapFrom(x => x.beneficiary_state))
                .ForMember(to => to.Country, from => from.MapFrom(x => x.beneficiary_country))
                .ForMember(to => to.Email, from => from.MapFrom(x => x.beneficiary_email))
                .ForMember(to => to.ZipCode, from => from.MapFrom(x => x.beneficiary_zip_code))
                .ForMember(to => to.SenderPhoneNumber, from => from.MapFrom(x => x.sender_phone_number))
                .ForMember(to => to.SenderName, from => from.MapFrom(x => x.sender_name))
                .ForMember(to => to.SenderCountry, from => from.MapFrom(x => x.sender_country))
                .ForMember(to => to.SenderState, from => from.MapFrom(x => x.sender_state))
                .ForMember(to => to.SenderAddress, from => from.MapFrom(x => x.sender_address))
                .ForMember(to => to.SenderTaxid, from => from.MapFrom(x => x.sender_taxid))
                .ForMember(to => to.SenderBirthDate, from => from.MapFrom(x => x.sender_birthdate))
                .ForMember(to => to.SenderEmail, from => from.MapFrom(x => x.sender_email))
                .ForMember(to => to.SenderZipCode, from => from.MapFrom(x => x.sender_zip_code));


            CreateMap<PayOut.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel>()
                .ForMember(to => to.SubMerchantIdentification, from => from.MapFrom(x => x.submerchant_code));

            CreateMap<List<PayOut.Create.Request>, SharedModelDTO.Models.LotBatch.LotBatchModel>().ConvertUsing((lRequest, LotBatch) =>
            {
                LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel()
                {
                    Transactions = new List<SharedModelDTO.Models.Transaction.TransactionModel>()
                };

                SharedModelDTO.Models.Transaction.TransactionModel TransactionToAdd;

                foreach (PayOut.Create.Request item in lRequest)
                {
                    TransactionToAdd = Mapper.Map<SharedModelDTO.Models.Transaction.TransactionModel>(item);
                    TransactionToAdd.TransactionRecipientDetail = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel>(item);
                    TransactionToAdd.TransactionCustomerInformation = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>(item);
                    TransactionToAdd.TransactionSubMerchantDetail = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel>(item);
                    LotBatch.Transactions.Add(TransactionToAdd);
                }

                return LotBatch;
            });
            #endregion

            #region POST DTO-API
            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOut.Create.Response>()
                .ForMember(to => to.bank_cbu, from => from.MapFrom(x => x.CBU))
                .ForMember(to => to.payout_date, from => from.MapFrom(x => x.TransactionAcreditationDate))
                .ForMember(to => to.beneficiary_cuit, from => from.MapFrom(x => x.RecipientCUIT))
                .ForMember(to => to.currency, from => from.MapFrom(x => x.CurrencyType))
                .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.Recipient))
                .ForMember(to => to.beneficiary_phone_number, from => from.MapFrom(x => x.RecipientPhoneNumber))
                .ForMember(to => to.merchant_id, from => from.MapFrom(x => x.InternalDescription))
                .ForMember(to => to.bank_account_type, from => from.MapFrom(x => x.BankAccountType))
                .ForMember(to => to.concept_code, from => from.MapFrom(x => Convert.ToInt32(x.ConceptCode)));

            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation, PayOut.Create.Response>()
                .ForMember(to => to.beneficiary_address, from => from.MapFrom(x => x.Address))
                .ForMember(to => to.beneficiary_birth_date, from => from.MapFrom(x => x.BirthDate))
                .ForMember(to => to.beneficiary_state, from => from.MapFrom(x => x.City))
                .ForMember(to => to.beneficiary_country, from => from.MapFrom(x => x.Country))
                .ForMember(to => to.beneficiary_email, from => from.MapFrom(x => x.Email))
                .ForMember(to => to.beneficiary_zip_code, from => from.MapFrom(x => x.ZipCode))
                .ForMember(to => to.sender_phone_number, from => from.MapFrom(x => x.SenderPhoneNumber))
                .ForMember(to => to.sender_name, from => from.MapFrom(x => x.SenderName))
                .ForMember(to => to.sender_country, from => from.MapFrom(x => x.SenderCountry))
                .ForMember(to => to.sender_state, from => from.MapFrom(x => x.SenderState))
                .ForMember(to => to.sender_address, from => from.MapFrom(x => x.SenderAddress))
                .ForMember(to => to.sender_taxid, from => from.MapFrom(x => x.SenderTaxid))
                .ForMember(to => to.sender_birthdate, from => from.MapFrom(x => x.SenderBirthDate))
                .ForMember(to => to.sender_email, from => from.MapFrom(x => x.SenderEmail))
                .ForMember(to => to.sender_zip_code, from => from.MapFrom(x => x.SenderZipCode));

            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOut.Create.Response>()
                .ForMember(to => to.submerchant_code, from => from.MapFrom(x => x.SubMerchantIdentification));

            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, List<PayOut.Create.Response>>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new List<PayOut.Create.Response>();

                PayOut.Create.Response ResponseTransaction = new PayOut.Create.Response();

                foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LotBatch.Transactions)
                {
                    ResponseTransaction = Mapper.Map<PayOut.Create.Response>(item.TransactionCustomerInformation);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOut.Create.Response>(item.TransactionRecipientDetail, ResponseTransaction);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOut.Create.Response>(item.TransactionSubMerchantDetail, ResponseTransaction);
                    ResponseTransaction.amount = item.Value;
                    ResponseTransaction.transaction_id = item.idTransaction;
                    ResponseTransaction.payout_id = item.idTransactionLot;
                    ResponseTransaction.status = item.Status;
                    ResponseTransaction.exchange_rate = item.TransactionDetail.ExchangeRate;
                    ResponseTransaction.withholding_tax_afip = item.TransactionDetail.TaxWithholdings_Afip;
                    ResponseTransaction.withholding_tax_agip = item.TransactionDetail.TaxWithholdings_Agip;
                    ResponseTransaction.withholding_tax_arba = item.TransactionDetail.TaxWithholdings_Arba;
                    ResponseTransaction.Ticket = item.Ticket;


                    if (item.StatusObservation == "ERROR::NF")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The submerchant_code not found in database.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_NOTFOUND");
                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "submerchant_code", Messages = messages, CodeTypeError = codeTypeError });
                        //ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_CREATE_NOTFOUND", Messages = messages });
                    }
                    else if (item.StatusObservation == "ERROR::NUNIQUE")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The merchant_id specified already exist in the same request, must be unique.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_NOTUNIQUE");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "merchant_id", Messages = messages, CodeTypeError = codeTypeError });
                    }
                    else if (item.StatusObservation == "ERROR::AEXIST")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The merchant_id specified already exist in the system, must be unique.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_ALREADYEXIST");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "merchant_id", Messages = messages, CodeTypeError = codeTypeError });
                    }

                    lRequest.Add(ResponseTransaction);
                }

                return lRequest;
            });
            #endregion

            #endregion

            #region PUT API-DTO|DTO-API

            #region PUT API-DTO
            CreateMap<PayOut.Update.Request, SharedModelDTO.Models.Transaction.TransactionModel>()
            .ForMember(to => to.Value, from => from.MapFrom(x => x.amount))
            .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")))
            .ForMember(to => to.idTransactionLot, from => from.MapFrom(x => x.payout_id))
            .ForMember(to => to.idTransaction, from => from.MapFrom(x => x.transaction_id));

            CreateMap<PayOut.Update.Request, SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel>()
            .ForMember(to => to.CBU, from => from.MapFrom(x => x.bank_cbu))
            .ForMember(to => to.TransactionAcreditationDate, from => from.MapFrom(x => x.payout_date))
            .ForMember(to => to.RecipientCUIT, from => from.MapFrom(x => x.beneficiary_cuit))
            .ForMember(to => to.CurrencyType, from => from.MapFrom(x => x.currency))
            .ForMember(to => to.Recipient, from => from.MapFrom(x => x.beneficiary_name))
            .ForMember(to => to.InternalDescription, from => from.MapFrom(x => x.merchant_id))
            .ForMember(to => to.BankAccountType, from => from.MapFrom(x => x.bank_account_type))
            .ForMember(to => to.ConceptCode, from => from.MapFrom(x => x.concept_code));

            CreateMap<PayOut.Update.Request, SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>()
                .ForMember(to => to.Address, from => from.MapFrom(x => x.beneficiary_address))
                .ForMember(to => to.BirthDate, from => from.MapFrom(x => x.beneficiary_birth_date))
                .ForMember(to => to.City, from => from.MapFrom(x => x.beneficiary_state))
                .ForMember(to => to.Country, from => from.MapFrom(x => x.beneficiary_country))
                .ForMember(to => to.Email, from => from.MapFrom(x => x.beneficiary_email))
                .ForMember(to => to.SenderName, from => from.MapFrom(x => x.sender_name))
                .ForMember(to => to.SenderCountry, from => from.MapFrom(x => x.sender_country))
                .ForMember(to => to.SenderState, from => from.MapFrom(x => x.sender_state))
                .ForMember(to => to.SenderAddress, from => from.MapFrom(x => x.sender_address))
                .ForMember(to => to.SenderTaxid, from => from.MapFrom(x => x.sender_taxid))
                .ForMember(to => to.SenderBirthDate, from => from.MapFrom(x => x.sender_birthdate))
                .ForMember(to => to.SenderEmail, from => from.MapFrom(x => x.sender_email));

            CreateMap<PayOut.Update.Request, SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel>()
                .ForMember(to => to.SubMerchantIdentification, from => from.MapFrom(x => x.submerchant_code));

            CreateMap<List<PayOut.Update.Request>, SharedModelDTO.Models.LotBatch.LotBatchModel>().ConvertUsing((lRequest, LotBatch) =>
            {
                LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel()
                {
                    Transactions = new List<SharedModelDTO.Models.Transaction.TransactionModel>()
                };

                SharedModelDTO.Models.Transaction.TransactionModel TransactionToAdd;

                foreach (PayOut.Update.Request item in lRequest)
                {
                    TransactionToAdd = Mapper.Map<SharedModelDTO.Models.Transaction.TransactionModel>(item);
                    TransactionToAdd.TransactionRecipientDetail = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel>(item);
                    TransactionToAdd.TransactionCustomerInformation = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>(item);
                    TransactionToAdd.TransactionSubMerchantDetail = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel>(item);
                    LotBatch.Transactions.Add(TransactionToAdd);
                }

                return LotBatch;
            });
            #endregion

            #region DTO-API
            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOut.Update.Response>()
            .ForMember(to => to.bank_cbu, from => from.MapFrom(x => x.CBU))
            .ForMember(to => to.payout_date, from => from.MapFrom(x => x.TransactionAcreditationDate))
            .ForMember(to => to.beneficiary_cuit, from => from.MapFrom(x => x.RecipientCUIT))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.CurrencyType))
            .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.Recipient))
            .ForMember(to => to.merchant_id, from => from.MapFrom(x => x.InternalDescription))
            .ForMember(to => to.bank_account_type, from => from.MapFrom(x => x.BankAccountType))
            .ForMember(to => to.concept_code, from => from.MapFrom(x => Convert.ToInt32(x.ConceptCode)));

            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation, PayOut.Update.Response>()
                .ForMember(to => to.beneficiary_address, from => from.MapFrom(x => x.Address))
                .ForMember(to => to.beneficiary_birth_date, from => from.MapFrom(x => x.BirthDate))
                .ForMember(to => to.beneficiary_state, from => from.MapFrom(x => x.City))
                .ForMember(to => to.beneficiary_country, from => from.MapFrom(x => x.Country))
                .ForMember(to => to.beneficiary_email, from => from.MapFrom(x => x.Email))
                .ForMember(to => to.sender_name, from => from.MapFrom(x => x.SenderName))
                .ForMember(to => to.sender_country, from => from.MapFrom(x => x.SenderCountry))
                .ForMember(to => to.sender_state, from => from.MapFrom(x => x.SenderState))
                .ForMember(to => to.sender_address, from => from.MapFrom(x => x.SenderAddress))
                .ForMember(to => to.sender_taxid, from => from.MapFrom(x => x.SenderTaxid))
                .ForMember(to => to.sender_birthdate, from => from.MapFrom(x => x.SenderBirthDate))
                .ForMember(to => to.sender_email, from => from.MapFrom(x => x.SenderEmail));


            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOut.Update.Response>()
                .ForMember(to => to.submerchant_code, from => from.MapFrom(x => x.SubMerchantIdentification));

            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, List<PayOut.Update.Response>>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new List<PayOut.Update.Response>();

                PayOut.Update.Response ResponseTransaction = new PayOut.Update.Response();

                foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LotBatch.Transactions)
                {
                    //ResponseTransaction = Mapper.Map<PayOut.Update.Response>(item.TransactionRecipientDetail);
                    ResponseTransaction = Mapper.Map<PayOut.Update.Response>(item.TransactionCustomerInformation);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOut.Update.Response>(item.TransactionRecipientDetail, ResponseTransaction);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOut.Update.Response>(item.TransactionSubMerchantDetail, ResponseTransaction);
                    ResponseTransaction.amount = item.Value;
                    ResponseTransaction.transaction_id = item.idTransaction;
                    ResponseTransaction.payout_id = item.idTransactionLot;
                    ResponseTransaction.status = item.Status;

                    if(item.StatusObservation == "ERROR::STATUS")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The current state is InProgress/Canceled/Excecuted, therefore it can not be modified.");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_UPDATE_STATUS", Messages = messages });
                    }
                    else if (item.StatusObservation == "ERROR::NF")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The payout_id/transaction_id not found in database.");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_UPDATE_NOTFOUND", Messages = messages });
                    }
                    else if (item.StatusObservation == "ERROR::NOTMATCH")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The merchant_id or transaction_id specified not match.");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_CREATE_ALREADYEXIST", Messages = messages });
                    }

                    lRequest.Add(ResponseTransaction);
                }

                return lRequest;
            });
            #endregion

            #endregion

            #region DELETE API-DTO|DTO-API

            #region DELETE API-DTO
            CreateMap<PayOut.Delete.Request, SharedModelDTO.Models.Transaction.TransactionModel>()
            .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")))
            .ForMember(to => to.idTransactionLot, from => from.MapFrom(x => x.payout_id))
            .ForMember(to => to.idTransaction, from => from.MapFrom(x => x.transaction_id));

            CreateMap<List<PayOut.Delete.Request>, SharedModelDTO.Models.LotBatch.LotBatchModel>().ConvertUsing((lRequest, LotBatch) =>
            {
                LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel()
                {
                    Transactions = new List<SharedModelDTO.Models.Transaction.TransactionModel>()
                };

                SharedModelDTO.Models.Transaction.TransactionModel TransactionToAdd;

                foreach (PayOut.Delete.Request item in lRequest)
                {
                    TransactionToAdd = Mapper.Map<SharedModelDTO.Models.Transaction.TransactionModel>(item);
                    LotBatch.Transactions.Add(TransactionToAdd);
                }

                return LotBatch;
            });
            #endregion

            #region DELETE DTO-API
            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, List<PayOut.Delete.Response>>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new List<PayOut.Delete.Response>();

                PayOut.Delete.Response ResponseTransaction;

                foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LotBatch.Transactions)
                {
                    ResponseTransaction = new PayOut.Delete.Response();

                    ResponseTransaction.transaction_id = item.idTransaction;
                    ResponseTransaction.payout_id = item.idTransactionLot;
                    ResponseTransaction.status = item.Status;

                    if (item.StatusObservation == "ERROR::STATUS")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The current state is InProgress/Canceled/Excecuted, therefore it can not be modified.");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_UPDATE_STATUS", Messages = messages });
                    }
                    else if (item.StatusObservation == "ERROR::NF")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The payout_id/transaction_id not found in database.");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_UPDATE_NOTFOUND", Messages = messages });
                    }

                    lRequest.Add(ResponseTransaction);
                }

                return lRequest;
            });

            CreateMap<List<SharedModelDTO.Models.LotBatch.LotBatchModel>, List<PayOut.Delete.Response>>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new List<PayOut.Delete.Response>();

                PayOut.Delete.Response ResponseTransaction;

                foreach (SharedModelDTO.Models.LotBatch.LotBatchModel LT in LotBatch)
                {
                    foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LT.Transactions)
                    {
                        ResponseTransaction = new PayOut.Delete.Response();

                        ResponseTransaction.transaction_id = item.idTransaction;
                        ResponseTransaction.payout_id = item.idTransactionLot;
                        ResponseTransaction.status = item.Status;

                        if (item.StatusObservation == "ERROR::STATUS")
                        {
                            List<string> messages = new List<string>();
                            messages.Add("The current state is InProgress/Canceled/Excecuted, therefore it can not be modified.");

                            ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_UPDATE_STATUS", Messages = messages });
                        }
                        else if (item.StatusObservation == "ERROR::NF")
                        {
                            List<string> messages = new List<string>();
                            messages.Add("The payout_id/transaction_id not found in database.");

                            ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_UPDATE_NOTFOUND", Messages = messages });
                        }

                        lRequest.Add(ResponseTransaction);
                    }
                }

                return lRequest;
            });
            #endregion

            #endregion

            #region LIST API-DTO|DTO-API

            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, PayOut.LotBatch>()
            .ForMember(to => to.payout_id, from => from.MapFrom(x => x.idTransactionLot))
            .ForMember(to => to.lot_date, from => from.MapFrom(x => x.LotDate))
            .ForMember(to => to.customer_name, from => from.MapFrom(x => x.CustomerName))
            .ForMember(to => to.transaction_type, from => from.MapFrom(x => x.TransactionType))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            //.ForMember(to => to.account_balance, from => from.MapFrom(x => x.Balance))
            .ForMember(to => to.gross_amount, from => from.MapFrom(x => x.GrossAmount))
            .ForMember(to => to.net_amount, from => from.MapFrom(x => x.NetAmount))
            .ForMember(to => to.transaction_list, from => from.MapFrom(x => x.Transactions));

            CreateMap<SharedModelDTO.Models.Transaction.TransactionModel, PayOut.List.Response>()
            .ForMember(to => to.transaction_id, from => from.MapFrom(x => x.idTransaction))
            .ForMember(to => to.merchant_id, from => from.MapFrom(x => x.TransactionRecipientDetail.InternalDescription))
            .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.TransactionRecipientDetail.Recipient))
            .ForMember(to => to.type_of_id, from => from.MapFrom(x => x.TransactionRecipientDetail.EntityIdentificationType))
            .ForMember(to => to.beneficiary_cuit, from => from.MapFrom(x => x.TransactionRecipientDetail.RecipientCUIT))
            .ForMember(to => to.beneficiary_phone_number, from => from.MapFrom(x => x.TransactionRecipientDetail.RecipientPhoneNumber))
            .ForMember(to => to.bank_account_type, from => from.MapFrom(x => x.TransactionRecipientDetail.BankAccountType))
            //.ForMember(to => to.bank_code, from => from.MapFrom(x => x.TransactionRecipientDetail.BankCode))
            .ForMember(to => to.bank_account, from => from.MapFrom(x => x.TransactionRecipientDetail.RecipientAccountNumber))
            .ForMember(to => to.bank_cbu, from => from.MapFrom(x => x.TransactionRecipientDetail.CBU))
            .ForMember(to => to.amount, from => from.MapFrom(x => x.Value))
            .ForMember(to => to.transaction_date, from => from.MapFrom(x => x.TransactionRecipientDetail.TransactionAcreditationDate))
            //.ForMember(to => to.beneficiary_softd, from => from.MapFrom(x => x.TransactionRecipientDetail.Description))
            .ForMember(to => to.concept_code, from => from.MapFrom(x => x.TransactionRecipientDetail.ConceptCode))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.TransactionRecipientDetail.CurrencyType))
            //.ForMember(to => to.payment_type, from => from.MapFrom(x => x.TransactionRecipientDetail.PaymentType))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            .ForMember(to => to.submerchant_code, from => from.MapFrom(x => x.TransactionSubMerchantDetail.SubMerchantIdentification))
            .ForMember(to => to.exchange_rate, from => from.MapFrom(x=>x.TransactionDetail.ExchangeRate))
            .ForMember(to => to.withholding_tax_afip, from => from.MapFrom(x => x.TransactionDetail.TaxWithholdings_Afip))
            .ForMember(to => to.withholding_tax_agip, from => from.MapFrom(x => x.TransactionDetail.TaxWithholdings_Agip))
            .ForMember(to => to.withholding_tax_arba, from => from.MapFrom(x => x.TransactionDetail.TaxWithholdings_Arba))
            .ForMember(to => to.sender_name, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderName : null))
            .ForMember(to => to.sender_address, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderAddress : null))
            .ForMember(to => to.sender_birthdate, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderBirthDate : null))
            .ForMember(to => to.sender_country, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderCountry : null))
            .ForMember(to => to.sender_state, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderState : null))
            .ForMember(to => to.sender_email, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderEmail : null))
            .ForMember(to => to.sender_taxid, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderTaxid : null))
            .ForMember(to => to.sender_phone_number, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderPhoneNumber : null));

            //CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOut.List.Response>()
            //    .ForMember(to => to.subclient_code, from => from.MapFrom(x => x.SubMerchantIdentification));

            CreateMap<List<SharedModelDTO.Models.LotBatch.LotBatchModel>, List<PayOut.LotBatch>>().ConvertUsing((LotBatch, ResponseLotBat) =>
            {             
                List<PayOut.LotBatch> ResultLotBat =  new List<PayOut.LotBatch>();

                PayOut.List.Response ResponseTrans;

                foreach (SharedModelDTO.Models.LotBatch.LotBatchModel Lot in LotBatch) {
                    foreach (SharedModelDTO.Models.Transaction.TransactionModel item in Lot.Transactions)
                    {
                        ResponseTrans = Mapper.Map<PayOut.List.Response>(item);
                        //ResponseTrans = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOut.List.Response>(item.TransactionSubMerchantDetail, ResponseTrans);                                                
                    }
                    ResultLotBat.Add(Mapper.Map<PayOut.LotBatch>(Lot));
                }

                return ResultLotBat;
            });

            #endregion
        }
    }
}
