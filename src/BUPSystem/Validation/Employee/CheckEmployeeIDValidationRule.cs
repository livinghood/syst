using System;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;
using System.Collections.Generic;

namespace BUPSystem
{
    public class CheckEmployeeIDValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str != null)
            {
                long employeeid;
                if (long.TryParse(str, out employeeid))
                {
                    if(employeeid < 1000000000 || employeeid > 9999999999)
                        return new ValidationResult(false, "Personnummer är inte korrekt (använd 10 siffror)");

                    if (EmployeeManagement.Instance.EmployeeExist(employeeid))
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
