using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace SharedModel.Models.Shared
{
    public class ValidationModel
    {
        public static List<General.ErrorModel.ValidationErrorGroup> ValidatorModel(object model,IDictionary<object,object> contextItems = null)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: contextItems);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);

            List<General.ErrorModel.ValidationErrorGroup> errores = new List<General.ErrorModel.ValidationErrorGroup>();

            if (!isValid)
            {
                List<General.ErrorModel.ValidationError> PseudoError = new List<General.ErrorModel.ValidationError>();
                foreach (ValidationResult vr in validationResults)
                {
                    if (vr.ErrorMessage.Contains("#"))
                    {

                        string[] aux = vr.ErrorMessage.Split('#');
                        PseudoError.Add(new General.ErrorModel.ValidationError { Key = ((string[])vr.MemberNames).FirstOrDefault(), Message = aux[0], CodeTypeError = aux[1].Trim() });
                    }
                    else
                    {
                        PseudoError.Add(new General.ErrorModel.ValidationError { Key = ((string[])vr.MemberNames).FirstOrDefault(), Message = vr.ErrorMessage });
                    }
                }

                errores =           (
                                    from pe in PseudoError
                                    group pe
                                    by pe.Key into KeyGroup
                                    select new General.ErrorModel.ValidationErrorGroup { Key = KeyGroup.Key, Messages = KeyGroup.Select(o => o.Message).ToList(), CodeTypeError = KeyGroup.Select(o => o.CodeTypeError).ToList() }
                                    ).ToList();
            }

            return errores;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
