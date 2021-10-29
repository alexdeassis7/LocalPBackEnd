using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.File.FromExcel.Services
{
    class PayOut
    {
        public List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> ReadExcelBatchLotTransaction(string data)
        {
            var bytes = Convert.FromBase64String(data);

            List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response> ExcelData = new List<SharedModel.Models.Services.Banks.Galicia.PayOut.Create.Response>();

            //// PROCESAR EXCEL...

            return ExcelData;
        }
    }
}
