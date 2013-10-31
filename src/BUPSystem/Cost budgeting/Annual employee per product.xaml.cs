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

namespace BUPSystem.Kostnadsbudgetering
{
    /// <summary>
    /// Interaction logic for AnnualEmployeeViaProduct.xaml
    /// </summary>
    public partial class AnnualEmployeeViaProduct : Window
    {
        // List to be used in the combobox
        public ObservableCollection<Department> Departments { get { return EmployeeManagement.Instance.Departments; } }

        public ObservableCollection<Employee> EmployeeList { get; set; }

        public ObservableCollection<Product> SelectedProducts { get; set; }


        public AnnualEmployeeViaProduct(string departmentID)
        {   //FÖR TESTNING SÅ SKICKAS DEPARTMENTID MED SOM UF
            InitializeComponent();

            SelectedProducts = new ObservableCollection<Product>();
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeByDepartment(departmentID));
            CalculateAttributeForEachEmployee();
            DataContext = this;
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
                SelectedProducts.Add(productRegister.SelectedProduct);

                // Skapa manuellt kolumner för varje produkt som läggs till
            }
        }
    }
}
