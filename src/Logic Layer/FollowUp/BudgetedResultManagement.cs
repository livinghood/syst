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

        public IEnumerable<Activity> ActivityList
        {
            get { return ActivityManagement.Instance.Activities; }
        }

        public ObservableCollection<Employee> EmployeeList { get; set; }

        public ObservableCollection<ProductPlacement> ProductPlacementList { get; set; }

        public ObservableCollection<ActivityPlacement> ActivityPlacementList { get; set; }

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

            foreach (GeneralFollowUp gfu in db.Product.Select(item => new GeneralFollowUp
            {
                ObjectID = item.ProductID,
                ObjectName = item.ProductName
            }))
            {
                GeneralFollowUps.Add(gfu);
            }
        }

        public void FillGeneralFollowUpsWithDepartments()
        {
            GeneralFollowUps.Clear();

            foreach (GeneralFollowUp gfu in db.Department.Select(item => new GeneralFollowUp
            {
                ObjectID = item.DepartmentID,
                ObjectName = item.DepartmentName
            }))
            {
                GeneralFollowUps.Add(gfu);
            }
        }

        public void FillGeneralFollowUpsWithProductGroups()
        {
            GeneralFollowUps.Clear();

            foreach (GeneralFollowUp gfu in db.ProductGroup.Select(item => new GeneralFollowUp
            {
                ObjectID = item.ProductGroupID,
                ObjectName = item.ProductGroupName
            }))
            {
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

                    gfu.Costs = (int)GetDirectProductCostByProductID(objectID);
                    gfu.Revenues = GetRevenueByProduct(objectID);

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Productgroup:
                    var productGroup = db.ProductGroup.Single(p => p.ProductGroupID.Equals(objectID));
                    gfu.ObjectName = productGroup.ProductGroupName;

                    gfu.Costs = GetProductGroupCostByID(objectID);
                    gfu.Revenues = GetProductGroupInmcomeByID(objectID);

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

        private decimal GetCalculatedDepartmentSchablonCost(string departmentID)
        {
            //SCHABLONSKOSTNADER PER AVDELNING
            decimal totalDepartmentSchablonCost = 0;
            foreach (Account account in Cost_Budgeting_Logic.DCPPDManagement.Instance.GetAccountsByDepartment(departmentID)
                .Where(account => account.AccountID != 5021 && account.AccountID != 9999))
            {
                int tempCost;
                if (account.AccountCost == null)
                    tempCost = 0;
                else
                    tempCost = (int)account.AccountCost;
                totalDepartmentSchablonCost += tempCost;
            }
            return totalDepartmentSchablonCost;
        }

        private decimal GetTotalCalculatedSchablonCost()
        {
            //SCHABLONSKOSTNADER FÖR ALLA AVDELNINGAR
            return DepartmentList.Sum(department => GetCalculatedDepartmentSchablonCost(department.DepartmentID));
        }

        private decimal GetAnnualEmployeeAtProductByDepartment(string departmentID)
        {
            // ÅRSARBETARE VIA PRODUKT PER AVDELNING
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (ProductPlacement productPlacement in EmployeeList.Select(employee =>
                new ObservableCollection<ProductPlacement>(ProductManagement.Instance.GetProductPlacementsByEmployee(employee))).SelectMany(tempPlacements => tempPlacements))
            {
                // MÅSTE RÄKNA IHOP PER PRODUKT
                ProductPlacementList.Add(productPlacement);
            }
            decimal totalAnnualProduct =
                (from product in ProductList
                 from pp in ProductPlacementList
                 where product.ProductID.Equals(pp.ProductID)
                 select pp).Aggregate<ProductPlacement, decimal>(0, (current, pp) => current + (int)pp.ProductAllocate);
            return totalAnnualProduct / 100;
        }

        private decimal GetAnnualEmployeeAtActivityByDepartment(string departmentID)
        {
            // ÅRSARBETARE VIA AKTIVITET PER AVDELNING
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (ActivityPlacement activityPlacement in EmployeeList.Select(employee =>
                new ObservableCollection<ActivityPlacement>(ActivityManagement.Instance.GetActivityPlacementsByEmployee(employee))).SelectMany(tempPlacements => tempPlacements))
            {
                // MÅSTE RÄKNA IHOP PER AKTIVITET
                ActivityPlacementList.Add(activityPlacement);
            }
            decimal totalAnnualActivity =
                (from activity in ActivityList
                 from ap in ActivityPlacementList
                 where activity.ActivityID.Equals(ap.ActivityID)
                 select ap).Aggregate<ActivityPlacement, decimal>(0, (current, ap) => current + (int)ap.ActivityAllocate);
            return totalAnnualActivity / 100;
        }

        private int GetEmployeeSallaryCostByDepartment(string departmentID)
        {
            // ANSTÄLLDAS LÖNEKOSTNADER PER AVDELNING
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            return EmployeeList.Sum(employee => employee.MonthSallary * (employee.AnnualRate / 100));
        }

        private decimal GetDirectProductCostByProductID(string productID)
        {
            //DIREKTA KOSTNADER FÖR PRODUKT
            return Cost_Budgeting_Logic.DCPPDManagement.Instance.DirectProductCosts
                .Where(dp => dp.Product.ProductID.Equals(productID)).Aggregate<DirectProductCost, decimal>(0, (current, dp) => current + dp.ProductCost);
        }

        //private decimal GetCalculatedDirectProductCosts()
        //{
        //    //DIREKTA KOSTNADER FÖR PRODUKTER
        //    return Cost_Budgeting_Logic.DCPPDManagement.Instance.DirectProductCosts.Aggregate<DirectProductCost, decimal>(0, (current, dp) => current + dp.ProductCost);
        //}

        //private decimal GetCalculatedTotalProductionCostByDepartment(string departmentID)
        //{
        //    //PRODUKTIONSKOSTNAD PER AVDELNING
        //    return (GetAnnualEmployeeAtProductByDepartment(departmentID) * GetTotalCalculatedSchablonCost()) + GetEmployeeSallaryCostByDepartment(departmentID) + GetCalculatedDirectProductCosts();
        //}

        //private decimal GetCalculatedTotalProductionCost()
        //{
        //    //PRODUKTIONSKOSTNAD FÖR DA UF AVDELNINGAR
        //    return DepartmentList.Where(department => department.DepartmentID.Equals("DA") || department.DepartmentID.Equals("UF"))
        //        .Sum(department => GetCalculatedTotalProductionCostByDepartment(department.DepartmentID));
        //}

        private decimal GetCalculatedDirectActivityCost()
        {
            return Cost_Budgeting_Logic.DCPADManagement.Instance.DirectActivityCosts.Aggregate<DirectActivityCost, decimal>(0, (current, dp) => current + dp.ActivityCost);
        }

        //private decimal GetAdditionCostByDepartment(string departmentID)
        //{
        //    //PÅLÄGGSKOSTNAD PER AVDELNING
        //    return GetEmployeeSallaryCostByDepartment(departmentID) + GetCalculatedDepartmentSchablonCost(departmentID) + GetCalculatedDirectActivityCost();
        //}

        //private decimal GetCalculatedTotalAFFOCost()
        //{
        //    //PRODUKTIONSKOSTNAD FÖR AO FO AVDELNINGAR
        //    return DepartmentList.Where(department => department.DepartmentID.Equals("AO") || department.DepartmentID.Equals("FO"))
        //        .Sum(department => GetAdditionCostByDepartment(department.DepartmentID));
        //}

        //public decimal GetAddition()
        //{
        //    //RETURNERAR PÅLÄGGET
        //    return GetCalculatedTotalAFFOCost() / GetCalculatedTotalProductionCost();
        //}

        private int GetRevenueByProduct(string productID)
        {
            //RETURNERAR INTÄKT PER PRODUKT
            return ForecastingManagement.Instance.GetIncomeByProduct(productID);
        }

        public int GetCalculatedTotalRevenue()
        {
            //RETURNERAR INTÄKT PÅ ALLA PRODUKTER
            return ProductList.Sum(product => GetRevenueByProduct(product.ProductID));
        }

        public int GetProductGroupInmcomeByID(string groupID)
        {
            // RETURNERAR INKOMST PER PRODUKTGRUPP
            decimal sum = 0;
            ProductGroup calculateGroup = new ProductGroup();
            calculateGroup = ProductGroupManagement.Instance.GetProductGroupByID(groupID);
            foreach (Product p in calculateGroup.Product)
            { 
                sum += GetRevenueByProduct(p.ProductID);
            }
            return (int)sum;
        }

        public int GetProductGroupCostByID(string groupID)
        {
            // RETURNERAR KOSTNAD PER PRODUKTGRUPP
            decimal sum = 0;
            ProductGroup calculateGroup = new ProductGroup();
            calculateGroup = ProductGroupManagement.Instance.GetProductGroupByID(groupID);
            foreach (Product p in calculateGroup.Product)
            {
                sum += GetDirectProductCostByProductID(p.ProductID);
            }
            return (int)sum;
        }

        public int GetProductionDepartmentCostByID(string departmentID)
        {
            //Schablonkostnad
            int sCost = (int)GetTotalCalculatedSchablonCost();
            
            decimal aCost = 0; //AnnualCost

            //Årsarbetare för produkter
            aCost += GetAnnualEmployeeAtProductByDepartment("DO");
            aCost += GetAnnualEmployeeAtProductByDepartment("UF");

            //Årsarbetare för aktiviteter
            aCost += GetAnnualEmployeeAtActivityByDepartment("AO");
            aCost += GetAnnualEmployeeAtActivityByDepartment("FO");

            // Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            int sum = sCost / (int)aCost;

            // summan + lönekostnaden för vald avdelning
            sum += GetEmployeeSallaryCostByDepartment(departmentID);

            return sum;

        }

        //public int GetAFFODepartmentCostByID(string departmentID)
        //{ 
        //}

        //public int GetProductionDepartmentIncomeByID(string departmentID)
        //{ 
        //}

        //public int GetAFFODepartmentCostByID(string departmentID)
        //{ 
        //}

        //public int DecideDepartmentGroup(string departmentID)
        //{ //SWITCH SATSEN FÅR NOG VARA DÄR UTRÄKNINGEN VISAS IST
        //    int sum = 0;
        //    switch (departmentID)
        //    {
        //        case "AO":

        //            break;
        //        case "DO":

        //            break;
        //        case "UF":

        //            break;
        //        case "FO":

        //            break;
        //    }
        //}
    }
}
