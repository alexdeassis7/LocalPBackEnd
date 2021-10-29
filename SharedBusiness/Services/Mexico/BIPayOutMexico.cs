using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityTools = Tools;

namespace SharedBusiness.Services.Mexico
{
    public class BIPayOutMexico
    {
        static readonly object _locker = new object();
        public List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response> LotTransaction(List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Request> data, string customer, bool TransactionMechanism, string countryCode)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Mexico.PayOutMapperMexico LPMapper = new SharedMaps.Converters.Services.Mexico.PayOutMapperMexico();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                SharedBusiness.Services.Payouts.BIPayOut LPDAO = new SharedBusiness.Services.Payouts.BIPayOut();
                lock (_locker)
                {
                    LotBatch = LPDAO.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                }

                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Mexico.PayOutMexico.Create.Response> Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response DownloadLotBatchTransactionToBank(List<string> tickets, bool TransactionMechanism, int internalFiles, string providerName)
        {
            DAO.DataAccess.Services.Mexico.DbPayOutMexico DbPayOut = new DAO.DataAccess.Services.Mexico.DbPayOutMexico();
            SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Mexico.PayOutMexico.DownloadLotBatchTransactionToBank.Response();

            string JSON = JsonConvert.SerializeObject(tickets);

            if (providerName == "MIFEL") {
                result = DbPayOut.DownloadBatchLotTransactionToBank(TransactionMechanism, JSON, internalFiles);
            }
            else if (providerName == "SABADELL") {
                result = DbPayOut.DownloadBatchLotTransactionToBankSabadell(TransactionMechanism, JSON);
            }
            else if (providerName == "SRM") { 
                //SANTANDER
                result = DbPayOut.DownloadBatchLotTransactionToBankSantander(TransactionMechanism, JSON);
            }
            else if (providerName == "BANORTE")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBanorte(TransactionMechanism, JSON);
            }

            if (result.PayoutFiles.Count > 0)
            {
                //Payouts Txt
                
                for (int i = 0; i < result.PayoutFiles.Count; i++)
                {
                    byte[] bytesPayouts = null;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(memory, Encoding.GetEncoding(437)))
                        {

                            for (int j = 0; j < result.PayoutFiles[i].LinesPayouts.Count; j++)
                            {
                                writer.WriteLine(result.PayoutFiles[i].LinesPayouts[j]);
                            }

                            writer.Flush();
                            memory.Position = 0;
                            bytesPayouts = memory.ToArray();
                            result.PayoutFiles[i].FileBase64_Payouts = Convert.ToBase64String(bytesPayouts);
                            result.PayoutFiles[i].LinesPayouts = null;
                        }

                    }
                }
            }
            if (result.RowsPreRegister > 0)
            { 
                //Beneficiaries Txt
                byte[] bytesBeneficiaries = null;
                using (MemoryStream memory = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memory, Encoding.GetEncoding(437)))
                    {
                        for (int i = 0; i < result.LinesPreRegister.Length; i++)
                        {
                            writer.WriteLine(result.LinesPreRegister[i]);
                        }

                        writer.Flush();
                        memory.Position = 0;
                        bytesBeneficiaries = memory.ToArray();
                        result.FileBase64_PreRegister = Convert.ToBase64String(bytesBeneficiaries);
                        result.LinesPreRegister = null;
                    }
                }

            }

            return result;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response UploadTxtFromBank(SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            int idxLine = 1;

            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel>();

            try
            {
                DAO.DataAccess.Services.Mexico.DbPayOutMexico DbPayOut = new DAO.DataAccess.Services.Mexico.DbPayOutMexico();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                string line;

                int totalFileLines = 0;
                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();
                        totalFileLines++;
                    }
                }

                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(bytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        string DetailStatus = "";
                        string BeneficiaryName = "";
                        decimal Amount = 0;
                        string StatusCode = "ACCERROR";

                        if (idxLine > 17 && totalFileLines > 17) // archivo de multiple registros
                        {
                            BeneficiaryName = fields[1];
                            Amount = decimal.Parse(fields[2].Replace("MXP", "").Replace("\"", "").Trim(), CultureInfo.InvariantCulture);
                            DetailStatus = fields[3];
                        }

                        if (idxLine > 2 && totalFileLines < 17) // archivo de registro simple
                        {
                            BeneficiaryName = fields[3];
                            Amount = decimal.Parse(fields[4].Replace("MXP", "").Trim(), CultureInfo.InvariantCulture);
                            DetailStatus = fields[8];
                        }

                        if (DetailStatus.ToLower().Trim() == "aplicado exitosamente" || DetailStatus.ToLower().Trim() == "liquidado" || DetailStatus.ToLower().Trim() == "liquidada")
                        {
                            StatusCode = "SUC";
                        }

                        if(!string.IsNullOrWhiteSpace(BeneficiaryName) && !string.IsNullOrWhiteSpace(StatusCode) && Amount > 0) { 
                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel() { ticket = "", status = StatusCode, beneficiaryName = BeneficiaryName, amount = Amount });
                        }


                        idxLine++;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBank(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

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
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response UploadTxtFromBankSantander(SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            int idxLine = 1;

            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel>();

            try
            {
                DAO.DataAccess.Services.Mexico.DbPayOutMexico DbPayOut = new DAO.DataAccess.Services.Mexico.DbPayOutMexico();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                using (TextFieldParser parser = new TextFieldParser(new MemoryStream(bytes)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        string Ticket = "";
                        string BeneficiaryName = "";
                        decimal Amount = 0;
                        string temporalStatusCode = "";
                        string finalStatusCode = "REJECTED";

                        if (idxLine > 1) // archivo de multiple registros
                        {
                            Ticket = fields[6].Replace("'", "");
                            BeneficiaryName = "";
                            Amount = decimal.Parse(fields[5].Trim(), CultureInfo.InvariantCulture);
                            temporalStatusCode = fields[18].ToUpper().Trim();
                            switch (temporalStatusCode)
                            {
                                case "EJECUTADO":
                                    finalStatusCode = "EXECUTED";
                                    break;
                                case "ANULADO":
                                    finalStatusCode = "REJECTED";
                                    break;
                                default:
                                    finalStatusCode = "INPROGRESS";
                                    break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(finalStatusCode) && Amount > 0)
                        {
                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModelMifel() { ticket = Ticket, status = finalStatusCode, beneficiaryName = BeneficiaryName, amount = Amount });
                        }

                        idxLine++;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBankSantander(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

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
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response UploadTxtFromBankPreRegister(SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            int idxLine = 1;

            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail();

            try
            {
                DAO.DataAccess.Services.Mexico.DbPayOutMexico DbPayOut = new DAO.DataAccess.Services.Mexico.DbPayOutMexico();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;
                    string accountWithErrors = "";
                    string idBankPreRegisterLot = txtUpload.FileName.Split('_')[3].Replace(".txt", "");

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        // Verificando si el registro contiene un error
                        if(line.ToLower().Contains("original instruction") && line.ToLower().Contains("error"))
                        {
                            string account = line.Split('|')[4].ToString();
                            accountWithErrors += string.Format("{0},", account);
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }

                    if(accountWithErrors.Length > 0)
                    {
                        accountWithErrors = accountWithErrors.Substring(0, accountWithErrors.Length - 1);
                    }

                    List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> DetailList = DbPayOut.UpdateLotBatchFromBankPreRegister(idBankPreRegisterLot, accountWithErrors, TransactionMechanism);

                    ResponseModel.TransactionDetail = DetailList;
                }

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
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "The upload file does not have a valid format";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }
    }
}
