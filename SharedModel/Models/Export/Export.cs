using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModel.Models.View;
namespace SharedModel.Models.Export
{
   public class Export
    {

        public class Request {

            public List<string> ColumnsReport { get; set; }
            public Report.List.Request requestReport { get; set; }
            public string TypeReport { get; set; }

        }

        public class Excel
        {
            public int OrderColumn { get; set; }
            public string FieldFromSP { get; set; }
            public string DisplayName { get; set; }
        }

    }
}
