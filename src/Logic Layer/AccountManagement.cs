using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic_Layer
{
    /// <summary>
    /// Management class for Accounts
    /// </summary>
    public class AccountManagement
    {
        /// <summary>
        /// Lazy Instance of AccountManagement singelton
        /// </summary>
        private static readonly Lazy<AccountManagement> instance = new Lazy<AccountManagement>(() => new AccountManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static AccountManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Get a list of all accounts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Account> GetAccounts()
        {
            IEnumerable<Account> accounts = from a in db.Account
                                              orderby a.AccountID
                                              select a;

            return accounts;
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        /// <param name="account"></param>
        public void CreateAccount(Account account)
        {
            db.Account.Add(account);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="account"></param>
        public void DeleteAccount(Account account)
        {
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
    }
}
