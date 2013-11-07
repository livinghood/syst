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
using Microsoft.Win32;

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

        public IEnumerable<Product> ProductList
        {
            get { return RevenueManagement.Instance.ProductList; }
        }

        public Customer SelectedCustomer { get; set; }

        private FinancialIncomeYear CurrentFinancialIncomeYear 
        {
            get { return RevenueManagement.Instance.CurrentFinancialIncomeYear; }
            set { RevenueManagement.Instance.CurrentFinancialIncomeYear = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RevenueBudgetingViaCustomer()
        {
            CurrentFinancialIncomeYear = RevenueManagement.Instance.CreateFinancialIncomeYear();
            InitializeComponent();
            dgIncomeProduct.IsEnabled = false;
            DataContext = this;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = false;

            if (CurrentFinancialIncomeYear.FinancialIncomeLock)
            {
                dgIncomeProduct.IsReadOnly = true;
                btnLock.IsEnabled = false;
            }

            Logic_Layer.UserAccount userAccount = null;

            userAccount = UserManagement.Instance.GetUserAccountByUsername(System.Threading.Thread.CurrentPrincipal.Identity.Name);

            switch (userAccount.PermissionLevel)
            {
                // Försäljningschefen
                case 2:
                    break;
                // Säljare
                case 6:
                    btnLock.IsEnabled = false;
                    break;
                //System Admin
                case 5:

                    break;
                //Ekonomichef
                case 1:
                    btnLock.IsEnabled = false;
                    btnDelete.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    break;
                default:
                    MessageBox.Show("Du har inte tillgång till detta");
                    this.Close();
                    break;
            }

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
                if (CurrentFinancialIncomeYear.FinancialIncomeLock == false)
                {
                    btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = true;
                }
            }

        }

        /// <summary>
        /// Save all fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FinancialIncomeList = RevenueManagement.Instance.RemoveEmptyCustomerIncomes();
                RevenueManagement.Instance.UpdateFinancialIncome();
                MessageBox.Show("Intäktsbudgeteringen är nu sparad");
            }
            catch
            {
                MessageBox.Show("Du kan inte spara samma produkt flera gånger på samma kund");
            }
        }

        /// <summary>
        /// Delete the selected row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FinancialIncome fi = (FinancialIncome)dgIncomeProduct.SelectedItem;
                RevenueManagement.Instance.DeleteFinancialIncome(fi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Du kommer nu att låsa intäktsbudgetering för både via kund och via produkt." + Environment.NewLine + "Vill du fortsätta?", "Låsa intäktsbudget",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CurrentFinancialIncomeYear.FinancialIncomeLock = true;
                RevenueManagement.Instance.UpdateFinancialIncomeYear();
                dgIncomeProduct.IsReadOnly = true;
                btnLock.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnSave.IsEnabled = false;
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
                child = v as T ?? GetVisualChild<T>(v);

                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void btnExportToTextfile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Title = "Spara fil", Filter = @"TXT Filer | *.txt;", FileName = "BudgetProduktKund.txt" };
            var result = sfd.ShowDialog();

            if (result == true)
            {
                try
                {
                    RevenueManagement.Instance.ExportRevenueBudgetingToTextFile(sfd.FileName);
                    MessageBox.Show("Intäktsbudgeten har sparats till textfil", "Sparad");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error");
                }
            }
        }
    }
}
