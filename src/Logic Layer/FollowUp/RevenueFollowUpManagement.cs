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
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();

                    if (!String.IsNullOrEmpty(row))
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
        }

        public void FillGeneralFollowUpsWithProducts()
        {
            GeneralFollowUps.Clear();

            foreach (var item in db.CostProduct)
            {
                GeneralFollowUp gfu = new GeneralFollowUp
                {
                    ObjectID = item.CeProductID,
                    ObjectName = item.CeProductName,
                    Date = item.CeIncomeDate
                };

                GeneralFollowUp gfuToRemove = null;

                foreach (var tempGFU in GeneralFollowUps
                    .Where(tempGFU => tempGFU.ObjectID.Equals(gfu.ObjectID) && tempGFU.Date <= gfu.Date))
                {
                    gfuToRemove = tempGFU;
                }

                if (gfuToRemove != null)
                    GeneralFollowUps.Remove(gfuToRemove);

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
                        gfu.Date = cp.CeIncomeDate;

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
                        gfu.Date = cp.CeIncomeDate;

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

            DateTime date = DateTime.ParseExact("20000101", "yyyyMMdd", CultureInfo.InvariantCulture);

            foreach (var item in products)
            {
                gfu.Costs += item.CeAmount;

                if (item.CeIncomeDate > date)
                {
                    date = item.CeIncomeDate;
                }
            }
            gfu.ObjectName = "Företag";
            gfu.ObjectID = "FÖ";
            gfu.Date = date;

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

            var products = from p in CostProducts
                           where p.CeProductID.Equals(inGFU.ObjectID)
                           orderby p.CeIncomeDate descending
                           select p;

            CostProduct product = null;

            var pg = from p in ProductManagement.Instance.Products
                     where p.ProductGroupID.Equals(inGFU.ObjectID)
                     select p;

            var pd = from p in ProductManagement.Instance.Products
                     where p.DepartmentID.Equals(inGFU.ObjectID)
                     select p;

            var cps = from cp in CostProducts
                      orderby cp.CeIncomeDate descending
                      select cp;

            Product productToLookFor = null;


            switch (cpo)
            {
                case CostProductOption.Product:

                    if (products.Any())
                        product = products.First(s => s.CeProductID.Equals(inGFU.ObjectID));

                    if (product != null)
                    {
                        var revenues = from c in db.IncomeProductCustomer
                                       where c.IeProductID.Equals(product.CeProductID)
                                       orderby c.IeAmount ascending
                                       select c;

                        var costs = from r in db.CostProduct
                                    where r.CeProductID.Equals(product.CeProductID)
                                    orderby r.CeAmount descending
                                    select r;

                        int revenue = 0, cost = 0;

                        if (revenues.Any())
                        {
                            var p = revenues.First();
                            revenue = ~p.IeAmount + 1;
                        }

                        if (costs.Any())
                        {
                            var re = costs.First();
                            cost = re.CeAmount;
                        }

                        gfu.ObjectName = product.CeProductName;
                        gfu.Costs = cost;
                        gfu.Revenues = revenue;
                        gfu.Result = gfu.Revenues - gfu.Costs;
                        gfu.Date = inGFU.Date;
                    }
                    break;

                case CostProductOption.Productgroup:


                    foreach (var p in pg.Where(p => p.ProductGroupID.Equals(inGFU.ObjectID)))
                    {
                        productToLookFor = p;
                    }

                    if (cps.Any() && productToLookFor != null)
                        gfu.Date = cps.First(s => s.CeProductID.Equals(productToLookFor.ProductID)).CeIncomeDate;

                    gfu.Revenues = GetProductGroupRevenues(productToLookFor.ProductID);
                    gfu.Costs = GetProductGroupCosts(productToLookFor.ProductID);

                    var productGroup = db.ProductGroup.Single(p => p.ProductGroupID.Equals(inGFU.ObjectID));
                    gfu.ObjectName = productGroup.ProductGroupName;

                    gfu.Result = gfu.Revenues - gfu.Costs;

                    break;

                case CostProductOption.Department:

                    foreach (var p in from p in pd.Where(p => p.DepartmentID.Equals(inGFU.ObjectID))
                                      from item in cps
                                      where item.CeProductID.Equals(p.ProductID)
                                      select p)
                    {
                        productToLookFor = p;
                    }

                    if (cps.Any() && productToLookFor != null)
                        gfu.Date = cps.First(s => s.CeProductID.Equals(productToLookFor.ProductID)).CeIncomeDate;


                    gfu.Costs = GetProductGroupCosts(productToLookFor.ProductID);
                    gfu.Revenues = GetProductGroupRevenues(productToLookFor.ProductID);

                    var department = db.Department.First(p => p.DepartmentID.Equals(inGFU.ObjectID));
                    gfu.ObjectName = department.DepartmentName;

                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;

                case CostProductOption.Company:
                    gfu.ObjectName = "IT-Service";


                    foreach (var item in db.CostProduct)
                    {
                        gfu.Costs += item.CeAmount;
                    }


                    foreach (var item in db.IncomeProductCustomer)
                    {
                        gfu.Revenues += ~item.IeAmount +1;
                    }

                    gfu.Date = inGFU.Date;
                    gfu.Result = gfu.Revenues - gfu.Costs;
                    break;
            }

            return gfu;
        }

        private int GetProductGroupCosts(string id)
        {
            var costs = from r in db.CostProduct
                        where r.CeProductID.Equals(id)
                        orderby r.CeAmount descending
                        select r;

            return Enumerable.Sum(costs, item => item.CeAmount);
        }

        private int GetProductGroupRevenues(string id)
        {
            var revenues = from c in db.IncomeProductCustomer
                           where c.IeProductID.Equals(id)
                           orderby c.IeAmount ascending
                           select c;

            int sum = Enumerable.Sum(revenues, item => item.IeAmount);
            return ~sum + 1;
        }
    }
}
