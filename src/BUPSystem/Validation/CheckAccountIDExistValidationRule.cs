using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckAccountIDExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                int accountid;
                if (Int32.TryParse(str, out accountid))
                {
                    if (AccountManagement.Instance.AccountExist(accountid))
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
