﻿using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.Account
{
    /// <summary>
    /// Interaction logic for AccountRegister.xaml
    /// </summary>
    public partial class AccountRegister : Window
    {
        // Member list with all the accounts
        ObservableCollection<Logic_Layer.Account> m_AccountList = new ObservableCollection<Logic_Layer.Account>(AccountManagement.Instance.GetAccounts());

        /// <summary>
        /// Property for retriving the list of accounts
        /// </summary>
        public ObservableCollection<Logic_Layer.Account> AccountList
        {
            get
            {
                return m_AccountList;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountRegister()
        {
            InitializeComponent();
            // Datacontext
            DataContext = this;
        }

        /// <summary>
        /// Button action for adding a new account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for adding a new account
            AccountGUI.AccountManager accountManager = new AccountGUI.AccountManager();

            // Show the window
            accountManager.ShowDialog();

            // If the users presses OK, add the new user
            if (accountManager.DialogResult.Equals(true))
            {
                // Add the user to the database
                AccountManagement.Instance.CreateAccount(accountManager.Account);
                // Add the user to our account list
                AccountList.Add(accountManager.Account);
            }
            
            

        }

        /// <summary>
        /// Button action for removing an account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Delete the account from the database  
            AccountManagement.Instance.DeleteAccount(AccountList[lvAccounts.SelectedIndex]);
            // Delete the account from our account list
            m_AccountList.Remove(AccountList[lvAccounts.SelectedIndex]);
        }

        /// <summary>
        /// Button action for edting an existing account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Change_Click(object sender, RoutedEventArgs e)
        {
            // Initilize a new window for editing an account
            AccountGUI.AccountManager accountManager = new AccountGUI.AccountManager(AccountList[lvAccounts.SelectedIndex]);
            
            // Show the window
            accountManager.ShowDialog();
            
            // If the users presses OK, update the item
            if (accountManager.DialogResult.Equals(true))
            {
                // Update the database context
                AccountManagement.Instance.UpdateAccount();
            }
        }
    }
}
