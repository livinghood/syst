using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.Account
{
    /// <summary>
    /// Interaction logic for AccountRegister.xaml
    /// </summary>
    public partial class AccountRegister : Window
    {
        public ObservableCollection<Logic_Layer.Account> AccountList
        {
            get
            {
                return new ObservableCollection<Logic_Layer.Account>(AccountManagement.Instance.GetAccounts());
            }
        }

        public AccountRegister()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AccountGUI.AccountManager accountManager = new AccountGUI.AccountManager();

            accountManager.ShowDialog();
        }
    }
}
