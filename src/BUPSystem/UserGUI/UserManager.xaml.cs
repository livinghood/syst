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
            cbPermissionLevel.ItemsSource = PermissionLevels;
            Logic_Layer.UserAccount ua = new UserAccount();
            DataContext = ua;
            UserAccount = ua;
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
            tbPassword.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            cbPermissionLevel.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateSource();     
            DialogResult = true;
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
