using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.View
{
    public class BlCycles
    {
        public SharedModel.Models.View.CycleModel.Create.Response InsertEntity(string customer, SharedModel.Models.View.CycleModel.Create.Response data)
        {
            try
            {
                DAO.DataAccess.View.DbCycles DbCycles = new DAO.DataAccess.View.DbCycles();
                DbCycles.InsertEntity(customer, data);
            }
            catch (Exception ex)
            {
                data.Status = "Error";
                data.StatusMessage = ex.ToString();
            }
            return data;
        }
    }
}
