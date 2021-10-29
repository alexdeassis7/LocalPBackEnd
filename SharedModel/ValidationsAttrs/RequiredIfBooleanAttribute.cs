using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace SharedModel.ValidationsAttrs
{
    public class RequiredIfBooleanAttribute : ValidationAttribute
    {
        RequiredAttribute _innerAttribute = new RequiredAttribute();
        public string _dependentProperty { get; set; }
        public object _targetValue { get; set; }

        public RequiredIfBooleanAttribute(string dependentProperty, object targetValue)
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
                if (Convert.ToBoolean(dependentValue) == Convert.ToBoolean(_targetValue))
                {
                    if (Convert.ToInt32(value) > 0)
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
