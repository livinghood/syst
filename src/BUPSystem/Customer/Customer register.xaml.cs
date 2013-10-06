using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using Common;

namespace BUPSystem.Customer
{
    /// <summary>
    /// Interaction logic for CustomerRegister.xaml
    /// </summary>
    public partial class CustomerRegister : Window
    {
        public ObservableCollection<Common.Customer> CustomerList
        {
            get
            {
                return new ObservableCollection<Common.Customer>(CustomerManager.Instance.getCustomers());
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

        }

        /// <summary>
        /// Button used to create a new customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagement customerManagement = new CustomerManagement();
            customerManagement.ShowDialog();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagement customerManagement = new CustomerManagement();

            // Kod som lägger in vald kunds uppgifter i kundhanteringsfönstret

            customerManagement.ShowDialog();
        }
    }
}
