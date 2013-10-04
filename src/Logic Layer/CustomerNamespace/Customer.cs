using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.CustomerNamespace
{
    /// <summary>
    /// A customer belongs to a category, which is either "näringsliv" or "offentlig"
    /// </summary>
    public enum CustomerCategorys
    {
        Näringsliv,
        Offentlig
    }

    /// <summary>
    /// Class that defines a customer
    /// </summary>
    public class Customer
    {
        public int CustomerID { get; set; }
        public CustomerCategorys CustomerCategory { get; set; }
        public string CustomerName { get; set; }

        /// <summary>
        /// Constructor for creating a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="customerCategory"></param>
        /// <param name="customerName"></param>
        public Customer(int customerId, CustomerCategorys customerCategory, string customerName)
        {
            CustomerID = customerId;
            CustomerCategory = customerCategory;
            CustomerName = customerName;
        }
    }
}
