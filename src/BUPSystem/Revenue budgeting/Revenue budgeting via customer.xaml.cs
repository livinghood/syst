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

namespace BUPSystem.Revenue_budgeting
{
    /// <summary>
    /// Interaction logic for RevenueBudgetingViaCustomer.xaml
    /// </summary>
    public partial class RevenueBudgetingViaCustomer : Window
    {
        //Lista som fylls och töms varje gång man väljer kund
        public ObservableCollection<FinancialIncome> TempIncomeList = new ObservableCollection<FinancialIncome>();

        /// <summary>
        /// List with financialincomes from RevenueManagement
        /// </summary>
        public ObservableCollection<FinancialIncome> FinancialIncomeList
        {
            get { return RevenueManagement.Instance.FinancialIncomeList; }
            set { RevenueManagement.Instance.FinancialIncomeList = value; }
        }

        private ObservableCollection<Product> ProductList
        {
            get { return RevenueManagement.Instance.ProductList; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RevenueBudgetingViaCustomer()
        {
            InitializeComponent();
            dgIncomeProduct.IsEnabled = false;
            DataContext = this;
            dgIncomeProduct.ItemsSource = TempIncomeList;
        }

        /// <summary>
        /// Loads CustomerRegister and clear the temporary list TempIncomeList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoseCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegister customerRegister = new CustomerRegister(true);
            customerRegister.ShowDialog();
            Customer customer = customerRegister.SelectedCustomer;
            TempIncomeList.Clear();
            FillTempListCustomer(customer);
            lblCustomerID.Content = customer.CustomerID.ToString();
            lblCustomerName.Content = customer.CustomerName;
            dgIncomeProduct.IsEnabled = true;
        }


        /// <summary>
        /// Fills the temporary list TempIncomeList with FinancialIncomes of a certain customer
        /// </summary>
        /// <param name="customer">Customer that the FinancialIncomes matches</param>
        private void FillTempListCustomer(Customer customer)
        {
            foreach (FinancialIncome fI in FinancialIncomeList)
            {
                if (fI.CustomerID.Equals(customer.CustomerID))
                {
                    TempIncomeList.Add(fI);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //för varje rad skall en ny FinancialIncome sparas
        }

        private void dgIncomeProduct_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var productID = dgIncomeProduct.Columns[0].GetCellContent(e.Row);
            var productName = dgIncomeProduct.Columns[1].GetCellContent(e.Row);

            foreach (Product p in ProductList)
            {
                if (productID is TextBox)
                {
                    if ((((TextBox)productID).Text).ToUpper().Equals(p.ProductID.ToUpper()))
                    {
                        ((TextBlock)productName).Text = p.ProductName;
                        ((TextBox)productID).Text = p.ProductID;
                    }
                }
                if (productName is TextBox)
                {
                    if ((((TextBox)productName).Text).ToUpper().Equals(p.ProductName.ToUpper()))
                    {
                        ((TextBlock)productID).Text = p.ProductID;
                        ((TextBox)productName).Text = p.ProductName;
                    }
                }
            }

            //FinancialIncome fi = TempIncomeList[dgIncomeProduct.SelectedIndex];
            //var h = dgIncomeProduct.Columns[1].GetCellContent(e.Row);
            //ProductName = ((TextBox)h).Text;
            //((TextBlock)h).Text = Budget.ToString(); //--> FUNKAR ATT TA TEXT IFRÅN EN CELL
        }

        //private void dgIncomeProduct_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    int sum = 0;
        //    foreach (var item in TempIncomeList)
        //    {
        //        if (item.Agreement != null)
        //            sum += item.Agreement.Value;
        //    }
        //    lblAgreement.Content = sum.ToString();
        //}

        //private void SearchByID(string text)
        //{ 
        //    foreach (Product p in RevenueManagement.Instance.ProductList)
        //    {
        //        if (p.ProductID.Contains(text))
        //        {
        //        }
        //    }
        //}

        //private void SearchByName()
        //{ 
        //}

        //private void dgIncomeProduct_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        //{
        //}


    }
}
