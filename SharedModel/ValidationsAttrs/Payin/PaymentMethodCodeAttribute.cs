using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs.Payin
{
    public class PaymentMethodCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> paymentMethods = GetPaymentCodesList(validationContext);

            string paymentMethodCode = value as string;

            if(paymentMethods.IndexOf(paymentMethodCode) > -1)
            {
                return ValidationResult.Success;
            }

            string membername = validationContext.MemberName;
            return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
        }

        private List<string> GetPaymentCodesList(ValidationContext validationContext)
        {
            List<string> result = (List<string>)validationContext.Items["paymentMethods"];

            return result;
        }
    }
}
