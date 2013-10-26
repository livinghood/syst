using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Logic_Layer;
using Logic_Layer.General_Logic;

namespace BUPSystem.AccountGUI
{
    /// <summary>
    /// Interaction logic for AccountRegister.xaml
    /// </summary>
    public partial class AccountRegister : Window
    {
        /// <summary>
        /// Property for retriving the list of accounts
        /// </summary>
        public ObservableCollection<Logic_Layer.Account> Accounts
        {
            get
            {
                return AccountManagement.Instance.Accounts;
            }
        }

        // Containing the selected customer
        public Account SelectedAccount { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountRegister(bool SelectingAccount = false)
        {
            InitializeComponent();
            // Datacontext
            DataContext = this;

            if (!SelectingAccount)
                btnSelect.Visibility = Visibility.Collapsed;
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
            }
        }

        /// <summary>
        /// Button action for removing an account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvAccounts.SelectedItem != null)
            {
                // Confirm that the user wishes to delete 
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort det här kontot?", "Ta bort konto", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    // Delete the account from the database
                    AccountManagement.Instance.DeleteAccount(SelectedAccount);
                }
            }
            else
                MessageBox.Show("Markera ett konto att ta bort", "Inget valt konto");   
           
        }

        /// <summary>
        /// Button action for edting an existing account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Change_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvAccounts.SelectedItem == null)
            {
                MessageBox.Show("Markera ett konto att ändra", "Inget valt konto");
                return;
            }

            // Initilize a new window for editing a customer
            AccountGUI.AccountManager accountManager = new AccountGUI.AccountManager(SelectedAccount);

            // Show the window
            accountManager.ShowDialog();

            // If the users presses OK, update the item
            if (accountManager.DialogResult.Equals(true))
            {
                // Update the database context
                AccountManagement.Instance.UpdateAccount();
            }
            else
            {
                // The user pressed cancel, revert changes
                AccountManagement.Instance.ResetAccount(SelectedAccount);
            }
        }

        //Sorting function
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvAccounts.ItemsSource);

            // figure out what is the new direction
            ListSortDirection direction = ListSortDirection.Ascending;

            // if already sorted by this column, reverse the direction
            if (view.SortDescriptions.Count > 0 && view.SortDescriptions[0].PropertyName == propertyName)
            {
                direction = view.SortDescriptions[0].Direction == ListSortDirection.Ascending 
                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvAccounts.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem;
        }

        public bool FilterCustomerItem(object obj)
        {
            Account item = obj as Account;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true;

            // apply the filter
            return item.AccountName.ToLower().Contains(textFilter.ToLower()) 
                || item.AccountID.ToString(CultureInfo.InvariantCulture).ToLower().Contains(textFilter.ToLower());
        }

    }
}
