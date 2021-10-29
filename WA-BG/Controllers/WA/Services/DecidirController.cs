using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using SharedModel.Models.Services.Decidir;
using SharedModel.Models.Services.Decidir.Errors;

namespace WA_BG.Controllers.WA.Services
{
    [ApiExplorerSettings(IgnoreApi = true)]


    public class DecidirController : ApiController
    {

        //Test
        private string baseAdress = "https://developers.decidir.com/api/v2/";
        private string publicKey = "e9cdb99fff374b5f91da4480c8dca741";
        private string privateKey = "92b71cf711ca41f78362a7134f87ff65";

        //Production
        //private string baseAdress = "https://live.decidir.com/api/v2";
        //private string publicKey = "a74388ace4d54212b055d4852d71b7c9";
        //private string privateKey = "5cfb587cd19848438b4587612768ca59";
        public HttpResponseMessage PostTokenRequest(PayToken.Request cardData, HttpRequestMessage R)
        {
            var address = baseAdress + "tokens";
            var apipath = new Uri(address);            
            HttpClient client = clientResponse(publicKey,apipath);
            var content = new StringContent(JsonConvert.SerializeObject(cardData), Encoding.UTF8, "application/json");
            HttpResponseMessage responseClient = client.PostAsync(apipath.PathAndQuery, content).Result;

                if (responseClient.IsSuccessStatusCode)
                {
                    var result = responseClient.Content.ReadAsStringAsync();
                    var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                    responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                    return responseMsg;
                }
                else
                {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                var a = errors.validation_errors.Select(o => o.param);
                throw new Exception(string.Join(",", errors.validation_errors.Select(o => o.code).Distinct()), new Exception(string.Join(",", a)));
            }

        }

        public HttpResponseMessage PostTokenRequestWithTokenizedCard(PayToken.RequestWithTokenizedCard tokenizedData, HttpRequestMessage R)
        {
            var address = baseAdress + "tokens";
            var apipath = new Uri(address);
            HttpClient client = clientResponse(publicKey, apipath);
            var content = new StringContent(JsonConvert.SerializeObject(tokenizedData), Encoding.UTF8, "application/json");
            HttpResponseMessage responseClient = client.PostAsync(apipath.PathAndQuery, content).Result;

            if (responseClient.IsSuccessStatusCode)
            {
                var result = responseClient.Content.ReadAsStringAsync();
                var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return responseMsg;
            }
            else
            {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                var a = errors.validation_errors.Select(o => o.param);
                throw new Exception(string.Join(",", errors.validation_errors.Select(o => o.code).Distinct()), new Exception(string.Join(",", a)));
            }

        }


        public HttpResponseMessage PostPayment(PayExecution.Request data, HttpRequestMessage R) {
                         
            data.site_transaction_id = "Testing_"+new Random().Next();
            var address = baseAdress + "payments";
            var apipath = new Uri(address);
            HttpClient client = clientResponse(privateKey, apipath);
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            HttpResponseMessage responseClient = client.PostAsync(apipath.PathAndQuery, content).Result;
            if (responseClient.IsSuccessStatusCode)
            {
                var result = responseClient.Content.ReadAsStringAsync();
                var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return responseMsg;
            }
            else
            {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                var a = errors.validation_errors.Select(o => o.param);
                throw new Exception(string.Join(",",errors.validation_errors.Select(o=>o.code).Distinct()), new Exception(string.Join(",", a)));
            }
        }

        public HttpResponseMessage GetTokenizeCards(Customer customer, HttpRequestMessage R) {
            var address = baseAdress + "usersite/" +customer.customer_id + "/cardtokens";
            var apipath = new Uri(address);
            HttpClient client = clientResponse(privateKey, apipath);
            HttpResponseMessage responseClient = client.GetAsync(apipath.PathAndQuery).Result;
            if (responseClient.IsSuccessStatusCode)
            {

                var result = responseClient.Content.ReadAsStringAsync();
                
                var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return responseMsg;

            }
            else
            {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                var a = errors.validation_errors.Select(o => o.param);
                throw new Exception(string.Join(",", errors.validation_errors.Select(o => o.code).Distinct()), new Exception(string.Join(",", a)));
            }
        }

        public HttpResponseMessage DeleteTokenizeCard(TokenizeCard card, HttpRequestMessage R)
        {
            var address = baseAdress + "cardtokens/"+card.token_card ;
            var apipath = new Uri(address);
            HttpClient client = clientResponse(privateKey, apipath);
            HttpResponseMessage responseClient = client.DeleteAsync(apipath.PathAndQuery).Result;
            if (responseClient.IsSuccessStatusCode)
            {
                var result = responseClient.Content.ReadAsStringAsync();
                var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return responseMsg;
            }
            else
            {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                var a = errors.validation_errors.Select(o => o.param);
                throw new Exception(string.Join(",", errors.validation_errors.Select(o => o.code).Distinct()), new Exception(string.Join(",", a)));
            }
        }

             
        public HttpResponseMessage Refund(Refund refundData, HttpRequestMessage R)
        {
            var address = baseAdress + "payments/" + refundData.payment_id + "/refunds";
            var apipath = new Uri(address);
            HttpClient client = clientResponse(privateKey, apipath);
            HttpResponseMessage responseClient = client.DeleteAsync(apipath.PathAndQuery).Result;
            if (responseClient.IsSuccessStatusCode)
            {
                var result = responseClient.Content.ReadAsStringAsync();
                var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return responseMsg;
            }
            else
            {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                throw new Exception(errors.error_type);
            }
        }

        public HttpResponseMessage GetPaymentInfo(Payment payInfo, HttpRequestMessage R)
        {
            var address = baseAdress + "payments/" + payInfo.payment_id;
            var apipath = new Uri(address);
            HttpClient client = clientResponse(privateKey, apipath);
            HttpResponseMessage responseClient = client.GetAsync(apipath.PathAndQuery).Result;
            if (responseClient.IsSuccessStatusCode)
            {

                var result = responseClient.Content.ReadAsStringAsync();


                var responseMsg = R.CreateResponse(HttpStatusCode.OK);
                responseMsg.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return responseMsg;

            }
            else
            {
                var resultError = responseClient.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<Result>(resultError.Result);
                throw new Exception(errors.error_type);

            }
        }



        //private TokenizeCard TokenizeCardData(Customer customer, PayToken.Request request)
        //{
        //    try
        //    {
        //        var cards = TokenizeCards(customer, new HttpRequestMessage());
        //        string tokenizeCards = cards.Content.ReadAsStringAsync().Result;
        //        Tokens data = JsonConvert.DeserializeObject<Tokens>(tokenizeCards);
        //        return data.tokens.FirstOrDefault(o => o.bin == request.card_number.Substring(0, 6) && o.last_four_digits == request.card_number.Substring((request.card_number.Length - 4), 4));
        //    }
        //    catch
        //    {

        //        return null;
        //    }
        //}



        private HttpClient clientResponse(string key, Uri apipath) {

            HttpClientHandler authtHandler = null;
            HttpClient cliente = null;            
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente = new HttpClient(authtHandler);
            cliente.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Add("apikey", key);
            CacheControlHeaderValue CacheHeader = new CacheControlHeaderValue();
            CacheHeader.NoCache = true;
            cliente.DefaultRequestHeaders.CacheControl = CacheHeader;
            cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            return cliente;
        }


    }

}