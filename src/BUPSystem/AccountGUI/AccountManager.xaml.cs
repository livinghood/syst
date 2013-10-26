using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Logic_Layer;

namespace BUPSystem.AccountGUI
{
    /// <summary>
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : Window
    {
        /// <summary>
        /// Property for returning the object
        /// </summary>
        public Logic_Layer.Account Account { get; private set; }


        /// <summary>
        /// Default constructor
        /// </summary>
        public AccountManager()
        {
            InitializeComponent();

            Logic_Layer.Account account = new Logic_Layer.Account ();

            DataContext = account;

            this.Account = account;
        }

        /// <summary>
        /// Contructor for editing an existing object
        /// </summary>
        /// <param name="account"></param>
        public AccountManager(Logic_Layer.Account account)
        {
            InitializeComponent();

            DataContext = account;

            this.Account = account;

            // We cant edit the primary key, disable it
            tbNumber.IsEnabled = false;

            // Disable validation for account id
            Binding binding = BindingOperations.GetBinding(tbNumber, TextBox.TextProperty);
            binding.ValidationRules.Clear();
        }

        /// <summary>
        /// Button action for saving object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update the source object with the new values
            tbNumber.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            
            if (Validation.GetHasError(tbNumber))
                return;
            
            DialogResult = true;
            Close();  
        }

    }
}
