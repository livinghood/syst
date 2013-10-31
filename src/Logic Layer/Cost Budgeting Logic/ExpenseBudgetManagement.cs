using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.Cost_Budgeting_Logic
{
    public class ExpenseBudgetManagement
    {
        /// <summary>
        /// Lazy Instance of ExpenseBudgetManagement singelton
        /// </summary>
        private static readonly Lazy<ExpenseBudgetManagement> instance = new Lazy<ExpenseBudgetManagement>(() => new ExpenseBudgetManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static ExpenseBudgetManagement Instance
        {
            get { return instance.Value; }
        }

        public ObservableCollection<ExpenseBudget> ExpenseBudgets { get; set; }

        /// <summary>
        /// Constructor with initialization of expense budgets list
        /// </summary>
        ExpenseBudgetManagement()
        {
            ExpenseBudgets = new ObservableCollection<ExpenseBudget>(GetExpenseBudgets());
        }

        /// <summary>
        /// Get a list of all expense budgets
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ExpenseBudget> GetExpenseBudgets()
        {
            return db.ExpenseBudget.OrderBy(c => c.ExpenseBudgetID);
        }

        /// <summary>
        /// Returns the current year as ID
        /// </summary>
        /// <returns></returns>
        public int GetExpenseBudgetID()
        {
            return DateTime.Now.Year;
        }

        /// <summary>
        /// Creates a new expense budget
        /// </summary>
        /// <param name="eb"></param>
        public void Create(ExpenseBudget eb)
        {
            eb.SellLock = 10000;
            eb.ProductionLock = 10000;
            db.ExpenseBudget.Add(eb);
            db.SaveChanges();
        }

        /// <summary>
        /// Checks if an expense budget exists in the database
        /// </summary>
        /// <returns></returns>
        public bool DoesExpenseBudgetExist()
        {
            int id = GetExpenseBudgetID();
            var query = db.ExpenseBudget.Where(p => p.ExpenseBudgetID.Equals(id));
            return query.Any();
        }

        /// <summary>
        /// Method to lock an expense budget
        /// </summary>
        /// <returns></returns>
        public bool LockExpenseBudget()
        {
            int id = GetExpenseBudgetID();
            var expensbudget = db.ExpenseBudget.FirstOrDefault(e => e.ExpenseBudgetID.Equals(id));

            // Found the expense budget
            if (expensbudget != null)
            {
                // The first digit in production lock indicates whether DCPPD is locked or not
                expensbudget.ProductionLock += 100;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool LockAnnualExpenseBudget(string departmentID)
        {
            int id = GetExpenseBudgetID();
            var expensbudget = db.ExpenseBudget.FirstOrDefault(e => e.ExpenseBudgetID.Equals(id));

            // Found the expense budget
            if (expensbudget != null)
            {
                switch (departmentID)
                {
                    case "AO":
                        expensbudget.SellLock += 1;
                        break;
                    case "DA":
                        expensbudget.SellLock += 10;
                        break;
                    case "FO":
                        expensbudget.SellLock += 100;
                        break;
                    case "UF":
                        expensbudget.SellLock += 1000;
                        break;
                }
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public int IsExpenseBudgetLocked()
        {
            int id = GetExpenseBudgetID();
            return db.ExpenseBudget.FirstOrDefault(e => e.ExpenseBudgetID.Equals(id)).ProductionLock;
        }

        public bool IsAnnualExpenseBudgetLocked(string departmentID)
        {
            int id = GetExpenseBudgetID();
            var expensbudget = db.ExpenseBudget.FirstOrDefault(e => e.ExpenseBudgetID.Equals(id));

            // Found the expense budget
            if (expensbudget != null)
            {
                string s_id = expensbudget.SellLock.ToString();
                switch (departmentID)
                {
                    case "AO":
                        if (s_id[0].Equals("1"))
                            return true;
                        break;
                    case "DA":
                        if (s_id[1].Equals("1"))
                            return true;
                        break;
                    case "FO":
                        if (s_id[2].Equals("1"))
                            return true;
                        break;
                    case "UF":
                        if (s_id[3].Equals("1"))
                            return true;
                        break;
                }
            }
            return false;
        }

    }
}
