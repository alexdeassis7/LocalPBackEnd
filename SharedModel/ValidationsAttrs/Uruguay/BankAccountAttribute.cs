using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedModel.ValidationsAttrs.Uruguay
{
    public class BankAccountAttribute : ValidationAttribute
    {
        public Dictionary<string, int> BankAccountsLength = new Dictionary<string, int>() {
            { "1001", 14 }, // BROU
            { "1091", 15 }, // Hipotecario
            { "1205", 10 }, // Citibank
            { "1110", 9 },  // Bandes 
            { "1113", 7 },  // ITAU
            { "1128", 10 }, // Scotiabank
            { "1137", 19 }, // Santander
            { "1246", 12 }, // Nacion
            { "1153", 9 },  // BBVA
            { "1157", 10 }, // HSBC
            { "1162", 9 },  // Heritage
            { "1361", 7 },  // BAPRO
            { "7905", 8 }   // FORTEX
        };
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string bankAccount = value as string;

            bool flag = false;
            object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty("bank_code");
            object propertyValue = property.GetValue(instance) == null ? "" : property.GetValue(instance);

            switch (propertyValue)
            {
                case "1153": // BBVA
                    flag = (bankAccount.Length <= BankAccountsLength[propertyValue.ToString()]);
                    break;
                case "1137": // Santander
                    flag = (bankAccount.Length <= BankAccountsLength[propertyValue.ToString()]);
                    break;
                default:
                    if (BankAccountsLength.ContainsKey(propertyValue.ToString())) { 
                        flag = (bankAccount.Length == BankAccountsLength[propertyValue.ToString()]);
                    }
                    break;
            }

            if (flag)
            {
                return ValidationResult.Success;
            }

            string membername = validationContext.MemberName;
            return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
        }
    }
}
