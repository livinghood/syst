using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Logic_Layer;


namespace BUPSystem.CustomerGUI
{
    /// <summary>
    /// Interaction logic for CustomerManager.xaml
    /// </summary>
    public partial class CustomerManager : Window
    {
        // Member account class
        // Lista på kategorier
        readonly List<CustomerCategories> categorylist = new List<CustomerCategories>(Enum.GetValues(typeof(CustomerCategories)).Cast<CustomerCategories>());

        /// <summary>
        /// Property for returning the object
        /// </summary>
        public Customer Customer { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomerManager()
        {
            InitializeComponent();
            cbCustomerCategory.ItemsSource = categorylist;
            Logic_Layer.Customer customer = new Logic_Layer.Customer();
            DataContext = customer;
            this.Customer = customer;
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
            cbCustomerCategory.ItemsSource = categorylist;
            // Set combobox selected value (ugh, ugly enum parse from string)
            cbCustomerCategory.SelectedItem = (CustomerCategories)Enum.Parse(typeof(CustomerCategories), customer.CustomerCategory, true);
            // Set the member customer object
            this.Customer = customer;
            // Disable the textbox for customer id as its a primary key
            tbCustomerId.IsEnabled = false;
            // Disable validation for customer id
            Binding binding = BindingOperations.GetBinding(tbCustomerId, TextBox.TextProperty);
            binding.ValidationRules.Clear();
        }

        /// <summary>
        /// Button action for 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            tbCustomerId.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (Validation.GetHasError(tbCustomerId) == true)
                return;
            DialogResult = true;
            Close(); 
        }
    }
}
