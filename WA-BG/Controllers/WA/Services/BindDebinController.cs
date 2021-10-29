using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http.Description;

namespace WA_BG.Controllers.WA.Services
{
    [ApiExplorerSettings(IgnoreApi = true)]


    public class BindDebinController : ApiController
    {
        public HttpResponseMessage PostTokenDebin(SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request T, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/login/jwt";
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var content = new StringContent(JsonConvert.SerializeObject(T), Encoding.UTF8, "application/json");
            HttpResponseMessage response_bind = cliente_bind.PostAsync(apipath.PathAndQuery, content).Result;

            if (response_bind.IsSuccessStatusCode)
            {
                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage PostCreateDebin(SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.Request Request, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + Request.bank_id + "/accounts/" + Request.account_id + "/" + Request.view_id + "/transaction-request-types/DEBIN/transaction-requests";
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", Request.token_id);
            var content = new StringContent(JsonConvert.SerializeObject(Request.body), Encoding.UTF8, "application/json");
            HttpResponseMessage response_bind = cliente_bind.PostAsync(apipath.PathAndQuery, content).Result;


            if (response_bind.IsSuccessStatusCode)
            {
                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }

        }

        public HttpResponseMessage GetConsultarDatosVendedor(SharedModel.Models.Services.Banks.Bind.Debin.ConsultaDatosVendedor.Request T, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + T.bank_id + "/accounts/" + T.account_id + "/" + T.view_id + "/transaction-request-types/DEBIN/info";
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", T.token_id);
            HttpResponseMessage response_bind = cliente_bind.GetAsync(apipath.PathAndQuery).Result;


            if (response_bind.IsSuccessStatusCode)

            {

                var result = response_bind.Content.ReadAsStringAsync();


                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage GetObtenerDebinesCobrar(SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request Request, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + Request.bank_id + "/accounts/" + Request.account_id + "/" + Request.view_id + "/transaction-request-types/DEBIN";
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", Request.token_id);
            //cliente_bind.DefaultRequestHeaders.Add("valor1", "33");
            HttpResponseMessage response_bind = cliente_bind.GetAsync(apipath.PathAndQuery).Result;


            if (response_bind.IsSuccessStatusCode)

            {
                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage GetObtenerDebinesCobrarId(SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request Request, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + Request.bank_id + "/accounts/" + Request.account_id + "/" + Request.view_id + "/transaction-request-types/DEBIN" + "/" + Request.transaction_id;
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", Request.token_id);
            //cliente_bind.DefaultRequestHeaders.Add("valor1", "33");
            HttpResponseMessage response_bind = cliente_bind.GetAsync(apipath.PathAndQuery).Result;


            if (response_bind.IsSuccessStatusCode)

            {
                var result = response_bind.Content.ReadAsStringAsync();
                
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage DeleteEliminarPedidoDebin(SharedModel.Models.Services.Banks.Bind.Debin.EliminarPedidoDebin.Request Request, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + Request.bank_id + "/accounts/" + Request.account_id + "/" + Request.view_id + "/transaction-request-types/DEBIN" + "/" + Request.transaction_id;
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", Request.token_id);
            //cliente_bind.DefaultRequestHeaders.Add("valor1", "33");
            HttpResponseMessage response_bind = cliente_bind.DeleteAsync(apipath.PathAndQuery).Result;

            if (response_bind.IsSuccessStatusCode)
            {
                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage PutAltaBajaCuentaVendedor(SharedModel.Models.Services.Banks.Bind.Debin.AltaBajaCuentaVendedor.Request Request, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + Request.bank_id + "/accounts/" + Request.account_id + "/" + Request.view_id + "/transaction-request-types/DEBIN";
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", Request.token_id);
            var content = new StringContent(JsonConvert.SerializeObject(Request.body), Encoding.UTF8, "application/json");
            HttpResponseMessage response_bind = cliente_bind.PutAsync(apipath.PathAndQuery, content).Result;


            if (response_bind.IsSuccessStatusCode)
            //if (response_bind.IsSuccessful)
            {
                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage PostCrearPedidoSuscripcion(SharedModel.Models.Services.Banks.Bind.Debin.CrearPedidoSuscripcion.Request T, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + T.bank_id + "/accounts/" + T.account_id + "/" + T.view_id + "/transaction-request-types/DEBIN-SUBSCRIPTION/transaction-requests";
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", T.token_id);
            var content = new StringContent(JsonConvert.SerializeObject(T.body), Encoding.UTF8, "application/json");
            HttpResponseMessage response_bind = cliente_bind.PostAsync(apipath.PathAndQuery, content).Result;

            if (response_bind.IsSuccessStatusCode)
            //if (response_bind.IsSuccessful)
            {
                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;
            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }

        public HttpResponseMessage GetObtenerSuscripcion(SharedModel.Models.Services.Banks.Bind.Debin.ObtenerSuscripcion.Request T, HttpRequestMessage R)
        {
            HttpClientHandler authtHandler = null;
            HttpClient cliente_bind = null;

            var address = "https://" + "sandbox.bind.com.ar/v1/banks/" + T.bank_id + "/accounts/" + T.account_id + "/" + T.view_id + "/transaction-request-types/DEBIN-SUBSCRIPTION" + "/" + T.transaction_id;
            var apipath = new Uri(address);
            authtHandler = new HttpClientHandler() { Credentials = CredentialCache.DefaultNetworkCredentials };
            cliente_bind = new HttpClient(authtHandler);
            cliente_bind.BaseAddress = new Uri(apipath.GetLeftPart(UriPartial.Authority));
            cliente_bind.DefaultRequestHeaders.Accept.Clear();
            cliente_bind.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            cliente_bind.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", T.token_id);
            HttpResponseMessage response_bind = cliente_bind.GetAsync(apipath.PathAndQuery).Result;


            if (response_bind.IsSuccessStatusCode)

            {

                var result = response_bind.Content.ReadAsStringAsync();
                var response = R.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(result.Result, Encoding.UTF8, "application/json");
                return response;

            }
            else
            {
                string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                return R.CreateResponse(HttpStatusCode.BadRequest, resultError);
            }
        }
        
    }
}
