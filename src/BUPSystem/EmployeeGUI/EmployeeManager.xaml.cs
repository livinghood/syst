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

            FillFieldsUpdate(employee);

        }

        private void FillFieldsUpdate(Employee employee)
        {
            tbEmployeeID.Text = employee.EmployeeID.ToString();
            tbEmployeeName.Text = employee.EmployeeName;
            tbEmployeeSallary.Text = employee.MonthSallary.ToString();
            tbEmployeeRate.Text = employee.EmployeementRate.ToString();
            tbEmployeeVacancy.Text = employee.VacancyDeduction.ToString();
            CalculateAnnualEmployee();
            foreach (EmployeePlacement emp in employee.EmployeePlacement)
            {
                if (emp.DepartmentID.Equals("AO"))
                {
                    tbAdmAvd.Text = emp.EmployeeAllocate.ToString();
                }
                if (emp.DepartmentID.Equals("DA"))
                {
                    tbDriftAvd.Text = emp.EmployeeAllocate.ToString();
                }
                if (emp.DepartmentID.Equals("FO"))
                {
                    tbSellAvd.Text = emp.EmployeeAllocate.ToString();
                }
                if (emp.DepartmentID.Equals("UF"))
                {
                    tbProdAvd.Text = emp.EmployeeAllocate.ToString();
                }
            }

            UpdateDiff();
            //Employee newEmployee = new Employee(tbEmployeeID.Text, );
            
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
                bool success = long.TryParse(tbEmployeeID.Text, out l_employeeId);
                if (!success) return; //if tryparse fails, abort
                
                bool createNewPlacement = false; //To be able to use same operation as a control and to create new DepartmentPlacement
                
                if (ControlDepartments(l_employeeId, createNewPlacement)) //createNewPlacement = false, only checks if at least 1 textbox got data
                {//if at least 1 textbox got data
                    createNewPlacement = true; //createNewPlacement = true

                    int i_sallery;
                    success = int.TryParse(tbEmployeeSallary.Text, out i_sallery);
                    i_sallery = FalseReturnZero(success, i_sallery);//if tryparse fails, int = 0

                    int i_employeeRate;
                    success = int.TryParse(tbEmployeeRate.Text, out i_employeeRate);
                    i_employeeRate = FalseReturnZero(success, i_employeeRate);//if tryparse fails, int = 0

                    decimal d_vacancy;
                    int i_vacancy;
                    success = int.TryParse(tbEmployeeVacancy.Text, out i_vacancy);
                    i_vacancy = FalseReturnZero(success, i_vacancy);//if tryparse fails, int = 0
                    d_vacancy = i_vacancy / 100;


                    EmployeeManagement.Instance.CreateEmployee(l_employeeId, tbEmployeeName.Text, i_sallery, i_employeeRate, d_vacancy);
                    ControlDepartments(l_employeeId, createNewPlacement); //Creates a new placement
                }
            }
            else
            {
                EmployeeManagement.Instance.UpdateEmployee();
            }

            this.Close();
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
            int i_rate;
            bool success = int.TryParse(tbEmployeeRate.Text, out i_rate);
            i_rate = FalseReturnZero(success, i_rate);//if tryparse fails, int = 0

            int i_vacancy;
            success = int.TryParse(tbEmployeeVacancy.Text, out i_vacancy);
            i_vacancy = FalseReturnZero(success, i_vacancy);//if tryparse fails, int = 0

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
            int i_adm;
            bool success = int.TryParse(tbAdmAvd.Text, out i_adm);
            i_adm = FalseReturnZero(success, i_adm);//if tryparse fails, int = 0

            int i_drift;
            success = int.TryParse(tbDriftAvd.Text, out i_drift);
            i_drift = FalseReturnZero(success, i_drift);//if tryparse fails, int = 0

            int i_sell;
            success = int.TryParse(tbSellAvd.Text, out i_sell);
            i_sell = FalseReturnZero(success, i_sell);//if tryparse fails, int = 0

            int i_prod;
            success = int.TryParse(tbProdAvd.Text, out i_prod);
            i_prod = FalseReturnZero(success, i_prod);//if tryparse fails, int = 0

            int i_annual;
            success = int.TryParse(tbAnnualEmployee.Text, out i_annual);//will always be a number so wont need tryparse
            i_annual = FalseReturnZero(success, i_annual);

            tbDiff.Text = (i_annual - i_adm - i_drift - i_sell - i_prod).ToString();//calculate Diff
        }

        /// <summary>
        /// Made for returning 0 if tryparse fails
        /// </summary>
        /// <param name="success">tryparse success</param>
        /// <param name="number">the number to tryparse</param>
        /// <returns>if = true, returns the parameter number. if = false, returns 0</returns>
        private int FalseReturnZero(bool success, int number)
        {
            if (success)
            {
                return number;
            }
            else
            {
                number = 0;
                return number;
            } 
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
