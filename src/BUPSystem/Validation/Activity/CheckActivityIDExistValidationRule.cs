using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckActivityIDExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {

                if (str.Length != 6)
                {
                    return new ValidationResult(false, "Välj avdelning och skriv in del-ID");
                }

                return ActivityManagement.Instance.ActivityExist(str) ? new ValidationResult(false, "Aktivitets-ID existerar redan.") : ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Välj avdelning och skriv in del-ID");
        }

        public String Message { get; set; }
    }
}
