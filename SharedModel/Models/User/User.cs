using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.Shared;

namespace SharedModel.Models.User
{
    public class User
    {
        public string Admin { get; set; }
        public string UserSiteIdentification { get; set; }
        public string TypeUser { get; set; }
        public string Merchant { get; set; }
        public Int64 idEntityUser { get; set; }
        public Int64 idEntityAccount { get; set; }
        private List<GeographyModel.Country> _pCountryList = new List<GeographyModel.Country>();
        public List<GeographyModel.Country> lCountry { get { return _pCountryList; } set { _pCountryList = value; } }

        public string ReportEmail { get; set; }
        public string Key { get; set; }
    }
}
