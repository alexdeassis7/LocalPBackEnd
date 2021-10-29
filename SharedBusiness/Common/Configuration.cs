using SharedBusiness.Log;
using DAO.DataAccess.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedModel.Models.Database.General.BankCodesModel;

namespace SharedBusiness.Common
{
    public static class Configuration
    {
        public static void Init()
        {
            SharedMaps.Maps.LocalPaymentMapper.InitializeAutoMapper();
            LogService.Configure();

        }

        public static List<BankCodes> GetBankCodes() 
        {
            DbPayOut LPDAO = new DbPayOut();
            var BankCodes = LPDAO.GetBankCodes();
            return BankCodes;
        }

        public static List<string> GetPaymentMethodCodes()
        {
            DbPayIn LPDAO = new DbPayIn();
            var PaymentMethods = LPDAO.GetPayinPaymentMethods();
            return PaymentMethods;
        }
    }
}
