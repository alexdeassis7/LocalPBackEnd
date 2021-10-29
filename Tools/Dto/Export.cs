using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Dto
{
    public class Export
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public bool IsSP { get; set; }
        public IEnumerable<ParameterInExport> Parameters { get; set; }
    }

    public class ParameterInExport
    {
        public string Key { get; set; }
        public string Val { get; set; }
    }
}
