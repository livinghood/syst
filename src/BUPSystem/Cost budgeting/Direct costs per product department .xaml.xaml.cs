﻿using System;
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

        private Product product { get; set; }

        private Account account { get; set; }

        public ObservableCollection<DirectProductCost> DirectProductCosts
        {
            get { return DCPPDManagement.Instance.DirectProductCosts; }
            set { DCPPDManagement.Instance.DirectProductCosts = value; }
        }

        public ObservableCollection<Account> Accounts
        {
            get { return AccountManagement.Instance.Accounts; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public DirectCostsPerProductDepartment()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Method used to make some initial preparations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void winDKPPA_Loaded(object sender, RoutedEventArgs e)
        {
            dgAccounts.ItemsSource = Accounts;

            //brnLock är alltid låst från början. Enablas när produktionschefen loggar in, 
            //utifall den är låst i databasen(100, 101) så diseables knappen för honom också.

            int isLocked = ExpenseBudgetManagement.Instance.IsExpenseBudgetLocked();
            
            if (isLocked == 100 || isLocked == 101)
            {
                btnLock.IsEnabled = false;
            }
        }

        /// <summary>
        /// Connect a new product to the selected account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductGUI.ProductRegister pr = new ProductGUI.ProductRegister();
            pr.ShowDialog();

            if (pr.DialogResult == true)
            {
                product = pr.Product;

                // Check if user attempts to add a product that is already connected to the selected account
                bool productConnected = DCPPDManagement.Instance.CheckIfProductConnected(product.ProductID);

                if (productConnected)
                {
                    MessageBox.Show(String.Format
                        ("Du försöker lägga till en produkt som redan är kopplad till konto {0}.",
                        account.AccountName), "Produkt redan kopplad");
                    return;
                }

                objToAdd = new DirectProductCost
                {
                    AccountID = account.AccountID,
                    ProductID = product.ProductID
                };

                DCPPDManagement.Instance.SaveNewProduct(objToAdd, dgAccounts.SelectedItem as Account);
            }
            lblSum.Content = "Summa: " + DCPPDManagement.Instance.CalculateSum(account);
        }

        /// <summary>
        /// Lock the costbudget
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Vill du verkligen låsa kostnadsbudgeten?", "Låsa kostnadsbudget?", MessageBoxButton.YesNo);

            if (mbr == MessageBoxResult.Yes)
            {
                UserPermissionLevels upl = UserPermissionLevels.Produktchef;

                if (upl == UserPermissionLevels.Produktchef)
                {
                    bool success = ExpenseBudgetManagement.Instance.LockExpenseBudget();
                    if (success)
                    {
                            MessageBox.Show("Kostnadsbudgeten har låsts", "Låsning lyckades");
                    }
                    else
                        MessageBox.Show("Kunde inte låsa kostnadsbudgeten", "Låsning misslyckades");
                }
            }
        }

        /// <summary>
        /// Change selected account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DirectProductCosts.Clear();
            account = dgAccounts.SelectedItem as Account;
            DirectProductCosts = new ObservableCollection<DirectProductCost>(DCPPDManagement.Instance.GetAccounts(account));
            dgDPPC.ItemsSource = DirectProductCosts;
            lblSum.Content = "Summa: " + DCPPDManagement.Instance.CalculateSum(account);
        }

        /// <summary>
        /// Change selected DirectProductCost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDPPC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDPPC.SelectedItem != null)
            {
                objToAdd = dgDPPC.SelectedItem as DirectProductCost;
            }
        }

        /// <summary>
        /// Allows editing of cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    var column2 = ((TextBox)elementProductCost).Text;
                    objToAdd.ProductCost = Convert.ToInt32(column2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Fel");
            }
        }

        /// <summary>
        /// Saves edited row changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDPPC_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DCPPDManagement.Instance.SaveEditing(objToAdd, dgAccounts.SelectedItem as Account);
            lblSum.Content = "Summa: " + DCPPDManagement.Instance.CalculateSum(account);
        }
    }
}
