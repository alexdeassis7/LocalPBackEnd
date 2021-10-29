using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedModel.ValidationsAttrs
{
    public class RequiredIfHasValueAttribute : ValidationAttribute
    {
        RequiredAttribute _innerAttribute = new RequiredAttribute();
        public string _dependentProperty { get; set; }
        public object _targetValue { get; set; }

        public RequiredIfHasValueAttribute(string dependentProperty, object targetValue)
        {
            this._dependentProperty = dependentProperty;
            this._targetValue = targetValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (field != null)
            {
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);

                if (!string.IsNullOrEmpty(dependentValue.ToString()) && Convert.ToBoolean(_targetValue))
                {
                    if (value != null)
                    {
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            string name = validationContext.DisplayName;
                            string membername = validationContext.MemberName;
                            return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { membername });
                        }
                    }
                    else
                    {
                        string name = validationContext.DisplayName;
                        string membername = validationContext.MemberName;
                        return new ValidationResult(ErrorMessage = base.ErrorMessage, new string[] { membername });
                    }                    
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult(FormatErrorMessage(_dependentProperty));
            }
        }
    }
}
