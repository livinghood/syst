using System.Windows;
using System;
using Logic_Layer;
namespace BUPSystem.AccountGUI
{
    /// <summary>
    /// Interaction logic for AccountManager.xaml
    /// </summary>
    public partial class AccountManager : Window
    {
        private Logic_Layer.Account account;

        private bool changeExistingAccount;

        public AccountManager()
        {
            InitializeComponent();
        }

        public AccountManager(Logic_Layer.Account account)
        {
            InitializeComponent();

            changeExistingAccount = true;

            DataContext = account;

            this.account = account;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExistingAccount)
            {
                AccountManagement.Instance.CreateAccount(Convert.ToInt32(tbNumber.Text), tbName.Text, Convert.ToInt32(tbAmount.Text));
            }
            else
            {
                AccountManagement.Instance.UpdateAccount();
            } 
        }

    }
}
