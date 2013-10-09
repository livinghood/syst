using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic_Layer
{
    /// <summary>
    /// Management class for customers
    /// </summary>
    public class CustomerManagement
    {
        /// <summary>
        /// Lazy Instance of CustomerManager singelton
        /// </summary>
        private static readonly Lazy<CustomerManagement> instance = new Lazy<CustomerManagement>(() => new CustomerManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static CustomerManagement Instance
        {
            get { return instance.Value; }
        }


        /// <summary>
        /// Get a list of all customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomers()
        {
            IEnumerable<Customer> customers = from c in db.Customer
                                             orderby c.CustomerName
                                             select c;

            return customers;
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        public void CreateCustomer(string id, string name, CustomerCategories category)
        {
            Customer newCustomer = new Customer { CustomerID = id, CustomerName = name, CustomerCategory = category.ToString() };
            db.Customer.Add(newCustomer);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteCustomer(Customer customer)
        {
            db.Customer.Remove(customer);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a customer
        /// </summary>
        /// <param name="customer"></param>
        public void UpdateCustomer()
        {
            db.SaveChanges();
        }
    }
}
