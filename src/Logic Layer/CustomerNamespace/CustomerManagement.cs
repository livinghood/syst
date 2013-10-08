using System;
using System.Collections.Generic;
using System.Linq;
using Database_Layer;

namespace Logic_Layer.CustomerNamespace
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
        /// The instance property
        /// </summary>
        public static CustomerManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// CustomerRepository object for handling data
        /// </summary>
        private readonly IRepository<Customer> objCustomerRepository = new Repository<Customer>();

        /// <summary>
        /// Get a list of all customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomers()
        {
            return from c in objCustomerRepository.FindAll() select c;
        }

        /// <summary>
        /// Create a new customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="category"></param>
        public void CreateCustomer(string id, string name, CustomerCategories category)
        {
            int customerId = Convert.ToInt32(id);
            Customer customer = new Customer {CustomerID = customerId, CustomerName = name, CustomerCategory = category};
            Repository<Customer> repository = new Repository<Customer>();
            repository.Add(customer);
        }
    }
}
