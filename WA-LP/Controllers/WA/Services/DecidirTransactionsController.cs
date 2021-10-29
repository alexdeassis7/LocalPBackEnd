using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using SharedModel.Models.Services.Decidir;
using System.Linq;
using SharedModel.Models.Shared;
using SharedModelDTO.Models.Transaction.Decidir;
using SharedBusiness.Services.Decidir;
using System.Collections.Generic;
using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.Services
{
    //  [Authorize]
    [RoutePrefix("v2/payins/cards")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class DecidirTransactionsController : BaseApiController
    {
        /// <summary>
        /// Lee Json desde el cliente, que contiene una carga masiva de transacciones a efectuar.
        /// </summary>string data
        /// 
        [HttpPost]
        [Route("payment")]
        public HttpResponseMessage PayTransactionRequest()
        {
            try
            {
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();
                bool TransactionMechanism = Request.Headers.Contains("TransactionMechanism") == false ? false : Convert.ToBoolean(((string[])Request.Headers.GetValues("TransactionMechanism"))[0]) == false ? false : true;


                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                PayExecution.Response payData = new PayExecution.Response();
                if (data != null && data.Length > 0)
                {
                    SimpleTransactionModel transactionData = JsonConvert.DeserializeObject<SimpleTransactionModel>(data);
                    transactionData.ErrorRow.Errors = ValidationModel.ValidatorModel(transactionData);
                    if (transactionData.ErrorRow.HasError == false)
                    {
                        PayToken.Request cardData = transactionData.cardData;
                        PayToken.RequestWithTokenizedCard tokenizedCardData = transactionData.tokenizedCardData;

                        var decidirController = new WA_BG.Controllers.WA.Services.DecidirController();
                        HttpResponseMessage token = null;

                        if (cardData != null && tokenizedCardData == null)
                        {
                            cardData.ErrorRow.Errors = ValidationModel.ValidatorModel(cardData);
                            if (cardData.ErrorRow.HasError == false)
                            {
                                token = decidirController.PostTokenRequest(cardData, Request);
                            }
                        }
                        else if (tokenizedCardData != null && cardData == null)
                        {
                            tokenizedCardData.ErrorRow.Errors = ValidationModel.ValidatorModel(tokenizedCardData);
                            //if (tokenizedCardData.ErrorRow.HasError == false)
                            //{
                                token = decidirController.PostTokenRequestWithTokenizedCard(tokenizedCardData, Request);
                            //}
                        }

                        PayExecution.Request payment = new PayExecution.Request();
                        payment = transactionData.paymentData;

                        payment.ErrorRow.Errors = ValidationModel.ValidatorModel(payment);
                        if (payment.ErrorRow.HasError == false)
                        {
                            var BiDecidir = new BIDecidir();
                            
                            //Creo ticket para referenciar la transaccion
                            var ticket = new SharedBusiness.Common.Ticket();
                            string id_ticket = ticket.CrearTicket("");
                            
                            //Mercado 
                            //Establecimiento desde el cual se realiza el pago
                            payment.establishment_name = "LocalPayment";
                            
                            //Usuario que realiza el pago
                            payment.customer = new Customer { finalclient_id = customer };               

                            HttpContent requestContent_token = token.Content;
                            string data_token = requestContent_token.ReadAsStringAsync().Result;
                            PayToken.Response ResponseToken = JsonConvert.DeserializeObject<PayToken.Response>(data_token);
                            payment.token = ResponseToken.id;

                            payment.site_transaction_id = id_ticket;
                            var paymentResponse = decidirController.PostPayment(payment, Request);

                            HttpContent payContent = paymentResponse.Content;
                            string pay = payContent.ReadAsStringAsync().Result;
                            payData = JsonConvert.DeserializeObject<PayExecution.Response>(pay);
                            payData.customer_token = null;

                            //Guardo el response en una nueva transaccion
                            DecidirTransaction tx = new DecidirTransaction();
                            tx.customerID = customer;
                            tx.amount = Convert.ToInt32(payment.amount);
                            tx.clientIdentification = cardData.card_holder_identification.number;
                            tx.comprobanteTransaction = payData.ticket;
                            tx.installments = payData.installments;
                            tx.ticket = id_ticket;
                            tx.detail = payment.description;
                            tx.currency = payment.currency;
                            tx.paymentId = payData.id;
                            tx.internalStatus = payData.status;
                            tx.transactionType = "TC";
                            //tx.statusDetail = payData.status_details.ToString().Replace("{", "").Replace("\r\n", "").Replace("}", "").Replace("\"", "").Trim();
                            tx.statusDetail = "Ticket: " + payData.status_details.ticket + " - Card Auth Code: " + payData.status_details.card_authorization_code + " - Address Validation Code: " + payData.status_details.address_validation_code.ToString() + " - Error: " + (payData.status_details.error.type == null ? "No" : payData.status_details.error.type);

                            BiDecidir.Payment(tx, TransactionMechanism);

                            return Request.CreateResponse(HttpStatusCode.OK, payData);
                        }
                        else
                        {
                            payData.ErrorRow.Errors = payment.ErrorRow.Errors;
                        }
                    }
                    else
                    {
                        payData.ErrorRow.Errors = transactionData.ErrorRow.Errors;
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, payData);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Message{0}, Errors: {1}", ex.Message, ex.InnerException.Message));
            }
        }

        [HttpGet]
        [Route("tokenized_cards")]
        public HttpResponseMessage GetUserTokenizeCards()
        {
            try
            {
                var re = Request;
                var headers = re.Headers;
                string customer = headers.GetValues("customer_id").First();

                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                Customer cmrData = JsonConvert.DeserializeObject<Customer>(data);
                cmrData.customer_id = customer;

                // mandarle el final client id del customerId 

                TokenizeCards userCards = new TokenizeCards();
                var decidirController = new WA_BG.Controllers.WA.Services.DecidirController();
                var result = decidirController.GetTokenizeCards(cmrData, Request);
                string tokenizeCards = result.Content.ReadAsStringAsync().Result;
                userCards = JsonConvert.DeserializeObject<TokenizeCards>(tokenizeCards);

                return Request.CreateResponse(HttpStatusCode.OK, userCards);
            }

            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }

        [HttpDelete]
        [Route("delete_card")]
        public HttpResponseMessage DeleteTokenizeCard()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                if (data != null && data.Length > 0)
                {
                    TokenizeCard token_card = JsonConvert.DeserializeObject<TokenizeCard>(data);
                    var decidirController = new WA_BG.Controllers.WA.Services.DecidirController();
                    var result = decidirController.DeleteTokenizeCard(token_card, Request);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                string resultError = "{\"Code\":500,\"CodeDescription\":\"BadRequest\",\"Message\":\"Error ==> {0}\" }";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format(resultError, ex.ToString()));
            }
        }


        //Si se ingresa el amount total o no se envia dicho campo se toma como anulacion.
        [HttpPost]
        [Route("refunds")]
        public HttpResponseMessage Refund()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                if (data != null && data.Length > 0)
                {
                    Refund refundData = JsonConvert.DeserializeObject<Refund>(data);

                    // con el refundData.site_transaction_id  buscar el refundData.payment_id en base de datos y pasarlo al decidirController

                    var decidirController = new WA_BG.Controllers.WA.Services.DecidirController();
                    var result = decidirController.Refund(refundData, Request);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Error: No encuentra el paymentId. Exception Error: {0}", ex.ToString()));
            }
        }


        [HttpGet]
        [Route("payment_info")]
        public HttpResponseMessage GetPaymentInfo()
        {
            try
            {
                var re = Request;
                var headers = re.Headers;
                //string customer = headers.GetValues("customer_id").First();

                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;
                HttpResponseMessage response = null;

                if (data != null && data.Length > 0)
                {
                    Payment payInfo = JsonConvert.DeserializeObject<Payment>(data);

                    // con el payInfo.site_transaction_id  buscar el payInfo.payment_id en base de datos y pasarlo al decidirController

                    var decidirController = new WA_BG.Controllers.WA.Services.DecidirController();
                    response = decidirController.GetPaymentInfo(payInfo, Request);
                }

                return Request.CreateResponse(HttpStatusCode.OK, response);
            }

            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Error: No encuentra el paymentId. Exception Error: {0}", ex.ToString()));
            }
        }

    }
}