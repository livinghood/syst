#region Using Directive
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Principal;
#endregion

namespace Logic_Layer
{
    /// <summary>
    /// Implements the IPrincipal nterface which defines the basic functionality
    /// of a principal object
    /// </summary>
    public class AuthIPrincipal : IPrincipal
    {
        // Fields

        /// <summary>
        /// To store the identity associated with this principal
        /// </summary>
        private AuthIIdentity identityValue;

        // Methods

        /// <summary>
        /// Initializes the class with a new instance of SampleIIdentity given a 
        /// user name and password
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public AuthIPrincipal(string name, string password)
        {
            this.identityValue = new AuthIIdentity(name, password);
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public bool IsInRole(string auth)
        {
            return (auth == identityValue.Role.ToString(CultureInfo.InvariantCulture));
        }

        // Properties

        /// <summary>
        /// Returns the user identity of the current principal
        /// </summary>
        public IIdentity Identity
        {
            get
            {
                return this.identityValue;
            }
        }
    }
}