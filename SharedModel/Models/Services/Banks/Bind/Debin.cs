using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Banks.Bind
{
    public class Debin
    {

        /// <summary>
        /// Request y Responses para de LP.
        /// </summary>

        public class RequestDebin
        {
            
            public string id_ticket { get; set; }
            public string bank_transaction { get; set; }
            public string currency { get; set; }
            public Int64 amount { get; set; }
            public string beneficiary_softd { get; set; }
            public string site_transaction_id { get; set; }
            public string description { get; set; }
            public string concept_code { get; set; }
            public string payment_type { get; set; }
            public string payout_date { get; set; }
            public string alias { get; set; }
            public string cbu { get; set; }
            public string buyer_cuit { get; set; }
            public string buyer_name { get; set; }
            public string buyer_bank_code { get; set; }
            public string buyer_cbu { get; set; }
            public string buyer_alias { get; set; }
            public string buyer_bank_description { get; set; }
            public string status { get; set; }

            private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
            public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }

        public class ResponseDebin
        {
            public int transaction_id { get; set; }
            public string currency { get; set; }
            public Int64 amount { get; set; }
            public string beneficiary_softd { get; set; }
            public string site_transaction_id { get; set; }
            public string description { get; set; }
            public string concept_code { get; set; }
            public string payment_type { get; set; }
            public string payout_date { get; set; }
            public string buyer_cuit { get; set; }
            public string buyer_name { get; set; }
            public string buyer_bank_code { get; set; }
            public string buyer_cbu { get; set; }
            public string buyer_alias { get; set; }
            public string buyer_bank_description { get; set; }
            public string status { get; set; }

            private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
            public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }

        }

        public class RequestGetDebines
        {

            public string site_transaction_id { get; set; }
            private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
            public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }

        public class ResponseGetDebines
        {

            public int idTransactionDebinDetail { get; set; }
            public int idTransaction { get; set; }
            public string id_ticket { get; set; }
            public int idStatus { get; set; }
            public string status { get; set; }
            public int amount { get; set; }
            public bool Active { get; set; }

            private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
            public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
        }

        public class RequestDeleteDebines
        {
            public string id { get; set; }
            public string site_transaction_id { get; set; }
            

        }

        public class ResponseDeleteDebines
        {
            public string id { get; set; }
            public string site_transaction_id { get; set; }
            public int idStatus { get; set; }
            public string status { get; set; }
            public int amount { get; set; }
            public bool Active { get; set; }
            private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
            public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }

        }

        /// <summary>
        /// Request y Responses para el Bind.
        /// </summary>


        public class TokenDebin
        {
            public class Request
            {
                public string username { get; set; }
                public string password { get; set; }
            }
            public class Response
            {
                public string token { get; set; }
                public Int64 expires_in { get; set; }
            }
        }

        public class CreateDebin
        {
            public class HeaderRequest
            {
                public string Autorization { get; set; }
            }
            public class To
            {
                public string label { get; set; }
                public string cbu { get; set; }
            }
            public class Value
            {
                public string currency { get; set; }
                public Int64 amount { get; set; }
            }
            public class BodyRequest
            {
                public string origin_id { get; set; }
                public To to { get; set; }
                public Value value { get; set; }
                public string concept { get; set; }
                public string description { get; set; }
                public Int64 expiration { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
                public BodyRequest body { get; set; }
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class From
            {
                public string bank_id { get; set; }
                public string account_id { get; set; }
            }
            public class Buyer
            {
                public string cuit { get; set; }
                public string alias { get; set; }
                public string cbu { get; set; }
                public string name { get; set; }
                public string bank_code { get; set; }
                public string bank_description { get; set; }
            }
            public class Details
            {
                public string origin_id { get; set; }
                public string warnings { get; set; }
                public Buyer buyer { get; set; }

            }
            public class ValueResponse
            {
                public string currency { get; set; }
                public Int64 amount { get; set; }
            }
            public class Charge
            {
                public string summary { get; set; }
                public ValueResponse value { get; set; }
            }
            public class Response
            {
                public string id { get; set; }
                public string type { get; set; }
                public From from { get; set; }
                public Details details { get; set; }
                public List<string> transaction_ids { get; set; }
                public string status { get; set; }
                public string status_description { get; set; }
                public string start_date { get; set; }
                public string end_date { get; set; }
                public string challenge { get; set; }
                public Charge charge { get; set; }
            }   
        }

        public class ConsultaDatosVendedor
        {
            public class HeaderRequest
            {
                public string Autorization { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class AccountRouting
            {
                public string scheme { get; set; }
                public string address { get; set; }
            }

            public class Account
            {
                public string id { get; set; }
                public object label { get; set; }
                public string number { get; set; }
                public string type { get; set; }
                public string status { get; set; }
                public object owners { get; set; }
                public object balance { get; set; }
                public object bank_id { get; set; }
                public AccountRouting account_routing { get; set; }
            }
            public class Response
            {
                public string id { get; set; }
                public string display_name { get; set; }
                public string id_type { get; set; }
                public bool is_physical_person { get; set; }
                public List<string> emails { get; set; }
                public List<Account> accounts { get; set; }

            }
        }

        public class ObtenerDebinesCobrar
        {
            public class HeaderRequest
            {
                public string Authorization { get; set; }
                public string obp_status { get; set; }
                public Int64 obp_limit { get; set; }
                public Int64 obp_offset { get; set; }
                public string obp_from_date { get; set; }
                public string obp_to_date { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
                public string transaction_id { get; set; }

                
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class From
            {
                public string bank_id { get; set; }
                public string account_id { get; set; }
            }
            public class Details
            {
                public Boolean preauthorized { get; set; }
                public string sellerCuit { get; set; }
                public string sellerAccountLabel { get; set; }
                public string sellerAccountCBU { get; set; }
                public string origin_id { get; set; }
                public string buyerAccountCBU { get; set; }
                public string buyerAccountLabel { get; set; }
                public string buyerCuit { get; set; }
            }
            public class Value
            {
                public string currency { get; set; }
                public int amount { get; set; }
            }
            public class Charge
            {
                public string summary { get; set; }
                public Value value { get; set; }
            }
            public class Response
            {
                public string id { get; set; }
                public string type { get; set; }
                public From from { get; set; }
                public Details details { get; set; }
                public List<string> transaction_ids { get; set; }
                public string status { get; set; }
                public string status_description { get; set; }
                public DateTime start_date { get; set; }
                public DateTime end_date { get; set; }
                public string challenge { get; set; }
                public Charge charge { get; set; }

                

            }
        }

        public class EliminarPedidoDebin
        {
            public class HeaderRequest
            {
                public string Authorization { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
                public string transaction_id { get; set; }
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class Response
            {
                public string id { get; set; }
            }
        }

        public class AltaBajaCuentaVendedor
        {
            public class HeaderRequest
            {
                public string Authorization { get; set; }
            }
            public class BodyRequest
            {
                public Boolean adhered { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
                public BodyRequest body { get; set; }
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class Response
            {
                public Boolean adhered { get; set; }
                public string account_id { get; set; }
            }
        }

        public class CrearPedidoSuscripcion
        {
            public class HeaderRequest
            {
                public string Authorization { get; set; }
            }
            public class To
            {
                public string label { get; set; }
                public string cbu { get; set; }
            }
            public class ValueRequest
            {
                public string currency { get; set; }
                public double amount_per_period { get; set; }
                public double amount_per_debin { get; set; }
                public double debin_count_per_period { get; set; }
            }
            public class BodyRequest
            {
                public string origin_id { get; set; }
                public To to { get; set; }
                public ValueRequest value { get; set; }
                public string concept { get; set; }
                public string period { get; set; }
                public string detail { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
                public BodyRequest body { get; set; }
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class From
            {
                public string bank_id { get; set; }
                public string account_id { get; set; }
            }

            public class Value
            {
                public double amount_per_debin { get; set; }
                public string period { get; set; }
                public double amount_per_period { get; set; }
                public string concept { get; set; }
                public string detail { get; set; }
                public double debin_count_per_period { get; set; }
            }

            public class Buyer
            {
                public string cuit { get; set; }
                public string alias { get; set; }
                public string cbu { get; set; }
                public string name { get; set; }
                public string bank_code { get; set; }
                public string bank_description { get; set; }
            }

            public class Details
            {
                public string origin_id { get; set; }
                public Value value { get; set; }
                public Buyer buyer { get; set; }
            }

            public class Value2
            {
                public string currency { get; set; }
                public object amount { get; set; }
            }

            public class Charge
            {
                public string summary { get; set; }
                public Value2 value { get; set; }
            }
            public class Response
            {
                public string id { get; set; }
                public string type { get; set; }
                public From from { get; set; }
                public Details details { get; set; }
                public List<string> transaction_ids { get; set; }
                public string status { get; set; }
                public DateTime start_date { get; set; }
                public DateTime end_date { get; set; }
                public string challenge { get; set; }
                public Charge charge { get; set; }
            }
        }

        public class ObtenerSuscripcion
        {
            public class HeaderRequest
            {
                public string Authorization { get; set; }
            }
            public class Request
            {
                public string token_id { get; set; }
                public Int64 bank_id { get; set; }
                public string account_id { get; set; }
                public string view_id { get; set; }
                public string transaction_id { get; set; }
            }
            public class HeaderResponse
            {
                public string process { get; set; }
            }
            public class From
            {
                public string bank_id { get; set; }
                public string account_id { get; set; }
            }

            public class Value
            {
                public double amount_per_debin { get; set; }
                public string period { get; set; }
                public double amount_per_period { get; set; }
                public string concept { get; set; }
                public string detail { get; set; }
                public double debin_count_per_period { get; set; }
            }

            public class Buyer
            {
                public string cuit { get; set; }
                public string alias { get; set; }
                public string cbu { get; set; }
                public string name { get; set; }
                public string bank_code { get; set; }
                public string bank_description { get; set; }
            }

            public class Details
            {
                public string origin_id { get; set; }
                public Value value { get; set; }
                public Buyer buyer { get; set; }
            }

            public class Value2
            {
                public string currency { get; set; }
                public object amount { get; set; }
            }

            public class Charge
            {
                public string summary { get; set; }
                public Value2 value { get; set; }
            }
            public class Response
            {
                public string id { get; set; }
                public string type { get; set; }
                public From from { get; set; }
                public Details details { get; set; }
                public List<string> transaction_ids { get; set; }
                public string status { get; set; }
                public DateTime start_date { get; set; }
                public DateTime end_date { get; set; }
                public string challenge { get; set; }
                public Charge charge { get; set; }
            }
        }
    }
}
