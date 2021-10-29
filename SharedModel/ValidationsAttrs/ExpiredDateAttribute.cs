using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs
{
    public class ExpiredDateAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string date = value as string;

            if (date.Length == 8)
            {
                DateTime ActualDate = DateTime.Now.Date;
                DateTime PODate = new DateTime(Convert.ToInt16(date.Substring(0, 4)), Convert.ToInt16(date.Substring(4, 2)), Convert.ToInt16(date.Substring(6, 2)));
                

                if (PODate.Date >= ActualDate)
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
