using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckUserAccounUsernameExistValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (!String.IsNullOrEmpty(str))
            {

                if (str.Length > 30)
                {
                    return new ValidationResult(false, "Användarnamn får max ha 30 bokstäver");
                }
                else
                {
                    if (UserManagement.Instance.UserNameExist(str))
                        return new ValidationResult(false, Message);
                }

                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Fyll i användarnamn");
        }

        public String Message { get; set; }
    }
}
