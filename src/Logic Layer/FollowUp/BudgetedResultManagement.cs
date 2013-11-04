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
            EmployeeList = new ObservableCollection<Employee>();
            ProductPlacementList = new ObservableCollection<ProductPlacement>();
            ActivityPlacementList = new ObservableCollection<ActivityPlacement>();
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

            GeneralFollowUp gfu = new GeneralFollowUp();
            gfu.ObjectName = "IT-Service";
            GeneralFollowUps.Add(gfu);
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

                    if (objectID == "DO" || objectID == "UF")
                    {
                        gfu.Costs = GetProductionDepartmentCostByDepartmentID(objectID);
                        gfu.Revenues = GetProductionDepartmentIncomeByID(objectID);
                    }
                    if (objectID == "AO" || objectID == "FO")
                    {
                        gfu.Costs = GetAFFODepartmentCostByDepartmentID(objectID);
                        gfu.Revenues = 0;
                    }

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Company: // Var kommer företaget ifrån?
                    gfu.ObjectName = "IT-Service";

                    gfu.Costs = GetTotalCost();
                    gfu.Revenues = GetCalculatedTotalRevenue();

                    gfu.Result = gfu.Revenues - gfu.Costs;
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
            decimal totalAnnualProduct = 0;
            if (ProductPlacementList.Any())
            {
                totalAnnualProduct =
                    (from product in ProductList
                     from pp in ProductPlacementList
                     where product.ProductID.Equals(pp.ProductID)
                     select pp).Aggregate<ProductPlacement, decimal>(0, (current, pp) => current + (int)pp.ProductAllocate);
                totalAnnualProduct /= 100;
            }
            return totalAnnualProduct;
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

        private decimal GetDirectActivityCostByActivityID(string activityID)
        {
            //DIREKT KOSTNAD FÖR AKTIVITET
            return Cost_Budgeting_Logic.DCPADManagement.Instance.DirectActivityCosts
                .Where(dp => dp.Activity.ActivityID.Equals(activityID)).Aggregate<DirectActivityCost, decimal>(0, (current, dp) => current + dp.ActivityCost);
        }

        private int GetRevenueByProduct(string productID)
        {
            //RETURNERAR INTÄKT PER PRODUKT
            return ForecastingManagement.Instance.GetIncomeByProduct(productID);
        }

        private int GetCalculatedTotalRevenue()
        {
            //RETURNERAR INTÄKT PÅ ALLA PRODUKTER
            return ProductList.Sum(product => GetRevenueByProduct(product.ProductID));
        }

        private int GetProductGroupInmcomeByID(string groupID)
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

        private int GetProductGroupCostByID(string groupID)
        {
            // RETURNERAR KOSTNAD PER PRODUKTGRUPP
            decimal sum = 0;

            foreach (Product p in ProductManagement.Instance.GetProductsByProductGroup(groupID))
            {
                sum += GetDirectProductCostByProductID(p.ProductID);
            }
            return (int)sum;
        }

        private decimal GetSchablonDividedAnnual()
        {// Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            //Schablonkostnad
            decimal sCost = (int)GetTotalCalculatedSchablonCost();

            decimal aCost = 0; //AnnualCost

            //Årsarbetare för produkter
            aCost += GetAnnualEmployeeAtProductByDepartment("DO");
            aCost += GetAnnualEmployeeAtProductByDepartment("UF");

            //Årsarbetare för aktiviteter
            aCost += GetAnnualEmployeeAtActivityByDepartment("AO");
            aCost += GetAnnualEmployeeAtActivityByDepartment("FO");

            // Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            if (aCost == 0)
                return sCost;
            if (sCost == 0)
                return 0;
            else
                return sCost / aCost;
        }

        private int GetProductionDepartmentCostByDepartmentID(string departmentID)
        {
            // Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            decimal sum = GetSchablonDividedAnnual();

            // SUMMAN MULTIPLICERAT MED ÅRSARBETARE FÖR VALD AVDELNING
            sum *= GetAnnualEmployeeAtProductByDepartment(departmentID);

            // summan + lönekostnaden för vald avdelning
            sum += GetEmployeeSallaryCostByDepartment(departmentID);

            // summan + direkta kostnader för varje produkt för vald avdelning
            foreach (Product product in ProductManagement.Instance.GetProductsByDepartment(departmentID))
            {
                sum += GetDirectProductCostByProductID(product.ProductID);
            }

            return (int)sum;
        }

        private int GetAFFODepartmentCostByDepartmentID(string departmentID)
        {
            // Schablonskostnad totalt / årsarbetare totalt (ej konto 5021)
            decimal sum = GetSchablonDividedAnnual();

            // SUMMAN MULTIPLICERAT MED ÅRSARBETARE FÖR VALD AVDELNING
            sum *= GetAnnualEmployeeAtActivityByDepartment(departmentID);

            // summan + lönekostnaden för vald avdelning
            sum += GetEmployeeSallaryCostByDepartment(departmentID);

            // summan + direkta kostnader för varje produkt för vald avdelning
            foreach (Activity activity in ActivityManagement.Instance.GetActivitiesByDepartment(departmentID))
            {
                sum += GetDirectActivityCostByActivityID(activity.ActivityID);
            }

            return (int)sum;
        }

        private int GetTB()
        {
            // LÖNEKOSTNAD PÅ AVDELNINGEN
            int sum = (int)GetEmployeeSallaryCostByDepartment("AO");
            sum += (int)GetEmployeeSallaryCostByDepartment("FO");

            // SUMMERAR MED SCHABLONSKOSTADER FRÅN KONTONA 5025-8571
            var list = from a in AccountList
                        where a.AccountID > 5024
                        where a.AccountID < 8572
                        select a;
            // SUMMERAR KOSTNADEN PÅ KONTONA    
            foreach (Account a in list)
            {
                if (a.AccountCost == null)
                    sum += 0;
                else
                    sum += (int)a.AccountCost;
            }

            // SUMMERAR ALLA DIREKTA KOSTNADER PÅ AKTIVITETER TILL AVDELNINGEN
            foreach (Activity a in ActivityList)
            {
                sum += (int)GetDirectActivityCostByActivityID(a.ActivityID);
            }

            return sum;
        }

        private int GetAddition()
        {
            //RETURNERAR PÅLÄGGET

            int producion = 0;
            int tb = 0;

            // TILLVERKNINGSKOSTNAD
            producion += GetProductionDepartmentCostByDepartmentID("DA");
            producion += GetProductionDepartmentCostByDepartmentID("UF");

            //TB
            tb += GetTB();

            return tb / producion;
        }

        private int GetProductionDepartmentIncomeByID(string departmentID)
        {
            //HÄMTAR INKOMST FÖR VARJE PRODUKT SOM TILLHÖR VALD AVDELNING

            int sum = 0;

            var list = ProductManagement.Instance.GetProductsByDepartment(departmentID);
            foreach (Product product in list)
            {
                sum += GetRevenueByProduct(product.ProductID);
            }

            return sum;
        }

        public int GetTotalCost()
        {
            int sallary = 0;
            int schablon = 0;
            int directCost = 0;
            int tillVCost = 0;
            int tb = 0;
            int totalCost = 0;

            // LÖNER FÖR ALLA AVDELNINGAR
            foreach (Department department in DepartmentList)
            {
                sallary += GetEmployeeSallaryCostByDepartment(department.DepartmentID);
            }

            // SCHABLONSKOSTNADER FÖR ALLA KONTON
            schablon = (int)GetTotalCalculatedSchablonCost();

            // DIREKTA KOSTNADER FÖR ALLA PRODUKTER
            foreach (Product product in ProductList)
            {
                directCost = (int)GetDirectProductCostByProductID(product.ProductID);
            }

            // SUMMERAR IHOP FÖR TOTALA TILLVERKNINGSKOSTNADEN
            tillVCost = sallary + schablon + directCost;

            // BERÄKNAR TB BELOPPET
            tb = GetTB() * tillVCost;

            // SUMMERAR IHOP TB MED TILLVERKNINGSKOSTNADEN FÖR TOTALKOSTNAD
            totalCost = tillVCost + tb;

            return totalCost;
        }

    }
}
