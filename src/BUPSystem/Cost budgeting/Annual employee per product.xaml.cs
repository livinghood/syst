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
        // List to be used in the combobox

        public ObservableCollection<Employee> EmployeeList { get; set; }

        public ObservableCollection<Product> SelectedProducts { get; set; }

        public ObservableCollection<ProductPlacement> ProductPlacementList { get; set; }

        public ObservableCollection<DataItem> MyList { get; set; }

        public ObservableCollection<Department> Departments { get { return EmployeeManagement.Instance.Departments;} }

        private string DepartmentID;


        public AnnualEmployeeViaProduct()
        {   //FÖR TESTNING SÅ SKICKAS DEPARTMENTID MED SOM UF
            InitializeComponent();
            DataContext = this;


            // ALLT NEDANFÖR SKALL IN I EGEN OPERATION SOM ANROPAS IFRÅN COMBOBOXEN
            // OM DET ÄR ADMIN, ANNARS SKALL DET SKE AUTOMATISKT

            DepartmentID = "UF";

            MyList = new ObservableCollection<DataItem>();

            ProductPlacementList = new ObservableCollection<ProductPlacement>();

            SelectedProducts = new ObservableCollection<Product>();

            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeByDepartment(DepartmentID));

            CalculateAttributeForEachEmployee();
            CreateRow();
            
            LoadExistingPlacements();
            
        }

        /// <summary>
        /// Fyller listan med redan existerande placeringar
        /// </summary>
        private void LoadExistingPlacements()
        {
            foreach (Employee e in EmployeeList)
            {
                foreach (ProductPlacement p in ProductManagement.Instance.GetProductPlacementsByEmployee(e))
                {
                    bool found = false;
                    foreach (DataGridColumn dgc in dgProductPlacements.Columns)
                    {
                        if (dgc.Header.Equals(p.Product.ProductName))
                        {
                            foreach (DataItem di in MyList)
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
                    foreach (DataItem di in MyList)
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

        private void CalculateAttributeForEachEmployee()
        {
            if (EmployeeList != null)
            {
                ObservableCollection<Employee> tempEmployees = new ObservableCollection<Employee>(EmployeeList);
                EmployeeList = EmployeeManagement.Instance.CalculateEmployeeAtributes(tempEmployees);
            }
        }

        private void btnChooseProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductRegister productRegister = new ProductRegister(true);

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
                foreach (DataItem di in MyList)
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
                var m = new DataItem() { EmployeeID = e.EmployeeID };
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
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

    }

    public class DataItem
    {   //KLASS FÖR ATT LÄGGA TILL EGNA RADER
        public long EmployeeID { get; set; }
        public ObservableCollection<ProductPlacement> DataList { get; set; }
        public DataItem()
        {
            this.DataList = new ObservableCollection<ProductPlacement>();
        }

    }
}
