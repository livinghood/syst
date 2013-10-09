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
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        public void CreateAccount(Account account)
        {
            db.Account.Add(account);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a account
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteAccount(Account account)
        {
            db.Account.Remove(account);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a account
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateAccount()
        {
            db.SaveChanges();
        }
    }
}
