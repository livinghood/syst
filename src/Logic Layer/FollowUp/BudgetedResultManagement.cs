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

        public IEnumerable<FinancialIncome> FinancialIncomeList
        {
            get { return RevenueManagement.Instance.FinancialIncomeList; }
        }

        public IEnumerable<Product> ProductList { get; set; }

        public IEnumerable<Activity> ActivityList { get; set; }

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
            EmployeeList = new ObservableCollection<Employee>();
            ProductPlacementList = new ObservableCollection<ProductPlacement>();
            ActivityPlacementList = new ObservableCollection<ActivityPlacement>();
            ProductList = new ObservableCollection<Product>(ProductManagement.Instance.GetProducts());
            ActivityList = new ObservableCollection<Activity>(ActivityManagement.Instance.GetActivities());
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
            GeneralFollowUp gfu = new GeneralFollowUp { ObjectName = "IT-Service" };
            GeneralFollowUps.Add(gfu);
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
                    gfu.Revenues = GetCalculatedIncomeBudgetByProduct(objectID);

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Productgroup:
                    var productGroup = db.ProductGroup.Single(p => p.ProductGroupID.Equals(objectID));
                    gfu.ObjectName = productGroup.ProductGroupName;

                    gfu.Costs = GetProductGroupCostByID(objectID);
                    gfu.Revenues = GetCalculatedIncomeBudgetByGroup(objectID);

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Department:
                    var department = db.Department.Single(p => p.DepartmentID.Equals(objectID));
                    gfu.ObjectName = department.DepartmentName;

                    if (objectID == "DA" || objectID == "UF")
                    {
                        gfu.Costs = GetProductionDepartmentCostByDepartmentID(objectID);
                        gfu.Revenues = GetCalculatedIncomeBudgetProductByDepartment(objectID);
                    }
                    if (objectID == "AO" || objectID == "FO")
                    {
                        gfu.Costs = GetAFFODepartmentCostByDepartmentID(objectID);
                        gfu.Revenues = 0;
                    }

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Company:
                    gfu.ObjectName = "IT-Service";

                    gfu.Costs = (int)GetTotalCost();
                    gfu.Revenues = GetCalculatedTotalIncomeBudget();

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;
            }

            return gfu;
        }

        public decimal GetCalculatedDepartmentSchablonCost(string departmentID)
        {
            //SCHABLONSKOSTNADER PER AVDELNING
            if (departmentID.Equals("DA") || departmentID.Equals("UF"))
            {
                return Cost_Budgeting_Logic.DCPPDManagement.Instance.GetAccountsByProductDepartment(departmentID)
                    .Where(account => account.AccountID != 5021 && account.AccountID != 9999)
                    .Select(account => account.AccountCost == null ? 0 : (int)account.AccountCost)
                    .Aggregate<int, decimal>(0, (current, tempCost) => current + tempCost);
            }
            if (departmentID.Equals("AO") || departmentID.Equals("FO"))
            {
                return Cost_Budgeting_Logic.DCPPDManagement.Instance.GetAccountsByActivityDepartment(departmentID)
                    .Where(account => account.AccountID != 5021 && account.AccountID != 9999)
                    .Select(account => account.AccountCost == null ? 0 : (int)account.AccountCost)
                    .Aggregate<int, decimal>(0, (current, tempCost) => current + tempCost);
            }
            else
                return 0;
        }

        public int GetTotalCalculatedSchablonCost()
        {//SCHABLONSKOSTNADER
            int sum = 0;
            foreach (Account acc in AccountManagement.Instance.Accounts)
            {
                if (acc.AccountID.Equals(5021) || acc.AccountID.Equals(9999))
                    sum += 0;
                else
                {
                    if (acc.AccountCost == null)
                        sum += 0;
                    else
                        sum += (int)acc.AccountCost;
                }
            }
            
            // DIREKTA KOSTNADER FÖR ALLA PRODUKTER
            decimal directProdCost = ProductList.Aggregate<Product, decimal>(0, (current, product) => current + (int)GetDirectProductCostByProductID(product.ProductID));
            // DIREKTA KOSTNADER FÖR ALLA AKTIVITETER
            decimal directActCost = ActivityList.Aggregate<Activity, decimal>(0, (current, activity) => current + (int)GetDirectActivityCostByActivityID(activity.ActivityID));

            return sum -= (int)(directProdCost + directActCost);
        }

        public double GetAnnualEmployeeAtProductByDepartment(string departmentID)
        {
            // ÅRSARBETARE VIA PRODUKT PER AVDELNING
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (ProductPlacement productPlacement in EmployeeList.Select(employee =>
                new ObservableCollection<ProductPlacement>(ProductManagement.Instance.GetProductPlacementsByEmployee(employee))).SelectMany(tempPlacements => tempPlacements))
            {
                // MÅSTE RÄKNA IHOP PER PRODUKT
                ProductPlacementList.Add(productPlacement);
            }
            double totalAnnualProduct = 0;
            if (ProductPlacementList.Any())
            {
                totalAnnualProduct =
                    (from product in ProductList
                     from pp in ProductPlacementList
                     where product.ProductID.Equals(pp.ProductID)
                     select pp).Aggregate<ProductPlacement, double>(0, (current, pp) => current + (int)pp.ProductAllocate);
                totalAnnualProduct /= 100;
            }
            return totalAnnualProduct;
        }

        public double GetAnnualEmployeeAtActivityByDepartment(string departmentID)
        {
            // ÅRSARBETARE VIA AKTIVITET PER AVDELNING
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            foreach (ActivityPlacement activityPlacement in EmployeeList.Select(employee =>
                new ObservableCollection<ActivityPlacement>(ActivityManagement.Instance.GetActivityPlacementsByEmployee(employee))).SelectMany(tempPlacements => tempPlacements))
            {
                // MÅSTE RÄKNA IHOP PER AKTIVITET
                ActivityPlacementList.Add(activityPlacement);
            }
            double totalAnnualActivity =
                (from activity in ActivityList
                 from ap in ActivityPlacementList
                 where activity.ActivityID.Equals(ap.ActivityID)
                 select ap).Aggregate<ActivityPlacement, double>(0, (current, ap) => current + (int)ap.ActivityAllocate);
            return totalAnnualActivity / 100;
        }

        public int GetEmployeeSallaryCostByDepartment(string departmentID)
        {
            // ANSTÄLLDAS LÖNEKOSTNADER PER AVDELNING
            EmployeeList = new ObservableCollection<Employee>(EmployeeManagement.Instance.GetEmployeeAtributes(departmentID));
            return EmployeeList.Sum(employee => employee.MonthSallary * (employee.AnnualRate / 100));
        }

        public decimal GetDirectProductCostByProductID(string productID)
        {
            //DIREKTA KOSTNADER FÖR PRODUKT
            return Cost_Budgeting_Logic.DCPPDManagement.Instance.DirectProductCosts
                .Where(dp => dp.Product.ProductID.Equals(productID)).Aggregate<DirectProductCost, decimal>(0, (current, dp) => current + dp.ProductCost);
        }

        public decimal GetDirectActivityCostByActivityID(string activityID)
        {
            //DIREKT KOSTNAD FÖR AKTIVITET
            return Cost_Budgeting_Logic.DCPADManagement.Instance.DirectActivityCosts
                .Where(dp => dp.Activity.ActivityID.Equals(activityID)).Aggregate<DirectActivityCost, decimal>(0, (current, dp) => current + dp.ActivityCost);
        }

        public int GetRevenueByProduct(string productID)
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
            ProductGroup calculateGroup = ProductGroupManagement.Instance.GetProductGroupByID(groupID);
            return (int)calculateGroup.Product.Aggregate<Product, decimal>(0, (current, p) => current + GetRevenueByProduct(p.ProductID));
        }

        public int GetProductGroupCostByID(string groupID)
        {
            // RETURNERAR KOSTNAD PER PRODUKTGRUPP
            return (int)ProductManagement.Instance.GetProductsByProductGroup(groupID).Cast<Product>().Sum(p => GetDirectProductCostByProductID(p.ProductID));
        }

        public double GetSchablonMultipliedAnnual(string departmentID)
        {
            //Schablonkostnad
            double sCost = (int)GetTotalCalculatedSchablonCost();

            double aCost = 0; //AnnualCost

            //Årsarbetare för produkter
            if (departmentID.Equals("DA") || departmentID.Equals("UF"))
                aCost += GetAnnualEmployeeAtProductByDepartment(departmentID);
            //Årsarbetare för aktiviteter
            if (departmentID.Equals("AO") || departmentID.Equals("FO"))
                aCost += GetAnnualEmployeeAtActivityByDepartment(departmentID);

            // Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            if (aCost == 0)
                return sCost;
            if (sCost == 0)
                return 0;
            return sCost * aCost;
        }

        public int GetProductionDepartmentCostByDepartmentID(string departmentID)
        {
            // Schablonskostnad totalt * årsarbetare totalt (ej konto 5021)
            double sum = (double)GetSchablonMultipliedAnnual(departmentID);

            // summan + lönekostnaden för vald avdelning
            sum += GetEmployeeSallaryCostByDepartment(departmentID);

            // summan + direkta kostnader för varje produkt för vald avdelning
            sum += (double)ProductManagement.Instance.GetProductsByDepartment(departmentID).Cast<Product>().Sum(product => GetDirectProductCostByProductID(product.ProductID));

            return (int)sum;
        }

        public int GetAFFODepartmentCostByDepartmentID(string departmentID)
        {
            // Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            double sum = GetSchablonMultipliedAnnual(departmentID); 

            // summan + lönekostnaden för vald avdelning
            sum += GetEmployeeSallaryCostByDepartment(departmentID);

            // summan + direkta kostnader för varje produkt för vald avdelning
            sum += (double)ActivityManagement.Instance.GetActivitiesByDepartment(departmentID).Sum(activity => GetDirectActivityCostByActivityID(activity.ActivityID));

            return (int)sum;
        }

        public decimal GetTB(decimal production)
        {
            // LÖNEKOSTNAD PÅ AVDELNINGEN
            decimal sum = GetEmployeeSallaryCostByDepartment("AO");
            sum += GetEmployeeSallaryCostByDepartment("FO");

            // SUMMERAR MED SCHABLONSKOSTADER FRÅN KONTONA 5025-8571
            var list = from a in AccountList
                       where a.AccountID > 5024
                       where a.AccountID < 8572
                       select a;
            // SUMMERAR KOSTNADEN PÅ KONTONA    
            sum += list.Where(a => a.AccountCost != null).Aggregate(sum, (current, a) => current + (int)a.AccountCost);

            // SUMMERAR ALLA DIREKTA KOSTNADER PÅ AKTIVITETER
            sum += ActivityList.Aggregate(sum, (current, a) => current + (int)GetDirectActivityCostByActivityID(a.ActivityID));

            Account tempAcc = AccountList.Where(a => a.AccountID.Equals(9999)).FirstOrDefault();

            if (tempAcc.AccountCost == null)
                tempAcc.AccountCost = 0;

            sum += (int)tempAcc.AccountCost;

            sum /= production;

            return sum;
        }

        public int GetProductionDepartmentIncomeByID(string departmentID)
        {
            //HÄMTAR INKOMST FÖR VARJE PRODUKT SOM TILLHÖR VALD AVDELNING
            var list = ProductManagement.Instance.GetProductsByDepartment(departmentID);
            return list.Cast<Product>().Sum(product => GetRevenueByProduct(product.ProductID));
        }

        public decimal GetTotalCost()
        {
            double schablon = 0;
            decimal tillVCost = 0;
            decimal tb = 0;
            decimal totalCost = 0;
            double annual = 0;

            // LÖNER FÖR ALLA AVDELNINGAR
            decimal sallary = DepartmentList.Aggregate<Department, decimal>(0, (current, department) => current + GetEmployeeSallaryCostByDepartment(department.DepartmentID));

            // SCHABLONSKOSTNADER FÖR ALLA KONTON
            schablon += (double)GetTotalCalculatedSchablonCost();

            // ÅRSARBETARE 
            annual += GetAnnualEmployeeAtProductByDepartment("DA");
            annual += GetAnnualEmployeeAtProductByDepartment("UF");

            // DIREKTA KOSTNADER FÖR ALLA PRODUKTER
            decimal directProdCost = ProductList.Aggregate<Product, decimal>(0, (current, product) => current + (int)GetDirectProductCostByProductID(product.ProductID));

            decimal d_schablon = (decimal)(schablon * annual);

            // SUMMERAR IHOP FÖR TOTALA TILLVERKNINGSKOSTNADEN
            tillVCost = sallary + d_schablon + directProdCost;

            // BERÄKNAR TB BELOPPET
            decimal d_tb = GetTB(tillVCost);

            tb = d_tb * tillVCost;

            // SUMMERAR IHOP TB MED TILLVERKNINGSKOSTNADEN FÖR TOTALKOSTNAD
            totalCost = tillVCost + tb;

            return totalCost;

        }

        public int GetCalculatedTotalIncomeBudget()
        {
            // SUMMERAR BUDGETTEN FRÅN ALLA INKOMSTBUDGETTAR
            int sum = 0;
            foreach (FinancialIncome fi in FinancialIncomeList)
            {
                if (fi.Budget != null)
                    sum += (int)fi.Budget;
            }

            return sum;
        }

        public int GetCalculatedIncomeBudgetProductByDepartment(string departmentID)
        {
            // HÄMTAR ALLA BUDGETTERADE INTÄKTE UTIFRÅN AVDELNING
            int sum = 0;
            foreach (Product p in ProductList)
            {
                if (p.DepartmentID.Equals(departmentID))
                {
                    sum += GetCalculatedIncomeBudgetByProduct(p.ProductID);
                }
            }
            return sum;
        }

        public int GetCalculatedIncomeBudgetByGroup(string groupID)
        {
            // HÄMTAR ALLA BUDGETTERADE INTÄKTE UTIFRÅN PRODUKTGRUPP
            int sum = 0;
            foreach (Product p in ProductList)
            {
                if (p.ProductGroupID.Equals(groupID))
                {
                    sum += GetCalculatedIncomeBudgetByProduct(p.ProductID);
                }
            }
            return sum;
        }

        public int GetCalculatedIncomeBudgetByProduct(string productID)
        {
            // HÄMTAR ALLA BUDGETTERADE INTÄKTE UTIFRÅN PRODUKT
            int sum = 0;
            foreach (FinancialIncome fi in FinancialIncomeList)
            {
                if (fi.ProductID.Equals(productID))
                {
                    if (fi.Budget != null)
                        sum += (int)fi.Budget;
                }
            }
            return sum;
        }

    }
}
