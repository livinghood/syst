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

        private ObservableCollection<FinancialIncome> NewFinancialIncomeList { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public RevenueBudgetingViaCustomer()
        {
            InitializeComponent();

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
                    dgIncomeProduct.IsReadOnly = true;
                    break;

                // Försäljningschef
                case 2:
                    btnExportToTextfile.Visibility = Visibility.Collapsed;
                    break;

                // Systemadministratör
                case 5:
                    // Kan göra allt?
                    break;

                // Säljare
                case 6:
                    btnLock.Visibility = Visibility.Collapsed;
                    btnExportToTextfile.Visibility = Visibility.Collapsed;
                    break;
            }

            NewFinancialIncomeList = new ObservableCollection<FinancialIncome>();
            CurrentFinancialIncomeYear = RevenueManagement.Instance.CreateFinancialIncomeYear();

            dgIncomeProduct.IsEnabled = false;
            DataContext = this;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = false;

            if (CurrentFinancialIncomeYear.FinancialIncomeLock)
            {
                dgIncomeProduct.IsReadOnly = true;
                btnLock.IsEnabled = false;
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
                dgIncomeProduct.IsEnabled = true;
                SelectedCustomer = customerRegister.SelectedCustomer;
                FinancialIncomeList = new ObservableCollection<FinancialIncome>(RevenueManagement.Instance.GetFinancialIncomesByCustomer(SelectedCustomer.CustomerID));
                dgIncomeProduct.ItemsSource = FinancialIncomeList;
                lblCustomerID.Content = SelectedCustomer.CustomerID;
                lblCustomerName.Content = SelectedCustomer.CustomerName;
                
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
            if (!IsValid(dgIncomeProduct))
            {
                MessageBox.Show("Det finns felaktigt inmatade fält, var god ändra");
                return;
            }
            
            try
            {
                if (NewFinancialIncomeList.Any())
                    foreach (FinancialIncome fi in NewFinancialIncomeList)
                        RevenueManagement.Instance.AddIncome(fi);

                FinancialIncomeList = RevenueManagement.Instance.RemoveEmptyCustomerIncomes();
                RevenueManagement.Instance.UpdateFinancialIncome();
                MessageBox.Show("Intäktsbudgeteringen är nu sparad");
            }
            catch
            {
                MessageBox.Show("Du kan inte spara samma produkt flera gånger på samma kund");
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
                    FinancialIncome fi = (FinancialIncome)dgIncomeProduct.SelectedItem;
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
                obj.CustomerID = SelectedCustomer.CustomerID;
                NewFinancialIncomeList.Add(obj);
                UpdateLabels();
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
                //try
                //{
                    RevenueManagement.Instance.ExportRevenueBudgetingToTextFile(sfd.FileName);
                    MessageBox.Show("Intäktsbudgeten har sparats till textfil", "Sparad");
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.ToString(), "Error");
                //}
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
