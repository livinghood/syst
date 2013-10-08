using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic_Layer.EmployeeNamespace;

namespace Logic_Layer.UserNamespace
{
    /// <summary>
    /// Defines a user
    /// </summary>
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int PermissionLevel { get; set; }
        public Employee Employee { get; set; }
    }
}
