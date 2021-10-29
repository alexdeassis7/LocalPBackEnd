using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Wallet
{
    public class InternalWalletTransfer
    {
        public class Create
        {
            public class Request
            {

                public Int64 customer_id { get; set; }

                [Required(ErrorMessage = "Parameter :: amount :: is required.")]
                //[Range(000000001, 100000000, ErrorMessage = "Parameter :: amount :: value out range [000000001 - 100000000].")]
                //[RegularExpression("^[0-9]{9}$", ErrorMessage = "Parameter :: amount :: invalid format, only allow: numbers.")] 
                public string amount { get; set; }
                public Int64 transaction_type { get; set; }

                [Required(ErrorMessage = "Parameter :: currency :: is required.")]
                [RegularExpression("^[ARS|USD]{3}$", ErrorMessage = "Parameter :: currency :: invalid format, only allow codes: 'ARS' | 'USD'.")]
                public string currency { get; set; }

                public string credit_tax { get; set; }
                private string _payout_date { get; set; }
                public string payout_date { get { return this._payout_date == null || string.IsNullOrEmpty(this._payout_date) ? DateTime.Now.ToString("yyyyMMdd") : this._payout_date; } set { this._payout_date = value; } }

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }
            }

            public class Response : Request
            {
                public Int64 transactionLot_id { get; set; }

                public string status { get; set; }
            }

        }
        public class Delete
        {
            public class Request
            {
                //[Required(ErrorMessage = "Parameter :: transaction_id :: is required.")]
                [RegularExpression("^[0-9]$", ErrorMessage = "Parameter :: transaction_id :: invalid format, only allow: numbers.")]
                public Int64 transaction_id { get; set; }

                private SharedModel.Models.General.ErrorModel.Error errorrow = new General.ErrorModel.Error();
                public SharedModel.Models.General.ErrorModel.Error ErrorRow { get { return errorrow; } set { errorrow = value; } }

            }

            public class Response : Request
            {
                public string status { get; set; }
            }
        }

    }
}
