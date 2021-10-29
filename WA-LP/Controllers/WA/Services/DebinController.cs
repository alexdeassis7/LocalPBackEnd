using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;


using System.Linq;

using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.Services
{
    //[Authorize]
    [RoutePrefix("v2/debin")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class DebinController : BaseApiController
    {
        [HttpPost]
        [Route("create_debin")]
        public HttpResponseMessage CreateDebin()
        {
            try
            {
                // Leo el Header

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;


                // creo el ticket
                var ticket = new SharedBusiness.Common.Ticket();
                string id_ticket = ticket.CrearTicket("");
                var BiD = new SharedBusiness.Services.Banks.Bind.BIDebin();




                //Armo Request para solicitar token bind
                string user_bind = "ariel@localpayment.com";
                string pass_bind = "KfkXD44LJiLixMd";
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request RequestTokenDebin = new SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request() { username = user_bind, password = pass_bind };

                var datos_bind_token = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado_bind_token = datos_bind_token.PostTokenDebin(RequestTokenDebin, Request);

                HttpContent requestContent_token = resultado_bind_token.Content;
                string data_token = requestContent_token.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response ResponseDebinToken = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response>(data_token);

                // Leo y Valido el Request LP 

                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin RequestLP = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin>(data_lp);
                RequestLP.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(RequestLP);

                SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin ResponseLP = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin();

                // Armo Request para el BIND, agregandole token BIND y ticket creado

                if (RequestLP.ErrorRow.HasError == false)
                {


                    SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.Request RequestBind = new SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.Request()
                    {
                        token_id = ResponseDebinToken.token,
                        bank_id = 322,
                        account_id = "21-1-99999-4-6",
                        view_id = RequestLP.site_transaction_id,
                        body = new SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.BodyRequest()
                        {
                            origin_id = id_ticket,
                            to = new SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.To()
                            {

                                label = RequestLP.alias,
                                cbu = RequestLP.cbu
                            },
                            value = new SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.Value()
                            {
                                amount = RequestLP.amount,
                                currency = RequestLP.currency
                            },
                            concept = RequestLP.concept_code,
                            description = RequestLP.description,
                            expiration = 36
                        },


                    };

                    if (RequestBind.body.to.cbu.Length == 0) RequestBind.body.to.cbu = null;
                    if (RequestBind.body.to.label.Length == 0) RequestBind.body.to.label = null;

                    // Hago el Post de create_debin al BIND
                    var datos_bind_create = new WA_BG.Controllers.WA.Services.BindDebinController();
                    var resultado = datos_bind_create.PostCreateDebin(RequestBind, Request);
                    HttpContent requestContent_bind = resultado.Content;
                    string data_bind = requestContent_bind.ReadAsStringAsync().Result;
                    SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.Response ResponseBind = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.CreateDebin.Response>(data_bind);

                    //Completo el RequestLP

                    RequestLP.id_ticket = ResponseBind.details.origin_id;
                    RequestLP.bank_transaction = ResponseBind.id;
                    RequestLP.currency = ResponseBind.charge.value.currency;
                    RequestLP.amount = ResponseBind.charge.value.amount;
                    RequestLP.payout_date = ResponseBind.start_date;
                    RequestLP.buyer_cuit = ResponseBind.details.buyer.cuit;
                    RequestLP.buyer_name = ResponseBind.details.buyer.name;
                    RequestLP.buyer_bank_code = ResponseBind.details.buyer.bank_code;
                    RequestLP.buyer_cbu = ResponseBind.details.buyer.cbu;
                    RequestLP.buyer_alias = ResponseBind.details.buyer.alias;
                    RequestLP.buyer_bank_description = ResponseBind.details.buyer.bank_description;
                    RequestLP.status = ResponseBind.status;


                    //Guardo el response en una nueva transaccion
                    ResponseLP = BiD.DebinOp(RequestLP, customer, TransactionMechanism);

                    //Actualizo el ResponseLP2 para el cliente
                }
                else
                {
                    ResponseLP.ErrorRow.Errors = RequestLP.ErrorRow.Errors;

                }

                //retorno ResponseLP armado
                return Request.CreateResponse(HttpStatusCode.OK, ResponseLP);

            }
            catch (Exception ex)
            {
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpGet]
        [Route("consultar_datos_vendedor")]
        public HttpResponseMessage ConsultarDatosVendedor()
        {
            try
            {
                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.ConsultaDatosVendedor.Request Transaction = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.ConsultaDatosVendedor.Request>(data_lp);



                if (Transaction.token_id.Length > 0 && Transaction.bank_id != 0 && Transaction.account_id.Length > 0 && Transaction.view_id.Length > 0)
                {
                    //enviar data al bind

                    var datos_bind = new WA_BG.Controllers.WA.Services.BindDebinController();
                    var resultado = datos_bind.GetConsultarDatosVendedor(Transaction, Request);
                    return resultado;


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpGet]
        [Route("collect_debin")]
        public HttpResponseMessage ObtenerDebinesCobrar()
        {
            try
            {
                // Leo el Header

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                //Armo Request para solicitar token bind
                string user_bind = "ariel@localpayment.com";
                string pass_bind = "KfkXD44LJiLixMd";
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request RequestTokenDebin = new SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request() { username = user_bind, password = pass_bind };

                var datos_bind_token = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado_bind_token = datos_bind_token.PostTokenDebin(RequestTokenDebin, Request);

                HttpContent requestContent_token = resultado_bind_token.Content;
                string data_token = requestContent_token.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response ResponseDebinToken = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response>(data_token);

                // Leo y Valido el Request LP 

                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.RequestGetDebines RequestDebines = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.RequestGetDebines>(data_lp);
                RequestDebines.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(RequestDebines);

                List<SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines> ResponseDebines = new List<SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines>();
                SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines ResponseDebines_tmp = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines();

                // Armo Request para el BIND, agregandole token BIND y ticket creado

                if (RequestDebines.ErrorRow.HasError == false)
                {


                    SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request RequestBind = new SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request()
                    {
                        token_id = ResponseDebinToken.token,
                        bank_id = 322,
                        account_id = "21-1-99999-4-6",
                        view_id = RequestDebines.site_transaction_id,

                    };

                    // Hago el Post de create_debin al BIND
                    var datos_bind_debines = new WA_BG.Controllers.WA.Services.BindDebinController();
                    var resultado = datos_bind_debines.GetObtenerDebinesCobrar(RequestBind, Request);
                    HttpContent requestContent_bind = resultado.Content;
                    string data_bind = requestContent_bind.ReadAsStringAsync().Result;
                    List<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response> ResponseBind = JsonConvert.DeserializeObject<List<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response>>(data_bind);


                    //Envio Datos a la Base de Datos
                    var BiD = new SharedBusiness.Services.Banks.Bind.BIDebin();

                    foreach (var item in ResponseBind)
                    {
                        ResponseDebines_tmp = BiD.Debines(item, customer, TransactionMechanism);
                        if (ResponseDebines_tmp != null)
                        {
                            ResponseDebines.Add(ResponseDebines_tmp);
                        }


                    }


                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Request);

                }

                //retorno ResponseLP armado
                return Request.CreateResponse(HttpStatusCode.OK, ResponseDebines);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpGet]
        [Route("collect_debin/{id}")]
        public HttpResponseMessage ObtenerDebinesCobrar(string id)
        {
            try
            {
                // Leo el Header

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                //Armo Request para solicitar token bind
                string user_bind = "ariel@localpayment.com";
                string pass_bind = "KfkXD44LJiLixMd";
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request RequestTokenDebin = new SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request() { username = user_bind, password = pass_bind };

                var datos_bind_token = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado_bind_token = datos_bind_token.PostTokenDebin(RequestTokenDebin, Request);

                HttpContent requestContent_token = resultado_bind_token.Content;
                string data_token = requestContent_token.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response ResponseDebinToken = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response>(data_token);

                // Leo y Valido el Request LP 

                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.RequestGetDebines RequestDebin = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.RequestGetDebines>(data_lp);
                RequestDebin.ErrorRow.Errors = SharedModel.Models.Shared.ValidationModel.ValidatorModel(RequestDebin);

                SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines ResponseDebin = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines();

                //SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines ResponseDebines_tmp = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines();

                // Armo Request para el BIND, agregandole token BIND y ticket creado

                if (RequestDebin.ErrorRow.HasError == false)
                {


                    SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request RequestBind = new SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request()
                    {
                        token_id = ResponseDebinToken.token,
                        bank_id = 322,
                        account_id = "21-1-99999-4-6",
                        view_id = RequestDebin.site_transaction_id,
                        transaction_id = id

                    };

                    // Hago el Post de create_debin al BIND
                    var datos_bind_debines = new WA_BG.Controllers.WA.Services.BindDebinController();
                    var resultado = datos_bind_debines.GetObtenerDebinesCobrarId(RequestBind, Request);
                    HttpContent requestContent_bind = resultado.Content;
                    string data_bind = requestContent_bind.ReadAsStringAsync().Result;
                    SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response ResponseBind = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response>(data_bind);


                    //Envio Datos a la Base de Datos
                    var BiD = new SharedBusiness.Services.Banks.Bind.BIDebin();
                    ResponseDebin = BiD.Debines(ResponseBind, customer, TransactionMechanism);



                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Request);

                }

                //retorno ResponseLP armado
                return Request.CreateResponse(HttpStatusCode.OK, ResponseDebin);

                //HttpContent requestContent_lp = Request.Content;
                //string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                //SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request Transaction = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Request>(data_lp);

                //Transaction.transaction_id = id;

                //if (Transaction.token_id.Length > 0 && Transaction.bank_id != 0 && Transaction.account_id.Length > 0 && Transaction.view_id.Length > 0 && id.Length >0)
                //{
                //    //enviar data al bind

                //    var datos_bind = new WA_BG.Controllers.WA.Services.BindDebinController();
                //    var resultado = datos_bind.GetObtenerDebinesCobrarId(Transaction, Request);
                //    return resultado;

                //}
                //else
                //{
                //    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                //}
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpDelete]
        [Route("delete_debin/{id}")]
        public HttpResponseMessage EliminarPedidoDebin(string id)
        {
            try
            {
                // Leo el Header

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                //Armo Request para solicitar token bind
                string user_bind = "ariel@localpayment.com";
                string pass_bind = "KfkXD44LJiLixMd";
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request RequestTokenDebin = new SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request() { username = user_bind, password = pass_bind };

                var datos_bind_token = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado_bind_token = datos_bind_token.PostTokenDebin(RequestTokenDebin, Request);

                HttpContent requestContent_token = resultado_bind_token.Content;
                string data_token = requestContent_token.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response ResponseDebinToken = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response>(data_token);

                // Leo y Valido el Request LP 

                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.RequestDeleteDebines RequestLP = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.RequestDeleteDebines>(data_lp);

                SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines ResponseLP = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines();

                // Armo Request para el BIND, agregandole token BIND y ticket creado

                SharedModel.Models.Services.Banks.Bind.Debin.EliminarPedidoDebin.Request RequestBind = new SharedModel.Models.Services.Banks.Bind.Debin.EliminarPedidoDebin.Request()
                {
                    token_id = ResponseDebinToken.token,
                    bank_id = 322,
                    account_id = "21-1-99999-4-6",
                    view_id = RequestLP.site_transaction_id,
                    transaction_id = id

                };

                // Hago el Post de create_debin al BIND
                var datos_bind_debines = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado = datos_bind_debines.DeleteEliminarPedidoDebin(RequestBind, Request);
                HttpContent requestContent_bind = resultado.Content;
                string data_bind = requestContent_bind.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.EliminarPedidoDebin.Response ResponseBind = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.EliminarPedidoDebin.Response>(data_bind);


                //Envio Datos a la Base de Datos
                var BiD = new SharedBusiness.Services.Banks.Bind.BIDebin();

                RequestLP.id = ResponseBind.id;
                ResponseLP = BiD.DebinDelete(RequestLP, customer, TransactionMechanism);


                //retorno ResponseLP armado
                return Request.CreateResponse(HttpStatusCode.OK, ResponseLP);


            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpPut]
        [Route("debin_account")]
        public HttpResponseMessage AltaBajaCuentaVendedor()
        {
            try
            {
                // Leo el Header

                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;

                //Armo Request para solicitar token bind
                string user_bind = "ariel@localpayment.com";
                string pass_bind = "KfkXD44LJiLixMd";
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request RequestTokenDebin = new SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Request() { username = user_bind, password = pass_bind };

                var datos_bind_token = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado_bind_token = datos_bind_token.PostTokenDebin(RequestTokenDebin, Request);

                HttpContent requestContent_token = resultado_bind_token.Content;
                string data_token = requestContent_token.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response ResponseDebinToken = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.TokenDebin.Response>(data_token);


                // Hago el Post de create_debin al BIND
                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.AltaBajaCuentaVendedor.Request RequestBind = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.AltaBajaCuentaVendedor.Request>(data_lp);
                RequestBind.token_id = ResponseDebinToken.token;

                var datos_bind_debines = new WA_BG.Controllers.WA.Services.BindDebinController();
                var resultado = datos_bind_debines.PutAltaBajaCuentaVendedor(RequestBind, Request);
                HttpContent requestContent_bind = resultado.Content;
                string data_bind = requestContent_bind.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.AltaBajaCuentaVendedor.Response ResponseBind = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.AltaBajaCuentaVendedor.Response>(data_bind);

                //retorno ResponseLP armado
                return Request.CreateResponse(HttpStatusCode.OK, ResponseBind);


                //if (data_lp != null && data_lp.Length > 0)
                //{
                //    //enviar data al bind
                //    var datos_bind = new WA_BG.Controllers.WA.Services.BindDebinController();
                //    var resultado = datos_bind.PutAltaBajaCuentaVendedor(RequestLP, Request);



                //    //return resultado;

                //}
                //else
                //{
                //    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                //    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                //}
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpPost]
        [Route("debin_subscribe")]
        public HttpResponseMessage CrearPedidoSuscripcion()
        {
            try
            {
                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.CrearPedidoSuscripcion.Request Transaction = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.CrearPedidoSuscripcion.Request>(data_lp);


                if (data_lp != null && data_lp.Length > 0)
                {
                    //enviar data al bind
                    var datos_bind = new WA_BG.Controllers.WA.Services.BindDebinController();
                    var resultado = datos_bind.PostCrearPedidoSuscripcion(Transaction, Request);
                    return resultado;

                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpGet]
        [Route("get_subscription")]
        public HttpResponseMessage ObtenerSuscripcion()
        {
            try
            {
                HttpContent requestContent_lp = Request.Content;
                string data_lp = requestContent_lp.ReadAsStringAsync().Result;
                SharedModel.Models.Services.Banks.Bind.Debin.ObtenerSuscripcion.Request Transaction = JsonConvert.DeserializeObject<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerSuscripcion.Request>(data_lp);

                if (Transaction.token_id.Length > 0 && Transaction.bank_id != 0 && Transaction.account_id.Length > 0 && Transaction.view_id.Length > 0 && Transaction.transaction_id.Length > 0)
                {
                    //enviar data al bind
                    var datos_bind = new WA_BG.Controllers.WA.Services.BindDebinController();
                    var resultado = datos_bind.GetObtenerSuscripcion(Transaction, Request);
                    return resultado;


                }
                else
                {
                    string resultError = "{\"Code\":400,\"CodeDescription\":\"BadRequest\",\"Message\":\"Bad Parameter Value or Parameters Values.\" }";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, resultError);
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }
    }
}