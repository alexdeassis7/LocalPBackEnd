using System;
using System.Collections.Generic;

namespace SharedModel.Models.General
{
    public class ErrorModel
    {
        public class ValidationError
        {
            public string Key { get; set; }
            public string Message { get; set; }

            public string CodeTypeError { get; set; }
        }
        public class ValidationErrorGroup
        {
            public string Key { get; set; }
            public List<string> Messages { get; set; }

            public List<string> CodeTypeError { get; set; }
        }
        public class Error
        {
            List<ValidationErrorGroup> errors = new List<ErrorModel.ValidationErrorGroup>();
            public List<ValidationErrorGroup> Errors { get { return errors; } set { this.errors = value; } }
            public bool HasError
            {
                get
                {
                    return Errors.Count == 0 || Errors == null ? false : true;
                }
            }
        }
    }

    public class GeneralErrorModel
    {
        public string Code { get; set; }
        public string CodeDescription { get; set; }
        public string Message { get; set; }
    }
    public class ErrorCode
    {
        public string Key { get; set; }
        public string Code { get; set; }
    }
}
