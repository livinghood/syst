using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using BUPSystem.ProductGUI;
using System.Windows.Data;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace BUPSystem.Kostnadsbudgetering
{
    /// <summary>
    /// Interaction logic for AnnualEmployeeViaProduct.xaml
    /// </summary>
    public partial class AnnualEmployeeViaProduct : Window
    {
        private ObservableCollection<Employee> m_EmployeeList = new ObservableCollection<Employee>();
        public ObservableCollection<Employee> EmployeeList { get { return m_EmployeeList; } }

        public ObservableCollection<Product> SelectedProducts { get; set; }

        public ObservableCollection<ProductPlacement> ProductPlacementList { get; set; }

        public ObservableCollection<DataItemProduct> MyList { get; set; }

        public ObservableCollection<Department> Departments { get { return EmployeeManagement.Instance.Departments;} }

        private string DepartmentID;


        public AnnualEmployeeViaProduct()
        {   //FÖR TESTNING SÅ SKICKAS DEPARTMENTID MED SOM UF
            InitializeComponent();
            DataContext = this;
            Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist();

            MyList = new ObservableCollection<DataItemProduct>();
            ProductPlacementList = new ObservableCollection<ProductPlacement>();
            SelectedProducts = new ObservableCollection<Product>();
            //EmployeeList = new ObservableCollection<Employee>();

            Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist();
            Logic_Layer.UserAccount userAccount = null;

            userAccount = UserManagement.Instance.GetUserAccountByUsername(System.Threading.Thread.CurrentPrincipal.Identity.Name);

            switch (userAccount.PermissionLevel)
            {
                //Drift Chef
                case 4:
                    DepartmentID = "DA";
                    cbDepartments.Visibility = Visibility.Collapsed;
                    lblChooseDepartment.Visibility = Visibility.Collapsed;
                    LoadEmployees();
                    break;
                //Utveckling Chef
                case 7:
                    DepartmentID = "UF";
                    cbDepartments.Visibility = Visibility.Collapsed;
                    lblChooseDepartment.Visibility = Visibility.Collapsed;
                    LoadEmployees();
                    break;
                //System Admin
                case 5:
                    DepartmentID = "DA";
                    break;
                //Ekonomichef
                case 1:
                    DepartmentID = "DA";
                    btnLock.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    dgProductPlacements.IsReadOnly = true;
                    btnChooseProduct.IsEnabled = false;
                    break;
            }
            LockedSettings();
        }

        private void LoadEmployees()
        {
            dgProductPlacements.Columns.Clear();
            EmployeeList.Clear();
            MyList.Clear();
            ProductPlacementList.Clear();
            SelectedProducts.Clear();

            foreach(Employee e in EmployeeManagement.Instance.GetEmployeeAtributes(DepartmentID))
            {
                m_EmployeeList.Add(e);
            }

            CreateRow();
            LoadExistingPlacements();
        }

        /// <summary>
        /// Fyller listan med redan existerande placeringar
        /// </summary>
        private void LoadExistingPlacements()
        {

            // Ber om ursäkt för dom här foreach-satserna. Dom är inte försvarbara.

            foreach (Employee e in EmployeeList)
            {
                foreach (ProductPlacement p in ProductManagement.Instance.GetProductPlacementsByEmployee(e))
                {
                    bool found = false;
                    foreach (DataGridColumn dgc in dgProductPlacements.Columns)
                    {
                        if (dgc.Header.Equals(p.Product.ProductName))
                        {
                            foreach (DataItemProduct di in MyList)
                            {
                                if (di.EmployeeID == e.EmployeeID)
                                {
                                    foreach (ProductPlacement pp in di.DataList)
                                    {
                                        if (pp.ProductID.Equals(p.ProductID))
                                        {
                                            pp.ProductAllocate = p.ProductAllocate;
                                            found = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (found)
                        continue;
                    DataGridTextColumn productColumn = new DataGridTextColumn();
                    productColumn.Header = p.Product.ProductName;
                    foreach (DataItemProduct di in MyList)
                    {
                        ProductPlacement pp = new ProductPlacement() { EmployeeID = di.EmployeeID, ProductID = p.ProductID, ProductAllocate = 0 };
                        if (di.EmployeeID == e.EmployeeID)
                            pp.ProductAllocate = p.ProductAllocate;
                        di.DataList.Add(pp);
                        SelectedProducts.Add(p.Product);
                    }
                    productColumn.Binding = new Binding("DataList[" + dgProductPlacements.Columns.Count + "].ProductAllocate");
                    dgProductPlacements.Columns.Add(productColumn);
                }
            }
        }

        private void btnChooseProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductRegister productRegister = new ProductRegister(true,DepartmentID);

            if (productRegister.ShowDialog() == true)
            {
                if (SelectedProducts.Contains(productRegister.SelectedProduct))
                {
                    MessageBox.Show("Du kan inte välja samma produkt flera gånger");
                }
                else
                {
                    SelectedProducts.Add(productRegister.SelectedProduct);
                    CreateColumn(productRegister.SelectedProduct);
                }
            }
        }

        private void CreateColumn(Product p)
        {
            DataGridTextColumn productColumn = new DataGridTextColumn();
            productColumn.Header = p.ProductName;
                foreach (DataItemProduct di in MyList)
                {
                    ProductPlacement pp = new ProductPlacement() { EmployeeID = di.EmployeeID, ProductID = p.ProductID, ProductAllocate = 0 };
                    di.DataList.Add(pp);
                    ProductPlacementList.Add(pp);
                }
            productColumn.Binding = new Binding("DataList[" + dgProductPlacements.Columns.Count + "].ProductAllocate");
            dgProductPlacements.Columns.Add(productColumn);
        }

        private void CreateRow()
        {
            foreach (Employee e in EmployeeList)
            {
                var m = new DataItemProduct() { EmployeeID = e.EmployeeID };
                MyList.Add(m);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (ProductPlacement pp in ProductPlacementList)
            {
                ProductManagement.Instance.AddProductPlacement(pp);
            }
            MessageBox.Show("Data är sparad");
        }

        private void cbDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;
            DepartmentID = Departments[cbDepartments.SelectedIndex].DepartmentID;
            LoadEmployees();
            LockedSettings();
        }

        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Vill du verkligen låsa denna budgeten?", "Låsa årsarbetare", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
            {
                if (Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.LockAnnualExpenseBudget(DepartmentID))
                {
                    LockedSettings();
                    MessageBox.Show("Årsarbetare per produkt är nu låst");
                }
                else
                    MessageBox.Show("Låsningen misslyckades");
            }
        }

        private void LockedSettings()
        {
            if (Logic_Layer.Cost_Budgeting_Logic.ExpenseBudgetManagement.Instance.IsAnnualExpenseBudgetLocked(DepartmentID))
            {
                btnLock.IsEnabled = false;
                btnSave.IsEnabled = false;
                dgProductPlacements.IsReadOnly = true;
                btnChooseProduct.IsEnabled = false;
            }
            else
            {
                btnLock.IsEnabled = true;
                btnSave.IsEnabled = true;
                dgProductPlacements.IsReadOnly = false;
                btnChooseProduct.IsEnabled = true;
            }
        }

    }


    public class DataItemProduct
    {   //KLASS FÖR ATT LÄGGA TILL EGNA RADER
        public long EmployeeID { get; set; }
        public ObservableCollection<ProductPlacement> DataList { get; set; }
        public DataItemProduct()
        {
            this.DataList = new ObservableCollection<ProductPlacement>();
        }

    }
}
