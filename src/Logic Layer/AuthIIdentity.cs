#region Using Directive
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Principal;
using System.Security.Cryptography;
#endregion

namespace Logic_Layer
{
    /// <summary>
    /// Implements the IIdentity Interface which defines the basic functionality
    /// of an identity object
    /// </summary>
    public class AuthIIdentity : IIdentity
    {
        // Fields
        public UserAccount UserAccount { get; set; }
        private int roleValue;
        // Add private fields to store the user name and a value that indicates 
        // if the user is authenticated

        // Methods

        /// <summary>
        /// Initializes the class by authenticating the user and then setting the 
        /// user name and role, based on a name and a password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public AuthIIdentity(string name, string password)
        {
            if (String.IsNullOrWhiteSpace(name) && String.IsNullOrWhiteSpace(password))
            {
                this.Name = "Allmän";
                this.IsAuthenticated = true;
                this.roleValue = 99;
            }
            else
            {
                this.UserAccount = UserManagement.Instance.GetUserAccountByPassword(name, password);

                if (UserAccount != null)
                {
                    this.Name = UserAccount.UserName;
                    this.IsAuthenticated = true;
                    this.roleValue = UserAccount.PermissionLevel;
                }
            }
        }

        /// <summary>
        /// Returns the user's role
        /// </summary>
        public int Role
        {
            get
            {
                return this.roleValue;
            }
        }

        /// <summary>
        /// Returns a string that indicates the current authentication mechanism
        /// </summary>
        public string AuthenticationType
        {
            get
            {
                return "Custom Authentication";
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the user has been authenticated
        /// </summary>
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Returns the user's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the type of authentication used
        /// </summary>
        string IIdentity.AuthenticationType
        {
            get
            {
                return "Custom Authentication";
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated
        /// </summary>
        bool IIdentity.IsAuthenticated
        {
            get
            {
                return this.IsAuthenticated;
            }
        }

        // Gets the name of the current user
        string IIdentity.Name
        {
            get
            {
                return this.Name;
            }
        }
    }
}
 
 
