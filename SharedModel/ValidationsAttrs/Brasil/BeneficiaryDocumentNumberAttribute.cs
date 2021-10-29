using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs.Brasil
{
    public class BeneficiaryDocumentNumberAttribute : ValidationAttribute
    {
        String[] fakeNumbers = new String[] {"00000000000", "00000000000000",
                                             "11111111111", "11111111111111",
                                             "22222222222", "22222222222222",
                                             "33333333333", "33333333333333",
                                             "44444444444", "44444444444444",
                                             "55555555555", "55555555555555",
                                             "66666666666", "66666666666666",
                                             "77777777777", "77777777777777",
                                             "88888888888", "88888888888888",
                                             "99999999999", "99999999999999"};

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string document = value as string;
            if(string.IsNullOrEmpty(document) || validateFakeDocument(document))
            {
                return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { validationContext.MemberName });
            }

            if(document.Length == 14)
            {
                if (validateCnpjDocument(document))
                {
                    return ValidationResult.Success;
                }
            }else if(document.Length == 11)
            {
                if (validateCpfDocument(document))
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { validationContext.MemberName });
        }

        private Boolean validateCpfDocument(string document)
        {
            int add = 0;
            int rev = 0;
            char[] arrayOfNumbers = document.ToCharArray();

            for (int i = 0; i < 9; i++)
            {
                int value = Int16.Parse(arrayOfNumbers[i].ToString());
                add += value * (10 - i);
            }

            rev = 11 - (add % 11);
            if (rev == 10 || rev == 11)
            {
                rev = 0;
            }

            if (rev != Int16.Parse(arrayOfNumbers[9].ToString())) return false;

            add = 0;

            for (int i = 0; i < 10; i++)
            {
                int value = Int16.Parse(arrayOfNumbers[i].ToString());
                add += value * (11 - i);
            }
            rev = 11 - (add % 11);
            if (rev == 10 || rev == 11)
            {
                rev = 0;
            }

            if (rev != Int16.Parse(arrayOfNumbers[10].ToString())) return false;

            return true;
        }

        private Boolean validateCnpjDocument(string document)
        {
            int sum = 0;
            int size = document.Length - 2;
            int pos = size - 7;
            String numbers = document.Substring(0, size);
            String digits = document.Substring(size);
            char[] arrayOfNumbers = numbers.ToCharArray();

            for (int i = size; i >= 1; i--)
            {
                int value = Int16.Parse(arrayOfNumbers[size - i].ToString());
                sum += value * pos--;
                if (pos < 2) pos = 9;
            }
            var result = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (result != Int16.Parse(digits[0].ToString())) return false;

            size = size + 1;
            numbers = document.Substring(0, size);
            arrayOfNumbers = numbers.ToCharArray();
            sum = 0;
            pos = size - 7;
            for (int i = size; i >= 1; i--)
            {
                int value = Int16.Parse(arrayOfNumbers[size - i].ToString());
                sum += value * pos--;
                if (pos < 2) pos = 9;
            }
            result = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (result != Int16.Parse(digits[1].ToString())) return false;

            return true;
        }

        private Boolean validateFakeDocument(String document)
        {
            if(fakeNumbers.ToList().IndexOf(document) == -1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
