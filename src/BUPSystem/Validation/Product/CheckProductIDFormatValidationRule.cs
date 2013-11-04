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
                //if (Regex.IsMatch(str, @"^[a-zåäöA-ZÅÄÖ]+$"))
                if (Regex.IsMatch(str, @"^[a-zåäöA-ZÅÄÖ0-9_-].*?$"))
                {
                    return str.Length != 4 ? new ValidationResult(false, "Skriv in ID (4 bokstäver)") : ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, "Skriv in ID (4 bokstäver)");                    
        }

        public String Message { get; set; }
    }
}
