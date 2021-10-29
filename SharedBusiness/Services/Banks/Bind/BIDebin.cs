using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedBusiness.Services.Banks.Bind
{
    public class BIDebin
    {

        public SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin DebinOp(SharedModel.Models.Services.Banks.Bind.Debin.RequestDebin data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.Services.Banks.Bind.Debin LPMapper = new SharedMaps.Converters.Services.Banks.Bind.Debin();
                SharedModelDTO.Models.Transaction.Debin.InternalDebinModel LotBatch = LPMapper.MapperModels(data);

                /* CONN DAO LOT - LOT */
                DAO.DataAccess.Services.DbDebin LPDAO = new DAO.DataAccess.Services.DbDebin();


                LotBatch = LPDAO.CreateDebinTransaction(LotBatch, customer, TransactionMechanism);


                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.Services.Banks.Bind.Debin.ResponseDebin Response = LPMapper.MapperModels(LotBatch);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines Debines(List<SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response> data, string customer, bool TransactionMechanism)
        public SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines Debines(SharedModel.Models.Services.Banks.Bind.Debin.ObtenerDebinesCobrar.Response data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CONN DAO */
                SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines Response = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseGetDebines();
                DAO.DataAccess.Services.DbDebin LPDAO = new DAO.DataAccess.Services.DbDebin();
                Response = LPDAO.ActualizarDebines(data, customer, TransactionMechanism);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines DebinDelete(SharedModel.Models.Services.Banks.Bind.Debin.RequestDeleteDebines data, string customer, bool TransactionMechanism)
        {
            try
            {
                /* CONN DAO */
                SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines Response = new SharedModel.Models.Services.Banks.Bind.Debin.ResponseDeleteDebines();
                DAO.DataAccess.Services.DbDebin LPDAO = new DAO.DataAccess.Services.DbDebin();
                Response = LPDAO.DeleteDebines(data, customer, TransactionMechanism);
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
