using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Services.Colombia.Banks.Bancolombia;
using AutoMapper;

namespace SharedMaps.Maps.Services.Colombia.Banks.Bancolombia
{
    public class PayOutMappingColombia : Profile
    {
        public PayOutMappingColombia()
        {
            #region POST API-DTO|DTO-API

            #region POST API-DTO
            CreateMap<PayOutColombia.Create.Request, SharedModelDTO.Models.Transaction.TransactionModel>()
                .ForMember(to => to.Value, from => from.MapFrom(x => x.amount))
                .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")));

            CreateMap<PayOutColombia.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel>()
                .ForMember(to => to.IdType, from => from.MapFrom(x => x.type_of_id))
                .ForMember(to => to.Id, from => from.MapFrom(x => x.id))
                .ForMember(to => to.Recipient, from => from.MapFrom(x => x.beneficiary_name))
                .ForMember(to => to.BankAccountType, from => from.MapFrom(x => x.account_type))
                .ForMember(to => to.BankCode, from => from.MapFrom(x => x.bank_code))
                .ForMember(to => to.RecipientAccountNumber, from => from.MapFrom(x => x.beneficiary_account_number))
                .ForMember(to => to.InternalDescription, from => from.MapFrom(x => x.merchant_id))
                .ForMember(to => to.CurrencyType, from => from.MapFrom(x => x.currency))
                .ForMember(to => to.TransactionAcreditationDate, from => from.MapFrom(x => x.payout_date))
                .ForMember(to => to.RecipientPhoneNumber, from => from.MapFrom(x => x.beneficiary_phone_number))
                .ForMember(to => to.ConceptCode, from => from.MapFrom(x => x.concept_code));
            //.ForMember(to => to.CBU, from => from.MapFrom(x => x.bank_cbu))
            //.ForMember(to => to.Description, from => from.MapFrom(x => x.beneficiary_softd))
            //.ForMember(to => to.ConceptCode, from => from.MapFrom(x => x.concept_code));

            CreateMap<PayOutColombia.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>()
                .ForMember(to => to.Email, from => from.MapFrom(x => x.beneficiary_email))
                .ForMember(to => to.BirthDate, from => from.MapFrom(x => x.beneficiary_birth_date))
                .ForMember(to => to.Address, from => from.MapFrom(x => x.beneficiary_address))
                .ForMember(to => to.Country, from => from.MapFrom(x => x.beneficiary_country))
                .ForMember(to => to.City, from => from.MapFrom(x => x.beneficiary_state))
                .ForMember(to => to.SenderName, from => from.MapFrom(x => x.sender_name))
                .ForMember(to => to.SenderCountry, from => from.MapFrom(x => x.sender_country))
                .ForMember(to => to.SenderState, from => from.MapFrom(x => x.sender_state))
                .ForMember(to => to.SenderAddress, from => from.MapFrom(x => x.sender_address))
                .ForMember(to => to.SenderTaxid, from => from.MapFrom(x => x.sender_taxid))
                .ForMember(to => to.SenderBirthDate, from => from.MapFrom(x => x.sender_birthdate))
                .ForMember(to => to.SenderPhoneNumber, from => from.MapFrom(x => x.sender_phone_number))
                .ForMember(to => to.SenderEmail, from => from.MapFrom(x => x.sender_email))
                .ForMember(to => to.ZipCode, from => from.MapFrom(x => x.beneficiary_zip_code))
                .ForMember(to => to.SenderZipCode, from => from.MapFrom(x => x.sender_zip_code));

            CreateMap<PayOutColombia.List.Response, SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>()
                //.ForMember(to => to.Email, from => from.MapFrom(x => x.beneficiary_email))
                //.ForMember(to => to.BirthDate, from => from.MapFrom(x => x.beneficiary_birth_date))
                //.ForMember(to => to.Address, from => from.MapFrom(x => x.beneficiary_address))
                //.ForMember(to => to.Country, from => from.MapFrom(x => x.beneficiary_country))
                //.ForMember(to => to.City, from => from.MapFrom(x => x.beneficiary_state))
                .ForMember(to => to.SenderName, from => from.MapFrom(x => x.sender_name))
                .ForMember(to => to.SenderCountry, from => from.MapFrom(x => x.sender_country))
                .ForMember(to => to.SenderState, from => from.MapFrom(x => x.sender_state))
                .ForMember(to => to.SenderAddress, from => from.MapFrom(x => x.sender_address))
                .ForMember(to => to.SenderTaxid, from => from.MapFrom(x => x.sender_taxid))
                .ForMember(to => to.SenderBirthDate, from => from.MapFrom(x => x.sender_birthdate))
                .ForMember(to => to.SenderPhoneNumber, from => from.MapFrom(x => x.sender_phone_number))
                .ForMember(to => to.SenderEmail, from => from.MapFrom(x => x.sender_email));


            //.ForMember(to => to.Annotation, from => from.MapFrom(x => x.annotation))
            //.ForMember(to => to.BirthDate, from => from.MapFrom(x => x.birth_date))


            CreateMap<PayOutColombia.Create.Request, SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel>()
                .ForMember(to => to.SubMerchantIdentification, from => from.MapFrom(x => x.submerchant_code));

            CreateMap<List<PayOutColombia.Create.Request>, SharedModelDTO.Models.LotBatch.LotBatchModel>().ConvertUsing((lRequest, LotBatch) =>
            {
                LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel()
                {
                    Transactions = new List<SharedModelDTO.Models.Transaction.TransactionModel>()
                };

                SharedModelDTO.Models.Transaction.TransactionModel TransactionToAdd;

                foreach (PayOutColombia.Create.Request item in lRequest)
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
            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.Create.Response>()
                .ForMember(to => to.type_of_id, from => from.MapFrom(x => x.IdType))
                .ForMember(to => to.id, from => from.MapFrom(x => x.Id))
                .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.Recipient))
                .ForMember(to => to.account_type, from => from.MapFrom(x => x.BankAccountType))
                .ForMember(to => to.bank_code, from => from.MapFrom(x => x.BankCode))
                .ForMember(to => to.beneficiary_account_number, from => from.MapFrom(x => x.RecipientAccountNumber))
                .ForMember(to => to.merchant_id, from => from.MapFrom(x => x.InternalDescription))
                .ForMember(to => to.currency, from => from.MapFrom(x => x.CurrencyType))
                .ForMember(to => to.beneficiary_phone_number, from => from.MapFrom(x => x.RecipientPhoneNumber))
                .ForMember(to => to.concept_code, from => from.MapFrom(x => x.ConceptCode));

            /*CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.List.Response>()
                .IncludeBase<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.Create.Response>();
            .ForMember(to => to.type_of_id, from => from.MapFrom(x => x.IdType))
            .ForMember(to => to.id, from => from.MapFrom(x => x.Id))
            .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.Recipient))
            .ForMember(to => to.account_type, from => from.MapFrom(x => x.BankAccountType))
            .ForMember(to => to.bank_code, from => from.MapFrom(x => x.BankCode))
            .ForMember(to => to.beneficiary_account_number, from => from.MapFrom(x => x.RecipientAccountNumber))
            .ForMember(to => to.merchant_id, from => from.MapFrom(x => x.InternalDescription))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.CurrencyType))
            .ForMember(to => to.concept_code, from => from.MapFrom(x => x.ConceptCode));
            //.ForMember(to => to.bank_cbu, from => from.MapFrom(x => x.CBU))
            //.ForMember(to => to.bank_cbu, from => from.MapFrom(x => x.CBU))
            //.ForMember(to => to.payout_date, from => from.MapFrom(x => x.TransactionAcreditationDate))
            //.ForMember(to => to.beneficiary_softd, from => from.MapFrom(x => x.Description))
            //.ForMember(to => to.concept_code, from => from.MapFrom(x => x.ConceptCode));*/


            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation, PayOutColombia.Create.Response>()
                .ForMember(to => to.beneficiary_email, from => from.MapFrom(x => x.Email))
                .ForMember(to => to.beneficiary_birth_date, from => from.MapFrom(x => x.BirthDate))
                .ForMember(to => to.beneficiary_address, from => from.MapFrom(x => x.Address))
                .ForMember(to => to.beneficiary_country, from => from.MapFrom(x => x.Country))
                .ForMember(to => to.beneficiary_state, from => from.MapFrom(x => x.City))
                .ForMember(to => to.sender_name, from => from.MapFrom(x => x.SenderName))
                .ForMember(to => to.sender_country, from => from.MapFrom(x => x.SenderCountry))
                .ForMember(to => to.sender_state, from => from.MapFrom(x => x.SenderState))
                .ForMember(to => to.sender_address, from => from.MapFrom(x => x.SenderAddress))
                .ForMember(to => to.sender_taxid, from => from.MapFrom(x => x.SenderTaxid))
                .ForMember(to => to.sender_birthdate, from => from.MapFrom(x => x.SenderBirthDate))
                .ForMember(to => to.sender_phone_number, from => from.MapFrom(x => x.SenderPhoneNumber))
                .ForMember(to => to.sender_email, from => from.MapFrom(x => x.SenderEmail))
                .ForMember(to => to.beneficiary_zip_code, from => from.MapFrom(x => x.ZipCode))
                .ForMember(to => to.sender_zip_code, from => from.MapFrom(x => x.SenderZipCode));
            //.ForMember(to => to.annotation, from => from.MapFrom(x => x.Annotation))
            //.ForMember(to => to.birth_date, from => from.MapFrom(x => x.BirthDate))

            //CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation, PayOutColombia.List.Response>()
            //  .IncludeBase<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation, PayOutColombia.Create.Response>();

            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOutColombia.Create.Response>()
                .ForMember(to => to.submerchant_code, from => from.MapFrom(x => x.SubMerchantIdentification));

            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, List<PayOutColombia.Create.Response>>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new List<PayOutColombia.Create.Response>();

                PayOutColombia.Create.Response ResponseTransaction = new PayOutColombia.Create.Response();

                foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LotBatch.Transactions)
                {
                    ResponseTransaction = Mapper.Map<PayOutColombia.Create.Response>(item.TransactionCustomerInformation);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.Create.Response>(item.TransactionRecipientDetail, ResponseTransaction);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOutColombia.Create.Response>(item.TransactionSubMerchantDetail, ResponseTransaction);
                    ResponseTransaction.amount = item.Value;
                    ResponseTransaction.transaction_id = item.idTransaction;
                    ResponseTransaction.payout_id = item.idTransactionLot;
                    ResponseTransaction.exchange_rate = item.TransactionDetail.ExchangeRate;
                    ResponseTransaction.status = item.Status;
                    ResponseTransaction.Ticket = item.Ticket;
                    ResponseTransaction.gmf_tax = item.TransactionDetail.GmfTax;
                    //ResponseTransaction.withholding_tax = item.TransactionDetail.TaxWithholdings;

                    if (item.StatusObservation == "ERROR::NF")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The submerchant_code not found in database.");

                        List<string> codeTypeError = new List<string>();
                        codeTypeError.Add("ERROR_CREATE_NOTFOUND");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "submerchant_code", Messages = messages, CodeTypeError = codeTypeError });
                    }
                    else if (item.StatusObservation == "ERROR::NUNIQUE")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The merchant_id must be unique.");

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

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "merchant_id ", Messages = messages, CodeTypeError = codeTypeError });
                    }

                    lRequest.Add(ResponseTransaction);
                }

                return lRequest;
            });

            //CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, List<PayOutColombia.List.Response>>().ConvertUsing((LotBatch, lresponses) =>
            //{
            //    lresponses = new List<PayOutColombia.List.Response>();

            //    var ResponseTransaction = new PayOutColombia.List.Response();

            //    foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LotBatch.Transactions)
            //    {
            //        ResponseTransaction = Mapper.Map<PayOutColombia.List.Response>(item.TransactionCustomerInformation);
            //        ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.List.Response>(item.TransactionRecipientDetail, ResponseTransaction);
            //        ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOutColombia.List.Response>(item.TransactionSubMerchantDetail, ResponseTransaction);
            //        ResponseTransaction.amount = item.Value;
            //        ResponseTransaction.transaction_id = item.idTransaction;
            //        ResponseTransaction.payout_id = item.idTransactionLot;
            //        ResponseTransaction.exchange_rate = Convert.ToInt64(item.TransactionDetail.ExchangeRate.ToString().PadRight(8, '0'));
            //        ResponseTransaction.status = item.Status;
            //        ResponseTransaction.Ticket = item.Ticket;
            //        ResponseTransaction.gmf_tax = item.TransactionDetail.GmfTax;
            //        //ResponseTransaction.withholding_tax = item.TransactionDetail.TaxWithholdings;

            //        lresponses.Add(ResponseTransaction);
            //    }

            //    return lresponses;
            //});

            #endregion

            #endregion

            #region PUT API-DTO|DTO-API

            #region PUT API-DTO
            CreateMap<PayOutColombia.Update.Request, SharedModelDTO.Models.Transaction.TransactionModel>()
            .ForMember(to => to.Value, from => from.MapFrom(x => x.amount))
            .ForMember(to => to.TransactionDate, from => from.MapFrom(x => DateTime.Now.ToString("yyyyMMdd")))
            .ForMember(to => to.idTransactionLot, from => from.MapFrom(x => x.payout_id))
            .ForMember(to => to.idTransaction, from => from.MapFrom(x => x.transaction_id));

            CreateMap<PayOutColombia.Update.Request, SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel>()
             //.ForMember(to => to.CBU, from => from.MapFrom(x => x.bank_cbu))
             .ForMember(to => to.IdType, from => from.MapFrom(x => x.type_of_id))
             .ForMember(to => to.Id, from => from.MapFrom(x => x.id))
             .ForMember(to => to.Recipient, from => from.MapFrom(x => x.beneficiary_name))
             .ForMember(to => to.BankAccountType, from => from.MapFrom(x => x.account_type))
             .ForMember(to => to.BankCode, from => from.MapFrom(x => x.bank_code))
             .ForMember(to => to.RecipientAccountNumber, from => from.MapFrom(x => x.beneficiary_account_number))
             .ForMember(to => to.InternalDescription, from => from.MapFrom(x => x.site_transaction_id))
             .ForMember(to => to.CurrencyType, from => from.MapFrom(x => x.currency))
             .ForMember(to => to.TransactionAcreditationDate, from => from.MapFrom(x => x.payout_date))
             .ForMember(to => to.Description, from => from.MapFrom(x => x.beneficiary_softd))
             .ForMember(to => to.ConceptCode, from => from.MapFrom(x => x.concept_code));

            CreateMap<PayOutColombia.Update.Request, SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation>()
                .ForMember(to => to.Address, from => from.MapFrom(x => x.address))
                .ForMember(to => to.Annotation, from => from.MapFrom(x => x.annotation))
                .ForMember(to => to.BirthDate, from => from.MapFrom(x => x.birth_date))
                .ForMember(to => to.City, from => from.MapFrom(x => x.city))
                .ForMember(to => to.Country, from => from.MapFrom(x => x.country))
                .ForMember(to => to.Email, from => from.MapFrom(x => x.email))
                .ForMember(to => to.SenderName, from => from.MapFrom(x => x.sender_name))
                .ForMember(to => to.SenderCountry, from => from.MapFrom(x => x.sender_country))
                .ForMember(to => to.SenderState, from => from.MapFrom(x => x.sender_state))
                .ForMember(to => to.SenderAddress, from => from.MapFrom(x => x.sender_address))
                .ForMember(to => to.SenderTaxid, from => from.MapFrom(x => x.sender_taxid))
                .ForMember(to => to.SenderBirthDate, from => from.MapFrom(x => x.sender_birthdate))
                .ForMember(to => to.SenderEmail, from => from.MapFrom(x => x.sender_email));

            CreateMap<PayOutColombia.Update.Request, SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel>()
                .ForMember(to => to.SubMerchantIdentification, from => from.MapFrom(x => x.subclient_code));

            CreateMap<List<PayOutColombia.Update.Request>, SharedModelDTO.Models.LotBatch.LotBatchModel>().ConvertUsing((lRequest, LotBatch) =>
            {
                LotBatch = new SharedModelDTO.Models.LotBatch.LotBatchModel()
                {
                    Transactions = new List<SharedModelDTO.Models.Transaction.TransactionModel>()
                };

                SharedModelDTO.Models.Transaction.TransactionModel TransactionToAdd;

                foreach (PayOutColombia.Update.Request item in lRequest)
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
            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.Update.Response>()
            //.ForMember(to => to.a, from => from.MapFrom(x => x.CBU))
            .ForMember(to => to.payout_date, from => from.MapFrom(x => x.TransactionAcreditationDate))
            .ForMember(to => to.type_of_id, from => from.MapFrom(x => x.IdType))
            .ForMember(to => to.id, from => from.MapFrom(x => x.Id))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.CurrencyType))
            .ForMember(to => to.beneficiary_softd, from => from.MapFrom(x => x.Description))
            .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.Recipient))
            .ForMember(to => to.site_transaction_id, from => from.MapFrom(x => x.InternalDescription))
            .ForMember(to => to.bank_code, from => from.MapFrom(x => x.BankCode))
            .ForMember(to => to.beneficiary_account_number, from => from.MapFrom(x => x.RecipientAccountNumber))
            .ForMember(to => to.account_type, from => from.MapFrom(x => x.BankAccountType));
            //.ForMember(to => to.concept_code, from => from.MapFrom(x => Convert.ToInt32(x.ConceptCode)));

            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionCustomerInformation, PayOutColombia.Update.Response>()
                .ForMember(to => to.address, from => from.MapFrom(x => x.Address))
                .ForMember(to => to.annotation, from => from.MapFrom(x => x.Annotation))
                .ForMember(to => to.birth_date, from => from.MapFrom(x => x.BirthDate))
                .ForMember(to => to.city, from => from.MapFrom(x => x.City))
                .ForMember(to => to.country, from => from.MapFrom(x => x.Country))
                .ForMember(to => to.email, from => from.MapFrom(x => x.Email));

            CreateMap<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOutColombia.Update.Response>()
                .ForMember(to => to.subclient_code, from => from.MapFrom(x => x.SubMerchantIdentification));

            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, PayOutColombia.LotBatchColombia>()
            .ForMember(to => to.payout_id, from => from.MapFrom(x => x.idTransactionLot))
            .ForMember(to => to.lot_date, from => from.MapFrom(x => x.LotDate))
            .ForMember(to => to.customer_name, from => from.MapFrom(x => x.CustomerName))
            .ForMember(to => to.transaction_type, from => from.MapFrom(x => x.TransactionType))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            //.ForMember(to => to.account_balance, from => from.MapFrom(x => x.Balance))
            .ForMember(to => to.gross_amount, from => from.MapFrom(x => x.GrossAmount))
            .ForMember(to => to.net_amount, from => from.MapFrom(x => x.NetAmount))
            .ForMember(to => to.transaction_list, from => from.MapFrom(x => x.Transactions));

        //    CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, PayOutColombia.List.Response>().IncludeBase<SharedModelDTO.Models.LotBatch.LotBatchModel, PayOutColombia.Create.Response>();
           // CreateMap<SharedModelDTO.Models.Transaction.TransactionModel, PayOutColombia.List.Response>().IncludeBase<SharedModelDTO.Models.Transaction.TransactionModel, PayOutColombia.Create.Response>();    
                /*.ForMember(to => to.payout_id, from => from.MapFrom(x=>x.idTransactionLot))
            .ForMember(to => to.Ticket, from => from.MapFrom(x=>x.Ticket))
            .ForMember(to => to.transaction_id, from => from.MapFrom(x => x.idTransaction))
            .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.TransactionRecipientDetail.Recipient))
            .ForMember(to => to.type_of_id, from => from.MapFrom(x => x.TransactionRecipientDetail.EntityIdentificationType))
            .ForMember(to => to.bank_code, from => from.MapFrom(x => x.TransactionRecipientDetail.BankCode))
            .ForMember(to => to.amount, from => from.MapFrom(x => x.Value))
            .ForMember(to => to.concept_code, from => from.MapFrom(x => x.TransactionRecipientDetail.ConceptCode))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.TransactionRecipientDetail.CurrencyType))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            .ForMember(to => to.submerchant_code, from => from.MapFrom(x => x.TransactionSubMerchantDetail.SubMerchantIdentification))
            .ForMember(to => to.exchange_rate, from => from.MapFrom(x => x.TransactionDetail.ExchangeRate))
            .ForMember(to => to.sender_name, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderName : null))
            .ForMember(to => to.sender_address, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderAddress : null))
            .ForMember(to => to.sender_birthdate, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderBirthDate : null))
            .ForMember(to => to.sender_country, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderCountry : null))
            .ForMember(to => to.sender_state, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderState : null))
            .ForMember(to => to.sender_email, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderEmail : null))
            .ForMember(to => to.sender_taxid, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderTaxid : null));
            */

            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, List<PayOutColombia.Update.Response>>().ConvertUsing((LotBatch, lRequest) =>
            {
                lRequest = new List<PayOutColombia.Update.Response>();

                PayOutColombia.Update.Response ResponseTransaction = new PayOutColombia.Update.Response();

                foreach (SharedModelDTO.Models.Transaction.TransactionModel item in LotBatch.Transactions)
                {
                    ResponseTransaction = Mapper.Map<PayOutColombia.Update.Response>(item.TransactionRecipientDetail);
                    ResponseTransaction = Mapper.Map<PayOutColombia.Update.Response>(item.TransactionCustomerInformation);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionRecipientDetailModel, PayOutColombia.Update.Response>(item.TransactionRecipientDetail, ResponseTransaction);
                    ResponseTransaction = Mapper.Map<SharedModelDTO.Models.Transaction.Detail.TransactionSubMerchantDetailModel, PayOutColombia.Update.Response>(item.TransactionSubMerchantDetail, ResponseTransaction);
                    ResponseTransaction.amount = item.Value;
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
                    else if (item.StatusObservation == "ERROR::NOTMATCH")
                    {
                        List<string> messages = new List<string>();
                        messages.Add("The site_transaction_id and transaction_id specified not match.");

                        ResponseTransaction.ErrorRow.Errors.Add(new SharedModel.Models.General.ErrorModel.ValidationErrorGroup { Key = "ERROR_CREATE_ALREADYEXIST", Messages = messages });
                    }

                    lRequest.Add(ResponseTransaction);
                }

                return lRequest;
            });
            #endregion

            #endregion

            #region LIST API-DTO|DTO-API
            CreateMap<SharedModelDTO.Models.LotBatch.LotBatchModel, SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.LotBatchColombia>()
            .ForMember(to => to.payout_id, from => from.MapFrom(x => x.idTransactionLot))
            .ForMember(to => to.lot_date, from => from.MapFrom(x => x.LotDate))
            .ForMember(to => to.customer_name, from => from.MapFrom(x => x.CustomerName))
            .ForMember(to => to.transaction_type, from => from.MapFrom(x => x.TransactionType))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            //.ForMember(to => to.account_balance, from => from.MapFrom(x => x.Balance))
            .ForMember(to => to.gross_amount, from => from.MapFrom(x => x.GrossAmount))
            .ForMember(to => to.net_amount, from => from.MapFrom(x => x.NetAmount))
            .ForMember(to => to.transaction_list, from => from.MapFrom(x => x.Transactions));

            CreateMap < SharedModelDTO.Models.Transaction.TransactionModel, SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.List.Response>()
            .ForMember(to => to.transaction_id, from => from.MapFrom(x => x.idTransaction))
            .ForMember(to => to.merchant_id, from => from.MapFrom(x => x.TransactionRecipientDetail.InternalDescription))
            .ForMember(to => to.beneficiary_name, from => from.MapFrom(x => x.TransactionRecipientDetail.Recipient))
            .ForMember(to => to.type_of_id, from => from.MapFrom(x => x.TransactionRecipientDetail.EntityIdentificationType))
            .ForMember(to => to.id, from => from.MapFrom(x => x.TransactionRecipientDetail.RecipientCUIT))
            .ForMember(to => to.bank_account_type, from => from.MapFrom(x => x.TransactionRecipientDetail.BankAccountType))
            //.ForMember(to => to.bank_code, from => from.MapFrom(x => x.TransactionRecipientDetail.BankCode))
            .ForMember(to => to.bank_account, from => from.MapFrom(x => x.TransactionRecipientDetail.RecipientAccountNumber))
            .ForMember(to => to.bank_code, from => from.MapFrom(x => x.TransactionRecipientDetail.BankCode))
            .ForMember(to => to.amount, from => from.MapFrom(x => x.Value))
            .ForMember(to => to.transaction_date, from => from.MapFrom(x => x.TransactionRecipientDetail.TransactionAcreditationDate))
            //.ForMember(to => to.beneficiary_softd, from => from.MapFrom(x => x.TransactionRecipientDetail.Description))
            .ForMember(to => to.concept_code, from => from.MapFrom(x => x.TransactionRecipientDetail.ConceptCode))
            .ForMember(to => to.currency, from => from.MapFrom(x => x.TransactionRecipientDetail.CurrencyType))
            //.ForMember(to => to.payment_type, from => from.MapFrom(x => x.TransactionRecipientDetail.PaymentType))
            .ForMember(to => to.status, from => from.MapFrom(x => x.Status))
            .ForMember(to => to.submerchant_code, from => from.MapFrom(x => x.TransactionSubMerchantDetail.SubMerchantIdentification))
            .ForMember(to => to.exchange_rate, from => from.MapFrom(x => x.TransactionDetail.ExchangeRate))
            .ForMember(to => to.gmf_tax, from => from.MapFrom(x => x.TransactionDetail.GmfTax))
            .ForMember(to => to.sender_name, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderName : null))
            .ForMember(to => to.sender_address, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderAddress : null))
            .ForMember(to => to.sender_birthdate, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderBirthDate : null))
            .ForMember(to => to.sender_country, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderCountry : null))
            .ForMember(to => to.sender_state, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderState : null))
            .ForMember(to => to.sender_email, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderEmail : null))
            .ForMember(to => to.sender_taxid, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderTaxid : null))
            .ForMember(to => to.beneficiary_phone_number, from => from.MapFrom(x => x.TransactionRecipientDetail != null ? x.TransactionRecipientDetail.RecipientPhoneNumber : null))
            .ForMember(to => to.sender_phone_number, from => from.MapFrom(x => x.TransactionCustomerInformation != null ? x.TransactionCustomerInformation.SenderPhoneNumber : null));

            #endregion
        }
    }
}
