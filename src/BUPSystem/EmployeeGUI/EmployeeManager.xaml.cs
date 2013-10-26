using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BUPSystem.EmployeeGUI
{
    /// <summary>
    /// Interaction logic for EmployeeManager.xaml
    /// </summary>
    public partial class EmployeeManager : Window
    {

        public Employee Employee {get; set;}

        private bool changeExistingEmployee;

        /// <summary>
        /// Nomral constructor
        /// </summary>
        public EmployeeManager()
        {
            InitializeComponent();
            Employee em = new Employee();
            DataContext = em;
            Employee = em;
        }

        /// <summary>
        /// Constructor for changing an employee
        /// </summary>
        /// <param name="employee">the employee to change</param>
        public EmployeeManager(Employee employee)
        {
            InitializeComponent();

            changeExistingEmployee = true;

            this.Employee = employee;

            tbEmployeeID.IsEnabled = false;

            DataContext = employee;

            tbEmployeeVacancy.Text = ((int)(employee.VacancyDeduction * 100)).ToString();

            foreach (EmployeePlacement emp in employee.EmployeePlacement)
            {
                if (emp.DepartmentID.Equals("AO"))
                {
                    grdAdm.DataContext = emp;
                }
                if (emp.DepartmentID.Equals("DA"))
                {
                    grdDrift.DataContext = emp;
                }
                if (emp.DepartmentID.Equals("FO"))
                {
                    grdSell.DataContext = emp;
                }
                if (emp.DepartmentID.Equals("UF"))
                {
                    grdProd.DataContext = emp;
                }
            }
        }

        private void UpdatePlacements()
        {
            ChangePlacement("AO", ConvertStringToInt(tbAdmAvd.Text) > 0 ? decimal.Parse(tbAdmAvd.Text) : 0);

            ChangePlacement("DA", ConvertStringToInt(tbDriftAvd.Text) > 0 ? decimal.Parse(tbDriftAvd.Text) : 0);

            ChangePlacement("FO", ConvertStringToInt(tbSellAvd.Text) > 0 ? decimal.Parse(tbSellAvd.Text) : 0);

            ChangePlacement("UF", ConvertStringToInt(tbProdAvd.Text) > 0 ? decimal.Parse(tbProdAvd.Text) : 0);
        }

        private void ChangePlacement(string departmentID, decimal allocation)
        {
            IEnumerable<EmployeePlacement> oldPlacements = EmployeeManagement.Instance.GetEmployeePlacements(Employee);
            EmployeePlacement ep = oldPlacements.SingleOrDefault(e => e.DepartmentID == departmentID);

            if (allocation == 0)
            {
                if (ep != null)
                    EmployeeManagement.Instance.DeleteEmployeePlacement(ep);
            }
            else
            {
                if (ep == null)
                {
                    EmployeeManagement.Instance.CreateEmployeePlacement(Employee.EmployeeID, departmentID, allocation);
                }
                else
                {
                    ep.EmployeeAllocate = allocation;
                }
            }
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
                long l_employeeId;
                // If employeeID is invalid, terminate
                if (!long.TryParse(tbEmployeeID.Text, out l_employeeId))
                    return;

                // If no department is set, terminate
                if (!ControlDepartments()) 
                    return;

                decimal d_vacancy = ConvertStringToInt(tbEmployeeVacancy.Text);
                d_vacancy = d_vacancy / 100;

                Employee = EmployeeManagement.Instance.CreateEmployee(l_employeeId, tbEmployeeName.Text, 
                    ConvertStringToInt(tbEmployeeSallary.Text), ConvertStringToInt(tbEmployeeRate.Text), d_vacancy);
                
            }
            else
            {// Update employee
                tbEmployeeID.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEmployeeName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEmployeeSallary.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEmployeeRate.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                decimal d_vacancy = ConvertStringToInt(tbEmployeeVacancy.Text);
                Employee.VacancyDeduction = d_vacancy / 100;

                DialogResult = true;
            }
            UpdatePlacements();
            this.Close();
        }

        /// <summary>
        /// Converts string to int with tryparse, if it fails it returns int = 0
        /// </summary>
        /// <param name="text">Textbox text</param>
        /// <returns>int converted from textbox.text</returns>
        private int ConvertStringToInt(string text)
        {
            int i;
            bool success = int.TryParse(text, out i);
            if (!success)
                i = 0;
            return i;
        }

        /// <summary>
        /// Control wich departments that got filled textboxes and create EmployeePlacement with right departments/departments
        /// </summary>
        /// <returns>false if no field got data, else true</returns>
        private bool ControlDepartments()
        {
            bool ok = tbAdmAvd.Text.Trim().Length > 0 && tbAdmAvd.Text != "0" 
                || tbDriftAvd.Text.Trim().Length > 0 && tbDriftAvd.Text != "0" 
                || tbSellAvd.Text.Trim().Length > 0 && tbSellAvd.Text != "0" 
                || tbProdAvd.Text.Trim().Length > 0 && tbProdAvd.Text != "0";

            if (!ok)
                MessageBox.Show("Måste placeras på minst en avdelning");

            return ok;
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
            int i_rate = ConvertStringToInt(tbEmployeeRate.Text);

            int i_vacancy = ConvertStringToInt(tbEmployeeVacancy.Text);

            tbAnnualEmployee.Text = (i_rate - i_vacancy).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Updates tbDiff when text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbAdmAvd_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDiff();
        }

        /// <summary>
        /// Updates tbDiff when text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDriftAvd_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDiff();
        }

        /// <summary>
        /// Updates tbDiff when text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSellAvd_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDiff();
        }

        /// <summary>
        /// Updates tbDiff when text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbProdAvd_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateDiff();
        }

        /// <summary>
        /// caclulates the textbox tbDiff
        /// </summary>
        private void UpdateDiff()
        {
            int i_adm = ConvertStringToInt(tbAdmAvd.Text);

            int i_drift = ConvertStringToInt(tbDriftAvd.Text);

            int i_sell = ConvertStringToInt(tbSellAvd.Text);

            int i_prod = ConvertStringToInt(tbProdAvd.Text);

            int i_annual = ConvertStringToInt(tbAnnualEmployee.Text);

            tbDiff.Text = (i_annual - i_adm - i_drift - i_sell - i_prod).ToString(CultureInfo.InvariantCulture);//calculate Diff
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
