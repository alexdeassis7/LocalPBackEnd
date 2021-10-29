using SharedBusiness.Log;
using SharedBusiness.Payin.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedModel.Models.Services.Payouts.Payouts;

namespace SharedBusiness.Services.Payouts
{
    public class BIPayOut
    {
        static readonly object _locker = new object();
        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ListTransaction(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {

            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> LotBatch;

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            if (countryCode == "MEX" && data.provider == "MIFEL")
            {
                LotBatch = dbPO.ListPayoOutsMEX(data, countryCode);
            }
            else if (countryCode == "MEX" && data.provider == "SABADELL")
            {
                LotBatch = dbPO.ListPayoOutsMEXSabadell(data, countryCode);
            }
            else if(countryCode == "COL" && data.provider == "FINANDINA")
            {
                LotBatch = dbPO.ListPayoOutsCOLFinandina(data, countryCode);
            }
            else if (countryCode == "COL" && data.provider == "BCSC")
            {
                LotBatch = dbPO.ListPayoOutsCOLCajaSocial(data, countryCode);
            }
            else if (countryCode == "COL" && data.provider == "ITAUCOL")
            {
                LotBatch = dbPO.ListPayoOutsCOLItau(data, countryCode);
            }
            else if (countryCode == "COL" && data.provider == "BBVACOL")
            {
                LotBatch = dbPO.ListPayoOutsCOLBbva(data, countryCode);
            }
            else 
            {
                LotBatch = dbPO.ListPayoOuts(data, countryCode);
            }

            return LotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ManageTransaction(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {

            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> LotBatch;

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            LotBatch = dbPO.ManagePayoOuts(data, countryCode);

            return LotBatch;
        }
        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> ReceivedPayouts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {

            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> LotBatch;

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            LotBatch = dbPO.ReceivedPayouts(data, countryCode);

            return LotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> GetOnHoldPayouts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {

            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> LotBatch;

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            LotBatch = dbPO.OnHoldPayouts(data, countryCode);

            return LotBatch;
        }

        public List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> GetExecutedPayouts(SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Request data, string countryCode)
        {

            List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> LotBatch;

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            LotBatch = dbPO.GetExecutedPayouts(data, countryCode);

            return LotBatch;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response UpdateTransaction(List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> data, string countryCode, string providerName, string internalStatus)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> LotBatch = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            try
            {
                foreach (var transaction in data)
                {
                    var detail = dbPO.UpdatePayoOuts(transaction.Ticket, countryCode, providerName, internalStatus);
                    ResponseModel.TransactionDetail.Add(detail);

                }

                if (ResponseModel.TransactionDetail.Count > 0)
                {
                    ResponseModel.Status = "OK";
                    ResponseModel.StatusMessage = "OK";

                    TicketsToCertificate tickets = new TicketsToCertificate();
                    tickets.tickets = ResponseModel.TransactionDetail.Where(x => x.InternalStatus == "60").Select(x => x.Ticket).ToList();
                    if (tickets.tickets.Count > 0)
                        dbPO.GenerateCertificates(tickets);

                }
                else {
                    ResponseModel.Status = "ERROR";
                    ResponseModel.StatusMessage = "An error ocurred processing a transaction";
                }
            }
            catch(Exception ex) {
                LogService.LogError(ex);
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "An error ocurred processing a transaction";
            }
            ResponseModel.Rows = ResponseModel.TransactionDetail.Count;

            return ResponseModel;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ReturnTransaction(List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> data, string countryCode, string providerName, string status)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> LotBatch = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            try
            {
                foreach (var transaction in data)
                {
                    var detail = dbPO.ReturnPayoOuts(transaction.Ticket, countryCode, providerName, status);
                    ResponseModel.TransactionDetail.Add(detail);

                }

                if (ResponseModel.TransactionDetail.Count > 0)
                {
                    ResponseModel.Status = "OK";
                    ResponseModel.StatusMessage = "OK";
                }
                else
                {
                    ResponseModel.Status = "ERROR";
                    ResponseModel.StatusMessage = "An error ocurred processing a transaction";
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "An error ocurred processing a transaction";
            }
            ResponseModel.Rows = ResponseModel.TransactionDetail.Count;

            return ResponseModel;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response CancelTransaction(List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> data, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> LotBatch = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            try
            {
                foreach (var transaction in data)
                {
                    var detail = dbPO.CancelPayouts(transaction.Ticket, countryCode);
                    ResponseModel.TransactionDetail.Add(detail);

                }

                if (ResponseModel.TransactionDetail.Count > 0)
                {
                    ResponseModel.Status = "OK";
                    ResponseModel.StatusMessage = "OK";

                }
                else
                {
                    ResponseModel.Status = "ERROR";
                    ResponseModel.StatusMessage = "An error ocurred processing a transaction";
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "An error ocurred processing a transaction";
            }
            ResponseModel.Rows = ResponseModel.TransactionDetail.Count;

            return ResponseModel;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ReceivedTransaction(List<SharedModel.Models.Services.Payouts.Payouts.ListPayoutsDownload.Response> data, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> LotBatch = new List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail>();

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            try
            {
                foreach (var transaction in data)
                {
                    var detail = dbPO.ReceivedPayouts(transaction.Ticket, countryCode);
                    ResponseModel.TransactionDetail.Add(detail);
                }

                if (ResponseModel.TransactionDetail.Count > 0)
                {
                    ResponseModel.Status = "OK";
                    ResponseModel.StatusMessage = "OK";
                }
                else
                {
                    ResponseModel.Status = "ERROR";
                    ResponseModel.StatusMessage = "An error ocurred processing a transaction";
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "An error ocurred processing a transaction";
            }
            ResponseModel.Rows = ResponseModel.TransactionDetail.Count;

            return ResponseModel;
        }

        public SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response RevertDownload(List<string> data, string countryCode)
        {
            SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response ResponseModel = new SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.Response();

            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();



            try
            {
                var detail = dbPO.RevertDownload(data, countryCode);
                ResponseModel.TransactionDetail = detail;

                if (ResponseModel.TransactionDetail.Count > 0)
                {
                    ResponseModel.Status = "OK";
                    ResponseModel.StatusMessage = "OK";

                }
                else
                {
                    ResponseModel.Status = "ERROR";
                    ResponseModel.StatusMessage = "An error ocurred processing a transaction";
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
                ResponseModel.Status = "ERROR";
                ResponseModel.StatusMessage = "An error ocurred processing a transaction";
            }
            ResponseModel.Rows = ResponseModel.TransactionDetail.Count;

            return ResponseModel;
        }

        public SharedModelDTO.Models.LotBatch.LotBatchModel CreateLotTransaction(SharedModelDTO.Models.LotBatch.LotBatchModel LotBatch, string customer, string countryCode, bool TransactionMechanism) 
        {
            lock (_locker)
            {
                switch (countryCode)
                {
                    case "ARG":
                        DAO.DataAccess.Services.DbPayOut LPDAO = new DAO.DataAccess.Services.DbPayOut();
                        LotBatch = LPDAO.CreateLotTransaction(LotBatch, customer, TransactionMechanism, countryCode);
                        break;
                    case "COL":
                        DAO.DataAccess.Services.Colombia.DbPayOutColombia LPDAOCOL = new DAO.DataAccess.Services.Colombia.DbPayOutColombia();
                        LotBatch = LPDAOCOL.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                        break;
                    case "BRA":
                        DAO.DataAccess.Services.Brasil.DbPayOutBrasil LPDAOBRA = new DAO.DataAccess.Services.Brasil.DbPayOutBrasil();
                        LotBatch = LPDAOBRA.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                        break;
                    case "MEX":
                        DAO.DataAccess.Services.Mexico.DbPayOutMexico LPDAOMEX = new DAO.DataAccess.Services.Mexico.DbPayOutMexico();
                        LotBatch = LPDAOMEX.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                        break;
                    case "URY":
                        DAO.DataAccess.Services.Uruguay.DbPayOutUruguay LPDAOURY = new DAO.DataAccess.Services.Uruguay.DbPayOutUruguay();
                        LotBatch = LPDAOURY.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                        break;
                    case "CHL":
                        DAO.DataAccess.Services.Chile.DbPayOutChile LPDAOCHL = new DAO.DataAccess.Services.Chile.DbPayOutChile();
                        LotBatch = LPDAOCHL.CreateLotTransaction(LotBatch, customer, countryCode, TransactionMechanism);
                        break;
                }
            }
            return LotBatch;
        }

        public int GetLastLotOutNumber()
        {
            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            return dbPO.GetLastLotOutNumber();
        }

        public List<string> HoldTransactions(List<string> tickets, string countryCode)
        {
            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();
            List<string> ticketResponse = new List<string>();

            ticketResponse = dbPO.HoldTransactions(tickets,countryCode);

            return ticketResponse;
        }

        public List<SharedModelDTO.Models.LotBatch.LotBatchModel> ChangeTxtToRecieved(List<string> tickets, string countryCode, string customer)
        {
            DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

            var lotBatchModels = dbPO.ChangeTxtToRecieved(tickets, countryCode, customer);

            return lotBatchModels;
        }


        public void RejectExpiredOnHold() 
        {
            try
            {
                DAO.DataAccess.Services.DbPayOut dbPO = new DAO.DataAccess.Services.DbPayOut();

                dbPO.RejectExpiredOnHold();
            }
            catch (Exception e) 
            {
                LogService.LogError(e);
            }
        }
    }
}
