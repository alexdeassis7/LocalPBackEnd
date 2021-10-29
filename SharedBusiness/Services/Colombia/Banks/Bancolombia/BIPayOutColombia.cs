using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UtilityTools = Tools;
using Newtonsoft.Json;
using SharedBusiness.Log;

namespace SharedBusiness.Services.Colombia.Banks.Bancolombia
{
    public class BIPayOutColombia
    {
        static readonly object _locker = new object();
        public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response> LotTransaction(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Request> data, string customer, bool TransactionMechanism, string countryCode)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia LPMapper = new SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                SharedBusiness.Services.Payouts.BIPayOut LPDAO = new SharedBusiness.Services.Payouts.BIPayOut();
                lock (_locker)
                {
                    LotBatch = LPDAO.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                }

                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Create.Response> Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Update.Response> LotTransaction(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Update.Request> data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia LPMapper = new SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                DAO.DataAccess.Services.Colombia.DbPayOutColombia LPDAO = new DAO.DataAccess.Services.Colombia.DbPayOutColombia();


                LotBatch = LPDAO.UpdateLotTransaction(LotBatch, customer, TransactionMechanism);


                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.Update.Response> Response = LPMapper.MapperModelsUpdate(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response DownloadLotBatchTransactionToBank(List<string> tickets, bool TransactionMechanism, string providerName)
        {
            DAO.DataAccess.Services.Colombia.DbPayOutColombia DbPayOut = new DAO.DataAccess.Services.Colombia.DbPayOutColombia();
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.DownloadLotBatchTransactionToBank.Response();

            string JSON = JsonConvert.SerializeObject(tickets);

            if (providerName == "BCOLOMBIA")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBColombia(TransactionMechanism, JSON);
            }
            else if (providerName == "OCCIDENTE") 
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankOccidente(TransactionMechanism, JSON);
            }
            else if (providerName == "BCOLOMBIA2")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBColombiaSAS(TransactionMechanism, JSON);
            }
            else if (providerName == "ACCIVAL")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankAccionesValores(TransactionMechanism, JSON);
            }
            else if (providerName == "SCOTIACOL")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankScotia(TransactionMechanism, JSON);
            }
            else if (providerName == "FINANDINA")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankFinandina(TransactionMechanism, JSON);
            }
            else if (providerName == "BCSC")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankCajaSocial(TransactionMechanism, JSON);
            }
            else if (providerName == "ITAUCOL")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankItau(TransactionMechanism, JSON);
            }
            else if (providerName == "BBVACOL")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBbva(TransactionMechanism, JSON);
            }

            if (result.RowsPayouts > 0 && result.RowsBeneficiaries > 0)
            {
                //Payouts Txt
                byte[] bytesPayouts = null;
                using (MemoryStream memory = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memory, Encoding.GetEncoding(437)))
                    {
                        for (int i = 0; i < result.LinesPayouts.Length; i++)
                        {
                            writer.WriteLine(result.LinesPayouts[i]);
                        }

                        writer.Flush();
                        memory.Position = 0;
                        bytesPayouts = memory.ToArray();
                        result.FileBase64_Payouts = Convert.ToBase64String(bytesPayouts);
                        result.LinesPayouts = null;
                    }
                }

                if (providerName == "BCOLOMBIA" || providerName == "BCOLOMBIA2")
                {
                    //Beneficiaries Txt
                    byte[] bytesBeneficiaries = null;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(memory, Encoding.GetEncoding(437)))
                        {
                            for (int i = 0; i < result.LinesBeneficiaries.Length; i++)
                            {
                                writer.WriteLine(result.LinesBeneficiaries[i]);
                            }

                            writer.Flush();
                            memory.Position = 0;
                            bytesBeneficiaries = memory.ToArray();
                            result.FileBase64_Beneficiaries = Convert.ToBase64String(bytesBeneficiaries);
                            result.LinesBeneficiaries = null;
                        }
                    }
                }

            }

            return result;
        }

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response UploadTxtFromBank(SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerName)
        {
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response();

            int ReferencePosIni = 116;             //REFERENCE: N° of ticket
            int ReferenceLength = 21;

            int DetailCodePosIni = 138;           //DETAIL CODE
            int DetailCodeLength = 3;

            int idxLine = 1;

            string FileStatus = "Upload";

            string DetailStatus = "";

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.Colombia.DbPayOutColombia DbPayOut = new DAO.DataAccess.Services.Colombia.DbPayOutColombia();

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

                        /* DETALLE */
                        if (line.Substring(0, 1) != "*")
                        {
                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            DetailStatus = line.Substring(DetailCodePosIni, DetailCodeLength);

                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBank(txtUpload.CurrencyFxClose,TransactionMechanism,uploadModel);

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

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response UploadTxtFromBankBColombiaSas(SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerName)
        {
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response();

            int ReferencePosIni = 116;             //REFERENCE: N° of ticket
            int ReferenceLength = 21;

            int DetailCodePosIni = 138;           //DETAIL CODE
            int DetailCodeLength = 3;

            int idxLine = 1;

            string FileStatus = "Upload";

            string DetailStatus = "";

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.Colombia.DbPayOutColombia DbPayOut = new DAO.DataAccess.Services.Colombia.DbPayOutColombia();

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

                        /* DETALLE */
                        if (line.Substring(0, 1) != "*")
                        {
                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            DetailStatus = line.Substring(DetailCodePosIni, DetailCodeLength);

                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBankBColombiaSas(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

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

        public SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response UploadTxtFromBankOccidente(SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerName)
        {
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.Response();

            int ReferencePosIni = 46;             //REFERENCE: N° of ticket
            int ReferenceLength = 12;

            int DetailCodePosIni = 81;           //DETAIL CODE
            int DetailCodeLength = 1;

            int idxLine = 1;

            string FileStatus = "Upload";

            string DetailStatus = "";

            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.Colombia.DbPayOutColombia DbPayOut = new DAO.DataAccess.Services.Colombia.DbPayOutColombia();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* ARRAY DE POSIBLES CAUSAS DE RECHAZO */
                    string[] status = new string[] { "CUENTA INACTIVA O BLOQUEADA", "Cuenta Invalida", "Cuenta no Abierta",
                                                             "CUENTA NO EXISTE", "Cuenta no Habilitada", "Id. no Coincide",
                                                             "IDENTIFICACION INCORRECTA", "IDENTIFICACION NO COINCIDE CON CUE",
                                                             "La estructura del Numero de Cuenta", "NRO CTA INVALIDO",
                                                             "NUMERO DE CUENTA INVALIDO"};

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */

                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        /* DETALLE */
                        if (line.Substring(0, 1) != "*")
                        {
                            int lineLength = line.Length - 1;
                            int detailStringLength = lineLength - DetailCodePosIni;
                            int detailCauseLength = lineLength - (DetailCodePosIni + 4);
                            DetailStatus = line.Substring(DetailCodePosIni, detailStringLength);
                            string DetailStatusCode = DetailStatus.Substring(0, DetailCodeLength);
                            string DetailRejectedCause = DetailStatus.Substring(4, detailCauseLength).Trim();
                            bool DetailIncludesR04 = DetailRejectedCause.Contains("R04");
                            bool DetailIncludesR17 = DetailRejectedCause.Contains("R17");
                            bool DetailIncludesR20 = DetailRejectedCause.Contains("R20");

                            //set OCCIDENTE StatusCode
                            switch (DetailStatusCode)
                            {
                                case "R":
                                    DetailStatusCode = "EXECUTED";
                                    break;
                                case "X":
                                    string sKeyResult = status.FirstOrDefault<string>(s => DetailRejectedCause.Contains(s));
                                    switch(sKeyResult)
                                    {
                                        case "CUENTA INACTIVA O BLOQUEADA":
                                            DetailStatusCode = "R07";
                                            break;
                                        case "Cuenta Invalida":
                                            DetailStatusCode = "R19";
                                            break;
                                        case "Cuenta no Abierta":
                                            DetailStatusCode = "R62";
                                            break;
                                        case "CUENTA NO EXISTE":
                                            DetailStatusCode = "R13";
                                            break;
                                        case "Cuenta no Habilitada":
                                            DetailStatusCode = "R52";
                                            break;
                                        case "Id. no Coincide":
                                            DetailStatusCode = "R51";
                                            break;
                                        case "IDENTIFICACION INCORRECTA":
                                            DetailStatusCode = "R13";
                                            break;
                                        case "IDENTIFICACION NO COINCIDE CON CUE":
                                            DetailStatusCode = "R03";
                                            break;
                                        case "La estructura del Numero de Cuenta":
                                            DetailStatusCode = "R14";
                                            break;
                                        case "NRO CTA INVALIDO":
                                        case "NUMERO DE CUENTA INVALIDO":
                                            DetailStatusCode = "R04";
                                            break;
                                        default:
                                            DetailStatusCode = "STATUSNFDB";
                                            if (DetailIncludesR04)
                                            {
                                                DetailStatusCode = "R04";
                                            }
                                            else if (DetailIncludesR17)
                                            {
                                                DetailStatusCode = "R17";
                                            }
                                            else if(DetailIncludesR20)
                                            {
                                                DetailStatusCode = "R20";
                                            }
                                            break;
                                    }
                                    break;
                                case "D":
                                    DetailStatusCode = "OK2";
                                    break;
                                default:
                                    DetailStatusCode = "STATUSNFDB";
                                    break;
                            }

                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);

                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatusCode });
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBankOccidente(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

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
    }
}
