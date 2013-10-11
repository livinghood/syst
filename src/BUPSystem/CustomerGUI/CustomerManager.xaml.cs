using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;

namespace BUPSystem.CustomerGUI
{
    /// <summary>
    /// Interaction logic for CustomerManager.xaml
    /// </summary>
    public partial class CustomerManager : Window
    {
        // Member account class
        private Logic_Layer.Customer m_customer;
        // Lista på kategorier
        readonly List<CustomerCategories> Categorylist = new List<CustomerCategories>(Enum.GetValues(typeof(CustomerCategories)).Cast<CustomerCategories>());

        /// <summary>
        /// Property for returning the object
        /// </summary>
        public Logic_Layer.Customer Customer
        {
            get
            {
                return m_customer;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomerManager()
        {
            InitializeComponent();

            cbCustomerCategory.ItemsSource = Categorylist;

            Logic_Layer.Customer customer = new Logic_Layer.Customer();

            DataContext = customer;

            this.m_customer = customer;
        }

        /// <summary>
        /// Constructor for editing existing customer object
        /// </summary>
        /// <param name="customer"></param>
        public CustomerManager(Customer customer)
        {
            InitializeComponent();
            // Bind the datacontext to the passed on customer (all the fields will be bound to it)
            DataContext = customer;
            // Set combobox itemsource
            cbCustomerCategory.ItemsSource = Categorylist;
            // Set combobox selected value (ugh, ugly enum parse from string)
            cbCustomerCategory.SelectedItem = (CustomerCategories)Enum.Parse(typeof(CustomerCategories), customer.CustomerCategory, true);
            // Set the member customer object
            this.m_customer = customer;
            // Disable the textbox for customer id as its a primary key
            tbCustomerId.IsEnabled = false;
        }

        /// <summary>
        /// Button action for 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update the source object with the new values
            tbCustomerId.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cbCustomerCategory.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateSource();
            tbCustomerName.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            this.DialogResult = true;
            this.Close(); 
        }
    }
}
