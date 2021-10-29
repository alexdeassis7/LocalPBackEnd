using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedModel.ValidationsAttrs
{
    class RUTAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string membername = validationContext.MemberName;
            Regex expresion = new Regex("^([0-9.K-]{9,12})$");
            if (!expresion.IsMatch(value.ToString()))
            {
                return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
            }

            var rut = value.ToString().Replace(".", "").Replace("-", "");
            string dv = rut.Substring(rut.Length - 1, 1);
            rut = rut.Substring(0, rut.Length - 1);
            if (dv != digitoVerificador(Int32.Parse(rut))) 
            {
                return new ValidationResult(ErrorMessage = ErrorMessage, new string[] { membername });
            }

            return ValidationResult.Success;

        }

        private string digitoVerificador(int rut)
        {
            int Digito;
            int Contador;
            int Multiplo;
            int Acumulador;
            string RutDigito;

            Contador = 2;
            Acumulador = 0;

            while (rut != 0)
            {
                Multiplo = (rut % 10) * Contador;
                Acumulador = Acumulador + Multiplo;
                rut = rut / 10;
                Contador = Contador + 1;
                if (Contador == 8)
                {
                    Contador = 2;
                }

            }

            Digito = 11 - (Acumulador % 11);
            RutDigito = Digito.ToString().Trim();
            if (Digito == 10)
            {
                RutDigito = "K";
            }
            if (Digito == 11)
            {
                RutDigito = "0";
            }
            return (RutDigito);
        }

    }
}