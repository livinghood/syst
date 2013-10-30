using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using System.ComponentModel;
using System.Windows.Data;
using BUPSystem.CustomerGUI;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace BUPSystem.Revenue_budgeting
{
    /// <summary>
    /// Interaction logic for RevenueBudgetingViaCustomer.xaml
    /// </summary>
    public partial class RevenueBudgetingViaCustomer : Window
    {
        /// <summary>
        /// List with financialincomes from RevenueManagement
        /// </summary>
        public ObservableCollection<FinancialIncome> FinancialIncomeList
        {
            get { return RevenueManagement.Instance.FinancialIncomeList; }
            set { RevenueManagement.Instance.FinancialIncomeList = value; }
        }

        public ObservableCollection<Product> ProductList
        {
            get { return RevenueManagement.Instance.ProductList; }
        }

        public Customer SelectedCustomer { get; set; }

        private string CurrentIncomeYearID { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RevenueBudgetingViaCustomer()
        {
            InitializeComponent();
            dgIncomeProduct.IsEnabled = false;
            DataContext = this;
        }

        /// <summary>
        /// Loads CustomerRegister and clear the temporary list TempIncomeList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoseCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegister customerRegister = new CustomerRegister(true);

            if (customerRegister.ShowDialog() == true)
            {
                SelectedCustomer = customerRegister.SelectedCustomer;
                FinancialIncomeList = new ObservableCollection<FinancialIncome>(RevenueManagement.Instance.GetFinancialIncomesByCustomer(SelectedCustomer.CustomerID));
                //FinancialIncomeList.Clear();
                dgIncomeProduct.ItemsSource = FinancialIncomeList;
                lblCustomerID.Content = SelectedCustomer.CustomerID;
                lblCustomerName.Content = SelectedCustomer.CustomerName;
                dgIncomeProduct.IsEnabled = true;
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CurrentIncomeYearID = RevenueManagement.Instance.CreateFinancialIncomeYear();
            RevenueManagement.Instance.UpdateFinancialIncome();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditedProductID(object sender, RoutedEventArgs e)
        {
            FinancialIncome fi = (FinancialIncome)dgIncomeProduct.SelectedItem;

            var autoCompleteBox = sender as AutoCompleteBox;

            // Check if ID is correct

            Product tempProduct = ProductManagement.Instance.GetProductByID(autoCompleteBox.Text);

            if (tempProduct != null)
            {
                fi.ProductID = tempProduct.ProductID;
                fi.ProductName = tempProduct.ProductName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditedProductName(object sender, RoutedEventArgs e)
        {
            FinancialIncome fi = (FinancialIncome)dgIncomeProduct.SelectedItem;

            var autoCompleteBox = sender as AutoCompleteBox;

            // Check if ID is correct

            Product tempProduct = ProductManagement.Instance.GetProductByName(autoCompleteBox.Text);

            if (tempProduct != null)
            {
                fi.ProductName = tempProduct.ProductName;
                fi.ProductID = tempProduct.ProductID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgIncomeProduct_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

            FinancialIncome obj = e.NewItem as FinancialIncome;
            if (obj != null)
            {
                RevenueManagement.Instance.AddIncome(obj);
                obj.CustomerID = SelectedCustomer.CustomerID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        bool isManualEdit;
        private void dgIncomeProduct_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEdit)
            {
                isManualEdit = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEdit = false;
            }
        }

    }
}
