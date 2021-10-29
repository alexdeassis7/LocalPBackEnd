using System;
using System.Collections.Generic;
using SharedModel.Models.View;

namespace SharedBusiness.View
{
    public class BIDashboard
    {
        public SharedModel.Models.View.Dashboard.List.Response LotTransaction(SharedModel.Models.View.Dashboard.List.Request data, string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
                SharedMaps.Converters.View.ViewMapper LPMapper = new SharedMaps.Converters.View.ViewMapper();
                List<SharedModelDTO.Models.LotBatch.Distributed.LotBatchAdminModel> LotBatch ;

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();


                 LotBatch = LPDAO.ListLots(data, customer);


                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.List.Response Response = LPMapper.MapperModels(LotBatch);
               
                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModel.Models.View.Dashboard.ListTransaction.Response ListTransaction(SharedModel.Models.View.Dashboard.ListTransaction.Request data, string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */
               // SharedMaps.Converters.View.ViewMapper LPMapper = new SharedMaps.Converters.View.ViewMapper();
               // List<SharedModel.Models.View.Dashboard.ListTransaction.Response> LotBatch;

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();


                //LotBatch = LPDAO.ListTransaction(data, customer);


                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.ListTransaction.Response Response = LPDAO.ListTransaction(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModel.Models.View.Dashboard.Indicators.Response DashboardIndicators(SharedModel.Models.View.Dashboard.Indicators.Request data, string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();


                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.Indicators.Response Response = LPDAO.DashboardIndicators(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Dashboard.DollarToday.Response DashboardDollarToday(string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();


                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.DollarToday.Response Response = LPDAO.DashboardDollarToday( customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Dashboard.ProviderCycle.Response.ResponseModel DashboardProviderCycle(string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();

                string str=LPDAO.DashboardProviderCycleReport(customer).Data;
                if(str == "USER ERROR")
                {
                    throw new Exception("USUARIO INCORRECTO");
                }
                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.ProviderCycle.Response.ResponseModel Response = new Dashboard.ProviderCycle.Response.ResponseModel(str);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModel.Models.View.Dashboard.MerchantCycle.Response.ResponseModel DashboardMerchantCycle(string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();

                string str = LPDAO.DashboardMerchantCycleReport(customer).Data;
                if (str == "USER ERROR")
                {
                    throw new Exception("USUARIO INCORRECTO");
                }
                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.MerchantCycle.Response.ResponseModel Response = new Dashboard.MerchantCycle.Response.ResponseModel(str);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dashboard.MainReport.Response DashboardMainReport(Dashboard.MainReport.Request request, string customer)
        {
            try
            {
                /* CASTEO REQUEST - LOT */

                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();


                /* CASTEO LOT - RESPONSE */
                SharedModel.Models.View.Dashboard.MainReport.Response Response = LPDAO.DashboardMainReport(request, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dashboard.ClientReport.Response DashboardClientReport(string customer)
        {
            try
            {
                /* CONN DAO LOT - LOT */
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();
                /* CASTEO LOT - RESPONSE */
                Dashboard.ClientReport.Response Response = LPDAO.DashboardClientReport(customer);

                return Response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dashboard.CashProviderCycle.Response DashboardCashProviderCycle(Dashboard.CashProviderCycle.Request request, string customer)
        {
            try
            {
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();
                SharedModel.Models.View.Dashboard.CashProviderCycle.Response response = LPDAO.DashboardCashProviderCycle(request, customer);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Dashboard.CashMerchantCycle.Response DashboardCashMerchantCycle(Dashboard.CashMerchantCycle.Request request, string customer)
        {
            try
            {
                DAO.View.DbDashboard LPDAO = new DAO.View.DbDashboard();
                SharedModel.Models.View.Dashboard.CashMerchantCycle.Response response = LPDAO.DashboardCashMerchantCycle(request, customer);

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
