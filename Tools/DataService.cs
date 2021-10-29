using SharedModel.Models.Export;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class DataService
    {
        const string connStringName = "connDbLocalPayment";
        public DataSet GetData(Tools.Dto.Export export)
        {
            // SqlDataReader reader = null;
            var ds = new DataSet();
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connStringName].ConnectionString))
            {
                var strComm = !export.IsSP ? string.Format("select * from {0}", export.Name) : export.Name;

                var command = new SqlCommand(strComm, connection);
                command.CommandTimeout = 180;

                if (export.IsSP)
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (export.Parameters != null)
                    {
                        export.Parameters.ToList().ForEach(p => {
                            var param = new SqlParameter();
                            param.ParameterName = string.Format("@{0}", p.Key);
                            param.Value = p.Val;

                            command.Parameters.Add(param);
                        });
                    }
                }
                connection.Open();
                using (var sda = new SqlDataAdapter(command))
                {
                    //  reader = command.ExecuteReader();
                    sda.Fill(ds);
                }
            }
            return ds;
        }
    }
}
