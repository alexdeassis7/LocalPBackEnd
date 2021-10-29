using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WA_LP.Models
{
    public class ExportModel
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public bool IsSP { get; set; }
        public IEnumerable<ParameterInExportModel> Parameters { get; set; }
    }
    public class ParameterInExportModel
    {
        public string Key { get; set; }
        public string Val { get; set; }
    }
}