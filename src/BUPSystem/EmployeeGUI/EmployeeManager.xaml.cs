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

        public Employee Employee {get; set;}

        private bool changeExistingEmployee;

        public EmployeeManager()
        {
            InitializeComponent();
            Employee em = new Employee();
            DataContext = em;
            Employee = em;
        }

        public EmployeeManager(Employee employee)
        {
            InitializeComponent();

            changeExistingEmployee = true;

            DataContext = employee;

            this.Employee = employee;

            tbEmployeeID.IsEnabled = false;
            
            DataContext = employee;

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

        /// <summary>
        /// When Save button is pressed, if no changing, create new Employee and EmployeePlacement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingEmployee)
            {//Create new employee
                long l_employeeId;
                bool success = long.TryParse(tbEmployeeID.Text, out l_employeeId);
                if (!success) return; //if tryparse fails, abort
                
                bool createNewPlacement = false; //To be able to use same operation as a control and to create new DepartmentPlacement
                
                if (ControlDepartments(l_employeeId, createNewPlacement)) //createNewPlacement = false, only checks if at least 1 textbox got data
                {//if at least 1 textbox got data
                    createNewPlacement = true; //createNewPlacement = true

                    int i_sallery = ConvertStringToInt(tbEmployeeSallary.Text);

                    int i_employeeRate = ConvertStringToInt(tbEmployeeVacancy.Text);

                    decimal d_vacancy;
                    int i_vacancy = ConvertStringToInt(tbEmployeeVacancy.Text);
                    d_vacancy = i_vacancy / 100;    //to get %


                    EmployeeManagement.Instance.CreateEmployee(l_employeeId, tbEmployeeName.Text, i_sallery, i_employeeRate, d_vacancy);
                    ControlDepartments(l_employeeId, createNewPlacement); //Creates a new placement
                }
            }
            else
            {// Update employee
                tbEmployeeID.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEmployeeName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEmployeeSallary.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbEmployeeRate.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                int i_vacancy = ConvertStringToInt(tbEmployeeVacancy.Text);

                Employee.VacancyDeduction = i_vacancy / 100;

                tbAdmAvd.GetBindingExpression(TextBox.TemplateProperty).UpdateSource();
                tbDriftAvd.GetBindingExpression(TextBox.TemplateProperty).UpdateSource();
                tbSellAvd.GetBindingExpression(TextBox.TemplateProperty).UpdateSource();
                tbProdAvd.GetBindingExpression(TextBox.TemplateProperty).UpdateSource();

                DialogResult = true;
            }
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
        /// <param name="employeeId"></param>
        /// <returns>false if no field got data, else true</returns>
        private bool ControlDepartments(long employeeId, bool createNew)
        {
            decimal allocate;
            bool ok = false;
            if (tbAdmAvd.Text.Trim().Length > 0 && tbAdmAvd.Text != "0")
            {
                if (createNew)
                {
                    allocate = decimal.Parse(tbAdmAvd.Text);
                    EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "AO", allocate);
                    //Create new placement with employeeid, departmentid and allocated time
                }
                ok = true;
            }
            if (tbDriftAvd.Text.Trim().Length > 0 && tbDriftAvd.Text != "0")
            {
                if (createNew)
                {
                    allocate = decimal.Parse(tbDriftAvd.Text);
                    EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "DA", allocate);
                    //Create new placement with employeeid, departmentid and allocated time
                }
                ok = true;
            }
            if (tbSellAvd.Text.Trim().Length > 0 && tbSellAvd.Text != "0")
            {
                if (createNew)
                {
                    allocate = decimal.Parse(tbSellAvd.Text);
                    EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "FO", allocate);
                    //Create new placement with employeeid, departmentid and allocated time
                }
                ok = true;
            }
            if (tbProdAvd.Text.Trim().Length > 0 && tbProdAvd.Text != "0")
            {
                if (createNew)
                {
                    allocate = decimal.Parse(tbProdAvd.Text);
                    EmployeeManagement.Instance.CreateEmployeePlacement(employeeId, "UF", allocate);
                    //Create new placement with employeeid, departmentid and allocated time
                }
                ok = true;
            }
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

            tbAnnualEmployee.Text = (i_rate - i_vacancy).ToString();
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

            tbDiff.Text = (i_annual - i_adm - i_drift - i_sell - i_prod).ToString();//calculate Diff
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
