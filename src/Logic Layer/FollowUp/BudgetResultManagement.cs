using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Media;

namespace Logic_Layer.FollowUp
{
    public class BudgetResultManagement
    {

        public IEnumerable<Account> AccountList
        {
            get { return AccountManagement.Instance.Accounts; }
        }

        public IEnumerable<Department> DepartmentList
        {
            get { return EmployeeManagement.Instance.Departments; }
        }

        public IEnumerable<Product> ProductList
        {
            get { return ProductManagement.Instance.Products; }
        }

        public ObservableCollection<Employee> EmployeeList { get; set; }

        public ObservableCollection<ProductPlacement> ProductPlacementList { get; set; }

        public BudgetResultManagement()
        { 
        }

        public int GetCalculatedSchablonCost()
        {
            int totalSchablonCost = 0;
            foreach (Account account in AccountList)
            {
                if (account.AccountID != 5021)
                {
                    int tempCost;
                    if (account.AccountCost == null)
                        tempCost = 0;
                    else
                        tempCost = (int)account.AccountCost;
                    totalSchablonCost += tempCost;
                }
            }
            return totalSchablonCost;
        }

        public decimal GetAnnualEmployeeAtProductByDepartment(string departmentID)
        {
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (Employee employee in EmployeeList)
            {
                ObservableCollection<ProductPlacement> tempPlacements = new ObservableCollection<ProductPlacement>(ProductManagement.Instance.GetProductPlacementsByEmployee(employee));
                foreach (ProductPlacement productPlacement in tempPlacements)
                {// MÅSTE RÄKNA IHOP PER PRODUKT
                    ProductPlacementList.Add(productPlacement);
                }
                
            }
            decimal totalAnnualProduct = 0;
            foreach (Product product in ProductList)
            {
                foreach (ProductPlacement pp in ProductPlacementList)
                {
                    if (product.ProductID.Equals(pp.ProductID))
                        totalAnnualProduct += (int)pp.ProductAllocate;
                }
            }
            return totalAnnualProduct / 100;
        }

        public int GetEmployeeSallaryCostByDepartment(string departmentID)
        {
            int totalSallary = 0;
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (Employee employee in EmployeeList)
            {
                int tempSallary = employee.MonthSallary * (employee.AnnualRate / 100);
                totalSallary += tempSallary;
            }
            return totalSallary;
        }

    }
}
