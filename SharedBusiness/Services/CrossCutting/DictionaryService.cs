using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAO.DataAccess.Services;
using DAO.Services.PayOut;

namespace SharedBusiness.Services.CrossCutting
{
     public static class DictionaryService
    {
        private const string ConnStringLocalPayment = "connDbLocalPayment";
        private const string SpPayoutsConcepts = "[LP_Entity].[Get_Payout_Concepts]";

        public static IEnumerable<PayoutConcept> PayoutConcepts { get; set; }

        public static void Init()
        {
            Load_Payouts_Concepts();
        }
        private static void Load_Payouts_Concepts()
        {
            PayoutConcepts = new DbDictionaryService().Get_Payouts_Concepts();
        }
        public static bool PayoutConceptExist(string code)
        {
          return PayoutConcepts.Any(x => x.Code == code);
        }
    }
}
