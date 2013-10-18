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

namespace BUPSystem.Revenue_budgeting
{
    /// <summary>
    /// Interaction logic for RevenueBudgetingViaCustomer.xaml
    /// </summary>
    public partial class RevenueBudgetingViaCustomer : Window
    {
        //Lista som fylls och töms varje gång man väljer kund
        private List<FinancialIncome> TempIncomeList = new List<FinancialIncome>();

        /// <summary>
        /// List with financialincomes from RevenueManagement
        /// </summary>
        public ObservableCollection<FinancialIncome> FinancialIncomeList
        {
            get { return RevenueManagement.Instance.FinancialIncomeList; }
            set { RevenueManagement.Instance.FinancialIncomeList = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RevenueBudgetingViaCustomer()
        {
            InitializeComponent();
            dgIncomeProduct.ItemsSource = TempIncomeList;
        }

        /// <summary>
        /// Loads CustomerRegister and clear the temporary list TempIncomeList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoseCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerRegister customerRegister = new CustomerRegister();
            Customer customer = customerRegister.ShowDialog();
            TempIncomeList.Clear();
            FillTempListCustomer(customer);
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

        private void SearchByID(string text)
        { 
            foreach (Product p in RevenueManagement.Instance.ProductList)
            {
                if (p.ProductID.Contains(text))
                {
                }
            }
        }

        private void SearchByName()
        { 
        }

        private void dgIncomeProduct_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
        }


    }
}
