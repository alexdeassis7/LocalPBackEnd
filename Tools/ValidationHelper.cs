using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class ValidationHelper
    {
        public static string ValidateAgenciaDV(string dv) 
        {
            if (dv.Length == 4) 
            {
                string reverseDV;
                reverseDV = reverseString(dv);

                int sum = 0;
                int weight = 2;
                int Digito;
                string DigitoVerificador;
                foreach (char dvChar in reverseDV)
                {
                    sum += Convert.ToInt32(dvChar.ToString()) * weight;
                    weight++;
                }

                Digito = 11 - (sum % 11);
                DigitoVerificador = Digito.ToString().Trim();
                if (Digito == 10)
                {
                    DigitoVerificador = "X";
                }
                if (Digito == 11)
                {
                    DigitoVerificador = "0";
                }

                return dv + DigitoVerificador;

            }
            return dv;
        }

        public static List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request> ValidateRequestBrasil(List<SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request> requestList) 
        {
            foreach (SharedModel.Models.Services.Brasil.PayOutBrasil.Create.Request request in requestList) 
            {
                string bankBranch = request.bank_branch;
                if(bankBranch.Length == 3)
                {
                    bankBranch = bankBranch.PadLeft(4, char.Parse("0"));
                }
                request.bank_branch = ValidateAgenciaDV(bankBranch);
            }

            return requestList;
        }

        public static string reverseString(string myStr) 
        {
            char[] stringArray = myStr.ToArray();
            stringArray = stringArray.Reverse().ToArray();
            return new string(stringArray);
        }
    }
}
