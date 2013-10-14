using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;

namespace BUPSystem.UserGUI
{
    /// <summary>
    /// Interaction logic for UserManager.xaml
    /// </summary>
    public partial class UserManager : Window
    {
        public UserAccount UserAccount { get; set; }

        public ObservableCollection<UserPermissionLevels> PermissionLevels
        {
            get { return UserManagement.Instance.GetPermissionLevels(); }
        }        

        /// <summary>
        /// Standard constructor
        /// </summary>
        public UserManager()
        {
            InitializeComponent();

            DataContext = this;
        }

        /// <summary>
        /// Constructor called when editing an existing user account
        /// </summary>
        /// <param name="userAccount"></param>
        public UserManager(UserAccount userAccount)
        {
            InitializeComponent();
            
            // Set the combobox items source to permission levels list
            cbPermissionLevel.ItemsSource = PermissionLevels;

            // Can't be edited since it's a primary key in the database
            tbUsername.IsEnabled = false;

            DataContext = userAccount;

            UserAccount = userAccount;
        }

        /// <summary>
        /// Create or update an user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // A new user account will be created
            if (UserAccount == null)
            {
                UserManagement.Instance.CreateUserAccount(tbUsername.Text, tbPassword.Text,
                    cbPermissionLevel.SelectedIndex);
            }

            // An existing user account will be updated
            else
            {
                UserManagement.Instance.UpdateUserAccount();
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Opens Employee register for selection of an employee to connect to the user account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetEmployee_Click(object sender, RoutedEventArgs e)
        {
            EmployeeGUI.EmployeeRegister er = new EmployeeGUI.EmployeeRegister();
            er.ShowDialog();

            // TODO : Hämta personal att koppla till användare
        }    
    }
}
