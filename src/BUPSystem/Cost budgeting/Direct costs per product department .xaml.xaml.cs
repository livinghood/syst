using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Logic_Layer;

namespace BUPSystem.Kostnadsbudgetering
{
    /// <summary>
    /// Interaction logic for DirectCostsPerProductDepartment.xaml
    /// </summary>
    public partial class DirectCostsPerProductDepartment : Window
    {
        public DirectProductCost dpc { get; set; }

        private Product product { get; set; }

        private Account account { get; set; }

        DatabaseConnection db = new DatabaseConnection();

        private ObservableCollection<DirectProductCost> list;

        public ObservableCollection<Logic_Layer.Account> Accounts
        {
            get
            {
                return AccountManagement.Instance.Accounts;
            }
        }

        public ObservableCollection<Logic_Layer.Product> Products
        {
            get
            {
                return ProductManagement.Instance.Products;
            }
        }

        public DirectCostsPerProductDepartment()
        {
            InitializeComponent();

            list = new ObservableCollection<DirectProductCost>(db.DirectProductCost.Local);
            DataContext = this;
        }


        private void winDKPPA_Loaded(object sender, RoutedEventArgs e)
        {
            dgAccounts.ItemsSource = Accounts;
            dgDPPC.ItemsSource = list;
        }

        private void btnSelectAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountGUI.AccountRegister ar = new AccountGUI.AccountRegister();
            ar.ShowDialog();

            if (ar.DialogResult == true)
            {
                account = ar.Account;
            }
        }

        private void btnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductGUI.ProductRegister pr = new ProductGUI.ProductRegister();
            pr.ShowDialog();

            if (pr.DialogResult == true)
            {
                product = pr.Product;

                dpc = new DirectProductCost
                {
                    Account = account,
                    AccountID = account.AccountID,
                    Product = product,
                    ProductID = product.ProductID
                };

                list.Add(dpc);
            }
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
                    
        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            list.Clear(); 
            account = Accounts[dgAccounts.SelectedIndex];

            var query = from u in db.DirectProductCost
                      where u.AccountID == account.AccountID
                      select u;

            foreach (var item in query)
            {
                list.Add(new DirectProductCost
                {
                    Account = account,
                    AccountID = account.AccountID,
                    Product = item.Product,
                    ProductID = item.ProductID,
                    ProductCost = item.ProductCost
                    
                });
            }

            dgDPPC.ItemsSource = list;



            //if (dpc == null)
            //{
            //    dpc = new DirectProductCost();
            //}
            //dpc.Account = Accounts[dgAccounts.SelectedIndex];
            //dpc.AccountID = Accounts[dgAccounts.SelectedIndex].AccountID;

            //if (!list.Contains(dpc))
            //{
            //    list.Add(dpc);
            //}
            //dgDPPC.Items.Refresh();
        }
    }
}
