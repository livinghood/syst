using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.AccountNamespace
{
    /// <summary>
    /// Defines an account
    /// </summary>
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int AccountCost { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountName"></param>
        /// <param name="accountCost"></param>
        public Account(int accountId, string accountName, int accountCost)
        {
            AccountID = accountId;
            AccountName = accountName;
            AccountCost = accountCost;
        }
    }
}
