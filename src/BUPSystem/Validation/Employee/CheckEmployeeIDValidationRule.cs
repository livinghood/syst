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
                    if(employeeid <= 0)
                        return new ValidationResult(false, Message);

                    if (!ValidatePersonNumber(str))
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
        
        /// <summary>
        /// Validate id
        /// </summary>
        /// <param name="pnr"></param>
        /// <returns></returns>
        private static bool ValidatePersonNumber(string pnr)
        {
            
            int summary = 0;
            for (int i = 0; i < pnr.Length; i++)
            {
                int number = int.Parse(pnr[i].ToString());
                int partialsummary = 0;

                if (i % 2 == 0)
                {
                    partialsummary = number * 2;

                    if (partialsummary > 9)
                        partialsummary -= 9;
                }
                else
                    partialsummary = number;

                summary += partialsummary;
            }

            return (summary % 10 == 0);
        }    
    }

}
