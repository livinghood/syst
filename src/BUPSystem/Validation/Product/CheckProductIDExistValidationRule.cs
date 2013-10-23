using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckProductIDExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                if (str.Length != 6)
                {
                    return new ValidationResult(false, "Välj produktgrupp och skriv in ID");
                }
                else
                {
                    if (ProductManagement.Instance.ProductExist(str))
                        return new ValidationResult(false, Message);
                }

                return ValidationResult.ValidResult;

            }
            return new ValidationResult(false, "Välj produktgrupp och skriv in ID"); ;
        }

        public String Message { get; set; }
    }
}
