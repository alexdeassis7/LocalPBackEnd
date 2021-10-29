using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Database.General
{
    public class Result
    {
        public class Validation
        {
            public bool ValidationStatus { get; set; }
            public string ValidationMessage { get; set; }
        }
    }
}
