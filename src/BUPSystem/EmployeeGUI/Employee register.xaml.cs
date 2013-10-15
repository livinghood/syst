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
            EmployeeManager employeeManager = new EmployeeManager(EmployeeList[lvEmployeeList.SelectedIndex]);
            employeeManager.ShowDialog();

            // Att göra: lägga till label som bekräftar om anställd har lagts till/ ändrats eller tagit bort
        }
    }
}
