using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.Customer
{
    /// <summary>
    /// Interaction logic for CustomerRegister.xaml
    /// </summary>
    public partial class CustomerRegister : Window
    {
        public ObservableCollection<Logic_Layer.CustomerNamespace.Customer> CustomerList
        {
            get
            {
                return LogicController.Instance.CustomerList;
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
            CustomerList.Add(new Logic_Layer.CustomerNamespace.Customer(6,
                Logic_Layer.CustomerNamespace.CustomerCategorys.Näringsliv, "hej"));
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
