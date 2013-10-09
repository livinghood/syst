using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer;
using System.Windows.Controls;
using System.Text.RegularExpressions;

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

        public EmployeeManager(Employee employee)
        {
            InitializeComponent();

            changeExistingEmployee = true;

            DataContext = employee;

            this.employee = employee;
        }

        /// <summary>
        /// When Save button is pressed, if no changing, create new Employee and EmployeePlacement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingEmployee)
            {
                long employeeId = long.Parse(tbEmployeeID.Text);
                if (ControlDepartments(employeeId) == true)
                {
                    int sallery = int.Parse(tbEmployeeSallary.Text);
                    int employeeRate = int.Parse(tbEmployeeRate.Text);
                    decimal vacancy = decimal.Parse(tbEmployeeVacancy.Text);
                    EmployeeManagement.Instance.CreateEmployee(employeeId, tbEmployeeName.Text, sallery, employeeRate, vacancy);
                }
            }
            else
            {
                EmployeeManagement.Instance.UpdateEmployee();
            }          
        }

        /// <summary>
        /// Control wich departments that got filled textboxes and create EmployeePlacement with right departments/departments
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns>false if no field got data, else true</returns>
        private bool ControlDepartments(long employeeId)
        {
            decimal allocate;
            if (tbAdmAvd.Text.Trim().Length > 0 && tbAdmAvd.Text != "0")
            {
                allocate = decimal.Parse(tbAdmAvd.Text);
                EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "Administrativ", allocate);
                return true;
            }
            if (tbDriftAvd.Text.Trim().Length > 0 && tbDriftAvd.Text != "0")
            {
                allocate = decimal.Parse(tbDriftAvd.Text);
                EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "Drift", allocate);
                return true;
            }
            if (tbSellAvd.Text.Trim().Length > 0 && tbSellAvd.Text != "0")
            {
                allocate = decimal.Parse(tbSellAvd.Text);
                EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "Försäljning", allocate);
                return true;
            }
            if (tbProdAvd.Text.Trim().Length > 0 && tbProdAvd.Text != "0")
            {
                allocate = decimal.Parse(tbProdAvd.Text);
                EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "Produktion", allocate);
                return true;
            }
            else
            {
                MessageBox.Show("Måste placeras på en avdelning");
                return false;
            }
        }

        /// <summary>
        /// When text are changed in Textbox tbEmployeeRate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbEmployeeRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateAnnualEmployee();
        }

        /// <summary>
        /// When text are changed in Textbox tbEmployeeVacancy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbEmployeeVacancy_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateAnnualEmployee();
        }

        /// <summary>
        /// Calculate the field AnnualEmployee
        /// </summary>
        private void CalculateAnnualEmployee()
        {
            if (tbEmployeeRate.Text == "")
            {
                tbAnnualEmployee.Text = "";
            }
            else
            {
                int rate = int.Parse(tbEmployeeRate.Text);
                int vacancy;

                if (tbEmployeeVacancy.Text == "")
                {
                    vacancy = 0;
                }
                else
                {
                    vacancy = int.Parse(tbEmployeeVacancy.Text);
                }

                tbAnnualEmployee.Text = (rate - vacancy).ToString();
            }
        }

        //private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    Regex regex = new Regex("[^0-9]+");
        //    e.Handled = regex.IsMatch(e.Text);
        //}


    }
}
