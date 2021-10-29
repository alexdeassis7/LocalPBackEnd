using Newtonsoft.Json;
using SharedModelDTO.Models.Transaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityTools = Tools;


namespace SharedBusiness.Tools
{
    public class BIRetention
    {
        public string ListStatusCashPayments(string customer_id)
        {
            string Result = string.Empty;
            try
            {
                DAO.DataAccess.Services.DbCashPayment DbCashPayment = new DAO.DataAccess.Services.DbCashPayment();
                Result = DbCashPayment.ListStatusCashPayments(customer_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Result;
        }

        public bool ValidFile(string filename)
        {
            bool isValid = false;

            try
            {
                DAO.DataAccess.Services.DbFile DbFile = new DAO.DataAccess.Services.DbFile();
                isValid = DbFile.ValidFile(filename);
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            return isValid;
        }
        public SharedModel.Models.Tools.RetentionModel.ReadFile ReadAfipFile(string file, string FileName, string customer, Boolean TransactionMechanism, string datetime)
        {
            SharedModel.Models.Tools.RetentionModel.ReadFile ReadFile = new SharedModel.Models.Tools.RetentionModel.ReadFile();

            List<SharedModel.Models.Tools.RetentionModel.TransactionResultAfip> TransactionResult = new List<SharedModel.Models.Tools.RetentionModel.TransactionResultAfip>();
            SharedModel.Models.Tools.RetentionModel.TransactionResultAfip ObjTrProcessDetail;

            /* SE SETEA LA POSICION Y LA LONGITUD DEL ACCOUNTNUMBER(CLIENTE EN LA GENERACION DEL BARCODE, QUE ES EL TICKET MAS TRES CEROS) */
            int CUITIni = 0;
            int CUITLenght = 11;

            int DenomintationPosIni = 11;
            int DenomintationLength = 30;

            int ImpGangIni = 41;
            int ImpGanLength = 2;

            int IMPIVAIni = 43;
            int IMPIVALength = 2;

            int MonotriIni = 45;
            int MonotriLength = 2;

            int IntegSocIni = 47;
            int IntegSocLength = 1;

            int EmpleadorIni = 48;
            int EmpleadorLength = 1;

            int ActMonotriIni = 49;
            int ActMonotriLength = 2;

            try
            {
                /* BASE64 A BYTES[] */
                //byte[] bytes = Convert.FromBase64String(file);
                byte[] bytes = File.ReadAllBytes(@"C:\Users\FEDE\Downloads\apellidoNombreDenominacion\utlfile\padr\SELE-SAL-CONSTA.p20out1.20190720 - Copy.txt"); /* PROBAR EN LOCAL */

                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */
                        line = stream.ReadLine();
                        ObjTrProcessDetail = new SharedModel.Models.Tools.RetentionModel.TransactionResultAfip();

                        string Cuit = line.Substring(CUITIni, CUITLenght);
                        string Denominacion = line.Substring(DenomintationPosIni, DenomintationLength);
                        string ImpGan = line.Substring(ImpGangIni, ImpGanLength);
                        string ImpIva = line.Substring(IMPIVAIni, IMPIVALength);
                        string Monotributo = line.Substring(MonotriIni, MonotriLength);
                        string IntegranteSoc = line.Substring(IntegSocIni, IntegSocLength);
                        string Empleador = line.Substring(EmpleadorIni, EmpleadorLength);
                        string ActvidadMonotr = line.Substring(ActMonotriIni, ActMonotriLength);

                        ObjTrProcessDetail.cuit = Cuit;
                        ObjTrProcessDetail.Denominacion = Denominacion;
                        ObjTrProcessDetail.ImpGan = ImpGan;
                        ObjTrProcessDetail.ImpIva = ImpIva;
                        ObjTrProcessDetail.Monotributo = Monotributo;
                        ObjTrProcessDetail.IntegranteSoc = IntegranteSoc;
                        ObjTrProcessDetail.Empleador = Empleador;
                        ObjTrProcessDetail.ActvidadMonotr = ActvidadMonotr;

                        TransactionResult.Add(ObjTrProcessDetail);
                    }
                }

                DAO.DataAccess.Tools.DBRetention DBAfip = new DAO.DataAccess.Tools.DBRetention();

                DBAfip.Clean_All("AFIP");

                int countCuit = TransactionResult.Count();

                for (int i = 0; i <= countCuit; i = i + 10000)
                {
                    DBAfip.Insert_Batch(customer, TransactionResult.Skip(i).Take(10000).ToList(), TransactionMechanism, datetime);
                }

                ReadFile.Status = "OK";
                ReadFile.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                ReadFile.Status = "ERROR";
                ReadFile.StatusMessage = ex.Message;
            }
            return ReadFile;
        }

        public SharedModel.Models.Tools.RetentionModel.ReadFile ReadArbaFile(string file, string FileName, string customer, Boolean TransactionMechanism, string datetime)
        {
            SharedModel.Models.Tools.RetentionModel.ReadFile ReadFile = new SharedModel.Models.Tools.RetentionModel.ReadFile();

            List<SharedModel.Models.Tools.RetentionModel.TransactionResultArba> TransactionResultArba = new List<SharedModel.Models.Tools.RetentionModel.TransactionResultArba>();
            SharedModel.Models.Tools.RetentionModel.TransactionResultArba ObjTrProcessDetail;

            try
            {
                /* BASE64 A BYTES[] */
                //byte[] bytes = Convert.FromBase64String(file);
                byte[] bytes = File.ReadAllBytes(@"D:\1_CLIENTES\LOCALPAYMENT\PROVEEDORES\ARBA\PadronIntermediarios092019.TXT"); /* PROBAR EN LOCAL */

                int REGIni = 0;
                int REGLenght = 2;

                int CUITIni = 2;
                int CUITLenght = 11;

                int RSOCIni = 13;
                int RSOCLenght = 44;

                int LETRAIni = 57;
                int LETRALenght = 1;

                int PERVIGIni = 58;
                int PERVIGLenght = 6;


                using (var stream = new StreamReader(new MemoryStream(bytes)))
                {
                    /* SE DECLARA UN STRING PARA OBTENER CADA LINEA DEL ARCHIVO */
                    string line;

                    /* WHILE MIENTRAS QUE NO SE TERMINE EL ARCHIVO */
                    while (!stream.EndOfStream)
                    {
                        /* VA LEYENDO LINEA POR LINEA Y SETEANDOLA A LA VARIABLE LINE */

                        

                        line = stream.ReadLine();
                        var properties = line.Split(';');

                        ObjTrProcessDetail = new SharedModel.Models.Tools.RetentionModel.TransactionResultArba();


                        ObjTrProcessDetail.Regimen          = line.Substring(REGIni, REGLenght); 
                        ObjTrProcessDetail.Cuit             = line.Substring(CUITIni, CUITLenght);
                        ObjTrProcessDetail.RazonSocial      = line.Substring(RSOCIni, RSOCLenght);
                        ObjTrProcessDetail.Letra            = line.Substring(LETRAIni, LETRALenght);
                        ObjTrProcessDetail.FechaVigencia = line.Substring(PERVIGIni, PERVIGLenght);


                        TransactionResultArba.Add(ObjTrProcessDetail);
                    }
                }

                DAO.DataAccess.Tools.DBRetention DBArba = new DAO.DataAccess.Tools.DBRetention();

                DBArba.Clean_All("ARBA");

                int countCuit = TransactionResultArba.Count();

                for (int i = 0; i <= countCuit; i = i + 10000)
                {
                    DBArba.Insert_Batch(customer, TransactionResultArba.Skip(i).Take(10000).ToList(), TransactionMechanism, datetime);
                }

                ReadFile.Status = "OK";
                ReadFile.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                ReadFile.Status = "ERROR";
                ReadFile.StatusMessage = ex.Message;
            }
            return ReadFile;
        }

        public SharedModelDTO.File.FileModel DownloadRetentionCertificates(string customer_id, string retentionType)
        {
            try
            {
                SharedModelDTO.File.FileModel zip = new SharedModelDTO.File.FileModel();
                DAO.DataAccess.Services.DbFile dbFile = new DAO.DataAccess.Services.DbFile();
                List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response> certificates = new List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>();
                certificates.AddRange(dbFile.DownloadRetentionCertificates(customer_id, retentionType));

                if (certificates.Count() > 0)
                {
                    List<SharedModelDTO.File.FileModel> compressedFiles = new List<SharedModelDTO.File.FileModel>();

                    foreach (SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response item in certificates)
                    {
                        SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                        FileDTO.file_bytes = item.FileBytes;
                        FileDTO.transaction_type = "RETENTION";
                        FileDTO.datetime_process = DateTime.Now.ToString("yyyyMMddhhmmss");
                        FileDTO.file_extension = "pdf";
                        FileDTO.file_name = item.FileName;
                        compressedFiles.Add(FileDTO);
                    }

                    zip = UtilityTools.File.Zip.FileCompression.FilesToZIPFile(compressedFiles, "RETENTIONS");

                    DAO.DataAccess.Tools.DBRetention dbafip = new DAO.DataAccess.Tools.DBRetention();

                    foreach (var item in certificates)
                    {
                        dbafip.ChangeRetentionCertificateStatus(item.idTransactionCertificate);
                    }
                }
                else
                {
                    zip.HasFile = false;
                }
                return zip;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModelDTO.File.FileModel DownloadRetentionCertificates(RetentionsByDateDownloadModel transactions, string certificateNumber = null, long? internalDescriptionMerchantId = null, long? payoutId = null)
        {
            try
            {
                SharedModelDTO.File.FileModel zip = new SharedModelDTO.File.FileModel();
                DAO.DataAccess.Services.DbFile dbFile = new DAO.DataAccess.Services.DbFile();
                List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response> certificates = new List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response>();

                if(transactions.GetType() != typeof(RetentionsByFilterDownloadModel))
                {
                    certificates.AddRange(dbFile.DownloadRetentionCertificates(transactions.Merchant.Id, transactions.Organism.Id, transactions.date));
                }
                else
                {
                    certificates.AddRange(dbFile.DownloadRetentionCertificates(transactions.Merchant.Id, transactions.Organism.Id, transactions.date, certificateNumber, internalDescriptionMerchantId, payoutId));
                }

                if (certificates.Count() > 0)
                {
                    List<SharedModelDTO.File.FileModel> compressedFiles = new List<SharedModelDTO.File.FileModel>();

                    foreach (SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response item in certificates)
                    {
                        SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                        FileDTO.file_bytes = item.FileBytes;
                        FileDTO.transaction_type = "RETENTION";
                        FileDTO.datetime_process = DateTime.Now.ToString("yyyyMMddhhmmss");
                        FileDTO.file_extension = "pdf";
                        FileDTO.file_name = item.FileName;
                        compressedFiles.Add(FileDTO);
                    }

                    zip = UtilityTools.File.Zip.FileCompression.RetentionsToZIPFile(compressedFiles, transactions);

                    DAO.DataAccess.Tools.DBRetention dbafip = new DAO.DataAccess.Tools.DBRetention();

                    foreach (var item in certificates)
                    {
                        dbafip.ChangeRetentionCertificateStatus(item.idTransactionCertificate);
                    }
                }
                else
                {
                    zip.HasFile = false;
                }
                return zip;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TransAgForRetentionsModel> GetTransactionsAgroupedForRetentions(TransAgForRetentionsFilterModel filter)
        {
            DAO.DataAccess.Tools.DBRetention dBRetention = new DAO.DataAccess.Tools.DBRetention();
            var result = dBRetention.GetTransactionsAgroupedForRetentions(filter);

            return result;
        }
        public SharedModel.Models.Tools.RetentionModel.Whitelist.Response CreateWhiteListMember(SharedModel.Models.Tools.RetentionModel.WhiteList data, string customer )
        {
            SharedModel.Models.Tools.RetentionModel.Whitelist.Response WhiteList = new SharedModel.Models.Tools.RetentionModel.Whitelist.Response();

            try
            {
                DAO.DataAccess.Tools.DBRetention dBRetention = new DAO.DataAccess.Tools.DBRetention();
                WhiteList = dBRetention.Insert_Cuit_To_Whitelist(data, customer);

                //WhiteList.Status = "OK";
                //WhiteList.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                WhiteList.Status = "ERROR";
                WhiteList.StatusMessage = ex.Message;
                
            }
            return WhiteList;
        }
        public SharedModel.Models.Tools.RetentionModel.WhiteList UpdateWhiteListMember(string customer, SharedModel.Models.Tools.RetentionModel.WhiteList whiteList, string updateType)
        {
            
            //string Json = JsonConvert.SerializeObject(whiteList);
            SharedModel.Models.Tools.RetentionModel.WhiteList response = new SharedModel.Models.Tools.RetentionModel.WhiteList();

            try
            {
                DAO.DataAccess.Tools.DBRetention dBRetention = new DAO.DataAccess.Tools.DBRetention();
                dBRetention.Update_Cuit_To_Whitelist(customer, whiteList, updateType);

                response.Status = "OK";
                response.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                response.Status = "ERROR";
                response.StatusMessage = ex.Message;
            }
            return response;
        }
        public SharedModel.Models.Tools.RetentionModel.WhiteList DeleteWhiteListMember(string Numberdoc, string customer)
        {
            SharedModel.Models.Tools.RetentionModel.WhiteList WhiteList = new SharedModel.Models.Tools.RetentionModel.WhiteList();

            try
            {
                DAO.DataAccess.Tools.DBRetention dBRetention = new DAO.DataAccess.Tools.DBRetention();
                dBRetention.Delete_Cuit_To_Whitelist(Numberdoc,customer);

                WhiteList.Status = "OK";
                WhiteList.StatusMessage = "OK";
            }
            catch (Exception ex)
            {
                WhiteList.Status = "ERROR";
                WhiteList.StatusMessage = ex.Message;
            }
            return WhiteList;
        }

        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt DownloadTxtAfipMonthly(string customer_id, int year, int month, string typeFile)
        {
            DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();
            SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt result = DAORetention.DownloadTxtAfipMonthly(customer_id, year, month, typeFile);

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
                        bytes = memory.ToArray();
                        result.FileBase64 = Convert.ToBase64String(bytes);
                        result.Lines = null;
                    }
                }
            }

            return result;


        }


        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel DownloadExcelAfipMonthly(string customer_id, int year, int month, string typeFile)
        {
            try
            {

                DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();
                SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel result = DAORetention.DownloadExcelAfipMonthly(customer_id, year, month, typeFile);
                return result;

            }
            catch (Exception ex) 
            {

                throw;
            }
   
            //return new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt();


        }

        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt DownloadTxtArbaMonthly(string customer_id, int year, int month, string typeFile)
        {
            DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();
            SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt result = DAORetention.DownloadTxtArbaMonthly(customer_id, year, month, typeFile);

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
                        bytes = memory.ToArray();
                        result.FileBase64 = Convert.ToBase64String(bytes);
                        result.Lines = null;
                    }
                }
            }

            return result;


        }

        public SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel DownloadExcelArbaMonthly(string customer_id, int year, int month, string typeFile)
        {
            try
            {

                DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();
                SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseExcel result = DAORetention.DownloadExcelArbaMonthly(customer_id, year, month, typeFile);
                return result;

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public SharedModel.Models.Tools.RetentionModel.Whitelist.Response ListWhitelist(string customer, SharedModel.Models.Tools.RetentionModel.Whitelist.Request data, string countryCode)
        {
            try
            {

                DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();
                SharedModel.Models.Tools.RetentionModel.Whitelist.Response result = DAORetention.ListWhitelist(customer, data, countryCode);
                return result;

            }
            catch (Exception ex)
            {
                throw;
            }

            //return new SharedModel.Models.Tools.RetentionModel.RetentionFiles.ResponseTxt();

        }

        public List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus> ListCertificatesProcessInfo ()
            { 
                try
                {
                    DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();
                    List<SharedModel.Models.Tools.RetentionModel.RetentionProcessStatus> result = DAORetention.ListCertificatesProcessInfo();
                    return result;

                }
                catch (Exception ex)
                {
                    throw;
                }

            }

        public SharedModel.Models.View.Report.List.Response ListRetentions(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            try
            {
                DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();

                SharedModel.Models.View.Report.List.Response Response = DAORetention.ListRetentions(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response RefundRetentions(SharedModel.Models.Tools.RetentionModel.Refund.Request data, string customer)
        {
            try
            {
                DAO.DataAccess.Tools.DBRetention DAORetention = new DAO.DataAccess.Tools.DBRetention();

                SharedModel.Models.View.Report.List.Response Response = DAORetention.RefundRetentions(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModelDTO.File.FileModel DownloadCertificates(List<long> idTransactions)
        {
            try
            {
                SharedModelDTO.File.FileModel zip = new SharedModelDTO.File.FileModel();
                DAO.DataAccess.Services.DbFile dbFile = new DAO.DataAccess.Services.DbFile();
                List<SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response> certificates = dbFile.DownloadCertificates(idTransactions);

                if (certificates.Count() > 0)
                {
                    List<SharedModelDTO.File.FileModel> compressedFiles = new List<SharedModelDTO.File.FileModel>();

                    foreach (SharedModel.Models.Tools.RetentionModel.RetentionCertificate.Response item in certificates)
                    {
                        SharedModelDTO.File.FileModel FileDTO = new SharedModelDTO.File.FileModel();
                        FileDTO.file_bytes = item.FileBytes;
                        FileDTO.transaction_type = "CERTIFICATES";
                        FileDTO.datetime_process = DateTime.Now.ToString("yyyyMMddhhmmss");
                        FileDTO.file_extension = "pdf";
                        FileDTO.file_name = item.FileName;
                        compressedFiles.Add(FileDTO);
                    }

                    zip = UtilityTools.File.Zip.FileCompression.FilesToZIPFile(compressedFiles, "CERTIFICATES");
                }
                else
                {
                    zip.HasFile = false;
                }
                return zip;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
