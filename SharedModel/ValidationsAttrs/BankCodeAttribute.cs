using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs
{
    class BankCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> bank_list = GetBankList(validationContext);
            string bank_code = value as string;

            if (bank_list.IndexOf(bank_code) > -1)
            {
                return ValidationResult.Success;
            }

            string membername = validationContext.MemberName;
            return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
        }
        private List<string> GetBankList(ValidationContext validationContext)
        {
            var countryCode = (string)validationContext.Items["countryCode"];
            var result = (List<SharedModel.Models.Database.General.BankCodesModel.BankCodesOrdered>)validationContext.Items["bankCodes"];

            return result.FirstOrDefault(x => x.countryCode == countryCode).bankCodes;
        }
    }
}
