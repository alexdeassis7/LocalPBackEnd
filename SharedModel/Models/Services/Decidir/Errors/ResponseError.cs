using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.Models.Services.Decidir.Errors
{
  

    public class Result
    {
        public string error_type { get; set; }
        public string entity_name { get; set; }
        public string id { get; set; }


        public List<ErrorInfo> validation_errors { get; set; }
    }

    public class ErrorInfo {
        public string code { get; set; }
        public string param { get; set; }
    }


}
