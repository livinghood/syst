using System;
using System.Windows;
using System.Windows.Controls;

namespace BUPSystem
{
    public class TextIsPositiveNumberOrNullValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (!String.IsNullOrEmpty(str))
            {
                int number;
                if (Int32.TryParse(str, out number))
                {
                    if (number < 0)
                    {
                        return new ValidationResult(false, Message);
                    }
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
