using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Shared
{
    public class GeographyModel
    {
        public class Country
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public Int32 idEntityUser {get;set;}
            public Int32 idEntityAccount { get; set; }
            public string Description { get; set; }
            public string DescriptionUser { get; set; }
            private List<StateProvince> _pStateProvinceList = new List<StateProvince>();
            public List<StateProvince> lStateProvince { get { return _pStateProvinceList; } set { _pStateProvinceList = value; } }
        }

        public class StateProvince
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            private List<Location> _pLocationList = new List<Location>();
            public List<Location> lLocation { get { return _pLocationList; } set { _pLocationList = value; } }
        }

        public class Location
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            private List<District> _pDistrictList = new List<District>();
            public List<District> lDistrict { get { return _pDistrictList; } set { _pDistrictList = value; } }
        }

        public class District
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}
