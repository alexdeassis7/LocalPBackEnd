using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedModel.ValidationsAttrs
{
    public class CUITAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string cuit = value as string;

            if (cuit.Length == 11)
            {
                Int32 acum = 0;
                char[] digits = cuit.ToCharArray();
                Int32 verifier_digit = Convert.ToInt32(digits.Last().ToString());
                digits = digits.Take(digits.Count() - 1).ToArray();

                for (int i = 0; i < digits.Length; i++)
                {
                    acum += Convert.ToInt32(digits[9 - i].ToString()) * (2 + (i % 6));
                }

                Int32 verif = 11 - (acum % 11);
                if (verif == 11)
                {
                    verif = 0;
                }

                if (verifier_digit == verif)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    string name = validationContext.DisplayName;
                    string membername = validationContext.MemberName;
                    return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
                }
            }
            else
            {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
            }
        }
    }
}
