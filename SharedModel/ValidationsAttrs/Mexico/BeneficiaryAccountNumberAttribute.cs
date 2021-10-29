using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs.Mexico
{
    public class BeneficiaryAccountNumberAttribute : ValidationAttribute
    {
        Dictionary<int, int> peso = new Dictionary<int, int>() {
            {0, 3 },
            {1, 7 },
            {2, 1 }
        };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string clabe = value as string;
            if(string.IsNullOrEmpty(clabe) || clabe.Length != 18)
            {
                return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { validationContext.MemberName });
            }
            string realCheckSum = computeCheckSum(clabe.Substring(0, 17));
            string sentCheckSum = clabe.Substring(17, 1);

            if(realCheckSum == sentCheckSum)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { validationContext.MemberName });
        }

        public string computeCheckSum(string clabe)
        {
            return compute(clabe).ToString();
        }

        public int add(int sum, int digit, int i)
        {
            

            return (digit * peso[i % 3]) % 10;
        }

        public int compute(string clabe)
        {
            int i = 0;
            int sum = 0;
            foreach (char clabeChar in clabe)
            {
                sum += add(0, Int16.Parse(clabeChar.ToString()), i);
                i++;
            }


            return (10 - sum % 10) % 10;
        }
    }
}
