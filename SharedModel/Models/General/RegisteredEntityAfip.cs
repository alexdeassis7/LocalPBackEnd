using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.General
{
    public class RegisteredEntityAfip
    {
        public long Cuit { get; set; }
        public string Alias { get; set; }
        public string IncomeTax { get; set; }
        public string VatTax { get; set; }
        public string MonoTax { get; set; }
        public string SocialMember { get; set; }
        public string Employer { get; set; }
        public string MonoTaxActivity { get; set; }
        public int Active = 1;
    }
}
