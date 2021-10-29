using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedModel.ValidationsAttrs
{
    public class CBUAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string cbu = value as string;
            if (cbu.Length == 22)
            {
                List<string> bank_list = GetBankList(validationContext);

                string pattern_01 = "7139713";
                string pattern_02 = "3971397139713";

                string bank_branchoffice = cbu.Substring(0, 7);
                Int32 verifier_digit_01 = Convert.ToInt32(cbu.Substring(7, 1));

                string accounttype_accountcurrency_filler_accountnumber = cbu.Substring(8, 13);
                Int32 verifier_digit_02 = Convert.ToInt32(cbu.Substring(21, 1));

                int Result01 = 0;
                int Result02 = 0;

                int Diff01 = 0;
                int Diff02 = 0;

                for (int i = 0; i < pattern_01.Length; i++)
                {
                    Result01 += Convert.ToInt32(bank_branchoffice.Substring(i, 1)) * Convert.ToInt32(pattern_01.Substring(i, 1));
                }

                for (int i = 0; i < pattern_02.Length; i++)
                {
                    Result02 += Convert.ToInt32(accounttype_accountcurrency_filler_accountnumber.Substring(i, 1)) * Convert.ToInt32(pattern_02.Substring(i, 1));
                }

                Diff01 = 10 - Convert.ToInt32(Result01.ToString().Substring(Result01.ToString().Length - 1, 1));
                Diff02 = 10 - Convert.ToInt32(Result02.ToString().Substring(Result02.ToString().Length - 1, 1));

                if (bank_list.Contains("00" + cbu.Substring(0, 3)))
                {
                    if (((verifier_digit_01 != 0 && Diff01 == verifier_digit_01) || (verifier_digit_01 == 0 && Diff01 == 10)) && ((verifier_digit_02 != 0 && Diff02 == verifier_digit_02) || (verifier_digit_02 == 0 && Diff02 == 10)))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        string name = validationContext.DisplayName;
                        string membername = validationContext.MemberName;
                        return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { membername });
                    }
                }
                else
                {
                    string name = validationContext.DisplayName;
                    string membername = validationContext.MemberName;
                    return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { membername });
                }
            }
            else {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { membername });
            }
        }

        private List<string> GetBankList(ValidationContext validationContext)
        {
            var result = (List<SharedModel.Models.Database.General.BankCodesModel.BankCodesOrdered>)validationContext.Items["bankCodes"];

            return result.FirstOrDefault(x=> x.countryCode == "ARG").bankCodes;
        }
    }
}
