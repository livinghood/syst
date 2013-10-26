using System;
using System.Windows;
using System.Windows.Controls;

namespace BUPSystem
{
    public class ComboBoxNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return value == null ? new ValidationResult(false, Message) : ValidationResult.ValidResult;
        }

        public String Message { get; set; }
    }
}
