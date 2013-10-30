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
            return db.UserAccount.OrderBy(u => u.EmployeeID);
        }

        /// <summary>
        /// Add created account to database
        /// </summary>
        /// <param name="useraccount"></param>
        public void AddAccount(UserAccount useraccount)
        {
            UserAccounts.Add(useraccount);
            db.UserAccount.Add(useraccount);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete an account
        /// </summary>
        /// <param name="useraccount"></param>
        public void DeleteUserAccount(UserAccount useraccount)
        {
            UserAccounts.Remove(useraccount);
            db.UserAccount.Remove(useraccount);
            db.SaveChanges();
        }

        /// <summary>
        /// Update the database
        /// </summary>
        public void UpdateUserAccount()
        {
            db.SaveChanges();
        }

        public void ResetUser(UserAccount useraccount)
        {
            db.Entry(useraccount).State = EntityState.Unchanged;
        }

        /// <summary>
        /// Check if a specific user exists
        /// </summary>
        public bool UserNameExist(string id)
        {
            return db.UserAccount.Any(u => u.UserName == id);
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
