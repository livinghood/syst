using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<Customer> Customers { get; set; }

        /// <summary>
        /// Constructor with initialization of UserAccounts list
        /// </summary>
        CustomerManagement()
        {
            Customers = new ObservableCollection<Customer>(GetCustomers());
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
        /// Get a specific customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomer(string id)
        {
            IEnumerable<Customer> customers = from c in db.Customer
                                              where c.CustomerID.Contains(id)
                                             select c;

            return customers;
        }


        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        public void CreateCustomer(Customer customer)
        {
            Customers.Add(customer);
            db.Customer.Add(customer);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer"></param>
        public void DeleteCustomer(Customer customer)
        {
            Customers.Remove(customer);
            db.Customer.Remove(customer);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a customer
        /// </summary>
        public void UpdateCustomer()
        {
            db.SaveChanges();
        }
    }
}
