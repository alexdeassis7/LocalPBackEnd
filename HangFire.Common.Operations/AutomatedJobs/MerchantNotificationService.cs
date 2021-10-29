using Dapper;
using Newtonsoft.Json;
using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace HangFire.Common.Operations.AutomatedJobs
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
                    LogService.LogInfo("Payoneer Response was not successful");
                }

            } catch (Exception e)
            {
                LogService.LogError("There was an error sending Payoneer push notification. Exception message: " + e.Message.ToString());
            }

            return 1;
        }

        public static async Task SetNotificationPushSent(IEnumerable<int> transactions)
        {
            var sp = "[LP_Operation].[Set_Notification_Push_Sent]";
            using (var _conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connDbLocalPayment"].ConnectionString))
            {
                _conn.Open();
                await _conn.ExecuteAsync(sp, new { JSON = JsonConvert.SerializeObject(transactions) }, commandType: CommandType.StoredProcedure);
            }
        }
        public static async Task<int> RetrieveAndSendTransactionsCOL(List<SharedModel.Models.Services.Colombia.Banks.Bancolombia.PayOutColombia.UploadTxtFromBank.TransactionDetail> transactionList)
        {
            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process. Retrieving Updated transactions information.");
                string countryCode = "COL";
                string customer = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();
                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                transactionList.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.TransactionId);

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                var LPMapper = new SharedMaps.Converters.Services.Colombia.Banks.Bancolombia.PayOutMapperColombia();

                var listResponse = LPMapper.MapperModelsBatch(PayoutListToSend);
                listResponse = listResponse.OrderBy(x => x.payout_id).ToList();

                if (listResponse.Count > 0)
                {
                    string jsonString = JsonConvert.SerializeObject(listResponse);

                    LogService.LogInfo("Got updated transactions information. Sending info to Payoneer API");

                    return await PayoneerNotificationSend(jsonString, "COL");
                }
            } catch(Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }
            

            return 0;
        }

        public static async Task<int> RetrieveAndSendTransactionsARG(List<SharedModel.Models.Services.Banks.Galicia.PayOut.UploadTxtFromBank.TransactionDetail> transactionList)
        {
            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process. Retrieving Updated transactions information.");
                string countryCode = "ARG";
                string customer = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();
                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                transactionList.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.TransactionId);

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

                    return await PayoneerNotificationSend(jsonString, "ARG");
                }
            } catch (Exception e)
            {
                LogService.LogError("There was an error retrieving Payouts Info: " + e.Message.ToString());
            }
            

            return 0;
        }

        public static async Task<int> RetrieveAndSendTransactionsMEX(List<SharedModel.Models.Services.Mexico.PayOutMexico.UploadTxtFromBank.TransactionDetail> transactionList)
        {
            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process. Retrieving Updated transactions information.");
                string countryCode = "MEX";
                string customer = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();
                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                transactionList.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.TransactionId);

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Mexico.PayOutMapperMexico LPMapper = new SharedMaps.Converters.Services.Mexico.PayOutMapperMexico();

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

        public static async Task<int> RetrieveAndSendTransactionsBRA(List<SharedModel.Models.Services.Brasil.PayOutBrasil.UploadTxtFromBank.TransactionDetail> transactionList)
        {
            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process. Retrieving Updated transactions information.");
                string countryCode = "BRA";
                string customer = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();
                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                transactionList.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.TransactionId);

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil LPMapper = new SharedMaps.Converters.Services.Brasil.PayOutMapperBrasil();

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

        public static async Task<int> RetrieveAndSendTransactionsURY(List<SharedModel.Models.Services.Uruguay.PayOutUruguay.UploadTxtFromBank.TransactionDetail> transactionList)
        {
            try
            {
                LogService.LogInfo("Starting Payoneer Notification Process. Retrieving Updated transactions information.");
                string countryCode = "URY";
                string customer = ConfigurationManager.AppSettings["PAYONEER_NOTIFICATION_" + countryCode + "_CUSTOMER"].ToString();
                SharedBusiness.Services.Banks.Galicia.BIPayOut BiPO = new SharedBusiness.Services.Banks.Galicia.BIPayOut();

                List<SharedModelDTO.Models.LotBatch.LotBatchModel> PayoutListToSend = new List<SharedModelDTO.Models.LotBatch.LotBatchModel>();

                transactionList.ForEach(item =>
                {
                    // simulate payout list service to get lot and transaction details
                    SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request lRequest = new SharedModel.Models.Services.Banks.Galicia.PayOut.List.Request();
                    lRequest.transaction_id = long.Parse(item.TransactionId);

                    var listLotBachModel = BiPO.ListTransaction(lRequest, customer, countryCode);
                    if (listLotBachModel.Count > 0)
                    {
                        PayoutListToSend.Add(listLotBachModel.ElementAt(0));
                    }
                });

                SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay LPMapper = new SharedMaps.Converters.Services.Uruguay.PayOutMapperUruguay();

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

                if (idEntityUser == idEntityUserArg) countryCode = "ARG";
                if (idEntityUser == idEntityUserCol) countryCode = "COL";
                if (idEntityUser == idEntityUserBra) countryCode = "BRA";
                if (idEntityUser == idEntityUserMex) countryCode = "MEX";

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
    }
}