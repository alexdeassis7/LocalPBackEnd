using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UtilityTools = Tools;
using Newtonsoft.Json;
using SharedBusiness.Log;
using System.Configuration;
using Tools;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using SharedBusiness.Payin.DTO;
using static SharedModel.Models.Services.Payouts.Payouts;

namespace SharedBusiness.Services.Banks.Galicia
{
    public class BIPayOut
    {
        static readonly object _locker = new object();
        static readonly object _lockerList = new object();
        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> LotTransaction(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Request> data, string customer, bool TransactionMechanism, string countryCode)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                SharedBusiness.Services.Payouts.BIPayOut LPDAO = new SharedBusiness.Services.Payouts.BIPayOut();
                lock (_locker) {
                    LotBatch = LPDAO.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                }
                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                throw ex;
            }
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Update.Response> LotTransaction(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Update.Request> data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                DAO.DataAccess.Services.DbPayOut LPDAO = new DAO.DataAccess.Services.DbPayOut();


                LotBatch = LPDAO.UpdateLotTransaction(LotBatch, customer, TransactionMechanism);


                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Banks.Galicia.PayOut.Update.Response> Response = LPMapper.MapperModelsUpdate(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                throw ex;
            }
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> LotTransaction(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Request> data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);
                List<SharedModelDTO.Models.LotBatch.LotBatchModel> LotBatchReturn = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                /* CONN DAO LOT - LOT */
                DAO.DataAccess.Services.DbPayOut LPDAO = new DAO.DataAccess.Services.DbPayOut();

                LotBatchReturn = LPDAO.DeleteLotTransaction(LotBatch, customer, TransactionMechanism);

                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> Response = LPMapper.MapperModelsDelete(LotBatchReturn);
                return Response;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                throw ex;
            }
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListTransaction(SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request data, string customer, string countryCode)
        {

            List<SharedModelDTO.Models.LotBatch.LotBatchModel> LotBatch;

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();
            lock (_lockerList)
            {
                if (countryCode == "ARG")
                {
                    LotBatch = dbPO.ListLotTransactionARG(data, customer);
                }
                else if (countryCode == "COL")
                {
                    LotBatch = dbPO.ListLotTransactionCOL(data, customer);
                }
                else if (countryCode == "BRA")
                {
                    LotBatch = dbPO.ListLotTransactionBRA(data, customer);
                }
                else if (countryCode == "URY")
                {
                    LotBatch = dbPO.ListLotTransactionURY(data, customer);
                }
                else if (countryCode == "CHL")
                {
                    LotBatch = dbPO.ListLotTransactionCHL(data, customer);
                }
                else if (countryCode == "MEX")
                {
                    LotBatch = dbPO.ListLotTransactionMEX(data, customer);
                }
                else
                {
                    LotBatch = null;
                }
            }


            SharedModelDTO.Models.LotBatch.LotBatchModel lotB;
            List<SharedModelDTO.Models.LotBatch.LotBatchModel> lotC = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            if(LotBatch != null)
            {
                foreach (var item in LotBatch)
                {
                    if (item.Transactions == null)
                    {
                        lotB = null;
                    }
                    else
                    {
                        lotB = item;

                        var trxsAgroupedByStatus = item.Transactions.GroupBy(x => x.Status);
                        if (trxsAgroupedByStatus.Count() > 1 && trxsAgroupedByStatus.Any(x => x.Key == "Rejected"))
                            lotB.Status = "EXEC_ERROR";
                        else if (trxsAgroupedByStatus.Count() > 1 && trxsAgroupedByStatus.Any(x => x.Key == "Executed"))
                            lotB.Status = "Executed";
                        else if (trxsAgroupedByStatus.Count() > 1 && trxsAgroupedByStatus.Any(x => x.Key == "InProgress"))
                            lotB.Status = "InProgress";
                        else
                            lotB.Status = trxsAgroupedByStatus.First().Key.ToUpper();
                    }
                    lotC.Add(lotB);
                }
            }

            return lotC;
        }

        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.ClientBalance> ClientBalance(string customer)
        {
            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            string clientBalanceString = dbPO.GetClientBalance(customer);

            List<SharedModel.Models.Services.Banks.Galicia.PayOut.ClientBalance> ClientBalance;

            ClientBalance = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Banks.Galicia.PayOut.ClientBalance>>(clientBalanceString);

            return ClientBalance;
        }

        static object obj = new object();
        public SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response DownloadLotBatchTransactionToBank(List<string> tickets, bool TransactionMechanism, string providerName)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Banks.Galicia.PayOut.DownloadLotBatchTransactionToBank.Response();
            DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

            string JSON = JsonConvert.SerializeObject(tickets);

            clsBS_ENCRIPT_PAP.BS_ENCRIPT_PAP clsEncrypt = new clsBS_ENCRIPT_PAP.BS_ENCRIPT_PAP();

            if (providerName == "BGALICIA")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankGALICIA(TransactionMechanism, JSON);
            }
            else if (providerName == "BSPVIELLE")
            {
                lock (obj)
                {
                    result = DbPayOut.DownloadBatchLotTransactionToBankSUPERVIELLE(TransactionMechanism, JSON);
                }
            }
            else if (providerName == "BBBVA")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBBVA(TransactionMechanism, JSON);
            }
            else if (providerName == "BBBVATP")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankBBVATuPago(TransactionMechanism, JSON);
            }
            else if (providerName == "SANTAR") 
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankSantanderARG(TransactionMechanism, JSON);
            }
            else if (providerName == "ITAU")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankItauARG(TransactionMechanism, JSON);
            }
            else if (providerName == "ICBC")
            {
                result = DbPayOut.DownloadBatchLotTransactionToBankIcbcARG(TransactionMechanism, JSON);
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

                        if (providerName == "BSPVIELLE")
                        {
                            LogService.LogInfo("Generating file for Supervielle bank");
                            string filesFolder = ConfigurationManager.AppSettings["BSPVIELLE_FILE_GEN_FOLDER"].ToString();
                            if (!Directory.Exists(filesFolder))
                            {
                                Directory.CreateDirectory(filesFolder);
                            }

                            string fileName = @"PAYOUT_ARG_" + providerName + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + ".txt";
                            string fullPath = Path.Combine(filesFolder, fileName);
                            
                            using (FileStream fs = File.Create(fullPath))
                            {
                                // Add some text to file    
                                Byte[] text = Encoding.ASCII.GetBytes(fullText);
                                fs.Write(text, 0, fullText.Length);
                            }

                            string encryptedFile = fileName.Replace(".txt", ".ENK");
                            string encryptedFullPath = Path.Combine(filesFolder, encryptedFile);
                            LogService.LogInfo("Encrypting file before uploading");
                            clsEncrypt.EncriptaArchivo(fullPath);

                            if (File.Exists(encryptedFullPath))
                            {
                                string host = ConfigurationManager.AppSettings["BSPVIELLE_SFTP_HOST"].ToString();
                                int port = int.Parse(ConfigurationManager.AppSettings["BSPVIELLE_SFTP_PORT"].ToString());
                                string username = ConfigurationManager.AppSettings["BSPVIELLE_SFTP_USER"].ToString();
                                string password = ConfigurationManager.AppSettings["BSPVIELLE_SFTP_PASSWORD"].ToString();
                                string workingDir = "IN";
                                LogService.LogInfo("File encrypted, starting upload");
                                SftpHelper.UploadFile(host, username, password, workingDir, encryptedFullPath, encryptedFile, port);
                                LogService.LogInfo("File uploaded succesfully to supervielle SFTP server. File : " + encryptedFullPath);
                            } else
                            {
                                result.StatusMessage = "Hubo un error al encriptar el archivo generado: " + fileName;
                            }
                        }

                        bytes = Encoding.ASCII.GetBytes(fullText);

                        //bytes = memory.ToArray();
                        result.FileBase64 = Convert.ToBase64String(bytes);

                        result.Lines = null;
                    }
                }
            }

            return result;
        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response UploadTxtFromGALICIA(SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerCode)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response();

            int FileStatusPosIni = 154;
            int FileStatusLength = 2;

            int FileStatusDescPosIni = 156;
            int FileStatusDescLength = 60;

            int ReferencePosIni = 107;
            int ReferenceLength = 22;

            int DetailPosIni = 154;
            int DetailLength = 2;

            int idxLine = 1;

            bool LotStatus = false;

            string FileStatus = "";
            string FileDescription = "";
            string DetailStatus = "";

            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\DEV-CONV.023052-ARCH.004746867 FINAL TREGGO (1).TXT"); /* PROBAR EN LOCAL */

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();

                        /* HEADER */
                        if (idxLine == 1)
                        {
                            FileStatus = line.Substring(FileStatusPosIni, FileStatusLength);
                            FileDescription = line.Substring(FileStatusDescPosIni, FileStatusDescLength);

                            BatchLotDetail.InternalStatus = FileStatus;
                            BatchLotDetail.InternalStatusDescription = FileDescription;
                        }

                        /* DETALLE */
                        if (line.Substring(0, 1) != "*")
                        {
                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            DetailStatus = line.Substring(DetailPosIni, DetailLength);

                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });            
                        }
                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromGALICIA(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                TicketsToCertificate tickets = new TicketsToCertificate();
                tickets.tickets = ResponseModel.TransactionDetail.Where(x => x.InternalStatus == "60").Select(x => x.Ticket).ToList();
                if(tickets.tickets.Count > 0)
                DbPayOut.GenerateCertificates(tickets);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerCode);

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

        public SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response UploadTxtFromBBVA(SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerCode)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response();

            int ReferencePosIni = 45;
            int ReferenceLength = 8;

            int DetailPosIni = 298;
            int DetailLength = 6;

            int idxLine = 1;

            bool LotStatus = false;

            string FileStatus = "";
            string FileDescription = "";
            string DetailStatus = "";

            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\DEV-CONV.023052-ARCH.004746867 FINAL TREGGO (1).TXT"); /* PROBAR EN LOCAL */

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
                        if (line.Substring(0, 7) == "0306020")
                        {
                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            DetailStatus = line.Substring(DetailPosIni, DetailLength).Trim();

                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }
                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBBVA(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                TicketsToCertificate tickets = new TicketsToCertificate();
                tickets.tickets = ResponseModel.TransactionDetail.Where(x => x.InternalStatus == "60").Select(x => x.Ticket).ToList();
                if (tickets.tickets.Count > 0)
                    DbPayOut.GenerateCertificates(tickets);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                //DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerCode);
                ResponseModel.BatchLotDetail = BatchLotDetail;
                ResponseModel.BatchLotDetail.InternalStatus = "OK";
                ResponseModel.BatchLotDetail.InternalStatusDescription = "Archivo procesado";
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

        public SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response UploadTxtFromBBVATuPago(SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerCode)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response();

            int ReferencePosIni = 45;
            int ReferenceLength = 8;

            int DetailPosIni = 298;
            int DetailLength = 6;

            int idxLine = 1;

            bool LotStatus = false;

            string FileStatus = "";
            string FileDescription = "";
            string DetailStatus = "";

            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

                byte[] bytes = Convert.FromBase64String(txtUpload.File);
                //byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\DEV-CONV.023052-ARCH.004746867 FINAL TREGGO (1).TXT"); /* PROBAR EN LOCAL */

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
                        if (line.Substring(0, 7) == "0306020")
                        {
                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            DetailStatus = line.Substring(DetailPosIni, DetailLength).Trim();

                            uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }
                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromBBVATuPago(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                TicketsToCertificate tickets = new TicketsToCertificate();
                tickets.tickets = ResponseModel.TransactionDetail.Where(x => x.InternalStatus == "60").Select(x => x.Ticket).ToList();
                if (tickets.tickets.Count > 0)
                    DbPayOut.GenerateCertificates(tickets);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                //DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerCode);
                ResponseModel.BatchLotDetail = BatchLotDetail;
                ResponseModel.BatchLotDetail.InternalStatus = "OK";
                ResponseModel.BatchLotDetail.InternalStatusDescription = "Archivo procesado";
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

        public SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response UploadTxtFromSantander(SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerCode)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response();

            int RecordType = 22;
            int RecordTypeLength = 1;

            int SecuencialRecordId = 42;
            int SecuencialRecordIdLength = 1;

            int ReferencePosIni = 224;
            int ReferenceLength = 14;

            int DetailPosIni = 105;
            int DetailLength = 1;

            int idxLine = 1;

            bool LotStatus = false;

            string FileStatus = "";
            string FileDescription = "";
            string DetailStatus = "";
            string Ticket = "";

            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail();
            List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();

            try
            {
                DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

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

                        string RecordTypeIdentf = line.Substring(RecordType, RecordTypeLength);
                        string SecuencialRecord = line.Substring(SecuencialRecordId, SecuencialRecordIdLength);

                        /* DETALLE */
                        if (RecordTypeIdentf == "D" && (SecuencialRecord == "0" || SecuencialRecord == "1"))
                        {
                            if (SecuencialRecord == "0")
                            {
                                Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            }
                            else
                            {
                                string AcreditationMark = line.Substring(DetailPosIni, DetailLength).Trim();
                                /* SET STATUS DETAIL */
                                DetailStatus = "PEND";
                                switch (AcreditationMark)
                                {
                                    case "A":
                                        DetailStatus = "EXECUTED";
                                        break;
                                    case "R":
                                        DetailStatus = line.Substring(185, 3).Trim();
                                        break;
                                    case "":
                                        DetailStatus = line.Substring(119, 3).Trim();
                                        if(DetailStatus == "005")
                                        {
                                            DetailStatus = "EXECUTED";
                                        }
                                        break;
                                }
                                uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });
                                Ticket = "";
                            }
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }
                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromSantander(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                TicketsToCertificate tickets = new TicketsToCertificate();
                tickets.tickets = ResponseModel.TransactionDetail.Where(x => x.InternalStatus == "60").Select(x => x.Ticket).ToList();
                if (tickets.tickets.Count > 0)
                    DbPayOut.GenerateCertificates(tickets);

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                //DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, providerCode);
                ResponseModel.BatchLotDetail = BatchLotDetail;
                ResponseModel.BatchLotDetail.InternalStatus = "OK";
                ResponseModel.BatchLotDetail.InternalStatusDescription = "Archivo procesado";
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

        public SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response UploadTxtFromSUPERVIELLE(SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode, string providerCode)
        {
            SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.Response();

            try
            {
                // Scanning SFTP for new files
                string bankFilesFolder = Path.Combine(ConfigurationManager.AppSettings["BSPVIELLE_FILE_GEN_FOLDER"].ToString(), "OUT");
                string host = ConfigurationManager.AppSettings["BSPVIELLE_SFTP_HOST"].ToString();
                int port = int.Parse(ConfigurationManager.AppSettings["BSPVIELLE_SFTP_PORT"].ToString());
                string username = ConfigurationManager.AppSettings["BSPVIELLE_SFTP_USER"].ToString();
                string password = ConfigurationManager.AppSettings["BSPVIELLE_SFTP_PASSWORD"].ToString();
                string workingDir = "OUT";

                if (!Directory.Exists(bankFilesFolder)) Directory.CreateDirectory(bankFilesFolder);

                string foundFile = SftpHelper.SearchAndDownloadFile(host, username, password, workingDir, bankFilesFolder, port);

                if (foundFile == String.Empty)
                {
                    ResponseModel.Status = "ERROR";
                    ResponseModel.StatusMessage = "No files found to process";
                    ResponseModel.Rows = 0;

                    return ResponseModel;
                }


                SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.BatchLotDetail();
                SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail();
                List<SharedModel.Models.Services.Payouts.Payouts.UploadModel> uploadModel = new List<SharedModel.Models.Services.Payouts.Payouts.UploadModel>();
                DAO.DataAccess.Services.DbPayOut DbPayOut = new DAO.DataAccess.Services.DbPayOut();

                using (TextFieldParser parser = new TextFieldParser(Path.Combine(bankFilesFolder, foundFile)))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(";");

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        string Ticket = fields[2];
                        int TicketNumber = Convert.ToInt32(Ticket);
                        string status = fields[3];

                        string DetailStatus;
                        if (status.ToUpper().Contains("RECHAZADA"))
                        {
                            DetailStatus = Regex.Match(status, @"\d+").Value;
                        } else
                        {
                            DetailStatus = "OK";
                        }

                        uploadModel.Add(new SharedModel.Models.Services.Payouts.Payouts.UploadModel() { ticket = Ticket, status = DetailStatus });
                    }
                }

                ResponseModel.TransactionDetail = DbPayOut.UpdateLotBatchTransactionFromSUPERVIELLE(txtUpload.CurrencyFxClose, TransactionMechanism, uploadModel);

                TicketsToCertificate tickets = new TicketsToCertificate();
                tickets.tickets = ResponseModel.TransactionDetail.Where(x => x.InternalStatus == "60").Select(x => x.Ticket).ToList();
                if (tickets.tickets.Count > 0)
                    DbPayOut.GenerateCertificates(tickets);

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
                ResponseModel.StatusMessage = "There was an error scaning sftp and downloading files";
                ResponseModel.Rows = 0;
            }

            return ResponseModel;
        }

        public SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog LogPayoutErrors(List<SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog> data, string customer, bool TransactionMechanism, string countryCode)
        {
            try
            {
                SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog LogResult = new SharedModel.Models.Services.Banks.Galicia.PayOut.ErrorsCreateLog();
                DAO.DataAccess.Services.DbPayOut LPDAO = new DAO.DataAccess.Services.DbPayOut();
                LogResult = LPDAO.LogPayoutErrors(data, customer, TransactionMechanism, countryCode);

                return LogResult;
            }
            catch (Exception ex)
            {
                LogService.LogError(ex.ToString());
                throw ex;
            }
        }


            public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ListNotificationTransaction(TransactionToNotify data, string customer, string countryCode)
            {

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> LotBatch;

                DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

                LotBatch = dbPO.ListTransactionsNotify(customer, countryCode, data);


                SharedModelDTO.Models.LotBatch.LotBatchModel lotB;
                List<SharedModelDTO.Models.LotBatch.LotBatchModel> lotC = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                if (LotBatch != null)
                {
                    foreach (var item in LotBatch)
                    {
                        if (item.Transactions == null)
                        {
                            lotB = null;
                        }
                        else
                        {
                            lotB = item;

                            var trxsAgroupedByStatus = item.Transactions.GroupBy(x => x.Status);
                            if (trxsAgroupedByStatus.Count() > 1 && trxsAgroupedByStatus.Any(x => x.Key == "Rejected"))
                                lotB.Status = "EXEC_ERROR";
                            else if (trxsAgroupedByStatus.Count() > 1 && trxsAgroupedByStatus.Any(x => x.Key == "Executed"))
                                lotB.Status = "Executed";
                            else if (trxsAgroupedByStatus.Count() > 1 && trxsAgroupedByStatus.Any(x => x.Key == "InProgress"))
                                lotB.Status = "InProgress";
                            else
                                lotB.Status = trxsAgroupedByStatus.First().Key.ToUpper();
                        }
                        lotC.Add(lotB);
                    }
                }

                return lotC;
            }
    }
}
