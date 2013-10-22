using System;
using System.Windows;
using System.Windows.Controls;

namespace BUPSystem
{
    public class TextIsNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (!String.IsNullOrEmpty(str))
            {
                int number;
                if (Int32.TryParse(str, out number))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, Message);
                }
                    
            }
            return ValidationResult.ValidResult;
        }

        public String Message { get; set; }
    }
}
