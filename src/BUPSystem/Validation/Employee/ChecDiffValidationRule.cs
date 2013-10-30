using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckDiffValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                int diff;
                if (Int32.TryParse(str, out diff))
                {
                    if (diff != 0)
                        return new ValidationResult(false, Message);
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
