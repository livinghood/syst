using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Logic_Layer
{
    /// <summary>
    /// Handle user accounts
    /// </summary>
    public class UserManagement
    {
        /// <summary>
        /// Lazy Instance of UserManagement Singelton
        /// </summary>
        private static readonly Lazy<UserManagement> instance = new Lazy<UserManagement>(() => new UserManagement());

        /// <summary>
        /// The instance property
        /// </summary>
        /// <remarks></remarks>
        public static UserManagement Instance
        {
            get { return instance.Value; }
        }

        public ObservableCollection<UserAccount> UserAccounts { get; set; }     

        public ObservableCollection<UserPermissionLevels> PermissionLevels { get; set; }

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// Constructor with initialization of UserAccounts list
        /// </summary>
        UserManagement()
        {
            UserAccounts = new ObservableCollection<UserAccount>(GetUserAccounts());
        }

        /// <summary>
        /// Get the list of accounts from database
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserAccount> GetUserAccounts()
        {
            IEnumerable<UserAccount> accounts = from u in db.UserAccount
                                            orderby u.EmployeeID
                                            select u;
            return accounts;
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        public void CreateUserAccount(string userName, string password, int permissionLevel)
        {
            UserAccount account = new UserAccount
            {
                UserName = userName,
                Password = password,
                PermissionLevel = permissionLevel
            };

            UserAccounts.Add(account);
            db.UserAccount.Add(account);
            db.SaveChanges();
        }

        public void AddAccount(UserAccount useraccount)
        {
            UserAccounts.Add(useraccount);
            db.UserAccount.Add(useraccount);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="account"></param>
        public void DeleteUserAccount(UserAccount account)
        {
            UserAccounts.Remove(account);
            db.UserAccount.Remove(account);
            db.SaveChanges();
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public void UpdateUserAccount()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a list of user permission levels
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<UserPermissionLevels> GetPermissionLevels()
        {
            return new ObservableCollection<UserPermissionLevels>(Enum.GetValues(typeof(UserPermissionLevels)).Cast<UserPermissionLevels>());   
        }
    }
}
