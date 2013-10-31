using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Logic_Layer
{
    /// <summary>
    /// Management class for Accounts
    /// </summary>
    public class AccountManagement
    {
        public ObservableCollection<Account> Accounts { get; set; }

        /// <summary>
        /// Lazy Instance of AccountManagement singelton
        /// </summary>
        private static readonly Lazy<AccountManagement> instance = new Lazy<AccountManagement>(() => new AccountManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static AccountManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Constructor with initialization of accounts list
        /// </summary>
        private AccountManagement()
        {
            Accounts = new ObservableCollection<Account>(GetAccounts());
        }

        /// <summary>
        /// Get a list of all accounts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Account> GetAccounts()
        {
            return db.Account.OrderBy(a => a.AccountID);
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        /// <param name="account"></param>
        public void CreateAccount(Account account)
        {
            Accounts.Add(account);
            db.Account.Add(account);
            db.SaveChanges();
        }

        /// <summary>
        /// Check if a specific customer exists
        /// </summary>
        public bool AccountExist(int id)
        {
            return db.Account.Any(a => a.AccountID == id);
        }
        
        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="account"></param>
        public void DeleteAccount(Account account)
        {
            Accounts.Remove(account);
            db.Account.Remove(account);
            db.SaveChanges();
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public void UpdateAccount()
        {
            db.SaveChanges();
        }

        public void ResetAccount(Account accountObj)
        {
            db.Entry(accountObj).State = EntityState.Unchanged;
        }

        public bool IsConnectedToDirectCost(Account accountObj)
        {
            var query = db.DirectProductCost.Where(d => d.AccountID.Equals(accountObj.AccountID));
            return query.Any();
        }

    }
}
