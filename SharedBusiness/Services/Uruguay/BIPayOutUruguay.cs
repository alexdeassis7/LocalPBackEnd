using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityTools = Tools;

namespace SharedBusiness.Services.Uruguay
{
    public class BIPayOutUruguay
    {
        static readonly object _locker = new object();
        public List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response> LotTransaction(List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Request> data, string customer, bool TransactionMechanism, string countryCode)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay LPMapper = new SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay();
                SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                SharedBusiness.Services.Payouts.BIPayOut LPDAO = new SharedBusiness.Services.Payouts.BIPayOut();
                lock (_locker)
                {
                    LotBatch = LPDAO.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                }

                /* CASTEO LOT - RESPONSE */
                List<SharedModel.Models.Services.Uruguay.PayOutUruguay.Create.Response> Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response DownloadLotBatchTransactionToBank(List<string> tickets, bool TransactionMechanism)
        {
            DAO.DataAccess.Services.Uruguay.DbPayOutUruguay DbPayOut = new DAO.DataAccess.Services.Uruguay.DbPayOutUruguay();
            SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response result = new SharedModel.Models.Services.Uruguay.PayOutUruguay.DownloadLotBatchTransactionToBank.Response();

            string JSON = JsonConvert.SerializeObject(tickets);

            result = DbPayOut.DownloadBatchLotTransactionToBank(TransactionMechanism, JSON);

            if (result.RowsPayouts > 0)
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
            }

            if (result.RowsBrou > 0)
            {
                //Brou Txt
                byte[] bytesBeneficiaries = null;
                using (MemoryStream memory = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(memory, Encoding.GetEncoding(437)))
                    {
                        for (int i = 0; i < result.LinesBrou.Length; i++)
                        {
                            writer.WriteLine(result.LinesBrou[i]);
                        }

                        writer.Flush();
                        memory.Position = 0;
                        bytesBeneficiaries = memory.ToArray();
                        result.FileBase64_PayoutsBrou = Convert.ToBase64String(bytesBeneficiaries);
                        result.LinesBrou = null;
                    }
                }
            }
            return result;
        }

        public SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.Response UploadTxtFromBank(SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.Request txtUpload, string datetime, bool TransactionMechanism, string countryCode)
        {
            SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.Response();

            int ReferencePosIni = 116;             //REFERENCE: N° of ticket
            int ReferenceLength = 21;

            int DetailCodePosIni = 138;           //DETAIL CODE
            int DetailCodeLength = 3;

            int idxLine = 1;

            string FileStatus = "Upload";

            string DetailStatus = "";

            SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.BatchLotDetail BatchLotDetail = new SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.BatchLotDetail();
            SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.TransactionDetail Detail = new SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.TransactionDetail();

            try
            {
                DAO.DataAccess.Services.Uruguay.DbPayOutUruguay DbPayOut = new DAO.DataAccess.Services.Uruguay.DbPayOutUruguay();

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
                        if (line.Substring(0, 1) != "*")
                        {
                            string Ticket = line.Substring(ReferencePosIni, ReferenceLength);
                            DetailStatus = line.Substring(DetailCodePosIni, DetailCodeLength);

                            Detail = DbPayOut.UpdateLotBatchTransactionFromBank(Ticket, txtUpload.CurrencyFxClose, DetailStatus, TransactionMechanism);
                            ResponseModel.TransactionDetail.Add(Detail);
                        }

                        /* SE INCREMENTA EL CONTADOR DE LINEAS */
                        idxLine += 1;
                    }
                }

                SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                FileDTO.file_bytes = bytes;
                FileDTO.transaction_type = "PODEPO";
                FileDTO.datetime_process = datetime;

                FileDTO = UtilityTools.File.Zip.FileCompression.ZipFileToBytes(FileDTO);

                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                DbFile.SaveFile(FileDTO, FileStatus, FileDTO.file_name, countryCode, null);

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
