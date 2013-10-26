using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckProductCategoryIDExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                if (ProductCategoryManagement.Instance.ProductCategoryIDExist(str))
                        return new ValidationResult(false, Message);

                return ValidationResult.ValidResult;

            }
            return new ValidationResult(false, "Skriv in kategori-id"); ;
        }

        public String Message { get; set; }
    }
}
