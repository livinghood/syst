using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.CustomerGUI
{
    /// <summary>
    /// Interaction logic for CustomerRegister.xaml
    /// </summary>
    public partial class CustomerRegister : Window
    {
        public ObservableCollection<Customer> CustomerList
        {
            get
            {
                return new ObservableCollection<Customer>(CustomerManagement.Instance.GetCustomers());
            }
        }

        public CustomerRegister()
        {
            InitializeComponent();

            // The datacontext must be set
            DataContext = this;

            // Kod som läser in användares behörighet
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

            CustomerManagement.Instance.DeleteCustomer(CustomerList[lvCustomerList.SelectedIndex]);
        }

        /// <summary>
        /// Button used to create a new customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CustomerManager customerManager = new CustomerManager();
            customerManager.ShowDialog();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            CustomerManager customerManager = new CustomerManager(CustomerList[lvCustomerList.SelectedIndex]);
            customerManager.ShowDialog();

            // Att göra: lägga till label som bekräftar om kund har lagts till/ ändrats eller tagit bort
        }
    }
}
