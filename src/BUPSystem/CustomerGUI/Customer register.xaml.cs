using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Logic_Layer;
using Logic_Layer.General_Logic;

namespace BUPSystem.CustomerGUI
{
    /// <summary>
    /// Interaction logic for CustomerRegister.xaml
    /// </summary>
    public partial class CustomerRegister : Window
    {

        public ObservableCollection<Customer> CustomerList
        {
            get { return CustomerManagement.Instance.Customers; }
            set { CustomerManagement.Instance.Customers = value; }
        }

        // Containing the selected customer
        public Customer SelectedCustomer { get; set; }

        public CustomerRegister(bool SelectingCustomer = false)
        {
            InitializeComponent();

            // The datacontext must be set
            DataContext = this;

            if (!SelectingCustomer)
                btnSelect.Visibility = Visibility.Collapsed;
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvCustomerList.SelectedItem != null)
            {
                // Confirm that the user wishes to delete 
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här kunden?", "Ta bort kund", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    if (CustomerManagement.Instance.IsConnectedToFinancialIncome(SelectedCustomer))
                    {
                        MessageBox.Show("Kunden är kopplad till en intäktsbudgetering, går ej ta bort", "Kund är kopplad");
                        return;
                    }

                    // Delete the customer from the database
                    CustomerManagement.Instance.DeleteCustomer(SelectedCustomer);
                }
            }
            else
                MessageBox.Show("Markera en kund att ta bort", "Ingen vald kund");       
        }

        /// <summary>
        /// Button used to create a new customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for adding a new customer
            CustomerManager customerManager = new CustomerManager();

            // Show the window
            customerManager.ShowDialog();

            // If the users presses OK, add the new customer
            if (customerManager.DialogResult.Equals(true))
            {
                // Add the customer to the database
                CustomerManagement.Instance.AddCustomer(customerManager.Customer);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvCustomerList.SelectedItem == null)
            {
                MessageBox.Show("Markera en kund att välja", "Ingen vald kund");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvCustomerList.ItemsSource);

            // figure out what is the new direction
            ListSortDirection direction = ListSortDirection.Ascending;

            // if already sorted by this column, reverse the direction
            if (view.SortDescriptions.Count > 0 && view.SortDescriptions[0].PropertyName == propertyName)
            {
                direction = view.SortDescriptions[0].Direction == ListSortDirection.Ascending 
                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvCustomerList.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem;  
        }

        public bool FilterCustomerItem(object obj)
        {
            Customer item = obj as Customer;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true; // the filter is empty - pass all items

            // apply the filter
            return item.CustomerName.ToLower().Contains(textFilter.ToLower()) || item.CustomerID.ToLower().Contains(textFilter.ToLower());
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvCustomerList.SelectedItem == null)
            {
                MessageBox.Show("Markera en kund att ändra", "Ingen vald kund");
                return;
            }

            // Initilize a new window for editing a customer
            CustomerGUI.CustomerManager customerManager = new CustomerGUI.CustomerManager(SelectedCustomer);

            // Show the window
            customerManager.ShowDialog();

            // If the users presses OK, update the item
            if (customerManager.DialogResult.Equals(true))
            {
                // Update the database context
                CustomerManagement.Instance.UpdateCustomer();
            }
            else
            {
                // The user pressed cancel, revert changes
                CustomerManagement.Instance.ResetCustomer(CustomerList[lvCustomerList.SelectedIndex]);
            }

        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvCustomerList.ItemsSource);

            view.Filter = null;
        }
    }
}
