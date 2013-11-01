using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.FollowUp
{
    public enum CostProductOption
    {
        Product,
        Productgroup,
        Department,
        Company
    }

    public class BudgetedResultManagement
    {
        public ObservableCollection<GeneralFollowUp> GeneralFollowUps { get; set; }

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

        /// <summary>
        /// Lazy Instance of RevenueFollowUpManagement singelton
        /// </summary>
        private static readonly Lazy<BudgetedResultManagement> instance = new Lazy<BudgetedResultManagement>(() => new BudgetedResultManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static BudgetedResultManagement Instance
        {
            get { return instance.Value; }


        }

        /// <summary>
        /// Constructor with initialization of accounts list
        /// </summary>
        private BudgetedResultManagement()
        {
            GeneralFollowUps = new ObservableCollection<GeneralFollowUp>();
        }

        public void FillGeneralFollowUpsWithProducts()
        {
            GeneralFollowUps.Clear();

            foreach (var item in db.Product)
            {
                GeneralFollowUp gfu = new GeneralFollowUp();
                gfu.ObjectID = item.ProductID;
                gfu.ObjectName = item.ProductName;
                GeneralFollowUps.Add(gfu);
            }
        }

        public void FillGeneralFollowUpsWithDepartments()
        {
            GeneralFollowUps.Clear();

            foreach (var item in db.Department)
            {
                GeneralFollowUp gfu = new GeneralFollowUp();
                gfu.ObjectID = item.DepartmentID;
                gfu.ObjectName = item.DepartmentName;
                GeneralFollowUps.Add(gfu);
            }
        }

        public void FillGeneralFollowUpsWithProductGroups()
        {
            GeneralFollowUps.Clear();

            foreach (var item in db.ProductGroup)
            {
                GeneralFollowUp gfu = new GeneralFollowUp();
                gfu.ObjectID = item.ProductGroupID;
                gfu.ObjectName = item.ProductGroupName;
                GeneralFollowUps.Add(gfu);
            }
        }

        public void FillGeneralFollowUpsWithCompany()
        {
            GeneralFollowUps.Clear();

            // Företaget
        }

        public GeneralFollowUp GetResults(CostProductOption cpo, string objectID)
        {
            GeneralFollowUp gfu = new GeneralFollowUp();

            switch (cpo)
            {
                case CostProductOption.Product:
                    var product = db.Product.Single(p => p.ProductID.Equals(objectID));
                    gfu.ObjectName = product.ProductName;

                    foreach (var p in db.DirectProductCost.Where(p => p.ProductID.Equals(objectID)))
                    {
                        gfu.Costs += p.ProductCost;
                        gfu.Revenues += 5;     // Vart kommer denna ifrån? 
                    }
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Productgroup:
                    var productGroup = db.ProductGroup.Single(p => p.ProductGroupID.Equals(objectID));
                    gfu.ObjectName = productGroup.ProductGroupName;
                    gfu.Costs = 4000; // Vart kommer denna ifrån?
                    gfu.Revenues = 5000;  // Vart kommer denna ifrån?
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Department:
                    var department = db.Department.Single(p => p.DepartmentID.Equals(objectID));
                    gfu.ObjectName = department.DepartmentName;
                    gfu.Costs = 4000; // Vart kommer denna ifrån?
                    gfu.Revenues = 5000;  // Vart kommer denna ifrån?
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Company: // Var kommer företaget ifrån?
                    break;
            }

            return gfu;
        }

        public decimal GetCalculatedDepartmentSchablonCost(string departmentID)
        {//SCHABLONSKOSTNADER PER AVDELNING
            decimal totalDepartmentSchablonCost = 0;
            foreach (Account account in Logic_Layer.Cost_Budgeting_Logic.DCPPDManagement.Instance.GetAccountsByDepartment(departmentID))
            {
                if (account.AccountID != 5021 && account.AccountID != 9999)
                {
                    int tempCost;
                    if (account.AccountCost == null)
                        tempCost = 0;
                    else
                        tempCost = (int)account.AccountCost;
                    totalDepartmentSchablonCost += tempCost;
                }
            }
            return totalDepartmentSchablonCost;
        }

        public decimal GetTotalCalculatedSchablonCost()
        {//SCHABLONSKOSTNADER FÖR ALLA AVDELNINGAR
            decimal sum = 0;
            foreach (Department department in DepartmentList)
            {
                sum += GetCalculatedDepartmentSchablonCost(department.DepartmentID);
            }
            return sum;
        }

        public decimal GetAnnualEmployeeAtProductByDepartment(string departmentID)
        {// ÅRSARBETARE VIA PRODUKT PER AVDELNING
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
            {// ÅRSARBETARE VIA PRODUKT PÅ ALLA AVDELNINGAR
                foreach (ProductPlacement pp in ProductPlacementList)
                {
                    if (product.ProductID.Equals(pp.ProductID))
                        totalAnnualProduct += (int)pp.ProductAllocate;
                }
            }
            return totalAnnualProduct / 100;
        }

        public int GetEmployeeSallaryCostByDepartment(string departmentID)
        {// ANSTÄLLDAS LÖNEKOSTNADER PER AVDELNING
            int totalSallary = 0;
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (Employee employee in EmployeeList)
            {
                int tempSallary = employee.MonthSallary * (employee.AnnualRate / 100);
                totalSallary += tempSallary;
            }
            return totalSallary;
        }

        public decimal GetCalculatedDirectProductCost()
        {
            decimal sum = 0;
            foreach (DirectProductCost dp in Logic_Layer.Cost_Budgeting_Logic.DCPPDManagement.Instance.DirectProductCosts)
                sum += dp.ProductCost;
            return sum;
        }

        public decimal GetCalculatedTotalProductionCostByDepartment(string departmentID)
        {//PRODUKTIONSKOSTNAD PER AVDELNING
            return (GetAnnualEmployeeAtProductByDepartment(departmentID) * GetTotalCalculatedSchablonCost()) + GetEmployeeSallaryCostByDepartment(departmentID) + GetCalculatedDirectProductCost();
        }

        public decimal GetCalculatedTotalProductionCost()
        {//PRODUKTIONSKOSTNAD FÖR DA UF AVDELNINGAR
            decimal sum = 0;
            foreach (Department department in DepartmentList)
            {
                if (department.DepartmentID.Equals("DA") || department.DepartmentID.Equals("UF"))
                    sum += GetCalculatedTotalProductionCostByDepartment(department.DepartmentID);
            }
            return sum;
        }

        public decimal GetCalculatedDirectActivityCost()
        {
            decimal sum = 0;
            foreach (DirectActivityCost dp in Logic_Layer.Cost_Budgeting_Logic.DCPADManagement.Instance.DirectActivityCosts)
                sum += dp.ActivityCost;
            return sum;
        }

        public decimal GetAdditionCostByDepartment(string departmentID)
        {//PÅLÄGGSKOSTNAD PER AVDELNING
            return GetEmployeeSallaryCostByDepartment(departmentID) + GetCalculatedDepartmentSchablonCost(departmentID) + GetCalculatedDirectActivityCost();
        }

        public decimal GetCalculatedTotalAFFOCost()
        {//PRODUKTIONSKOSTNAD FÖR AO FO AVDELNINGAR
            decimal sum = 0;
            foreach (Department department in DepartmentList)
            {
                if (department.DepartmentID.Equals("AO") || department.DepartmentID.Equals("FO"))
                    sum += GetAdditionCostByDepartment(department.DepartmentID);
            }
            return sum;
        }

        public decimal GetAddition()
        {//RETURNERAR PÅLÄGGET
            return GetCalculatedTotalAFFOCost() / GetCalculatedTotalProductionCost();
        }

    }
}
