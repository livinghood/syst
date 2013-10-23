using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BUPSystem
{
    public class CheckProductIDFormatValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                if (Regex.IsMatch(str, @"^[a-zA-Z]+$"))
                {
                    if (str.Length != 4)
                    {
                        return new ValidationResult(false, "Skriv in ID (4 bokstäver)");
                    }
                    else
                    {
                        return ValidationResult.ValidResult;
                    }
                }
            }
            return new ValidationResult(false, "Skriv in ID (4 bokstäver)");
            
            

        }

        public String Message { get; set; }
    }
}
