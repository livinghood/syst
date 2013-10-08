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
        readonly List<CustomerCategorys> list = new List<CustomerCategorys>(Enum.GetValues(typeof(CustomerCategorys)).Cast<CustomerCategorys>());

        public CustomerManager()
        {
            InitializeComponent();

            cbCustomerCategory.ItemsSource = list;
        }

        public CustomerManager(Logic_Layer.CustomerNamespace.Customer customer)
        {
            InitializeComponent();

            DataContext = customer;

            cbCustomerCategory.ItemsSource = list;

            cbCustomerCategory.SelectedIndex = customer.CustomerCategory == CustomerCategorys.Näringsliv ? 0 : 1;           
        }
    }
}
