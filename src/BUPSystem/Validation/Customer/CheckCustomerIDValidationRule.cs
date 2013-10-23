using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckCustomerIDValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                if (CustomerManagement.Instance.CustomerExist(str))
                    return new ValidationResult(false, Message);
            }
            return ValidationResult.ValidResult;
        }

        public String Message { get; set; }
    }
}
