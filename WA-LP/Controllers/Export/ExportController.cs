using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Newtonsoft.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using SharedModel.Models.View;
using System.Data;
using System.ComponentModel;
using TimeZoneConverter;
using System.Text;
using ITS_PDF = iTextSharp.text.pdf;
using ITS_TEXT = iTextSharp.text;
using DocumentFormat.OpenXml.Drawing;
using iTextSharp.text;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;
using System.Web.Http.Description;
using SharedBusiness.Log;
using Tools;
using WA_LP.Models;
using Cell = DocumentFormat.OpenXml.Spreadsheet.Cell;
using Row = DocumentFormat.OpenXml.Spreadsheet.Row;
using System.Reflection;
using System.Collections;
using Tools.Dto;

namespace WA_LP.Controllers.Export
{

    [Authorize]
    [RoutePrefix("v2/export")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class ExportController : BaseApiController
    {
        private string ExportToExcelAllDbMerchant(List<SharedModel.Models.View.Report.List.DataReport> dataExcel, string TypeReport, string MerchantCountryCode, string UserPermission)
        {
            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                if(TypeReport == "ACCOUNT")
                {
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Processed Date", FieldFromSP = "ProcessedDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Ticket", FieldFromSP = "Ticket" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Type of TX", FieldFromSP = "PayMethod" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Automatic", FieldFromSP = "Mechanism" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "ID Lot", FieldFromSP = "LotNumber" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Merchant", FieldFromSP = "Merchant" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "SubMerchant", FieldFromSP = "SubMerchantIdentification" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Merchant ID", FieldFromSP = "InternalClient_id" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Status", FieldFromSP = "Status" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "Detail Status", FieldFromSP = "DetailStatus" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "Amount", FieldFromSP = "Amount" });

                    switch (MerchantCountryCode)
                    {
                        case "ARG":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "AFIP Withholding", FieldFromSP = "Withholding" });
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "ARBA Withholding", FieldFromSP = "WithholdingArba" });
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "Local Tax", FieldFromSP = "TaxCountry" });
                            break;
                        case "COL":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "GMF 0.4%", FieldFromSP = "TaxCountry" });
                            break;
                        case "BRA":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "IOF 0.38%", FieldFromSP = "TaxCountry" });
                            break;
                    }

                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 16, DisplayName = "Payable", FieldFromSP = "Payable" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 17, DisplayName = "Fx Merchant", FieldFromSP = "FxMerchant" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 18, DisplayName = "Pending", FieldFromSP = "Pending" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 19, DisplayName = "Confirmed", FieldFromSP = "Confirmed" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 21, DisplayName = "Account(USD) Without COM", FieldFromSP = "AccountWhitoutCommission" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 22, DisplayName = "Net Com", FieldFromSP = "NetCom" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 23, DisplayName = "Vat Com", FieldFromSP = "Com" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 24, DisplayName = "Commision", FieldFromSP = "TotCom" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 25, DisplayName = "Account With Com", FieldFromSP = "AccountArs" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 26, DisplayName = "Account(USD) With Com", FieldFromSP = "AccountUsd" });
                }

                if (TypeReport == "MERCHANT")
                {
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Transaction Date", FieldFromSP = "TransactionDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Processed Date", FieldFromSP = "ProcessedDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Ticket", FieldFromSP = "Ticket" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Type of TX", FieldFromSP = "PayMethod" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Automatic", FieldFromSP = "Mechanism" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "ID Lot", FieldFromSP = "LotNumber" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Merchant", FieldFromSP = "Merchant" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "SubMerchant", FieldFromSP = "SubMerchantIdentification" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Merchant ID", FieldFromSP = "InternalClient_id" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Status", FieldFromSP = "Status" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "Detail Status", FieldFromSP = "DetailStatus" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "Amount", FieldFromSP = "Amount" });

                    switch (MerchantCountryCode)
                    {
                        case "ARG":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "AFIP Withholding", FieldFromSP = "Withholding" });
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "ARBA Withholding", FieldFromSP = "WithholdingArba" });
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "Local Tax", FieldFromSP = "TaxCountry" });
                            break;
                        case "COL":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "GMF 0.4%", FieldFromSP = "TaxCountry" });
                            break;
                        case "BRA":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "IOF 0.38%", FieldFromSP = "TaxCountry" });
                            break;
                    }

                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 16, DisplayName = "Payable", FieldFromSP = "Payable" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 17, DisplayName = "Fx Merchant", FieldFromSP = "FxMerchant" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 18, DisplayName = "Pending", FieldFromSP = "Pending" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 19, DisplayName = "Confirmed", FieldFromSP = "Confirmed" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 21, DisplayName = "Account(USD) Without COM", FieldFromSP = "AccountWhitoutCommission" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 22, DisplayName = "Net Com", FieldFromSP = "NetCom" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 23, DisplayName = "Vat Com", FieldFromSP = "Com" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 24, DisplayName = "Commision", FieldFromSP = "TotCom" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 25, DisplayName = "Account With Com", FieldFromSP = "AccountArs" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 26, DisplayName = "Account(USD) With Com", FieldFromSP = "AccountUsd" });
                }

                if (TypeReport == "ALLDATABASE")
                {
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Transaction Date", FieldFromSP = "TransactionDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "LotOut Date", FieldFromSP = "LotOutDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Processed Date", FieldFromSP = "ProcessedDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Payment Date", FieldFromSP = "PaymentDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Collection Date", FieldFromSP = "CollectionDate" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Cashed", FieldFromSP = "Cashed" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Paid", FieldFromSP = "Pay" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "ID LP", FieldFromSP = "idTransaction" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Ticket", FieldFromSP = "Ticket" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "Provider", FieldFromSP = "Provider" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Type of TX", FieldFromSP = "PayMethod" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Automatic", FieldFromSP = "Mechanism" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "ID Lot", FieldFromSP = "LotNumber" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "ID Lot Out", FieldFromSP = "LotOutId" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "User ID", FieldFromSP = "Identification" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "Status", FieldFromSP = "Status" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "Detail Status", FieldFromSP = "DetailStatus" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 16, DisplayName = "Merchant", FieldFromSP = "Merchant" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 17, DisplayName = "Submerchant", FieldFromSP = "SubMerchantIdentification" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 18, DisplayName = "Merchant ID", FieldFromSP = "InternalClient_id" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 19, DisplayName = "Amount", FieldFromSP = "Amount" });

                    switch (MerchantCountryCode)
                    {
                        case "ARG":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "AFIP Withholding", FieldFromSP = "Withholding" });
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 21, DisplayName = "ARBA Withholding", FieldFromSP = "WithholdingArba" });
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 27, DisplayName = "Local Tax", FieldFromSP = "TaxCountry" });
                            break;
                        case "COL":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 27, DisplayName = "GMF 0.4%", FieldFromSP = "TaxCountry" });
                            break;
                        case "BRA":
                            lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 27, DisplayName = "IOF 0.38%", FieldFromSP = "TaxCountry" });
                            break;
                    }

                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 22, DisplayName = "Payable", FieldFromSP = "Payable" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 23, DisplayName = "Fx Merchant", FieldFromSP = "FxMerchant" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 24, DisplayName = "Pending at merchant fx", FieldFromSP = "Pending" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 25, DisplayName = "Pending At LP Fx", FieldFromSP = "PendingAtLPFx" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 26, DisplayName = "Confirmed at merchant fx", FieldFromSP = "Confirmed" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 28, DisplayName = "Account(USD) WITHOUT COM", FieldFromSP = "AccountWhitoutCommission" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 29, DisplayName = "Net Com", FieldFromSP = "NetCom" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 30, DisplayName = "Vat Com", FieldFromSP = "Com" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 31, DisplayName = "Tot Com", FieldFromSP = "TotCom" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 32, DisplayName = "Account(ARS)", FieldFromSP = "AccountArs" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 33, DisplayName = "Account(USD)", FieldFromSP = "AccountUsd" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 34, DisplayName = "Costo Proveedor", FieldFromSP = "ProviderCost" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 35, DisplayName = "IVA", FieldFromSP = "VatCostProv" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 36, DisplayName = "Total", FieldFromSP = "TotalCostProv" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 37, DisplayName = "Perc IIBB", FieldFromSP = "PercIIBB" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 38, DisplayName = "Perc Iva", FieldFromSP = "PercVat" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 39, DisplayName = "Perc Ganancias", FieldFromSP = "PercProfit" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 40, DisplayName = "Otros", FieldFromSP = "PercOthers" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 41, DisplayName = "Sircreb", FieldFromSP = "Sircreb" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 42, DisplayName = "Impuesto Debito", FieldFromSP = "TaxDebit" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 43, DisplayName = "Impuesto Credito", FieldFromSP = "TaxCredit" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 44, DisplayName = "Rdo Operativo", FieldFromSP = "RdoOperative" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 45, DisplayName = "Iva a pagar", FieldFromSP = "VatToPay" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 46, DisplayName = "FX LP", FieldFromSP = "FxLP" });
                    lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 47, DisplayName = "Rdo FX", FieldFromSP = "RdoFx" });
                }

                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);
                return excelBase64;

            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return ex.ToString();
            }
        }


        private string ExportToExcelDetailTransactionClient(List<SharedModel.Models.View.Report.List.DetailTransactionClient> dataExcel, string TypeReport)
        {

            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "ID Transaction", FieldFromSP = "idTransaction" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "ID Lot", FieldFromSP = "LotNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Transaction Date", FieldFromSP = "TransactionDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Processed Date", FieldFromSP = "ProcessedDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Transaction Type", FieldFromSP = "TransactionType" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Ticket", FieldFromSP = "Ticket" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Alternative Ticket", FieldFromSP = "AlternativeTicket" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Status", FieldFromSP = "Status" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Status Detail", FieldFromSP = "DetailStatus" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "ID Lot Out", FieldFromSP = "idLotOut" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Lot Out Date", FieldFromSP = "LotOutDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Merchant", FieldFromSP = "Merchant" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "ID Merchant", FieldFromSP = "InternalDescription" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "SubMerchant", FieldFromSP = "SubMerchantIdentification" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "Recipient", FieldFromSP = "Recipient" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "Country", FieldFromSP = "Country" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 16, DisplayName = "City", FieldFromSP = "City" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 17, DisplayName = "Address", FieldFromSP = "Address" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 18, DisplayName = "Email", FieldFromSP = "Email" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 19, DisplayName = "Birthdate", FieldFromSP = "Birthdate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 20, DisplayName = "CUIT", FieldFromSP = "RecipientCUIT" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 21, DisplayName = "Recipient Phone Number", FieldFromSP = "RecipientPhoneNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 22, DisplayName = "Bank Branch", FieldFromSP = "BankBranch" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 23, DisplayName = "Bank Code", FieldFromSP = "BankCode" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 24, DisplayName = "Bank Account", FieldFromSP = "CBU" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 25, DisplayName = "Sender Email", FieldFromSP = "SenderEmail" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 26, DisplayName = "Sender Phone Number", FieldFromSP = "SenderPhoneNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 27, DisplayName = "Currency Type", FieldFromSP = "CurrencyType" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 28, DisplayName = "Amount", FieldFromSP = "GrossValueClient" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 29, DisplayName = "Currency Type USD", FieldFromSP = "CurrencyTypeUsd" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 30, DisplayName = "Amount USD", FieldFromSP = "GrossValueClientUsd" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 31, DisplayName = "Local Tax", FieldFromSP = "LocalTax" });





                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string ExportToExcelOperationRetention(List<SharedModel.Models.View.Report.List.OperationRetention> dataExcel, string TypeReport)
        {

            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "ID TX", FieldFromSP = "idTransaction" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Transaction Date", FieldFromSP = "TransactionDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Processed Date", FieldFromSP = "ProcessedDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Ticket", FieldFromSP = "Ticket" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "ID Merchant", FieldFromSP = "MerchantId" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "File Name", FieldFromSP = "FileName" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Certificate Number", FieldFromSP = "CertificateNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Type Retention", FieldFromSP = "Retention" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "Gross", FieldFromSP = "GrossAmount" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Tax With holdings", FieldFromSP = "TaxWithholdings" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Net Amount", FieldFromSP = "NetAmount" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "CUIT", FieldFromSP = "RecipientCUIT" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "Recipient", FieldFromSP = "Recipient" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "CBU", FieldFromSP = "CBU" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "Regime Number", FieldFromSP = "NroRegimen" });

                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string ExportToExcelRejectedTransactions(List<SharedModel.Models.Security.TransactionError.List.Model> dataExcel, string TypeReport)
        {

            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Processed Date", FieldFromSP = "ProcessDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Transaction Type", FieldFromSP = "TransactionType" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Merchant", FieldFromSP = "Merchant" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "ID Merchant", FieldFromSP = "MerchantId" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "SubMerchant", FieldFromSP = "SubMerchant" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Name", FieldFromSP = "BeneficiaryName" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Country", FieldFromSP = "Country" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "City", FieldFromSP = "City" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "Address", FieldFromSP = "Address" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Email", FieldFromSP = "Email" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Birthdate", FieldFromSP = "Birthdate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "Beneficiary Id", FieldFromSP = "BeneficiaryID" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "CBU", FieldFromSP = "CBU" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "Amount", FieldFromSP = "Amount" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "Reason for Rejected", FieldFromSP = "ListErrorsFormattedExport" });

                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private string ExportToExcelHistoricalFx(List<SharedModel.Models.View.Report.List.HistoricalFX> dataExcel, string TypeReport)
        {

            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Date", FieldFromSP = "ProcessDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Merchant", FieldFromSP = "Merchant" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Buy", FieldFromSP = "Buy" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Base Buy", FieldFromSP = "Base_Buy" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Spot", FieldFromSP = "Spot" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Base Sell", FieldFromSP = "Base_Sell" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Sell", FieldFromSP = "Sell" });

                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string ExportToExcelMerchantReport(List<SharedModel.Models.View.Report.List.MerchantReport> dataExcel, string TypeReport)
        {

            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Provider Name", FieldFromSP = "ProviderName" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "CCY", FieldFromSP = "Ccy" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Date", FieldFromSP = "Date" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Account Number", FieldFromSP = "AccountNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "BIC", FieldFromSP = "Bic" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Trx Type", FieldFromSP = "TrxType" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Description", FieldFromSP = "Description" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Payoneer ID", FieldFromSP = "PayoneerId" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "Internal ID", FieldFromSP = "InternalId" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Debit", FieldFromSP = "Debit" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Credit", FieldFromSP = "Credit" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "Available Balance", FieldFromSP = "AvailableBalance" });


                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal string ExportToCsvMerchantReport(List<SharedModel.Models.View.Report.List.MerchantReport_CSV_Format> dataCsv, string TypeReport)
        {

            try
            {
                string csvBase64 = "";
                DataTable dt = ConvertToDataTable(dataCsv);
                csvBase64 = GenerateCsv(dt);
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(csvBase64);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private string ExportToPdfMerchantReport(List<SharedModel.Models.View.Report.List.MerchantReport_CSV_Format> dataPdf, string TypeReport)
        {
            try
            {
                string pdfBase64 = "";
                DataTable dt = ConvertToDataTable(dataPdf);
                iTextSharp.text.Font fontTitle = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 14, iTextSharp.text.Color.BLACK);
                iTextSharp.text.Font fontCellsTitles = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8, iTextSharp.text.Color.WHITE);
                iTextSharp.text.Font fontCells = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 8);

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    //ITS_TEXT.Document document = new Document(iTextSharp.text.PageSize.A4, 10f, 10f, 50f, 10f);
                    ITS_TEXT.Document document = new ITS_TEXT.Document(new ITS_TEXT.Rectangle(288f, 144f), 10f, 10f, 45f, 35f);

                    document.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                    ITS_PDF.PdfWriter writer = ITS_PDF.PdfWriter.GetInstance(document, ms);
                    writer.PageEvent = new ITextEventsPayoneerReport();

                    document.Open();

                    var title = new ITS_TEXT.Paragraph("Payoneer Report", fontTitle);
                    title.SpacingBefore = 10;
                    title.SpacingAfter = 0;
                    title.Alignment = 1; //0-Left, 1 middle,2 Right
                    document.Add(title);
                    document.Add(ITS_TEXT.Chunk.NEWLINE);

                    ITS_PDF.PdfPTable table = new ITS_PDF.PdfPTable(dt.Columns.Count);
                    table.WidthPercentage = 100;

                    //Set columns names in the pdf file
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        ITS_PDF.PdfPCell cell = new ITS_PDF.PdfPCell(new ITS_TEXT.Phrase(dt.Columns[k].ColumnName, fontCellsTitles));

                        cell.HorizontalAlignment = ITS_PDF.PdfPCell.ALIGN_CENTER;
                        cell.VerticalAlignment = ITS_PDF.PdfPCell.ALIGN_CENTER;
                        cell.BackgroundColor = new iTextSharp.text.Color(12, 113, 195);

                        table.AddCell(cell);
                    }

                    //Add values of DataTable in pdf file
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ITS_PDF.PdfPCell cell = new ITS_PDF.PdfPCell(new ITS_TEXT.Phrase(dt.Rows[i][j].ToString(), fontCells));

                            //Align the cell in the center
                            cell.HorizontalAlignment = ITS_PDF.PdfPCell.ALIGN_CENTER;
                            cell.VerticalAlignment = ITS_PDF.PdfPCell.ALIGN_CENTER;

                            table.AddCell(cell);
                        }
                    }

                    document.Add(table);
                    document.Close();

                    byte[] result = ms.ToArray();
                    pdfBase64 = Convert.ToBase64String(result);
                    return pdfBase64;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        const string reportsFolder = "Reports";
        [HttpPost]
        [Route("ExportList")]
        public string ExportList([FromBody]ExportModel export)
        {
            var reportesPath = HttpContext.Current.Server.MapPath("~/" + reportsFolder);

            var exportUtilsService = new ExportUtilsService();
            var expListBytes = exportUtilsService.ExportarListado(reportesPath, new Tools.Dto.Export
            {
                FileName = export.FileName,
                IsSP = export.IsSP,
                Name = export.Name,
                Parameters = export.Parameters.Select(x => new Tools.Dto.ParameterInExport { Key = x.Key, Val = x.Val })
            }, true);

            return expListBytes;           
        }

        public string ExcelAfipRetentionMonthly(List<SharedModel.Models.Tools.RetentionModel.RetentionMonthly> dataExcel, string TypeReport)
        {

            try
            {
                string excelBase64 = "";
                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Field 1", FieldFromSP = "CodComprobante" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Field 2", FieldFromSP = "FechaEmision" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Field 3", FieldFromSP = "NumComprobante" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Field 4", FieldFromSP = "ImporteComprbante" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Field 5", FieldFromSP = "CodImpuesto" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Field 6", FieldFromSP = "CodRegimen" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 7, DisplayName = "Field 7", FieldFromSP = "CodOPeración" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 8, DisplayName = "Field 8", FieldFromSP = "BaseCalculo" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 9, DisplayName = "Field 9", FieldFromSP = "FechaEmisionRet" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 10, DisplayName = "Field 10", FieldFromSP = "CodCondicion" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 11, DisplayName = "Field 11", FieldFromSP = "SujetoSuspen" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 12, DisplayName = "Field 12", FieldFromSP = "ImporteRet" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 13, DisplayName = "Field 13", FieldFromSP = "PorcExclusion" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 14, DisplayName = "Field 14", FieldFromSP = "FechaemiBoletin" });
                //lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 15, DisplayName = "Field 15", FieldFromSP = "CodCondicion" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 16, DisplayName = "Field 16", FieldFromSP = "TipoDocRet" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 17, DisplayName = "Field 17", FieldFromSP = "NumDocRet" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 18, DisplayName = "Field 18", FieldFromSP = "NumCertOrig" });


                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ExcelArbaRetentionMonthly(List<SharedModel.Models.Tools.RetentionModel.RetentionARBAMonthly> dataExcel, string TypeReport)
        {
            try
            {
                string excelBase64 = "";

                List<SharedModel.Models.Export.Export.Excel> lExcelColumns = new List<SharedModel.Models.Export.Export.Excel>();

                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 1, DisplayName = "Field 1", FieldFromSP = "CUIT" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 2, DisplayName = "Field 2", FieldFromSP = "WithholdingDate" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 3, DisplayName = "Field 3", FieldFromSP = "BranchNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 4, DisplayName = "Field 4", FieldFromSP = "EmisionNumber" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 5, DisplayName = "Field 5", FieldFromSP = "Amount" });
                lExcelColumns.Add(new SharedModel.Models.Export.Export.Excel() { OrderColumn = 6, DisplayName = "Field 6", FieldFromSP = "OperationType" });

                lExcelColumns = lExcelColumns.OrderBy(x => x.OrderColumn).ToList();

                DataTable dt = ConvertToDataTable(dataExcel);
                excelBase64 = GenerateExcel(lExcelColumns, dt, TypeReport);

                return excelBase64;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("exportToExcel")]
        public HttpResponseMessage exportToExcel()
        {
            var reportesPath = HttpContext.Current.Server.MapPath("~/" + reportsFolder);
            var exportUtilsService = new ExportUtilsService();
            List<ParameterInExport> inExport = new List<ParameterInExport>();
            string expListBytes;

            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                string countryCode = headers.GetValues("countryCode").First();
                string _currentTimeZone = headers.GetValues("currentTimeZone").First();
                string UserPermission = headers.GetValues("permissionUser").First();

                if (data != null && data.Length > 0)
                {
                    SharedModel.Models.Export.Export.Request lRequest = JsonConvert.DeserializeObject<SharedModel.Models.Export.Export.Request>(data);

                    List<Report.List.DataReport> dataExport = new List<Report.List.DataReport>();
                    List<Report.List.DetailTransactionClient> dataExportDTC = new List<Report.List.DetailTransactionClient>();
                    List<Report.List.OperationRetention> dataExportOperationRetention = new List<Report.List.OperationRetention>();
                    List<Report.List.HistoricalFX> dataExportHistoricalFx = new List<Report.List.HistoricalFX>();
                    List<Report.List.MerchantReport> dataExportMerchantRep = new List<Report.List.MerchantReport>();
                    List<Report.List.MerchantReport_CSV_Format> dataExportMerchantRep_Csv = new List<Report.List.MerchantReport_CSV_Format>();
                    List<SharedModel.Models.Security.TransactionError.List.Model> dataExportRejectedTransactions = new List<SharedModel.Models.Security.TransactionError.List.Model>();
                    List<Report.List.MerchantReport_CSV_Format> dataExportMerchantRep_Pdf = new List<Report.List.MerchantReport_CSV_Format>();

                    SharedBusiness.Common.BIMerchant bIMerchant = new SharedBusiness.Common.BIMerchant();
                    string MerchantCountryCode = bIMerchant.GetCountryCodeByidMerchantSelected(customer, Convert.ToInt64(lRequest.requestReport.idEntityAccount));

                    string nameFile = "\\ExcelTempReport.xlsx";
                    string path_dir = System.Web.Hosting.HostingEnvironment.MapPath("~\\Files\\Excel");

                    if (!Directory.Exists(path_dir))
                        Directory.CreateDirectory(path_dir);

                    string path_xlsx = path_dir + nameFile;
                    string resultBase64 = "";
                    Report.List.Response lResponse = new Report.List.Response();
                    SharedBusiness.View.BIReport BiReport = new SharedBusiness.View.BIReport();
                    currentTimeZone = TZConvert.IanaToWindows(_currentTimeZone);
                    switch (lRequest.TypeReport)
                    {
                        case "ALLDATABASE":
                        case "MERCHANT":

                            lResponse = BiReport.ListTransaction(lRequest.requestReport, customer, countryCode);
                            dataExport = JsonConvert.DeserializeObject<List<Report.List.DataReport>>(lResponse.TransactionList);

             

                            if (dataExport.Count > 0)
                            {
                                resultBase64 = ExportToExcelAllDbMerchant(dataExport, lRequest.TypeReport, MerchantCountryCode, UserPermission);
                            }



                            break;
                        case "MERCHANTBALANCE":

                            inExport.Add(new ParameterInExport { Key = "json", Val = JsonConvert.SerializeObject(lRequest.requestReport) });
                            inExport.Add(new ParameterInExport { Key = "country_code", Val = countryCode });
                            inExport.Add(new ParameterInExport { Key = "returnAsJson", Val = "0" });

                            expListBytes = exportUtilsService.ExportarListado(reportesPath, new Tools.Dto.Export
                            {
                                FileName = "MerchantBalanceReport",
                                IsSP = true,
                                Name = "[LP_Operation].[MerchantReport]",
                                Parameters = inExport
                            }, true);

                            resultBase64 = expListBytes;

                            break;
                        case "MERCHANTPROYECTEDBALANCE":
                            inExport.Add(new ParameterInExport { Key = "json", Val = JsonConvert.SerializeObject(lRequest.requestReport) });
                            inExport.Add(new ParameterInExport { Key = "country_code", Val = countryCode });
                            inExport.Add(new ParameterInExport { Key = "returnAsJson", Val = "0" });

                            expListBytes = exportUtilsService.ExportarListado(reportesPath, new Tools.Dto.Export
                            {
                                FileName = "MerchantProyectedBalanceAccountReport",
                                IsSP = true,
                                Name = "[LP_Operation].[MerchantProyectedAccountBalanceReport]",
                                Parameters = inExport
                            }, true);

                            resultBase64 = expListBytes;

                            break;
                        case "ACTIVITYREPORT":
                            inExport.Add(new ParameterInExport { Key = "json", Val = JsonConvert.SerializeObject(lRequest.requestReport) });
                            inExport.Add(new ParameterInExport { Key = "country_code", Val = countryCode });
                            inExport.Add(new ParameterInExport { Key = "returnAsJson", Val = "0" });

                            expListBytes = exportUtilsService.ExportarListado(reportesPath, new Tools.Dto.Export
                            {
                                FileName = "ActivityReport",
                                IsSP = true,
                                Name = "[LP_Operation].[ActivityReport]",
                                Parameters = inExport
                            }, true);

                            resultBase64 = expListBytes;

                            break;
                        case "ACCOUNT":
                            lResponse = BiReport.AccountTransaction(lRequest.requestReport, customer, countryCode);
                            dataExport = JsonConvert.DeserializeObject<List<Report.List.DataReport>>(lResponse.TransactionList);

                            if (dataExport.Count > 0)
                            {
                                resultBase64 = ExportToExcelAllDbMerchant(dataExport, lRequest.TypeReport, MerchantCountryCode, UserPermission);
                            }

                            break;
                        case "DETAILS_TRANSACTION_CLIENT":

                            lResponse = BiReport.ListTransactionClientDetails(lRequest.requestReport, customer);
                            dataExportDTC = JsonConvert.DeserializeObject<List<Report.List.DetailTransactionClient>>(lResponse.TransactionList);

                            if (dataExportDTC.Count > 0)
                            {
                                resultBase64 = ExportToExcelDetailTransactionClient(dataExportDTC, lRequest.TypeReport);
                            }

                            break;

                        case "HISTORICAL_FX":

                            lResponse = BiReport.ListHistoricalFx(lRequest.requestReport, customer);
                            dataExportHistoricalFx = JsonConvert.DeserializeObject<List<Report.List.HistoricalFX>>(lResponse.TransactionList);
                            resultBase64 = ExportToExcelHistoricalFx(dataExportHistoricalFx, lRequest.TypeReport);
                            break;
                        case "OPERATION_RETENTION":
                            lResponse = BiReport.ListOperationRetention(lRequest.requestReport, customer);
                            dataExportOperationRetention = JsonConvert.DeserializeObject<List<Report.List.OperationRetention>>(lResponse.TransactionList);
                            if (dataExportOperationRetention.Count > 0)
                            {
                                resultBase64 = ExportToExcelOperationRetention(dataExportOperationRetention, lRequest.TypeReport);
                            }

                            break;
                        case "MERCHANT_REPORT_XLSX":
                            lResponse = BiReport.List_Merchant_Report(lRequest.requestReport, customer);
                            dataExportMerchantRep = JsonConvert.DeserializeObject<List<Report.List.MerchantReport>>(lResponse.TransactionList);
                            if (dataExportMerchantRep.Count > 0)
                            {
                                resultBase64 = ExportToExcelMerchantReport(dataExportMerchantRep, lRequest.TypeReport);
                            }
                            break;
                        case "MERCHANT_REPORT_CSV":
                            lResponse = BiReport.List_Merchant_Report(lRequest.requestReport, customer);
                            dataExportMerchantRep_Csv = JsonConvert.DeserializeObject<List<Report.List.MerchantReport_CSV_Format>>(lResponse.TransactionList);
                            if (dataExportMerchantRep_Csv.Count > 0)
                            {
                                resultBase64 = ExportToCsvMerchantReport(dataExportMerchantRep_Csv, lRequest.TypeReport);
                            }
                            break;
                        case "TRANSACTION_REJECTED":

                            lResponse = BiReport.ListTransactionsError(lRequest.requestReport, customer);
                            dataExportRejectedTransactions = JsonConvert.DeserializeObject<List<SharedModel.Models.Security.TransactionError.List.Model>>(lResponse.TransactionList);
                            if (dataExportRejectedTransactions.Count > 0)
                            {
                                resultBase64 = ExportToExcelRejectedTransactions(dataExportRejectedTransactions, lRequest.TypeReport);
                            }

                            break;
                        case "MERCHANT_REPORT_PDF":
                            lResponse = BiReport.List_Merchant_Report(lRequest.requestReport, customer);
                            dataExportMerchantRep_Pdf = JsonConvert.DeserializeObject<List<Report.List.MerchantReport_CSV_Format>>(lResponse.TransactionList);
                            if (dataExportMerchantRep_Pdf.Count > 0)
                            {
                                resultBase64 = ExportToPdfMerchantReport(dataExportMerchantRep_Pdf, lRequest.TypeReport);
                            }
                            break;
                        default:
                            break;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, resultBase64);
                }
                else
                {
                    string resultError = "";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
                //throw ex;
            }
        }
        private DocumentFormat.OpenXml.Spreadsheet.Cell ConstructCell(string value, CellValues dataType)
        {
            return new DocumentFormat.OpenXml.Spreadsheet.Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }


        private Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            DocumentFormat.OpenXml.Drawing.Fonts fonts = new DocumentFormat.OpenXml.Drawing.Fonts(
                new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 0 - default
                    new FontSize() { Val = 11 }

                ),
                new DocumentFormat.OpenXml.Spreadsheet.Font( // Index 1 - header
                    new FontSize() { Val = 11 },
                    new Bold(),
                    new DocumentFormat.OpenXml.Office2010.Excel.Color() { Rgb = "FFFFFF" }

                ));

            Fills fills = new Fills(
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(new DocumentFormat.OpenXml.Spreadsheet.PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(new DocumentFormat.OpenXml.Spreadsheet.PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new DocumentFormat.OpenXml.Spreadsheet.Fill(new DocumentFormat.OpenXml.Spreadsheet.PatternFill(new DocumentFormat.OpenXml.Spreadsheet.ForegroundColor { Rgb = new HexBinaryValue() { Value = "0c71c3" } })
                    { PatternType = PatternValues.Solid }) // Index 2 - header
                );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new DocumentFormat.OpenXml.Spreadsheet.LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DocumentFormat.OpenXml.Spreadsheet.BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true } // header
                );
            Columns columns = new Columns(
                new Column { Min = 1, Max = 43, Width = 450, CustomWidth = true },
                  new Column { Min = 1, Max = 100, Width = 200, CustomWidth = true },
                    new Column { Min = 1, Max = 150, Width = 350, CustomWidth = true }


            );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats, columns);

            return styleSheet;
        }

        public string GenerateCsv(DataTable dataTable)
        {
            StringBuilder sbData = new StringBuilder();

            // Only return Null if there is no structure.
            if (dataTable.Columns.Count == 0)
                return null;

            foreach (var col in dataTable.Columns)
            {
                if (col == null)
                    sbData.Append(",");
                else
                    //sbData.Append("\"" + col.ToString().Replace("\"", "\"\"") + "\",");
                    sbData.Append(col.ToString().Replace("\"", "\"\"") + ",");
            }

            sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    if (column == null)
                        sbData.Append(",");
                    else
                        //sbData.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\",");
                        sbData.Append(column.ToString().Replace("\"", "\"\"") + ",");
                }
                sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);
            }

            return sbData.ToString();
        }

    }
}