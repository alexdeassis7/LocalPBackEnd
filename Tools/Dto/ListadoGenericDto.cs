using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Dto
{
    public class ListadoGenericDto
    {
        public ListadoGenericDto()
        {
            Columns = new List<ColumnTypeDto>();
        }
        public string TableData { get; set; }
        public List<ColumnTypeDto> Columns { get; set; }
    }

    public class ColumnTypeDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
