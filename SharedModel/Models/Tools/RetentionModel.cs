using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Tools
{
    public class RetentionModel
    {

        public class Upload
        {
            public string File { get; set; }
            public string FileName { get; set; }
        }

        public class ReadFile
        {
            public string Status { get; set; }
            public string StatusMessage { get; set; }

            public string ini { get; set; }
            public string fin { get; set; }

        }

        public class TransactionResultAfip
        {
            public string cuit { get; set; }

            public string Denominacion { get; set; }

            public string ImpGan { get; set; }

            public string ImpIva { get; set; }

            public string Monotributo { get; set; }

            public string IntegranteSoc { get; set; }

            public string Empleador { get; set; }

            public string ActvidadMonotr { get; set; }
        }
        public class TransactionResultArba
        {
            public string Regimen { get; set; }

            public string Cuit { get; set; }

            public string RazonSocial { get; set; }

            public string Letra { get; set; }

            public string FechaVigencia { get; set; }
        }


        public class RetentionCertificate
        {
            public class Response
            {
                public int idTransactionCertificate { get; set; }
                public byte[] FileBytes { get; set; }
                public string FileName { get; set; }
            }
        }

        public class RetentionMonthly
        {

            public string CodComprobante { get; set; }
            public string FechaEmision { get; set; }
            public string NumComprobante { get; set; }

            public string ImporteComprbante { get; set; }
            public string CodImpuesto { get; set; }
            public string CodRegimen { get; set; }
            public string CodOPeración { get; set; }
            public string BaseCalculo { get; set; }
            public string FechaEmisionRet { get; set; }
            public string CodCondicion { get; set; }
            public string SujetoSuspen { get; set; }
            public string ImporteRet { get; set; }
            public string PorcExclusion { get; set; }
            public string FechaemiBoletin { get; set; }
            public string TipoDocRet { get; set; }
            public string NumDocRet { get; set; }
            public string NumCertOrig { get; set; }

        }

        public class RetentionARBAMonthly
        {

            public string CUIT { get; set; }
            public string WithholdingDate { get; set; }
            public string BranchNumber { get; set; }

            public string EmisionNumber { get; set; }
            public string Amount { get; set; }
            public string OperationType { get; set; }

        }

        public class RetentionFiles
        {

            public class Request
            {

                public int month { get; set; }
                public int year { get; set; }
                public string typeFile { get; set; }
            }

            public class ResponseTxt
            {
                public int Rows { get; set; }
                public string[] Lines { get; set; }
                public string Status { get; set; }
                public string StatusMessage { get; set; }
                public string FileBase64 { get; set; }

            }


            public class ResponseExcel
            {
                public string ListRetentions { get; set; }

            }

        }

        public class Whitelist
        {

            public class Request
            {

                public string IdentificationNumber { get; set; }
                public string idCountry { get; set; }
                public string idEntityUser { get; set; }
                public string idEntitySubMerchant { get; set; }
                public string idRetentionType { get; set; }
                public string offset { get; set; }
                public string pageSize { get; set; }
                //public string typeFile { get; set; }
            }

            public class Response
            {
                public string ListWhitelist { get; set; }
                public string Status { get; set; }
                public string StatusMessage { get; set; }
                public string idWhitelist { get; set; }

            }


        }

        public class WhiteList
        {
            public string idWhitelist { get; set; }
            public string Numberdoc { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public Decimal idEntityIdentificationType { get; set; }
            public List<DetailWL> Details { get; set; }
            public string Status { get; set; }
            public string StatusMessage { get; set; }
           

        }

        public class DetailWL
        {
            public bool Active { get; set;  }
            public string idWhiteListRetentionType { get; set; }
            //public string idWhitelist { get; set; }
            public string idRetentionType { get; set; }
            public string idEntitySubMerchant { get; set; }

        }

        public class RetentionProcessStatus
        {
            public string Description { get; set; }
            public bool ServiceIsRunning { get; set; }
            public int TotalOfCertificates { get; set; }
            public int CertificatesPendingGeneration { get; set; }
            public int CertificatesPendingDownload { get; set; }
            public string ProcessStart { get; set; }
            public string ProcessEnd { get; set; }

            public static implicit operator List<object>(RetentionProcessStatus v)
            {
                throw new NotImplementedException();
            }
        }

        public class Refund { 

            public class Request {

                public string transactionIds { get; set; }

            }
        }




    }
}
