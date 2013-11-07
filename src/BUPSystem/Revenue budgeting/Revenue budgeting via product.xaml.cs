using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using System.ComponentModel;
using System.Windows.Data;
using BUPSystem.ProductGUI;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Logic_Layer.General_Logic;
using Microsoft.Win32;

namespace BUPSystem.Revenue_budgeting
{
    /// <summary>
    /// Interaction logic for RevenueBudgetingViaProduct.xaml
    /// </summary>
    public partial class RevenueBudgetingViaProduct : Window
    {
       /// <summary>
        /// List with financialincomes from RevenueManagement
        /// </summary>
        public ObservableCollection<FinancialIncome> FinancialIncomeList
        {
            get { return RevenueManagement.Instance.FinancialIncomeList; }
            set { RevenueManagement.Instance.FinancialIncomeList = value; }
        }

        public IEnumerable<Customer> CustomerList
        {
            get { return RevenueManagement.Instance.CustomerList; }
        }

        public Product SelectedProduct { get; set; }

        private FinancialIncomeYear CurrentFinancialIncomeYear 
        {
            get { return RevenueManagement.Instance.CurrentFinancialIncomeYear; }
            set { RevenueManagement.Instance.CurrentFinancialIncomeYear = value; }
        }

        private ObservableCollection<FinancialIncome> NewFinancialIncomeList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RevenueBudgetingViaProduct()
        {
            NewFinancialIncomeList = new ObservableCollection<FinancialIncome>();
            CurrentFinancialIncomeYear = RevenueManagement.Instance.CreateFinancialIncomeYear();
            InitializeComponent();
            dgIncomeCustomer.IsEnabled = false;
            DataContext = this;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = false;

            if (CurrentFinancialIncomeYear.FinancialIncomeLock)
            {
                dgIncomeCustomer.IsReadOnly = true;
                btnLock.IsEnabled = false;
            }


            UserAccount userAccount = UserManagement.Instance.GetUserAccountByUsername(System.Threading.Thread.CurrentPrincipal.Identity.Name);

            if (userAccount == null)
                Application.Current.Shutdown();


            switch (userAccount.PermissionLevel)
            {

                // Ekonomichef
                case 1:
                    btnSave.Visibility = Visibility.Collapsed;
                    btnDelete.Visibility = Visibility.Collapsed;
                    btnLock.Visibility = Visibility.Collapsed;
                    dgIncomeCustomer.IsReadOnly = true;
                    break;

                // Försäljningschef
                case 2:
                    btnExportTextFile.Visibility = Visibility.Collapsed;
                    break;

                // Systemadministratör
                case 5:
                    // Kan göra allt?
                    break;

                // Säljare
                case 6:
                    btnLock.Visibility = Visibility.Collapsed;
                    btnExportTextFile.Visibility = Visibility.Collapsed;
                    break;
                
            }
        }

        /// <summary>
        /// Loads CustomerRegister and clear the temporary list TempIncomeList
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChoseProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductRegister productRegister = new ProductRegister(true);

            if (productRegister.ShowDialog() == true)
            {
                SelectedProduct = productRegister.SelectedProduct;
                FinancialIncomeList = new ObservableCollection<FinancialIncome>(RevenueManagement.Instance.GetFinancialIncomesByProduct(SelectedProduct.ProductID));
                dgIncomeCustomer.ItemsSource = FinancialIncomeList;
                lblProductID.Content = SelectedProduct.ProductID;
                lblProductName.Content = SelectedProduct.ProductName;
                dgIncomeCustomer.IsEnabled = true;
                LockPrimaryCells();
                if (CurrentFinancialIncomeYear.FinancialIncomeLock == false)
                {
                    btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = true;
                }

                UpdateLabels();
            }

        }

        /// <summary>
        /// Save all fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValid(dgIncomeCustomer)){
                MessageBox.Show("Det finns felaktigt inmatade fält, var god ändra");
                return;
            }

            try
            {
                if (NewFinancialIncomeList.Any())
                    foreach (FinancialIncome fi in NewFinancialIncomeList)
                        RevenueManagement.Instance.AddIncome(fi);

                FinancialIncomeList = RevenueManagement.Instance.RemoveEmptyProductIncomes();
                RevenueManagement.Instance.UpdateFinancialIncome();
                MessageBox.Show("Intäktsbudgeteringen är nu sparad");
            }
            catch
            {
                MessageBox.Show("Du kan inte spara samma kund flera gånger på samma produkt");
            }
        }

        public bool IsValid(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { return false; }
            }

            return true;
        }
        /// <summary>
        /// Delete the selected row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Är du säker på att du vill ta bort denna raden helt?" + Environment.NewLine + "Detta går inte att ångra.", "Ta Bort Rad", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    FinancialIncome fi = (FinancialIncome)dgIncomeCustomer.SelectedItem;
                    RevenueManagement.Instance.DeleteFinancialIncome(fi);
                    UpdateLabels();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Du kommer nu att låsa intäktsbudgetering för både via kund och via produkt." + Environment.NewLine + "Vill du fortsätta?", "Låsa intäktsbudget",MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CurrentFinancialIncomeYear.FinancialIncomeLock = true;
                RevenueManagement.Instance.UpdateFinancialIncomeYear();
                dgIncomeCustomer.IsReadOnly = true;
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
        private void EditedCustomerID(object sender, RoutedEventArgs e)
        {
            FinancialIncome fi = (FinancialIncome)dgIncomeCustomer.SelectedItem;

            var autoCompleteBox = sender as AutoCompleteBox;

            // Check if ID is correct

            Customer tempCustomer = CustomerManagement.Instance.GetCustomerByID(autoCompleteBox.Text);

            if (tempCustomer != null)
            {
                fi.CustomerID = tempCustomer.CustomerID;
                fi.CustomerName = tempCustomer.CustomerName;

                DataGridCell cellID = GetCell(dgIncomeCustomer.SelectedIndex, 0); //Pass the row and column
                cellID.IsEnabled = false;
                DataGridCell cellName = GetCell(dgIncomeCustomer.SelectedIndex, 1); //Pass the row and column
                cellName.IsEnabled = false;
                UpdateLabels();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditedCustomerName(object sender, RoutedEventArgs e)
        {
            FinancialIncome fi = (FinancialIncome)dgIncomeCustomer.SelectedItem;

            var autoCompleteBox = sender as AutoCompleteBox;

            // Check if Name is correct

            Customer tempCustomer = CustomerManagement.Instance.GetCustomerByName(autoCompleteBox.Text);

            if (tempCustomer != null)
            {
                fi.CustomerName = tempCustomer.CustomerName;
                fi.CustomerID = tempCustomer.CustomerID;

                DataGridCell cellID = GetCell(dgIncomeCustomer.SelectedIndex, 0); //Pass the row and column
                cellID.IsEnabled = false;
                DataGridCell cellName = GetCell(dgIncomeCustomer.SelectedIndex, 1); //Pass the row and column
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
        private void dgIncomeCustomer_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

            FinancialIncome obj = e.NewItem as FinancialIncome;
            if (obj != null)
            {
                NewFinancialIncomeList.Add(obj);
                obj.ProductID = SelectedProduct.ProductID;
                UpdateLabels();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        bool isManualEdit;
        private void dgIncomeCustomer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEdit)
            {
                isManualEdit = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEdit = false;
                UpdateLabels();
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
                    dgIncomeCustomer.ScrollIntoView(rowContainer, dgIncomeCustomer.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);

                return cell;

            }

            return null;
        }


        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)dgIncomeCustomer.ItemContainerGenerator.ContainerFromIndex(index);

            if (row == null)
            {
                dgIncomeCustomer.UpdateLayout();
                dgIncomeCustomer.ScrollIntoView(dgIncomeCustomer.Items[index]);
                row = (DataGridRow)dgIncomeCustomer.ItemContainerGenerator.ContainerFromIndex(index);
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

        private void btnExportTextFile_Click(object sender, RoutedEventArgs e)
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            foreach (FinancialIncome fi in FinancialIncomeList)
            {
                // Reset changes
                RevenueManagement.Instance.ResetFinancialIncome(fi);
            }
        }

        private void UpdateLabels()
        {
            int? add = 0;
            int? bud = 0;
            int? agr = 0;
            int? hour = 0;

            foreach (var item in FinancialIncomeList)
            {
                add += item.Addition;
                bud += item.Budget;
                agr += item.Agreement;
                hour += item.Hours;
            }

            lblAddition.Content = add;
            lblBudget.Content = bud;
            lblAgreement.Content = agr;
            lblHour.Content = hour;
        }
    }
}
