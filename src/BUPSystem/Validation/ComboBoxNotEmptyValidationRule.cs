using System;
using System.Windows;
using System.Windows.Controls;

namespace BUPSystem
{
    public class ComboBoxNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                    return new ValidationResult(false, Message);
                    
            }
            return ValidationResult.ValidResult;
        }

        public String Message { get; set; }
    }
}
