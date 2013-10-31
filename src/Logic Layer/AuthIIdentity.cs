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
        // Add private fields to store the user name and a value that indicates 
        // if the user is authenticated
        private bool authenticatedValue;
        private string nameValue;

        // Methods

        /// <summary>
        /// Initializes the class by authenticating the user and then setting the 
        /// user name and role, based on a name and a password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public AuthIIdentity(string name, string password)
        {
            this.UserAccount = UserManagement.Instance.GetUserAccountByPassword(name, password);

            if (UserAccount != null)
            {
                this.nameValue = UserAccount.UserName;
                this.authenticatedValue = true;
            }
        }
        // Properties

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
        public bool IsAuthenticated
        {
            get
            {
                return this.authenticatedValue;
            }
        }

        /// <summary>
        /// Returns the user's name
        /// </summary>
        public string Name
        {
            get
            {
                return this.nameValue;
            }
        }

        /// <summary>
        /// Gets the type of authentication used
        /// </summary>
        string System.Security.Principal.IIdentity.AuthenticationType
        {
            get
            {
                return "Custom Authentication";
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated
        /// </summary>
        bool System.Security.Principal.IIdentity.IsAuthenticated
        {
            get
            {
                return this.authenticatedValue;
            }
        }

        // Gets the name of the current user
        string System.Security.Principal.IIdentity.Name
        {
            get
            {
                return this.nameValue;
            }
        }
    }
}
 
 
