using System.Windows;
using System.Windows.Controls;
using System;
using System.Text.RegularExpressions;
using Logic_Layer;
namespace BUPSystem.AccountGUI
{
    /// <summary>
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : Window
    {
        // Member account class
        private Logic_Layer.Account m_account;

        /// <summary>
        /// Property for returning the object
        /// </summary>
        public Logic_Layer.Account Account
        {
            get 
            { 
                return m_account;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AccountManager()
        {
            InitializeComponent();

            Logic_Layer.Account account = new Logic_Layer.Account ();

            DataContext = account;

            this.m_account = account;
        }

        /// <summary>
        /// Contructor for editing an existing object
        /// </summary>
        /// <param name="account"></param>
        public AccountManager(Logic_Layer.Account account)
        {
            InitializeComponent();

            DataContext = account;

            this.m_account = account;

            // We cant edit the primary key, disable it
            tbNumber.IsEnabled = false;
        }

        /// <summary>
        /// Button action for saving object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Update the source object with the new values
            tbAmount.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbNumber.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            this.DialogResult = true;
            this.Close(); 
        }

    }
}
