using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Logic_Layer.CustomerNamespace;

namespace BUPSystem.Customer
{
    /// <summary>
    /// Interaction logic for CustomerManagement.xaml
    /// </summary>
    public partial class CustomerManagement : Window
    {
        readonly List<CustomerCategorys> list = new List<CustomerCategorys>(Enum.GetValues(typeof(CustomerCategorys)).Cast<CustomerCategorys>());

        public CustomerManagement()
        {
            InitializeComponent();

            cbCustomerCategory.ItemsSource = list;
        }

        public CustomerManagement(Logic_Layer.CustomerNamespace.Customer customer)
        {
            InitializeComponent();

            DataContext = customer;

            cbCustomerCategory.ItemsSource = list;

            cbCustomerCategory.SelectedIndex = customer.CustomerCategory == CustomerCategorys.Näringsliv ? 0 : 1;           
        }
    }
}
