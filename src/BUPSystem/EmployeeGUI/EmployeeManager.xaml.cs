using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.EmployeeGUI
{
    /// <summary>
    /// Interaction logic for EmployeeManager.xaml
    /// </summary>
    public partial class EmployeeManager : Window
    {

        private Employee employee;

        private bool changeExistingEmployee;

        public EmployeeManager()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingEmployee)
            {
                long employeeId = long.Parse(tbEmployeeID.Text);
                int sallery = int.Parse(tbEmployeeSallary.Text);
                int employeeRate = int.Parse(tbEmployeeRate.Text);
                decimal vacancy = decimal.Parse(tbEmployeeVacancy.Text);
                EmployeeManagement.Instance.CreateEmployee(employeeId, tbEmployeeName.Text, sallery, employeeRate, vacancy );
            }
            else
            {
                EmployeeManagement.Instance.UpdateEmployee();
            }          
        }

        //Kontroll för att avdelningarna är = 0 om de inte är ifyllda
        //Kontroll för vilka och hur många avdelningar som är ifyllda

        //public void CheckDigitDotOnly(object sender, KeyPressEventArgs e)
        //{ FUNKAR INTE EFTERSOM DET INTE ÄR WINFORMS
        //    char ch = e.KeyChar;
        //    if (!Char.IsDigit(ch) && ch != 8 && ch != 46)
        //    {
        //        e.Handled = true;
        //    }
        //}

        private void tbEmployeeID_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

    }
}
