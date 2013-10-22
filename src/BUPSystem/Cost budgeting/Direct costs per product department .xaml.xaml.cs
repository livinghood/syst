using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Logic_Layer;
using Logic_Layer.Cost_Budgeting_Logic;

namespace BUPSystem.Kostnadsbudgetering
{

    /// <summary>
    /// Interaction logic for DirectCostsPerProductDepartment.xaml
    /// </summary>
    public partial class DirectCostsPerProductDepartment : Window
    {
        private DirectProductCost objToAdd;

        public DirectProductCost dpc { get; set; }

        private Product product { get; set; }

        private Account account { get; set; }

        private DataTable dt;

        DatabaseConnection db = new DatabaseConnection();

        private readonly ObservableCollection<DirectProductCost> DirectProductCosts;

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

            DirectProductCosts = new ObservableCollection<DirectProductCost>(db.DirectProductCost.Local);
            DataContext = this;

            dt = new DataTable();
            dt = dgDPPC.ItemsSource as DataTable;

        }


        private void winDKPPA_Loaded(object sender, RoutedEventArgs e)
        {
            dgAccounts.ItemsSource = Accounts;
            dgDPPC.ItemsSource = DirectProductCosts;
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

                bool productExists = false;

                // Check if user attempts to add a product that is already connected to the selected account
                foreach (var directProductCost in DirectProductCosts.Where
                    (directProductCost => directProductCost.ProductID.Equals(product.ProductID)))
                {
                    productExists = true;
                }

                if (productExists)
                {
                    MessageBox.Show(String.Format
                        ("Du försöker lägga till en produkt som redan är kopplad till konto {0}.",
                        account.AccountName), "Produkt redan kopplad");
                    return;
                }

                dpc = new DirectProductCost
                {
                    AccountID = account.AccountID,
                    ProductID = product.ProductID
                };

                DirectProductCosts.Add(dpc);
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
            ExpenseBudgetManagement.Instance.Update();
        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DirectProductCosts.Clear();
            account = Accounts[dgAccounts.SelectedIndex];

            var query = from u in db.DirectProductCost
                        where u.AccountID == account.AccountID
                        select u;

            DirectProductCost dpc;

            foreach (var item in query)
            {
                dpc = new DirectProductCost
                {
                    Account = account,
                    AccountID = account.AccountID,
                    Product = item.Product,
                    ProductID = item.ProductID,
                    ProductCost = item.ProductCost

                };
                DirectProductCosts.Add(dpc);
            }
            dgDPPC.ItemsSource = DirectProductCosts;
        }

        private void dgDPPC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objToAdd = dgDPPC.SelectedItem as DirectProductCost;
        }

        private void dgDPPC_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement elementProductID = dgDPPC.Columns[0].GetCellContent(e.Row);
                if (elementProductID.GetType() == typeof(TextBox))
                {
                    var column1 = ((TextBox)elementProductID).Text;
                    objToAdd.ProductID = column1;

                }

                FrameworkElement elementProductCost = dgDPPC.Columns[1].GetCellContent(e.Row);
                if (elementProductCost.GetType() == typeof(TextBox))
                {
                    var column1 = ((TextBox)elementProductCost).Text;
                    objToAdd.ProductCost = Convert.ToInt32(column1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fel");
            }
        }

        private void dgDPPC_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            
            // Kontrollera om vi ändrar en befintlig produkt eller en ny
            // innan expensebudget i management
            
            
            ExpenseBudget eb = null;

             


                if (!ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist())
                {
                    eb = new ExpenseBudget
                    {
                        ExpenseBudgetID = ExpenseBudgetManagement.Instance.GetExpenseBudgetID(),
                        ProductionLock = 0,
                        SellLock = 0
                    };

                    ExpenseBudgetManagement.Instance.Create(eb);
                    db.DirectProductCost.Add(objToAdd);
                }
                else
                {
                    int id = ExpenseBudgetManagement.Instance.GetExpenseBudgetID();

                    var listOfExpenseBudgets = ExpenseBudgetManagement.Instance.GetExpenseBudgets();

                    foreach (var expenseBudget in listOfExpenseBudgets.Where(expenseBudget => expenseBudget.ExpenseBudgetID.Equals(id)))
                    {
                        eb = expenseBudget;
                    }
                    ExpenseBudgetManagement.Instance.Update();
                }

                objToAdd.AccountID = Accounts[dgAccounts.SelectedIndex].AccountID;
                objToAdd.ExpenseBudgetID = eb.ExpenseBudgetID;
     
                
               
            

        }
    }
}
