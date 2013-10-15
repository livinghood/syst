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
            get
            {
                return new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployee());
            }
        }

        public EmployeeRegister()
        {
            InitializeComponent();

            // The datacontext must be set
            DataContext = this;

            // Kod som läser in användares behörighet
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManager employeeManager = new EmployeeManager();
            employeeManager.ShowDialog();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManagement.Instance.DeleteEmployeePlacement(EmployeeList[lvEmployeeList.SelectedIndex]);
            EmployeeManagement.Instance.DeleteEmployee(EmployeeList[lvEmployeeList.SelectedIndex]);
           
            //ICollectionView view = CollectionViewSource.GetDefaultView(EmployeeList);
            //view.Refresh();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvEmployeeList.SelectedItem != null)
            {
                // Initilize a new window for editing an employee
                EmployeeManager em = new EmployeeManager(EmployeeList[lvEmployeeList.SelectedIndex]);
                em.ShowDialog();

                if (em.DialogResult.Equals(true))
                {
                    EmployeeManagement.Instance.UpdateEmployee();
                    lblInfo.Content = "Användaren uppdaterades";
                }
            }
            else
                MessageBox.Show("Markera en anställd att redigera först", "Ingen vald annvändare");
        }
    }
}
