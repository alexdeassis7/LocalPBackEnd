using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModelDTO.Models.Transaction
{
    public class TransAgForRetentionsModel
    {
        public MerchantModel Merchant { get; set; }
        public string OrganismoCode { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Importe_Retenciones { get; set; }
        public int Certificates_Created { get; set; }
        public int Certificates_Pending { get; set; }
        public int Trx_Pending { get; set; }
    }
    public class MerchantModel
    {
      public long Id { get; set; }
      public string Name { get; set; }
    }
    public class OrganismModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
