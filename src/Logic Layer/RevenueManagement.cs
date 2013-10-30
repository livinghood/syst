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
        public ObservableCollection<Customer> CustomerList
        {
            get { return CustomerManagement.Instance.Customers; }
        }

        /// <summary>
        /// Gets the List of Products so we can use them in calculations
        /// </summary>
        public ObservableCollection<Product> ProductList
        {
            get { return ProductManagement.Instance.Products; }
        }

        /// <summary>
        /// Database context
        /// </summary>
        private DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// List of FinancialIncomes
        /// </summary>
        public ObservableCollection<FinancialIncome> FinancialIncomeList
        {
            get;
            set;
        }

        public string NewID
        {
            get { return "FIY" + DateTime.Today.Year.ToString(); }
        }

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
            FinancialIncome newFinancialIncome = new FinancialIncome { };
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
            // set fiObj.FinancialIncomeYear
            db.FinancialIncome.Add(fiObj);
        }

        /// <summary>
        /// Delete a FinancialIncome
        /// </summary>
        /// <param name="fI"></param>
        public void DeleteFinancialIncome(FinancialIncome fI)
        {
            FinancialIncomeList.Remove(fI);
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

        //-----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Get a list of all FinancialIncomeYears
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FinancialIncomeYear> GetFinancialIncomeYears(FinancialIncomeYear fIY)
        {
            IEnumerable<FinancialIncomeYear> financialIncomesyears = from f in db.FinancialIncomeYear
                                                                     where f.FinancialIncomeYearID == fIY.FinancialIncomeYearID
                                                                     select f;


            return financialIncomesyears;
        }

        /// <summary>
        /// Create a new FinancialIncomeYear
        /// </summary>
        /// <param name="fIYID"></param>
        /// <returns></returns>
        public string CreateFinancialIncomeYear()
        {
            if (db.FinancialIncomeYear.Where(f => f.FinancialIncomeYearID == NewID).Any())
                UpdateFinancialIncomeYear();
            else
            {
                FinancialIncomeYear newFinancialIncomeYear = new FinancialIncomeYear { FinancialIncomeYearID = NewID, FinancialIncomeLock = false };
                db.FinancialIncomeYear.Add(newFinancialIncomeYear);
                db.SaveChanges();
            }
            return NewID;
        }

        /// <summary>
        /// Delete FinancialIncomeYear from database
        /// </summary>
        /// <param name="fIY"></param>
        public void DeleteFinancialIncomeYear(FinancialIncomeYear fIY)
        {
            db.FinancialIncomeYear.Remove(fIY);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a FinancialIncomeYear
        /// </summary>
        public void UpdateFinancialIncomeYear()
        {
            db.SaveChanges();
        }

    }
}
