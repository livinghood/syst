using System;
using System.Windows;
using System.Windows.Controls;

namespace BUPSystem
{
    public class EmployeeIDNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            long? eID = value as long?;
            if (eID != null)
            {
                if (eID > 0)
                    return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, Message);
        }

        public String Message { get; set; }
    }
}
