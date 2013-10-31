using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System;
using Logic_Layer;
using Logic_Layer.General_Logic;

namespace BUPSystem.UserGUI
{
    /// <summary>
    /// Interaction logic for UserRegister.xaml
    /// </summary>
    public partial class UserRegister : Window
    {

        // Property for retriving and setting the list of accounts
        public ObservableCollection<UserAccount> UserAccountList
        {
            get { return UserManagement.Instance.UserAccounts; }
            set { UserManagement.Instance.UserAccounts = value; }
        }

        // Containing the selected customer
        public UserAccount SelectedUserAccount{ get; set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public UserRegister()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Button for creating a new user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            UserManager um = new UserManager();
            um.ShowDialog();

            if (um.DialogResult == true)
            {
                UserManagement.Instance.AddAccount(um.UserAccount);
            }
        }

        /// <summary>
        /// Button for deleting an user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvUserList.SelectedItem != null)
            {
                // Confirm that the user wishes to delete 
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här användaren?", "Ta bort användare", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    // Delete the user from the database
                    UserManagement.Instance.DeleteUserAccount(SelectedUserAccount);
                }
            }
            else
                MessageBox.Show("Markera en användare att ta bort", "Ingen vald användare");  
        }

        /// <summary>
        /// Button for changing an existing user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvUserList.SelectedItem == null)
            {
                MessageBox.Show("Markera en användare att ändra", "Ingen vald användare");
                return;
            }

            // Initilize a new window for editing a user
            UserManager um = new UserManager(SelectedUserAccount);
            
            um.ShowDialog();

            // If the users presses OK, update the item
            if (um.DialogResult.Equals(true))
            {
                // Update the database context
                UserManagement.Instance.UpdateUserAccount();
            }
            else
            {
                // The user pressed cancel, revert changes
                UserManagement.Instance.ResetUser(SelectedUserAccount);
            }

        }

        public bool FilterCustomerItem(object obj)
        {
            UserAccount item = obj as UserAccount;
            if (item == null) return false;

            string textFilter = tbSearch.Text;

            if (textFilter.Trim().Length == 0) return true; // the filter is empty - pass all items

            // apply the filter
            return item.UserName.ToLower().Contains(textFilter.ToLower()) || item.EmployeeID.ToString().ToLower().Contains(textFilter.ToLower());
        }

        // Sort listview
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsInitialized) return;    // get out of here if the window is not initialized

            string propertyName = (sender as GridViewColumnHeader).Tag.ToString();

            // Get the default view from the listview
            ICollectionView view = CollectionViewSource.GetDefaultView(lvUserList.ItemsSource);

            // figure out what is the new direction
            ListSortDirection direction = ListSortDirection.Ascending;

            // if already sorted by this column, reverse the direction
            if (view.SortDescriptions.Count > 0 && view.SortDescriptions[0].PropertyName == propertyName)
            {
                direction = view.SortDescriptions[0].Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(lvUserList.ItemsSource);

            view.Filter = null;
            view.Filter = FilterCustomerItem; 
        }
    }
}
