using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.CustomerGUI
{
    /// <summary>
    /// Interaction logic for CustomerManager.xaml
    /// </summary>
    public partial class CustomerManager : Window
    {
        readonly List<CustomerCategories> list = new List<CustomerCategories>(Enum.GetValues(typeof(CustomerCategories)).Cast<CustomerCategories>());

        private Customer customer;
        private bool changeExistingCustomer;

        public CustomerManager()
        {
            InitializeComponent();

            cbCustomerCategory.ItemsSource = list;
        }

        public CustomerManager(Customer customer)
        {
            InitializeComponent();

            changeExistingCustomer = true;

            DataContext = customer;

            this.customer = customer;

            cbCustomerCategory.ItemsSource = list;

            cbCustomerCategory.SelectedIndex = customer.CustomerCategory == CustomerCategories.Näringsliv.ToString() ? 0 : 1;           
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingCustomer)
            {
                CustomerManagement.Instance.CreateCustomer(tbCustomerId.Text, tbCustomerName.Text,
                    list[cbCustomerCategory.SelectedIndex]);
            }
            else
            {
                CustomerManagement.Instance.UpdateCustomer();
            }          
        }
    }
}
