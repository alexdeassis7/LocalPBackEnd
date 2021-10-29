using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.General
{
    public class RegisteredEntityArba
    {
        public string Reg = "30";
        public string ToDate { get; set; }
        public long Cuit { get; set; }
        public string BussinessName { get; set; }
        public string Letter { get; set; }
        public int Active = 1;
    }
}
