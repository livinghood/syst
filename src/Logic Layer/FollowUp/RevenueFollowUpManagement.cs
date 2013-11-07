using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic_Layer.FollowUp;

namespace Logic_Layer.FollowUp
{
    public class RevenueFollowUpManagement
    {
        public ObservableCollection<CostProduct> CostProducts { get; set; }

        public ObservableCollection<GeneralFollowUp> GeneralFollowUps { get; set; }

        /// <summary>
        /// Lazy Instance of RevenueFollowUpManagement singelton
        /// </summary>
        private static readonly Lazy<RevenueFollowUpManagement> instance = new Lazy<RevenueFollowUpManagement>(() => new RevenueFollowUpManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static RevenueFollowUpManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Constructor with initialization of accounts list
        /// </summary>
        private RevenueFollowUpManagement()
        {
            CostProducts = new ObservableCollection<CostProduct>(GetCostProducts());
            GeneralFollowUps = new ObservableCollection<GeneralFollowUp>();
        }

        /// <summary>
        /// Get a list of all accounts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CostProduct> GetCostProducts()
        {
            return db.CostProduct.OrderBy(a => a.CeProductID);
        }

        public void CreateCostProductFromFile(string fileName)
        {
            // First, delete all current CostProduct items in database
            db.CostProduct.RemoveRange(db.CostProduct);
            db.SaveChanges();

            using (var reader = new StreamReader(fileName, Encoding.Default))
            {
                // Ignore first row since it's a header
                reader.ReadLine();
                string row;
                while ((row = reader.ReadLine()) != null)
                {
                    /* KostnadProdukt.txt is formatted in such a way that there are up to three tabs separating each
                     * 'column' in the text file. If a row contains multiple tabs they are replaced with one. */
                    if (row.Contains("\t\t\t"))
                    {
                        row = row.Replace("\t\t\t", "\t");
                    }

                    if (row.Contains("\t\t"))
                    {
                        row = row.Replace("\t\t", "\t");
                    }

                    // At this point each column is only separated by one tab which makes it easy to read the file
                    string[] field = row.Split('\t');

                    CostProduct cp = new CostProduct
                    {
                        CeProductID = field[0],
                        CeProductName = field[1],
                        CeIncomeDate = DateTime.ParseExact(field[2], "yyyyMMdd", CultureInfo.InvariantCulture),
                        CeAmount = int.Parse(field[3]),
                    };

                    bool updated = false;
                    var costProductsInDB = db.CostProduct.Select(s => s);

                    // Update a costproduct if it already exists in db
                    foreach (var costProduct in costProductsInDB
                        .Where(costProduct => costProduct.CeProductID.Equals(cp.CeProductID)
                            && costProduct.CeIncomeDate.Equals(cp.CeIncomeDate)))
                    {
                        costProduct.CeAmount += cp.CeAmount;
                        db.SaveChanges();
                        updated = true;
                    }

                    if (!updated)
                    {
                        // Add cp to database
                        AddCostProduct(cp);
                    }
                }
            }
        }

        public void FillGeneralFollowUpsWithProducts()
        {
            GeneralFollowUps.Clear();

            foreach (var item in db.CostProduct)
            {
                GeneralFollowUp gfu = new GeneralFollowUp();
                gfu.ObjectID = item.CeProductID;
                gfu.ObjectName = item.CeProductName;
                gfu.Month = item.CeIncomeDate;

                GeneralFollowUp gfuToRemove = null;

                foreach (var tempGFU in GeneralFollowUps
                    .Where(tempGFU => tempGFU.ObjectID.Equals(gfu.ObjectID) && tempGFU.Month <= gfu.Month))
                {
                    gfuToRemove = tempGFU;
                }

                if (gfuToRemove != null)
                {
                    GeneralFollowUps.Remove(gfuToRemove);
                }
               
                GeneralFollowUps.Add(gfu);                   
            }
        }

        public void FillGeneralFollowUpsWithDepartments()
        {
            GeneralFollowUps.Clear();

            var tempDepartmentsAdded = new List<string>();

            var products = from d in db.Product
                           select d;

            var cps = from c in db.CostProduct
                      select c;

            foreach (var product in products)
            {
                foreach (var cp in cps)
                {
                    if (product.ProductID.Equals(cp.CeProductID))
                    {
                        GeneralFollowUp gfu = new GeneralFollowUp();
                        gfu.ObjectID = product.DepartmentID;
                        gfu.ObjectName = product.Department.DepartmentName;

                        // Prevent same department from being added more than once
                        if (!tempDepartmentsAdded.Contains(product.DepartmentID))
                        {
                            GeneralFollowUps.Add(gfu);
                        }
                        tempDepartmentsAdded.Add(product.DepartmentID);
                    }
                }
            }
        }

        public void FillGeneralFollowUpsWithProductGroups()
        {
            GeneralFollowUps.Clear();

            var tempGroupsAdded = new List<string>();

            var products = from d in db.Product
                           select d;

            var cps = from c in db.CostProduct
                      select c;

            foreach (var product in products)
            {
                foreach (var cp in cps)
                {
                    if (product.ProductID.Equals(cp.CeProductID))
                    {
                        GeneralFollowUp gfu = new GeneralFollowUp();
                        gfu.ObjectID = product.ProductGroupID;
                        gfu.ObjectName = product.ProductGroup.ProductGroupName;

                        // Prevent same department from being added more than once
                        if (!tempGroupsAdded.Contains(product.ProductGroupID))
                        {
                            GeneralFollowUps.Add(gfu);
                        }
                        tempGroupsAdded.Add(product.ProductGroupID);
                    }
                }
            }
        }

        public void FillGeneralFollowUpsWithCompany()
        {
            GeneralFollowUps.Clear();

            var products = from d in db.CostProduct
                           select d;

            GeneralFollowUp gfu = new GeneralFollowUp();

            foreach (var item in products)
            {
                gfu.Costs += item.CeAmount;
            }
            gfu.ObjectName = "Företag";
            gfu.ObjectID = "FÖ";
            GeneralFollowUps.Add(gfu);
        }

        private void AddCostProduct(CostProduct cp)
        {
            CostProducts.Add(cp);
            db.CostProduct.Add(cp);
            db.SaveChanges();
        }

        public GeneralFollowUp GetResults(CostProductOption cpo, GeneralFollowUp inGFU)
        {
            GeneralFollowUp gfu = new GeneralFollowUp();

            switch (cpo)
            {
                case CostProductOption.Product:
                    var products = from p in db.CostProduct
                                   where p.CeProductID.Equals(inGFU.ObjectID)
                                   orderby p.CeIncomeDate descending
                                   select p;

                    CostProduct product = null;
                    if (products.Any())
                        product = products.First(s => s.CeProductID.Equals(inGFU.ObjectID));

                    if (product != null)
                    {
                        gfu.ObjectName = product.CeProductName;
                        gfu.Costs = (int)BudgetedResultManagement.Instance.GetDirectProductCostByProductID(inGFU.ObjectID);
                        gfu.Revenues = BudgetedResultManagement.Instance.GetRevenueByProduct(inGFU.ObjectID);
                        gfu.Result = gfu.Revenues - gfu.Costs;
                        gfu.Month = inGFU.Month;
                    }
                    break;

                case CostProductOption.Productgroup:
                    var productGroup = db.ProductGroup.Single(p => p.ProductGroupID.Equals(inGFU.ObjectID));
                    gfu.ObjectName = productGroup.ProductGroupName;

                    gfu.Costs = BudgetedResultManagement.Instance.GetProductGroupCostByID(inGFU.ObjectID);
                    gfu.Revenues = BudgetedResultManagement.Instance.GetProductGroupInmcomeByID(inGFU.ObjectID);
                    gfu.Month = inGFU.Month;
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Department:
                    var department = db.Department.Single(p => p.DepartmentID.Equals(inGFU.ObjectID));
                    gfu.ObjectName = department.DepartmentName;

                    if (inGFU.ObjectID == "DO" || inGFU.ObjectID == "UF")
                    {
                        gfu.Costs = BudgetedResultManagement.Instance.GetProductionDepartmentCostByDepartmentID(inGFU.ObjectID);
                        gfu.Revenues = BudgetedResultManagement.Instance.GetProductionDepartmentIncomeByID(inGFU.ObjectID);
                    }
                    if (inGFU.ObjectID == "AO" || inGFU.ObjectID == "FO")
                    {
                        gfu.Costs = BudgetedResultManagement.Instance.GetAFFODepartmentCostByDepartmentID(inGFU.ObjectID);
                        gfu.Revenues = 0;
                    }
                    gfu.Month = inGFU.Month;
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Company:
                    gfu.ObjectName = "IT-Service";

                    gfu.Costs = (int)BudgetedResultManagement.Instance.GetTotalCost();
                    gfu.Revenues = BudgetedResultManagement.Instance.GetCalculatedTotalRevenue();
                    gfu.Month = inGFU.Month;
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;
            }

            return gfu;
        }
    }
}
