using System;
using System.Reflection;

using log4net;

using LMS.Model;
using LMS.Model.Annotation;

namespace LMS.Data
{
    public abstract class DataValidator<T> : IDataValidator<T>
    {
        private static readonly ILog Log = LogManager.GetLogger("Trace");

        public const string ErrFieldRequired = "Field '{0}' is mandatory.";
        public const string ErrFieldLength = "Field '{0}' cannot exceed {1} characters.";

        public DataValidator()
        {
        }

        public virtual DataValidationResult Validate(T item)
        {
            Log.Info(String.Format("Validate resource of type '{0}'.", typeof(T).Name));

            DataValidationResult result = null;
            PropertyInfo[] props = typeof(T).GetProperties();

            foreach (PropertyInfo prop in props)
            {
                // Validate annotated properties.
                result = ValidateAttributes(item, prop);
                if (result != null)
                    return result;                
            }

            return new DataValidationResult() { IsValid = true };
        }

        private DataValidationResult ValidateAttributes(object item, PropertyInfo prop)
        {
            DataValidationResult result = null;
            object[] attrs = prop.GetCustomAttributes(true);
            bool required = false;

            foreach (object attr in attrs)
            {
                // Check if property is required (mandatory).
                RequiredAttribute requiredAttr = attr as RequiredAttribute;
                if (requiredAttr != null)
                {
                    required = true;
                    if (prop.GetValue(item) == null)
                        return new DataValidationResult() { IsValid = false, Message = string.Format(ErrFieldRequired, prop.Name) };
                }

                // Check if string does not exceed maximum string length.
                StringLengthAttribute strlAttr = attr as StringLengthAttribute;
                if (strlAttr != null)
                {
                    string str = prop.GetValue(item) as string;
                    if (str != null && str.Length > strlAttr.Length)
                        return new DataValidationResult() { IsValid = false, Message = string.Format(ErrFieldLength, prop.Name, strlAttr.Length) };
                }

                // Check if array of strings does not exceed maximum string length.
                ArrayStringLengthAttribute astrlAttr = attr as ArrayStringLengthAttribute;
                if (astrlAttr != null)
                {
                    string[] str = prop.GetValue(item) as string[];
                    if (str != null && (string.Join(" ", str)).Length > astrlAttr.Length)
                        return new DataValidationResult() { IsValid = false, Message = string.Format(ErrFieldLength, prop.Name, astrlAttr.Length) };
                }

                // Validate nested property of type 'CompositeDataType'.
                if (prop.PropertyType.BaseType.Equals(typeof(CompositeDataType)) && required)
                {                    
                    CompositeDataType compositeDt = prop.GetValue(item) as CompositeDataType;
                    PropertyInfo[] nestedProps = prop.PropertyType.GetProperties();

                    foreach (PropertyInfo nestedProp in nestedProps)
                    {
                        // Validate annotated properties of nested property.
                        result = ValidateAttributes(compositeDt, nestedProp);
                        if (result != null)
                            return result;
                    }
                }
            }

            return null;
        }
    }
}
