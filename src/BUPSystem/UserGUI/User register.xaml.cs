using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Logic_Layer;
using System.Collections.ObjectModel;

namespace BUPSystem.UserGUI
{
    /// <summary>
    /// Interaction logic for UserRegister.xaml
    /// </summary>
    public partial class UserRegister : Window
    {
        private bool userNameSorted;

        // Property for retriving and setting the list of accounts
        public ObservableCollection<UserAccount> UserAccountList
        {
            get { return UserManagement.Instance.UserAccounts; }
            set { UserManagement.Instance.UserAccounts = value; }
        }

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
                lblInfo.Content = "Ny användare skapad";
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
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här användaren?",
                    "Ta bort användare", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    UserManagement.Instance.DeleteUserAccount(UserAccountList[lvUserList.SelectedIndex]);
                    lblInfo.Content = "Användaren togs bort";
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
            if (lvUserList.SelectedItem != null)
            {
                // Initilize a new window for editing an account
                UserManager um = new UserManager(UserAccountList[lvUserList.SelectedIndex]);
                um.ShowDialog();

                if (um.DialogResult.Equals(true))
                {
                    UserManagement.Instance.UpdateUserAccount();
                    lblInfo.Content = "Användaren uppdaterades";
                }
            }
            else
                MessageBox.Show("Markera en användare att redigera först", "Ingen vald användare");
        }

        /// <summary>
        /// Search for an user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            int index = Search.Instance.UserRegisterSearch(tbSearch.Text.ToLower(), UserAccountList);

            if (index >= 0)
            {
                lvUserList.SelectedIndex = index;
                lvUserList.SelectedItem = lvUserList.SelectedIndex;
                lvUserList.ScrollIntoView(lvUserList.SelectedItem);
            }
            else
                MessageBox.Show("Ingen match på sökord", "Finns inte");
        }

        /// <summary>
        /// Sorts user name column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvchUserName_Click(object sender, RoutedEventArgs e)
        {
            //UserAccountList = isUserNameSorted 
            //    ? new ObservableCollection<UserAccount>(UserAccountList.OrderBy(u => u.UserName))
            //    : new ObservableCollection<UserAccount>(UserAccountList.OrderByDescending(u => u.UserName));        
            //isUserNameSorted = !isUserNameSorted;
            //lvUserList.ItemsSource = UserAccountList;

            UserAccountList = new ObservableCollection<UserAccount>(UserAccountList.AsQueryable().SortBy("UserName", userNameSorted));
            userNameSorted = !userNameSorted;
            lvUserList.ItemsSource = UserAccountList;
        }
    }
}
