using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer.CustomerNamespace;

namespace BUPSystem.CustomerGUI
{
    /// <summary>
    /// Interaction logic for CustomerManager.xaml
    /// </summary>
    public partial class CustomerManager : Window
    {
        readonly List<CustomerCategories> list = new List<CustomerCategories>(Enum.GetValues(typeof(CustomerCategories)).Cast<CustomerCategories>());

        public CustomerManager()
        {
            InitializeComponent();

            cbCustomerCategory.ItemsSource = list;
        }

        public CustomerManager(Customer customer)
        {
            InitializeComponent();

            DataContext = customer;

            cbCustomerCategory.ItemsSource = list;

            cbCustomerCategory.SelectedIndex = customer.CustomerCategory == CustomerCategories.Näringsliv ? 0 : 1;           
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CustomerManagement.Instance.CreateCustomer(tbCustomerId.Text, tbCustomerName.Text,
                list[cbCustomerCategory.SelectedIndex]);
        }
    }
}
