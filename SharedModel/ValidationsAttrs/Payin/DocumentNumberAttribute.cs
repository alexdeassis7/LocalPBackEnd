using SharedModel.Models.General;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs.Payin
{
    public class DocumentNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string documentNumber = value as string;
            documentNumber = documentNumber == null ? "" : documentNumber; 
            string country_code = GetCountryCode(validationContext);
            ValidationResult validation = ValidationResult.Success;
            

            switch (country_code) 
            {
                case "ARG":
                    validation = ValidateArg(documentNumber,validationContext);
                    break;
                case "COL":
                    validation = ValidateCol(documentNumber, validationContext);
                    break;
                case "BRA":
                    validation = ValidateBra(documentNumber, validationContext);
                    break;
                case "MEX":
                    validation = ValidateMex(documentNumber, validationContext);
                    break;
                case "URY":
                    validation = ValidateUry(documentNumber, validationContext);
                    break;
                case "CHL":
                    validation = ValidateChl(documentNumber, validationContext);
                    break;
            }

            return validation;
        }

        private string GetCountryCode(ValidationContext validationContext)
        {
            string result = (string)validationContext.Items["countryCode"];

            return result;
        }

        private ValidationResult ValidateArg(string cuit, ValidationContext validationContext) 
        {
            if (cuit.Length == 11)
            {
                if (Regex.Match(cuit, "^(\\b(20|23|24|27|30|33|34)(\\D)?[0-9]{8}(\\D)?[0-9])$").Success)
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
                        return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: arg invalid format, only allow: numbers.#INVALID", new string[] { membername });
                    }
                }
                else {
                    string name = validationContext.DisplayName;
                    string membername = validationContext.MemberName;
                    return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: arg invalid format, only allow: numbers.#INVALID", new string[] { membername });
                }
            }
            else
            {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: arg incorrect length, the correct format has 11 characters.#LENGTH", new string[] { membername });
            }
        }

        private ValidationResult ValidateCol(string document, ValidationContext validationContext) 
        {
            if (Regex.Match(document, "^[0-9]{1,15}$").Success) 
            {
                return ValidationResult.Success;
            }
            else
            {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: col invalid length, must be only numbers and between 1 and 15 characters.#INVALID", new string[] { membername });
            }
        }

        private ValidationResult ValidateBra(string document, ValidationContext validationContext) 
        {
            if (Regex.Match(document, "^(?=(?:.{14}|.{11})$)[0-9]*$").Success)
            {
                return ValidationResult.Success;
            }
            else
            {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: bra invalid length, must be only numbers and 11 or 14 characters length.#INVALID", new string[] { membername });
            }
        }

        private ValidationResult ValidateMex(string document, ValidationContext validationContext)
        {
            if (document.Trim().Length > 0)
            {
                if (Regex.Match(document, "^([A-ZÑ&]{3,4}) ?(?:- ?)?(\\d{2}(?:(?:0[1-9]|1[0-2])(?:0[1-9]|1[0-9]|2[0-8])|(?:0[469]|11)(?:29|30)|(?:0[13578]|1[02])(?:29|3[01]))|(?:0[048]|[2468][048]|[13579][26])0229) ?(?:- ?)?([A-Z\\d]{2})([A\\d])$").Success)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    string name = validationContext.DisplayName;
                    string membername = validationContext.MemberName;
                    return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: invalid RFC .#INVALID", new string[] { membername });
                }
            }
            else {
                return ValidationResult.Success;
            }
        }

        private ValidationResult ValidateChl(string document, ValidationContext validationContext)
        {
            if (Regex.Match(document, "^[0-9]{8,12}$").Success)
            {
                return ValidationResult.Success;
            }
            else
            {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: chl invalid length, must be only numbers and between 8 and 12 characters.#INVALID", new string[] { membername });
            }
        }

        private ValidationResult ValidateUry(string document, ValidationContext validationContext)
        {
            if (Regex.Match(document, "^[0-9]{8,12}$").Success)
            {
                return ValidationResult.Success;
            }
            else
            {
                string name = validationContext.DisplayName;
                string membername = validationContext.MemberName;
                return new ValidationResult(ErrorMessage = "Parameter :: payer_document_number :: ury invalid length, must be only numbers and between 8 and 12 characters.#INVALID", new string[] { membername });
            }
        }
    }    
}
