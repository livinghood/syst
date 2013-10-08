using System;
using System.Collections.Generic;
using System.Linq;
using Database_Layer;

namespace Logic_Layer.CustomerNamespace
{
    public class CustomerManagement
    {
        /// <summary>
        /// Lazy Instance of CustomerManager singelton
        /// </summary>
        private static readonly Lazy<CustomerManagement> instance = new Lazy<CustomerManagement>(() => new CustomerManagement());

        /// <summary>
        /// CustomerRepository object for handling data
        /// </summary>
        private readonly IRepository<Customer> objCustomerRepository = new Repository<Customer>();
        
        /// <summary>
        /// The instance property
        /// </summary>
        public static CustomerManagement Instance
        {
            get { return instance.Value; }
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return from c in objCustomerRepository.FindAll() select c;
        }
    }
}
