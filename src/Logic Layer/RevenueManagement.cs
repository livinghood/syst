using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Data.Entity;

namespace Logic_Layer
{
    public class RevenueManagement
    {
        /// <summary>
        /// Lazy Instance of CustomerManager singelton
        /// </summary>
        private static readonly Lazy<RevenueManagement> instance = new Lazy<RevenueManagement>(() => new RevenueManagement());

        /// <summary>
        /// Gets the List of Customers so we can use them in calculations
        /// </summary>
        public IEnumerable<Customer> CustomerList
        {
            get { return CustomerManagement.Instance.Customers; }
        }

        /// <summary>
        /// Gets the List of Products so we can use them in calculations
        /// </summary>
        public IEnumerable<Product> ProductList
        {
            get { return ProductManagement.Instance.Products; }
        }

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// List of FinancialIncomes
        /// </summary>
        public ObservableCollection<FinancialIncome> FinancialIncomeList { get; set; }

        /// <summary>
        /// FinancialIncomeYearID
        /// </summary>
        public string NewID
        {
            get { return "FIY" + DateTime.Today.Year; }
        }

        public FinancialIncomeYear CurrentFinancialIncomeYear { get; set; }

        /// <summary>
        /// The instance property
        /// </summary>
        public static RevenueManagement Instance
        {
            get { return instance.Value; }
        }

        RevenueManagement()
        {
            FinancialIncomeList = new ObservableCollection<FinancialIncome>(GetFinancialIncomeByYear());
        }

        /// <summary>
        /// Create a new FinancialIncome
        /// </summary>
        public FinancialIncome CreateFinancialIncome()
        {
            FinancialIncome newFinancialIncome = new FinancialIncome();
            FinancialIncomeList.Add(newFinancialIncome);
            db.FinancialIncome.Add(newFinancialIncome);
            db.SaveChanges();
            return newFinancialIncome;
        }
        /// <summary>
        /// Get a list of all Financial Incomes by customers
        /// </summary>
        /// <param name="customerid"></param>
        /// <returns></returns>
        public IEnumerable<FinancialIncome> GetFinancialIncomesByCustomer(string customerid)
        {
            IEnumerable<FinancialIncome> financialIncomes = from f in db.FinancialIncome
                                                            orderby f.ProductID
                                                            where f.CustomerID == customerid
                                                            select f;

            foreach (FinancialIncome fi in financialIncomes)
            {
                fi.ProductName = fi.Product.ProductName;
            }

            return financialIncomes;
        }
        /// <summary>
        /// Get a list of all Financial Incomes by products
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public IEnumerable<FinancialIncome> GetFinancialIncomesByProduct(string productid)
        {
            IEnumerable<FinancialIncome> financialIncomes = from f in db.FinancialIncome
                                                            orderby f.CustomerID
                                                            where f.ProductID == productid
                                                            select f;
            if (financialIncomes.Any())
            {
                foreach (FinancialIncome fi in financialIncomes)
                {
                    fi.CustomerName = fi.Customer.CustomerName;
                }
            }

            return financialIncomes;

        }

        /// <summary>
        /// Get a list of all FinancialIncomes current year
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FinancialIncome> GetFinancialIncomeByYear()
        {
            IEnumerable<FinancialIncome> financialIncomes = from f in db.FinancialIncome
                                                            orderby f.FinancialIncomeYearID
                                                            where f.FinancialIncomeYearID == NewID
                                                            select f;

            foreach (FinancialIncome fi in financialIncomes)
            {
                fi.ProductName = fi.Product.ProductName;
            }

            return financialIncomes;
        }

        /// <summary>
        /// Adds financial income to db
        /// </summary>
        /// <param name="fiObj"></param>
        public void AddIncome(FinancialIncome fiObj)
        {
            fiObj.FinancialIncomeYearID = NewID;
            db.FinancialIncome.Add(fiObj);
        }

        /// <summary>
        /// Delete a FinancialIncome
        /// </summary>
        /// <param name="fI"></param>
        public void DeleteFinancialIncome(FinancialIncome fI)
        {
            FinancialIncomeList.Remove(fI);
            //Om det inte är sparat i databasen än, bara sparat i listan
            db.FinancialIncome.Remove(fI);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a FinancialIncome
        /// </summary>
        public void UpdateFinancialIncome()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Prevent empty financialincomes from being saved to db
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FinancialIncome> RemoveEmptyCustomerIncomes()
        {
            ObservableCollection<FinancialIncome> tempIncome = new ObservableCollection<FinancialIncome>(FinancialIncomeList);
            foreach (FinancialIncome fi in FinancialIncomeList.Where(fi => fi.ProductID == null))
            {
                tempIncome.Remove(fi);
                db.FinancialIncome.Remove(fi);
            }
            return tempIncome;
        }

        /// <summary>
        /// Prevent empty financialincomes from being saved to db
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<FinancialIncome> RemoveEmptyProductIncomes()
        {
            ObservableCollection<FinancialIncome> tempIncome = new ObservableCollection<FinancialIncome>(FinancialIncomeList);
            foreach (FinancialIncome fi in FinancialIncomeList.Where(fi => fi.CustomerID == null))
            {
                tempIncome.Remove(fi);
                db.FinancialIncome.Remove(fi);
            }
            return tempIncome;
        }

        public void ResetFinancialIncome(FinancialIncome ppObj)
        {
            db.Entry(ppObj).State = EntityState.Unchanged;
        }


        //-----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Create a new FinancialIncomeYear
        /// </summary>
        /// <param name="fIYID"></param>
        /// <returns></returns>
        public FinancialIncomeYear CreateFinancialIncomeYear()
        {
            FinancialIncomeYear newFinancialIncomeYear = new FinancialIncomeYear();

            if (!db.FinancialIncomeYear.Any(f => f.FinancialIncomeYearID == NewID))
            {
                newFinancialIncomeYear.FinancialIncomeYearID = NewID;
                newFinancialIncomeYear.FinancialIncomeLock = false;
                db.FinancialIncomeYear.Add(newFinancialIncomeYear);
                return newFinancialIncomeYear;
            }
            return db.FinancialIncomeYear.Single(f => f.FinancialIncomeYearID == NewID);
        }

        /// <summary>
        /// Save to db
        /// </summary>
        public void UpdateFinancialIncomeYear()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Exports revenue budget to text file
        /// </summary>
        /// <param name="fileName"></param>
        public void ExportRevenueBudgetingToTextFile(string fileName)
        {
            // StreamWriter is put in 'using' statement for automatic disposal once finished
            using (var writer = new StreamWriter(fileName))
            {
                // First line in textfile makes a header
                writer.WriteLine("{0};{1};{2};{3};{4};{5};{6};", "Konto", "Ansvar", "ProduktID", "Produkt", "KundID", "Kund", "Belopp");

                foreach (string str in from row
                                       in FinancialIncomeList
                                       let department = GetDepartmentForPrinting(row.ProductID)
                                       let account = GetAccountForPrinting(row.ProductID)
                                       let amount = GetAmountForPrinting(row.ProductID)
                                       select String.Format("{0};{1};{2};{3};{4};{5};{6};",
                                           account, department,
                                           row.ProductID,
                                           row.Product.ProductName,
                                           row.CustomerID,
                                           row.Customer.CustomerName,
                                           amount))
                {
                    writer.WriteLine(str);
                }
            }
        }

        /// <summary>
        /// Returns amount used when printing to file
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        private string GetAmountForPrinting(string productID)
        {
            var firstOrDefault = db.FinancialIncome.FirstOrDefault(a => a.ProductID.Equals(productID));

            int value = (int) firstOrDefault.Budget*-1;

            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns department used when printing to file
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        private string GetDepartmentForPrinting(string productID)
        {
            var departments = db.Product.Select(d => d);
            var firstOrDefault = departments.FirstOrDefault(d => d.ProductID.Equals(productID));
            return firstOrDefault != null ? firstOrDefault.Department.DepartmentName : "";
        }

        /// <summary>
        /// Returns account used when printing to file
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        private string GetAccountForPrinting(string productID)
        {
            var accounts = db.DirectProductCost.Select(d => d);
            var firstOrDefault = accounts.FirstOrDefault(d => d.ProductID.Equals(productID));
            return firstOrDefault != null
                ? firstOrDefault.AccountID.ToString(CultureInfo.InvariantCulture) : "";
        }
    }
}
