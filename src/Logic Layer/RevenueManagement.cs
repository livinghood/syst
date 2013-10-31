using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

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

        public IEnumerable<FinancialIncome> GetFinancialIncomesByCustomer(string customerid)
        {
            IEnumerable<FinancialIncome> financialIncomes = from f in db.FinancialIncome
                                                            orderby f.ProductID
                                                            where f.CustomerID == customerid
                                                            select f;

            foreach (FinancialIncome fi in financialIncomes)
            {
                foreach (Product p in ProductList)
                {
                    if (fi.ProductID.Equals(p.ProductID))
                        fi.ProductName = p.ProductName;
                }
            }

            return financialIncomes;
        }

        public IEnumerable<FinancialIncome> GetFinancialIncomesByProduct(string productid)
        {
            IEnumerable<FinancialIncome> financialIncomes = from f in db.FinancialIncome
                                                            orderby f.CustomerID
                                                            where f.ProductID == productid
                                                            select f;

            foreach (FinancialIncome fi in financialIncomes)
            {
                foreach (Customer p in CustomerList)
                {
                    if (fi.CustomerID.Equals(p.CustomerID))
                        fi.CustomerName = p.CustomerName;
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
                foreach (Product p in ProductList)
                {
                    if (fi.ProductID.Equals(p.ProductID))
                        fi.ProductName = p.ProductName;
                }
            }

            return financialIncomes;
        }

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

        public void UpdateFinancialIncomeYear()
        {
            db.SaveChanges();
        }
    }
}
