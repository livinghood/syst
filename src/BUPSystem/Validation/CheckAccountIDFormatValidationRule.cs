using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckAccountIDFormatValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                int accountid;
                if (Int32.TryParse(str, out accountid))
                {
                    if (accountid >= 1000 && accountid <= 9999)
                    {
                        return ValidationResult.ValidResult;
                    }
                    return new ValidationResult(false, Message);
                }
                else
                {
                    return new ValidationResult(false, Message);
                }
               
            }
            return new ValidationResult(false, Message);

        }

        public String Message { get; set; }
    }
}
