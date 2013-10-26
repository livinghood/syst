using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BUPSystem
{
    public class CheckProductGroupIDValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                if (!Regex.IsMatch(str, @"^[a-zA-Z]+$"))
                {
                    return new ValidationResult(false, "Grupp-ID får bara innehålla bokstäver");
                }
                if (str.Length != 2)
                {
                    return new ValidationResult(false, "Grupp-ID ska bestå av 2 bokstäver");
                }
                else
                {
                    if (ProductGroupManagement.Instance.ProductGroupIDExist(str))
                        return new ValidationResult(false, Message);
                }

                return ValidationResult.ValidResult;

            }
            return new ValidationResult(false, "Skriv in ett grupp-ID"); ;
        }

        public String Message { get; set; }
    }
}
