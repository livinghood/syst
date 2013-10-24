﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.Cost_Budgeting_Logic
{
    public class DCPPDManagement
    {
        /// <summary>
        /// Lazy Instance of DCPPDManagement singelton
        /// </summary>
        private static readonly Lazy<DCPPDManagement> instance = new Lazy<DCPPDManagement>(() => new DCPPDManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static DCPPDManagement Instance
        {
            get { return instance.Value; }
        }

        public ObservableCollection<DirectProductCost> DirectProductCosts { get; set; }

        /// <summary>
        /// Constructor with inilization of list with DirectProductCosts
        /// </summary>
        public DCPPDManagement()
        {
            DirectProductCosts = new ObservableCollection<DirectProductCost>(GetDirectProductCosts());
        }

        /// <summary>
        /// Get a list of all DirectProductCosts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DirectProductCost> GetDirectProductCosts()
        {
            return db.DirectProductCost.OrderBy(d => d.ExpenseBudgetID);
        }

        /// <summary>
        /// Create a new DirectProductCost
        /// </summary>
        /// <param name="dpc"></param>
        public void CreateDirectProductCosts(DirectProductCost dpc)
        {
            DirectProductCosts.Add(dpc);
            db.DirectProductCost.Add(dpc);
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
        /// Get a list of DirectProductCosts connected to selected account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IEnumerable<DirectProductCost> GetAccounts(Account account)
        {
            return from u in db.DirectProductCost
                   where u.AccountID == account.AccountID
                   select u;
        }

        /// <summary>
        /// Method to check if user attempts to add a product that is already connected to the selected account
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool CheckIfProductConnected(string productID)
        {
            return DirectProductCosts.Any
                (directProductCost => directProductCost.ProductID.Equals(productID));
        }

        /// <summary>
        /// Save the edits in a datagrid cell
        /// </summary>
        /// <param name="objToAdd"></param>
        /// <param name="account"></param>
        public void SaveEditing(DirectProductCost objToAdd, Account account)
        {
            ExpenseBudget eb = null;

            if (!ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist())
            {
                eb = new ExpenseBudget
                {
                    ExpenseBudgetID = ExpenseBudgetManagement.Instance.GetExpenseBudgetID(),
                    ProductionLock = 0,
                    SellLock = 0
                };

                ExpenseBudgetManagement.Instance.Create(eb);
                objToAdd.ExpenseBudgetID = eb.ExpenseBudgetID;
                objToAdd.AccountID = account.AccountID;
                CreateDirectProductCosts(objToAdd);
            }
            else
            {
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
        }

        /// <summary>
        /// Almost the same as SaveEditing, only this method is used when a new product 
        /// was connected to an account
        /// </summary>
        /// <param name="objToAdd"></param>
        /// <param name="account"></param>
        public void SaveNewProduct(DirectProductCost objToAdd, Account account)
        {
            ExpenseBudget eb = null;

            if (!ExpenseBudgetManagement.Instance.DoesExpenseBudgetExist())
            {
                eb = new ExpenseBudget
                {
                    ExpenseBudgetID = ExpenseBudgetManagement.Instance.GetExpenseBudgetID(),
                    ProductionLock = 0,
                    SellLock = 0
                };

                ExpenseBudgetManagement.Instance.Create(eb);
            }
            else
            {
                int id = ExpenseBudgetManagement.Instance.GetExpenseBudgetID();

                var listOfExpenseBudgets = ExpenseBudgetManagement.Instance.GetExpenseBudgets();

                foreach (var expenseBudget in listOfExpenseBudgets.Where(expenseBudget => expenseBudget.ExpenseBudgetID.Equals(id)))
                {
                    eb = expenseBudget;
                }
            }
            objToAdd.ExpenseBudgetID = eb.ExpenseBudgetID;
            objToAdd.AccountID = account.AccountID;
            objToAdd.ProductCost = 0;
            CreateDirectProductCosts(objToAdd);
        }

        public string CalculateSum(Account acc)
        {
            var query = GetAccounts(acc);
            int sum = Enumerable.Sum(query, directProductCost => directProductCost.ProductCost);
            return sum.ToString(CultureInfo.InvariantCulture);
        }
    }
}
