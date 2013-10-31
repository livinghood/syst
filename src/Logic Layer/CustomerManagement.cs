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
        private readonly DatabaseConnection db = new DatabaseConnection();

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
            return db.Customer.OrderBy(c => c.CustomerName);
        }

        /// <summary>
        /// Get a specific customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomer(string id)
        {
            return from c in db.Customer
                   where c.CustomerID.Contains(id)
                   select c;
        }

        /// <summary>
        /// Returns products in autocompletebox for ProductID
        /// </summary>
        /// <param name="s">written text in box for autocomplete</param>
        /// <returns></returns>
        public Customer GetCustomerByID(string s)
        {
            return Customers.FirstOrDefault(p => p.CustomerID.Equals(s));
        }

        /// <summary>
        /// Returns products in autocompletebox for ProductName
        /// </summary>
        /// <param name="s">written text in box for autocomplete</param>
        /// <returns></returns>
        public Customer GetCustomerByName(string s)
        {
            return Customers.FirstOrDefault(p => p.CustomerName.Equals(s));
        }

        /// <summary>
        /// Check if a specific customer exists
        /// </summary>
        public bool CustomerExist(string id)
        {
            return db.Customer.Any(c => c.CustomerID == id);
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
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

        public void ResetCustomer(Customer customerObj)
        {
            db.Entry(customerObj).State = EntityState.Unchanged;
        }
    }
}
