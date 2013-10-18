using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using System.ComponentModel;
using System.Windows.Data;

namespace BUPSystem.EmployeeGUI
{
    /// <summary>
    /// Interaction logic for EmployeeRegister.xaml
    /// </summary>
    public partial class EmployeeRegister : Window
    {

        public ObservableCollection<Employee> EmployeeList
        {
            get { return EmployeeManagement.Instance.EmployeeList; }
            set { EmployeeManagement.Instance.EmployeeList = value; }
        }

        public EmployeeRegister()
        {
            InitializeComponent();

            // The datacontext must be set
            DataContext = this;
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManager employeeManager = new EmployeeManager();
            employeeManager.ShowDialog();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManagement.Instance.DeleteEmployeePlacements(EmployeeList[lvEmployeeList.SelectedIndex]);
            EmployeeManagement.Instance.DeleteEmployee(EmployeeList[lvEmployeeList.SelectedIndex]);
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = new Employee();
            // Make sure the sure the user has selected an item in the listview
            if (lvEmployeeList.SelectedItem != null)
            {
                // Initilize a new window for editing an employee
                EmployeeManager em = new EmployeeManager(EmployeeList[lvEmployeeList.SelectedIndex]);
                em.ShowDialog();

                if (em.DialogResult.Equals(true))
                {
                    EmployeeManagement.Instance.UpdateEmployee();
                    lblInfo.Content = "anställd uppdaterades";
                }
            }
            else
                MessageBox.Show("Markera en anställd att redigera först", "Ingen vald anställd");
        }
    }
}
