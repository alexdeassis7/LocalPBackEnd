using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs.Mexico
{
    public class BankCodeAttribute : ValidationAttribute
    {
        public string[] BankCodes = { 
            "002", "006", "009", "012", "014", "019", "021", "030", "032", "036", "037", "042", "044", "058", "059", "060", "062", 
            "072", "102", "103", "106", "108", "110", "112", "113", "116", "124", "126", "127", "128", "129", "130", "131", "132", "133", 
            "134", "135", "136", "137", "138", "139", "140", "141", "143", "145", "156", "166", "168", "600", "601", "602", "605", "606", 
            "607", "608", "610", "614", "615", "616", "617", "618", "619", "620", "621", "622", "623", "626", "627", "628", "629", "630", 
            "631", "632", "633", "634", "636", "637", "638", "640", "642", "646", "647", "648", "649", "651", "652", "653", "655", "656", 
            "659", "670", "901", "902","001" ,"147" ,"148" ,"150" ,"151" ,"152" ,"154" ,"155" ,"157" ,"158" ,"160" ,"613" ,"677" ,"680" 
                ,"683" ,"684" ,"685" ,"686" ,"689" ,"903" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string bankCode = value as string;

            object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty("beneficiary_account_number");
            object propertyValue = property.GetValue(instance);
            // check if bank code exists in our array
            if(Array.IndexOf(BankCodes, bankCode) > -1 && !string.IsNullOrEmpty(propertyValue.ToString()))
            {
                // check that bank code equals the specified in account number
                if(bankCode.Equals(propertyValue.ToString().Substring(0, 3)))
                {
                    return ValidationResult.Success;
                }
            }

            string membername = validationContext.MemberName;
            return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
        }
    }
}
