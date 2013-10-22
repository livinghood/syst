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

             dt = new DataTable();
            dt = dgDPPC.ItemsSource as DataTable;
            
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
            //foreach (var directProductCost in list)
            //{
            //    if (!db.DirectProductCost.ToList().Contains(directProductCost))
            //    {
            //        db.DirectProductCost.Add(directProductCost);                  
            //    }
            //}



            // save modified rows in DataTable modifiedRows


            DataTable modifiedRows = dt.GetChanges();
            dt.AcceptChanges();


            db.SaveChanges();

        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAccounts.SelectedIndex > Accounts.Count || dgAccounts.SelectedIndex < 0)
            {
                return;
            }

            list.Clear(); 
            account = Accounts[dgAccounts.SelectedIndex];

            var query = from u in db.DirectProductCost
                      where u.AccountID == account.AccountID
                      select u;

            DirectProductCost dpc = null;

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

                
            }



                list.Add(dpc);

            
            
            

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

        private void dgDPPC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            objToAdd = dgDPPC.SelectedItem as DirectProductCost;
        }

        private void dgDPPC_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                FrameworkElement elementProductID = dgDPPC.Columns[0].GetCellContent(e.Row);
                if (elementProductID.GetType() == typeof (TextBox))
                {
                    var column1 = ((TextBox) elementProductID).Text;
                    objToAdd.ProductID = column1;

                }

                FrameworkElement elementProductCost = dgDPPC.Columns[1].GetCellContent(e.Row);
                if (elementProductCost.GetType() == typeof(TextBox))
                {
                    var column1 = ((TextBox)elementProductCost).Text;
                    objToAdd.ProductCost = Convert.ToInt32(column1);
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private void dgDPPC_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

                var Res = MessageBox.Show("Create entry?", "Confirm", MessageBoxButton.YesNo);
            if (Res == MessageBoxResult.Yes)
            {
                objToAdd.AccountID = Accounts[dgAccounts.SelectedIndex].AccountID;
                objToAdd.ExpenseBudgetID = ExpenseBudgetManagement.Instance.GetExpenseBudgetID();


                if ()
                {
                    
                }
                ExpenseBudget eb = new ExpenseBudget
                {
                    ExpenseBudgetID 
                };

                db.DirectProductCost.Add(objToAdd);
                db.SaveChanges();
            }

        }
    }
}
