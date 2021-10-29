using Newtonsoft.Json;
using SharedBusiness.Log;
using SharedModel.Models.Services.Brasil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tools.Dto;
using UtilityTools = Tools;

namespace SharedBusiness.Services.Brasil
{
    public class BIPayOutBrasil
    {
        static readonly object _locker = new object();
        public List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response> LotTransaction(List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request> data, string customer, bool TransactionMechanism, string countryCode)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil LPMapper = new SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                SharedBusiness.Services.Payouts.BIPayOut LPDAO = new SharedBusiness.Services.Payouts.BIPayOut();
                lock (_locker)
                {
                    LotBatch = LPDAO.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                }

                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Response> Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse DownloadExcelLotBatchTransactionToBank(List<string> tickets, bool TransactionMechanism, string providerName, string path = null)
        {
            DAO.DataAccess.Services.Brasil.DbPayOutBrasil DbPayOut = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();
            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelResponse();

            string JSON = JsonConvert.SerializeObject(tickets);
            List<ParameterInExport> inExport = new List<ParameterInExport>();
            inExport.Add(new ParameterInExport { Key = "json", Val = JSON });

            var exportUtilsService = new ExportUtilsService();

            if (providerName == "FASTCASH")
            {
                result = DbPayOut.DownloadBatchLotTransactionToFastCash(TransactionMechanism, JSON);
            }
            else if (providerName == "PLURAL")
            {
                var expListBytes = exportUtilsService.ExportarListado(path, new Export
                {
                    FileName = "PluralDownload",
                    IsSP = true,
                    Name = "[LP_Operation].[BRA_Payout_PLURAL_Bank_Operation_Download]",
                    Parameters = inExport
                });

                result.FileBase64 = expListBytes;
            }

            return result;
        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response UploadExcelFromBankFastcash(List<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelFastcashResponse> excelData, int currencyFx, string datetime, bool TransactionMechanism, string countryCode) 
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response();

            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail();

            try {

                DAO.DataAccess.Services.Brasil.DbPayOutBrasil DbPayOut = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();

                foreach (var transaction in excelData) 
                {
                    Detail = DbPayOut.UpdateLotBatchTransactionFromBank(transaction.TID, currencyFx, Boolean.Parse(transaction.Success), transaction.Message, TransactionMechanism);
                    ResponseModel.TransactionDetail.Add(Detail);
                }

                if (ResponseModel.TransactionDetail == null)
                {
                    ResponseModel.TransactionDetail.Add(Detail);
                    ResponseModel.Rows = 0;
                }

                ResponseModel.Status = "OK";
                ResponseModel.StatusMessage = "OK";
                ResponseModel.Rows = ResponseModel.TransactionDetail.Count;
            }
            catch (Exception ex) {
                LogService.LogError(ex.ToString());
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response UploadExcelFromBankPlural(List<SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.ExcelPluralResponse> excelData, int currencyFx, string datetime, bool TransactionMechanism, string countryCode)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response();

            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModelPlural> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModelPlural>();

            try
            {

                DAO.DataAccess.Services.Brasil.DbPayOutBrasil DbPayOut = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();

                foreach (var transaction in excelData)
                {
                    if (transaction.SITUACAO == "LIQUIDADA" || transaction.SITUACAO == "DEVOLVIDO")
                    {
                        var status = transaction.SITUACAO == "LIQUIDADA" ? "EXECUTED" : "REJECTED";
                        var rejectionDetail = transaction.MOTIVO_DEVOLUCAO;
                        uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModelPlural() { ticket = transaction.OBSERVACAO, status = status, rejectDetail = rejectionDetail });
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromPlural(currencyFx, TransactionMechanism,uploadModel);

                if (ResponseModel.TransactionDetail == null)
                {
                    ResponseModel.TransactionDetail.Add(Detail);
                    ResponseModel.Rows = 0;
                }

                ResponseModel.Status = "OK";
                ResponseModel.StatusMessage = "OK";
                ResponseModel.Rows = ResponseModel.TransactionDetail.Count;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response UploadTxtFromBankSafra(SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerName)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response();

            int ReferencePosIni = 73;             //REFERENCE: N° of ticket
            int ReferenceLength = 10;

            int DetailCodePosIni = 230;           //DETAIL CODE
            int DetailCodeLength = 2;

            int idxLine = 0;
            int cantOfRows = 0;

            string FileStatus = "Upload";

            string DetailStatus = "";
            string TicketNumber = "";

            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.Brasil.DbPayOutBrasil DbPayOut = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */

                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        DetailStatus = line.Substring(DetailCodePosIni, DetailCodeLength);

                        /* DETALLE */
                        if (DetailStatus.Trim() != "" && DetailStatus != "BD")
                        {
                            TicketNumber = line.Substring(ReferencePosIni, ReferenceLength);
                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = TicketNumber, status = DetailStatus });
                            cantOfRows += 1;
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromSafra(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerName);

                ResponseModel.BatchLotDetail = BatchLotDetail;
                if (ResponseModel.TransactionDetail == null)
                {
                    ResponseModel.TransactionDetail.Add(Detail);
                    ResponseModel.Rows = 0;
                }

                ResponseModel.Status = "OK";
                ResponseModel.StatusMessage = "OK";
                ResponseModel.Rows = ResponseModel.TransactionDetail.Count;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response UploadTxtFromBankDoBrasil(SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerName)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response();

            int ReferencePosIni = 73;             //REFERENCE: N° of ticket
            int ReferenceLength = 10;

            int DetailCodePosIni = 230;           //DETAIL CODE
            int DetailCodeLength = 2;

            int RecordTypePosIni = 13;          //RECORD TYPE
            int RecordTypeLength = 1;

            int idxLine = 0;
            int cantOfRows = 0;

            string FileStatus = "Upload";

            string DetailStatus = "";
            string TicketNumber = "";
            string RecordType = "";

            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.Brasil.DbPayOutBrasil DbPayOut = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */

                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        DetailStatus = line.Substring(DetailCodePosIni, DetailCodeLength);
                        RecordType = line.Substring(RecordTypePosIni, RecordTypeLength);

                        /* DETALLE */
                        if (DetailStatus.Trim() != "" && DetailStatus != "BD" && RecordType == "A")
                        {
                            TicketNumber = line.Substring(ReferencePosIni, ReferenceLength);
                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = TicketNumber, status = DetailStatus });
                            cantOfRows += 1;
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBankDoBrasil(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerName);

                ResponseModel.BatchLotDetail = BatchLotDetail;
                if (ResponseModel.TransactionDetail == null)
                {
                    ResponseModel.TransactionDetail.Add(Detail);
                    ResponseModel.Rows = 0;
                }

                ResponseModel.Status = "OK";
                ResponseModel.StatusMessage = "OK";
                ResponseModel.Rows = ResponseModel.TransactionDetail.Count;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response UploadTxtFromBankSantander(SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerName)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.Response();

            int ReferencePosIni = 73;             //REFERENCE: N° of ticket
            int ReferenceLength = 10;

            int DetailCodePosIni = 230;           //DETAIL CODE
            int DetailCodeLength = 2;

            int RecordTypePosIni = 13;          //RECORD TYPE
            int RecordTypeLength = 1;

            int idxLine = 0;
            int cantOfRows = 0;

            string FileStatus = "Upload";

            string DetailStatus = "";
            string TicketNumber = "";
            string RecordType = "";

            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.Brasil.DbPayOutBrasil DbPayOut = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */

                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        DetailStatus = line.Substring(DetailCodePosIni, DetailCodeLength);
                        RecordType = line.Substring(RecordTypePosIni, RecordTypeLength);

                        /* DETALLE */
                        if (DetailStatus.Trim() != "" && DetailStatus != "BD" && RecordType == "A")
                        {
                            TicketNumber = line.Substring(ReferencePosIni, ReferenceLength);
                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = TicketNumber, status = DetailStatus });
                            cantOfRows += 1;
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBankSantander(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerName);

                ResponseModel.BatchLotDetail = BatchLotDetail;
                if (ResponseModel.TransactionDetail == null)
                {
                    ResponseModel.TransactionDetail.Add(Detail);
                    ResponseModel.Rows = 0;
                }

                ResponseModel.Status = "OK";
                ResponseModel.StatusMessage = "OK";
                ResponseModel.Rows = ResponseModel.TransactionDetail.Count;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public PayOutBrasil.DownloadLotBatchTransactionToBank.Response DownloadLotBatchTransactionToBank(List<string> tickets, bool TransactionMechanism, string providerName)
        {
            SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Brasil.PayOutBrasil.DownloadLotBatchTransactionToBank.Response();
            DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

            string JSON = JsonConvert.SerializeObject(tickets);

            clsBS_ENCRIPT_PAP.BS_ENCRIPT_PAP clsEncrypt = new clsBS_ENCRIPT_PAP.BS_ENCRIPT_PAP();

            if (providerName == "BDOBR")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBDOBRASIL(TransactionMechanism, JSON);
            } 
            else if(providerName == "SAFRA")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankSAFRA(TransactionMechanism, JSON);
            }
            else if(providerName == "SANTBR")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankSANTANDER(TransactionMechanism, JSON);
            }//alex
            else if (providerName == "ITAUBR")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankITAU(TransactionMechanism, JSON);
            }

            if (result.Rows > 0)
            {
                byte[] bytes = null;

                using (MemoryStream memory = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memory, Encoding.GetEncoding(437)))
                    {
                        for (int i = 0; i < result.Lines.Length; i++)
                        {
                            writer.WriteLine(result.Lines[i]);
                        }

                        writer.Flush();
                        memory.Position = 0;

                        StreamReader sr = new StreamReader(memory);
                        string fullText = sr.ReadToEnd();


                        bytes = Encoding.ASCII.GetBytes(fullText);

                        //bytes = memory.ToArray();
                        result.FileBase64 = Convert.ToBase64String(bytes);

                        result.Lines = null;
                    }
                }
            }

            return result;
        }
    }
}