using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.Cost_Budgeting_Logic
{
    public class DCPADManagement
    {
        /// <summary>
        /// Lazy Instance of DCPADManagement singelton
        /// </summary>
        private static readonly Lazy<DCPADManagement> instance = new Lazy<DCPADManagement>(() => new DCPADManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static DCPADManagement Instance
        {
            get { return instance.Value; }
        }

        public ObservableCollection<DirectActivityCost> DirectActivityCosts { get; set; }

        /// <summary>
        /// Constructor with inilization of list with DirectActivityCosts
        /// </summary>
        public DCPADManagement()
        {
            DirectActivityCosts = new ObservableCollection<DirectActivityCost>(GetDirectActivityCosts());
        }

        /// <summary>
        /// Get a list of all DirectActivityCosts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DirectActivityCost> GetDirectActivityCosts()
        {
            return db.DirectActivityCost.OrderBy(d => d.ExpenseBudgetID);
        }

        /// <summary>
        /// Create a new DirectActivityCosts
        /// </summary>
        /// <param name="dac"></param>
        public void CreateDirectActivityCosts(DirectActivityCost dac)
        {
            DirectActivityCosts.Add(dac);
            db.DirectActivityCost.Add(dac);
            db.SaveChanges();
        }

        /// <summary>
        /// Update database
        /// </summary>
        public void Update()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Get a list of DirectActivityCosts connected to selected account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<DirectActivityCost> GetAccounts(Account account)
        {
            return from u in db.DirectActivityCost
                   where u.AccountID == account.AccountID
                   select u;
        }

        /// <summary>
        /// Method to check if user attempts to add an activity that is already connected to the selected account
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        public bool CheckIfActivityConnected(string activityID)
        {
            return DirectActivityCosts.Any
                (directActivityCost => directActivityCost.ActivityID.Equals(activityID));
        }

        /// <summary>
        /// Save the edits in a datagrid cell
        /// </summary>
        /// <param name="objToAdd"></param>
        /// <param name="account"></param>
        public void SaveEditing(DirectActivityCost objToAdd, Account account)
        {
            ExpenseBudget eb = null;

            int id = ExpenseBudgetManagement.Instance.GetExpenseBudgetID();

            var listOfExpenseBudgets = ExpenseBudgetManagement.Instance.GetExpenseBudgets();

            foreach (var expenseBudget in listOfExpenseBudgets.Where(expenseBudget => expenseBudget.ExpenseBudgetID.Equals(id)))
            {
                eb = expenseBudget;
            }
            objToAdd.ExpenseBudgetID = eb.ExpenseBudgetID;
            objToAdd.AccountID = account.AccountID;
            Update();
        }

        /// <summary>
        /// Almost the same as SaveEditing, only this method is used when a new activity 
        /// was connected to an account
        /// </summary>
        /// <param name="objToAdd"></param>
        /// <param name="account"></param>
        public void SaveNewActivity(DirectActivityCost objToAdd, Account account)
        {
            ExpenseBudget eb = null;

            int id = ExpenseBudgetManagement.Instance.GetExpenseBudgetID();

            var listOfExpenseBudgets = ExpenseBudgetManagement.Instance.GetExpenseBudgets();

            foreach (var expenseBudget in listOfExpenseBudgets.Where(expenseBudget => expenseBudget.ExpenseBudgetID.Equals(id)))
            {
                eb = expenseBudget;
            }
            objToAdd.ExpenseBudgetID = eb.ExpenseBudgetID;
            objToAdd.AccountID = account.AccountID;
            objToAdd.ActivityCost = 0;
            CreateDirectActivityCosts(objToAdd);
        }

        public string CalculateSum(Account acc)
        {
            var query = GetAccounts(acc);
            int sum = Enumerable.Sum(query, directActivityCost => directActivityCost.ActivityCost);
            return sum.ToString(CultureInfo.InvariantCulture);
        }
    }
}
