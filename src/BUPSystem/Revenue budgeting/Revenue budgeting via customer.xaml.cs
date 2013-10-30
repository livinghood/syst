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
using System.Windows.Media;
using System.Windows.Controls.Primitives;

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
                dgIncomeProduct.ItemsSource = FinancialIncomeList;
                lblCustomerID.Content = SelectedCustomer.CustomerID;
                lblCustomerName.Content = SelectedCustomer.CustomerName;
                dgIncomeProduct.IsEnabled = true;
                LockPrimaryCells();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentIncomeYearID = RevenueManagement.Instance.CreateFinancialIncomeYear();
                RevenueManagement.Instance.UpdateFinancialIncome();
                MessageBox.Show("Intäktsbudgetteringen är nu sparad");
            }
            catch
            {
                MessageBox.Show("Du kan inte spara samma produkt flera gånger på samma kund");
            }
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

                DataGridCell cellID = GetCell(dgIncomeProduct.SelectedIndex, 0); //Pass the row and column
                if (cellID.Content == null)
                cellID.IsEnabled = false;
                DataGridCell cellName = GetCell(dgIncomeProduct.SelectedIndex, 1); //Pass the row and column
                cellName.IsEnabled = false;
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

            // Check if Name is correct

            Product tempProduct = ProductManagement.Instance.GetProductByName(autoCompleteBox.Text);

            if (tempProduct != null)
            {
                fi.ProductName = tempProduct.ProductName;
                fi.ProductID = tempProduct.ProductID;

                DataGridCell cellID = GetCell(dgIncomeProduct.SelectedIndex, 0); //Pass the row and column
                cellID.IsEnabled = false;
                DataGridCell cellName = GetCell(dgIncomeProduct.SelectedIndex, 1); //Pass the row and column
                cellName.IsEnabled = false;
            }
        }

        private void LockPrimaryCells()
        {
            for (int i = 0; i < FinancialIncomeList.Count; i++)
            {
                DataGridCell cellID = GetCell(i, 0); //Pass the row and column
                cellID.IsEnabled = false;
                DataGridCell cellName = GetCell(i, 1); //Pass the row and column
                cellName.IsEnabled = false;
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

        public DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    dgIncomeProduct.ScrollIntoView(rowContainer, dgIncomeProduct.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

                return cell;

            }

            return null;
        }


        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dgIncomeProduct.ItemContainerGenerator.ContainerFromIndex(index);

            if (row == null)
            {
                dgIncomeProduct.UpdateLayout();
                dgIncomeProduct.ScrollIntoView(dgIncomeProduct.Items[index]);
                row = (DataGridRow)dgIncomeProduct.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;

        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>
                    (v);
                }

                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

    }
}
