using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BUPSystem
{
    public class CheckProductGroupNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (!String.IsNullOrEmpty(str))
            {

                if (str.Length > 50)
                {
                    return new ValidationResult(false, "Produktnamn får max ha 50 bokstäver");
                }
                else
                {
                    if (ProductGroupManagement.Instance.ProductGroupNameExist(str))
                        return new ValidationResult(false, Message);
                }

                return ValidationResult.ValidResult;

            }
            return new ValidationResult(false, "Skriv in ett produktgruppnamn");
        }

        public String Message { get; set; }
    }
}
