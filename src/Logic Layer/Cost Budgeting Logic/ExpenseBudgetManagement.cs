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

        public int GetExpenseBudgetID()
        {
            return DateTime.Now.Year;
        }

        public void Create(ExpenseBudget eb)
        {
            db.ExpenseBudget.Add(eb);
            db.SaveChanges();
        }

        public void Update()
        {
            db.SaveChanges();
        }

        public bool DoesExpenseBudgetExist()
        {
            int id = GetExpenseBudgetID();
            var query = from p in db.ExpenseBudget
                        where p.ExpenseBudgetID.Equals(id)
                        select p;
            return query.Any();
        }
    }
}
