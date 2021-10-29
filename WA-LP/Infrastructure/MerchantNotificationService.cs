using Newtonsoft.Json;
using SharedBusiness.Log;
using SharedBusiness.Mail;
using SharedModel.Models.Services.Banks.Galicia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using static SharedModel.Models.Services.Payouts.Payouts;

namespace WA_LP.Infrastructure
{
    public static class MerchantNotificationService
    {
        public static async Task<int> PayoneerNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_URL"].ToString();
            string apiKey = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_API_KEY"].ToString();
            string handlerName = ConfigurationManager.AppSettings["PAYONEER_HANDLER_NAME"].ToString();
            string eventType = ConfigurationManager.AppSettings["PAYONEER_PAYMENT_EVENT"].ToString();
            string eventSubType = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_EVENTSUBTYPE"].ToString();

            string fullUrl = serviceUrl + "?apiKey=" + apiKey + "&handlerName=" + handlerName + "&eventType=" + eventType + "&eventSubType=" + eventSubType;

            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Starting Payoneer Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();
                
                // adding country code header
                client.DefaultRequestHeaders.Add("CountryCode", countryCode);

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(fullUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), fullUrl));

                if (response.IsSuccessStatusCode) { 
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("Payoneer Notification response: " + responseString.ToString());
                } else
                {
                    MailService.SendMail("Payoneer Callback request error", response.StatusCode.ToString(), supportEmails);
                    LogService.LogInfo("Payoneer Response was not successful");
                }

            } catch (Exception e)
            {
                LogService.LogError("There was an error sending Payoneer push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("Payoneer Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> RetrieveAndSendTransactionsPayoneer(TransactionToNotify transactions, string countryCode)
        {
            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process for country " + countryCode + ". Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();
                
                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                   await PayoneerNotificationSend(JsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }


            return 0;
        }

        public static async Task<int> NotifyCancelledTransactions(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {

            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to Payoneer API");

                    return await PayoneerNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> HandleNiumCancelCallback(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {
            try
            {
                LogService.LogInfo("Starting NIUM cancel Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to Payoneer API");

                    return await NiumNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }



            return 0;
        }

        public static async Task<int> HandleArfCancelCallback(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {
            try
            {
                LogService.LogInfo("Starting ARF cancel Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["ARF_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["ARF_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["ARF_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["ARF_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["ARF_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["ARF_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to Payoneer API");

                    return await NiumNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }



            return 0;
        }

        public static async Task<int> HandleRemiteeCancelCallback(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {
            try
            {
                LogService.LogInfo("Starting REMITEE cancel Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to REMITEE API");

                    return await NiumNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }



            return 0;
        }

        public static async Task<int> HandleThunesCancelCallback(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {
            try
            {
                LogService.LogInfo("Starting THUNES cancel Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to THUNES API");

                    return await ThunesNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }



            return 0;
        }

        public static List<string> GetTransactionsToNotifyJson(string countryCode, TransactionToNotify transactions, string customer)
        {
            SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

            List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListsToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

            PayoutListsToSend = BiPO.ListNotificationTransaction(transactions, customer, countryCode);

            List<string> JsonString = new List<string>();
            var splitValue = Int32.Parse(ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_QTY"]);
            var splitTransactions = PayoutListsToSend.Select((x, i) => new { Group = i / splitValue, Value = x })
             .GroupBy(item => item.Group, g => g.Value)
             .Select(g => g.Where(x => true)).ToList();

            foreach (var listTransactions in splitTransactions)
            {
                var PayoutListToSend = listTransactions.ToList();
                switch (countryCode)
                {
                    case "ARG":
                        SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapperArg = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();
                        var listResponseArg = LPMapperArg.MapperModelsBatch(PayoutListToSend);
                        if (listResponseArg.Count > 0) JsonString.Add(JsonConvert.SerializeObject(listResponseArg));
                        break;
                    case "COL":
                        SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia LPMapperCol = new SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia();
                        var listResponseCol = LPMapperCol.MapperModelsBatch(PayoutListToSend);
                        if (listResponseCol.Count > 0) JsonString.Add(JsonConvert.SerializeObject(listResponseCol));
                        break;
                    case "BRA":
                        SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil LPMapperBra = new SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil();
                        var listResponseBra = LPMapperBra.MapperModelsBatch(PayoutListToSend);
                        if (listResponseBra.Count > 0) JsonString.Add(JsonConvert.SerializeObject(listResponseBra));
                        break;
                    case "MEX":
                        SharedMaps.Converters.Services.Mexico.PayOutMapperMexico LPMapperMex = new SharedMaps.Converters.Services.Mexico.PayOutMapperMexico();
                        var listResponseMex = LPMapperMex.MapperModelsBatch(PayoutListToSend);
                        if (listResponseMex.Count > 0) JsonString.Add(JsonConvert.SerializeObject(listResponseMex));
                        break;
                    case "URY":
                        SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay LPMapperUry = new SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay();
                        var listResponseUry = LPMapperUry.MapperModelsBatch(PayoutListToSend);
                        if (listResponseUry.Count > 0) JsonString.Add(JsonConvert.SerializeObject(listResponseUry));
                        break;
                    case "CHL":
                        SharedMaps.Converters.Services.Chile.PayOutMapperChile LPMapperChl = new SharedMaps.Converters.Services.Chile.PayOutMapperChile();
                        var listResponseChl = LPMapperChl.MapperModelsBatch(PayoutListToSend);
                        if (listResponseChl.Count > 0) JsonString.Add(JsonConvert.SerializeObject(listResponseChl));
                        break;
                }
            }

            return JsonString;
        }

        public static async Task<int> HandleNiumCallback(string countryCode, TransactionToNotify transactions)
        {
            try
            {
                LogService.LogInfo("Starting NIUM Notification Process. Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();

                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                    await NiumNotificationSend(JsonString, countryCode);
                }
            } catch(Exception e)
            {
                LogService.LogError("There was an error retrieving NIUM Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> HandleArfCallback(string countryCode, TransactionToNotify transactions)
        {
            try
            {
                LogService.LogInfo("Starting ARF Notification Process. Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["ARF_NOTIFICATION_CUSTOMER"].ToString();
                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                    await ArfNotificationSend(JsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving NIUM Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> HandleRemiteeCallback(string countryCode, TransactionToNotify transactions)
        {
            try
            {
                LogService.LogInfo("Starting Remitee Notification Process. Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_CUSTOMER"].ToString();
                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                    await RemiteeNotificationSend(JsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving REMITEE Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> HandleThunesCallback(string countryCode, TransactionToNotify transactions)
        {
            try
            {
                LogService.LogInfo("Starting THUNES Notification Process. Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_CUSTOMER"].ToString();
                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                    await ThunesNotificationSend(JsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving THUNES Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> NiumNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_URL"].ToString();
            string apiKey = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_API_KEY"].ToString();
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();

            try
            {
                LogService.LogInfo("Starting NIUM Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();

                // adding country code header
                client.DefaultRequestHeaders.Add("x-api-key", apiKey);

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(serviceUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), serviceUrl));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("NIUM Notification response: " + responseString.ToString());
                }
                else
                {
                    MailService.SendMail("NIUM Callback request error", response.StatusCode.ToString(), supportEmails);
                    LogService.LogInfo("NIUM Response was not successful");
                }

            }
            catch (Exception e)
            {
                LogService.LogError("There was an error sending NIUM push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("NIUM Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> ArfNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["ARF_NOTIFICATION_URL"].ToString();
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Starting ARF Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(serviceUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), serviceUrl));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("ARF Notification response: " + responseString.ToString());
                }
                else
                {
                    LogService.LogInfo("ARF Response was not successful");
                    MailService.SendMail("ARF Callback request error", response.StatusCode.ToString(), supportEmails);
                }

            }
            catch (Exception e)
            {
                LogService.LogError("There was an error sending ARF push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("ARF Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> RemiteeNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_URL"].ToString();
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Starting REMITEE Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(serviceUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), serviceUrl));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("REMITEE Notification response: " + responseString.ToString());
                }
                else
                {
                    LogService.LogInfo("REMITEE Response was not successful");
                    MailService.SendMail("REMITEE Callback request error", response.StatusCode.ToString(), supportEmails);
                }

            }
            catch (Exception e)
            {
                LogService.LogError("There was an error sending REMITEE push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("REMITEE Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> ThunesNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_URL_" + countryCode].ToString();
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Starting THUNES Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(serviceUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), serviceUrl));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("THUNES Notification response: " + responseString.ToString());
                }
                else
                {
                    LogService.LogInfo("THUNES Response was not successful");
                    MailService.SendMail("THUNES Callback request error", response.StatusCode.ToString(), supportEmails);
                }

            }
            catch (Exception e)
            {
                LogService.LogError("There was an error sending THUNES push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("THUNES Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> HandleCancelTxsCallback(List<PayOut.Delete.Response> cancelledTxs, string customer)
        {
            string PayoneerColCustomerId = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_COL_CUSTOMER"].ToString();
            string PayoneerArgCustomerId = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_ARG_CUSTOMER"].ToString();
            string PayoneerMexCustomerId = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_MEX_CUSTOMER"].ToString();
            string PayoneerBraCustomerId = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_BRA_CUSTOMER"].ToString();
            string PayoneerUryCustomerId = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_URY_CUSTOMER"].ToString();
            string PayoneerChlCustomerId = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_CHL_CUSTOMER"].ToString();
            if (
                customer.Equals(PayoneerColCustomerId) ||
                customer.Equals(PayoneerArgCustomerId) ||
                customer.Equals(PayoneerMexCustomerId) ||
                customer.Equals(PayoneerBraCustomerId) ||
                customer.Equals(PayoneerUryCustomerId) ||
                customer.Equals(PayoneerChlCustomerId)
            )
            {
                await NotifyCancelledTransactions(cancelledTxs, customer);
            }


            string NiumColCustomerId = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_COL_CUSTOMER"].ToString();
            string NiumArgCustomerId = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_ARG_CUSTOMER"].ToString();
            string NiumMexCustomerId = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_MEX_CUSTOMER"].ToString();
            string NiumBraCustomerId = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_BRA_CUSTOMER"].ToString();
            string NiumUryCustomerId = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_URY_CUSTOMER"].ToString();
            string NiumChlCustomerId = ConfigurationManager.AppSettings["NIUM_NOTIFICATION_CHL_CUSTOMER"].ToString();
            if (
                customer.Equals(NiumColCustomerId) ||
                customer.Equals(NiumArgCustomerId) ||
                customer.Equals(NiumMexCustomerId) ||
                customer.Equals(NiumBraCustomerId) ||
                customer.Equals(NiumUryCustomerId) ||
                customer.Equals(NiumChlCustomerId)
            )
            {
                await HandleNiumCancelCallback(cancelledTxs, customer);
            }

            string ArfCustomerId = ConfigurationManager.AppSettings["ARF_NOTIFICATION_CUSTOMER"].ToString();
            if(customer.Equals(ArfCustomerId))
            {
                await HandleArfCancelCallback(cancelledTxs, customer);
            }

            string RemiteeCustomerId = ConfigurationManager.AppSettings["REMITEE_NOTIFICATION_CUSTOMER"].ToString();
            if (customer.Equals(RemiteeCustomerId))
            {
                await HandleRemiteeCancelCallback(cancelledTxs, customer);
            }

            string ThunesCustomerId = ConfigurationManager.AppSettings["THUNES_NOTIFICATION_CUSTOMER"].ToString();
            if (customer.Equals(ThunesCustomerId))
            {
                await HandleThunesCancelCallback(cancelledTxs, customer);
            }

            string InswitchCustomerId = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_CUSTOMER"].ToString();
            if (customer.Equals(InswitchCustomerId))
            {
                await HandleInswitchCancelCallback(cancelledTxs, customer);
            }

            string EFXCustomerId = ConfigurationManager.AppSettings["EFX_NOTIFICATION_CUSTOMER"].ToString();
            if (customer.Equals(EFXCustomerId))
            {
                await HandleEFXCancelCallback(cancelledTxs, customer);
            }

            return 0;
        }

        public static async Task<int> HandleInswitchCallback(string countryCode, TransactionToNotify transactions)
        {
            try
            {
                LogService.LogInfo("Starting INSWITCH Notification Process. Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_CUSTOMER"].ToString();

                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                    await InswitchNotificationSend(JsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving INSWITCH Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> InswitchNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_URL"].ToString();
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Starting INSWITCH Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(serviceUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), serviceUrl));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("INSWITCH Notification response: " + responseString.ToString());
                }
                else
                {
                    LogService.LogInfo("INSWITCH Response was not successful");
                    MailService.SendMail("INSWITCH Callback request error", response.StatusCode.ToString(), supportEmails);
                }

            }
            catch (Exception e)
            {
                LogService.LogError("There was an error sending INSWITCH push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("INSWITCH Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> HandleInswitchCancelCallback(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {
            try
            {
                LogService.LogInfo("Starting INSWITCH cancel Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["INSWITCH_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to INSWITCH API");

                    return await InswitchNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }



            return 0;
        }

        public static async Task<int> HandleEFXCallback(string countryCode, TransactionToNotify transactions)
        {
            try
            {
                LogService.LogInfo("Starting EFX Notification Process. Retrieving Updated transactions information.");
                string customer = ConfigurationManager.AppSettings["EFX_NOTIFICATION_CUSTOMER"].ToString();

                List<string> JsonList = GetTransactionsToNotifyJson(countryCode, transactions, customer);

                foreach (string JsonString in JsonList)
                {
                    await EFXNotificationSend(JsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving EFX Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }

        public static async Task<int> EFXNotificationSend(string body, string countryCode)
        {
            string serviceUrl = ConfigurationManager.AppSettings["EFX_NOTIFICATION_URL"].ToString();
            List<string> supportEmails = ConfigurationManager.AppSettings["SupportEmails"].Split(';').ToList();
            try
            {
                LogService.LogInfo("Starting EFX Push Notification. Data to be sent: " + body);
                HttpClient client = new HttpClient();

                var content = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(serviceUrl, content);

                LogService.LogInfo(string.Format("After post StatusCode: {0}, URL:{1}", response.StatusCode.ToString(), serviceUrl));

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    LogService.LogInfo("EFX Notification response: " + responseString.ToString());
                }
                else
                {
                    LogService.LogInfo("EFX Response was not successful");
                    MailService.SendMail("EFX Callback request error", response.StatusCode.ToString(), supportEmails);
                }

            }
            catch (Exception e)
            {
                LogService.LogError("There was an error sending EFX push notification. Exception message: " + e.Message.ToString());
                MailService.SendMail("EFX Callback request error", e.Message.ToString(), supportEmails);
            }

            return 1;
        }

        public static async Task<int> HandleEFXCancelCallback(List<SharedModel.Models.Services.Banks.Galicia.PayOut.Delete.Response> cancelledTxs, string customer)
        {
            try
            {
                LogService.LogInfo("Starting EFX cancel Notification Process. Retrieving Updated transactions information.");
                string idEntityUser = "";
                string countryCode = "";
                // get idEntityUser of first tx to know which country code we have to send
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
                {
                    connection.Open();
                    // truncate table
                    string sqlQuery = string.Format("SELECT idEntityUser FROM [LP_Operation].[Transaction] WHERE idTransaction = {0}", cancelledTxs[0].transaction_id);
                    using (var command = new SqlCommand(sqlQuery, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idEntityUser = reader.GetInt64(0).ToString();
                            }
                        }
                    }
                }

                string idEntityUserArg = ConfigurationManager.AppSettings["EFX_NOTIFICATION_ARG_ENTITY_ID"].ToString();
                string idEntityUserCol = ConfigurationManager.AppSettings["EFX_NOTIFICATION_COL_ENTITY_ID"].ToString();
                string idEntityUserBra = ConfigurationManager.AppSettings["EFX_NOTIFICATION_BRA_ENTITY_ID"].ToString();
                string idEntityUserMex = ConfigurationManager.AppSettings["EFX_NOTIFICATION_MEX_ENTITY_ID"].ToString();
                string idEntityUserUry = ConfigurationManager.AppSettings["EFX_NOTIFICATION_URY_ENTITY_ID"].ToString();
                string idEntityUserChl = ConfigurationManager.AppSettings["EFX_NOTIFICATION_CHL_ENTITY_ID"].ToString();

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";
                if (idEntityUser == idEntityUserUry) countryCode = "URY";
                if (idEntityUser == idEntityUserChl) countryCode = "CHL";

                if (string.IsNullOrEmpty(countryCode)) return 0;

                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                cancelledTxs.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.transaction_id.ToString());

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper LPMapper = new SharedMaps.Converters.Services.Banks.Galicia.PayOutMapper();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to EFX API");

                    return await EFXNotificationSend(jsonString, countryCode);
                }
            }
            catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }

            return 0;
        }
    }
}