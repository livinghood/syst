﻿using System.Collections.ObjectModel;
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
            get { return CustomerManagement.Instance.Customers; }
            set { CustomerManagement.Instance.Customers = value; }
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
            // Delete the customer from the database
            CustomerManagement.Instance.DeleteCustomer(CustomerList[lvCustomerList.SelectedIndex]);
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

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for editing a customer
            CustomerGUI.CustomerManager customerManager = new CustomerGUI.CustomerManager(CustomerList[lvCustomerList.SelectedIndex]);

            // Show the window
            customerManager.ShowDialog();

            // If the users presses OK, update the item
            if (customerManager.DialogResult.Equals(true))
            {
                // Update the database context
                CustomerManagement.Instance.UpdateCustomer();
            }
        }
    }
}
