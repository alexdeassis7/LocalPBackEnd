using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction.Detail
{
    public class TransactionCustomerInformation
    {
        #region Properties::Public

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string BirthDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Annotation { get; set; }
        public string ZipCode { get; set; }

        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderState { get; set; }
        public string SenderCountry { get; set; }
        public string SenderTaxid { get; set; }
        public string SenderBirthDate { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string SenderZipCode { get; set; }

        #endregion
    }
}
