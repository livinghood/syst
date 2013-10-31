using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
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
        }

        // Containing the selected Employee
        public Employee SelectedEmployee { get; set; }

        public EmployeeRegister(bool SelectingEmployee = false)
        {
            InitializeComponent();
            // The datacontext must be set
            DataContext = this;

            if (!SelectingEmployee)
                btnSelect.Visibility = Visibility.Collapsed;
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            EmployeeManager employeeManager = new EmployeeManager();
            employeeManager.ShowDialog();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvEmployeeList.SelectedItem != null)
            {
                // Confirm that the user wishes to delete 
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort?", "Ta bort anställd", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    if (EmployeeManagement.Instance.IsConnectedToActivityPlacement(SelectedEmployee))
                    {
                        MessageBox.Show("Anställd är kopplad till en aktivitet, går ej ta bort", "Anställd är kopplad");
                        return;
                    }

                    if (EmployeeManagement.Instance.IsConnectedToProductPlacement(SelectedEmployee))
                    {
                        MessageBox.Show("Anställd är kopplad till en produkt, går ej ta bort", "Anställd är kopplad");
                        return;
                    }

                    if (EmployeeManagement.Instance.IsConnectedToUserAccount(SelectedEmployee))
                    {
                        MessageBox.Show("Anställd är kopplad till ett användarkonto, går ej ta bort", "Anställd är kopplad");
                        return;
                    }

                    // Delete the Employee and placements from the database
                    EmployeeManagement.Instance.DeleteEmployeePlacements(SelectedEmployee);
                    EmployeeManagement.Instance.DeleteEmployee(SelectedEmployee);

                }
            }
            else
                MessageBox.Show("Markera en anställd att ta bort", "Ingen vald anställd");    
            
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvEmployeeList.SelectedItem == null)
            {
                MessageBox.Show("Markera en anställd att ändra", "Ingen anställd vald");
                return;
            }

            // Initilize a new window for editing an employee
            EmployeeManager em = new EmployeeManager(SelectedEmployee);

            // Show the window
            em.ShowDialog();

            // If the users presses OK, update the item
            if (em.DialogResult.Equals(true))
            {
                // Update the database context
                EmployeeManagement.Instance.UpdateEmployee();
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvEmployeeList.SelectedItem == null)
            {
                MessageBox.Show("Markera en anställd att välja", "Ingen anställd vald");
                return;
            }

            DialogResult = true;
            Close();
        }

        public bool FilterCustomerItem(object obj)
        {
            Employee item = obj as Employee;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true; 

            // apply the filter
            return item.EmployeeID.ToString(CultureInfo.InvariantCulture).ToLower().Contains(textFilter.ToLower()) || item.EmployeeName.ToLower().Contains(textFilter.ToLower());
        }

        // Sort listview
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvEmployeeList.ItemsSource);

            // figure out what is the new direction
            ListSortDirection direction = ListSortDirection.Ascending;

            // if already sorted by this column, reverse the direction
            if (view.SortDescriptions.Count > 0 && view.SortDescriptions[0].PropertyName == propertyName)
            {
                direction = view.SortDescriptions[0].Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvEmployeeList.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem; 
        }
    }
}
