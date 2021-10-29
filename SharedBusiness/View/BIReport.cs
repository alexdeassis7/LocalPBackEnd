using System;
using System.Collections.Generic;
using SharedModel.Models.Security;

namespace SharedBusiness.View
{
    public class BIReport
    {
        public SharedModel.Models.View.Report.List.Response AccountTransaction(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.AccountTransaction(data, customer, countryCode);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListTransaction(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListTransaction(data, customer, countryCode);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListMerchantBalance(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListMerchantBalance(data, customer, countryCode);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListMerchantProyectedBalance(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListMerchantProyectedBalance(data, customer, countryCode);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListActivityReport(SharedModel.Models.View.Report.List.Request data, string customer, string countryCode)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListActivityReport(data, customer, countryCode);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListTransactionClientDetails(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListTransactionClientDetails(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModel.Models.View.Report.List.Response ListHistoricalFx(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListHistoricalFx(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModel.Models.View.Report.List.Response ListOperationRetention (SharedModel.Models.View.Report.List.Request data, string customer)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListOperationRetention(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListOperationRetentionMonthly(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.ListOperationRetentionMonthly(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SharedModel.Models.View.Report.List.Response List_Merchant_Report(SharedModel.Models.View.Report.List.Request data, string customer)
        {
            try
            {
                DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

                SharedModel.Models.View.Report.List.Response Response = ReportDAO.List_Merchant_Report(data, customer);

                return Response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SharedModel.Models.View.Report.List.Response ListTransactionsError(SharedModel.Models.View.Report.List.Request data, string customer_id)
        {
            SharedModel.Models.View.Report.List.Response result = new SharedModel.Models.View.Report.List.Response();
            DAO.View.DbReport ReportDAO = new DAO.View.DbReport();

            result = ReportDAO.ListTransactionsError(data, customer_id);
            return result;
        }


    }
}
