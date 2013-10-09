using System.Windows;
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
        private Logic_Layer.Account m_account;

        public Logic_Layer.Account Account
        {
            get 
            { 
                return m_account;
            }
        }

        public AccountManager()
        {
            InitializeComponent();

            Logic_Layer.Account account = new Logic_Layer.Account ();

            DataContext = account;

            this.m_account = account;
        }

        public AccountManager(Logic_Layer.Account account)
        {
            InitializeComponent();

            DataContext = account;

            this.m_account = account;

            tbNumber.IsEnabled = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close(); 
        }

    }
}
