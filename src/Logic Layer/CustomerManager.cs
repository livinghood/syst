using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Collections;
using Common;
using Database_Layer;

namespace Logic_Layer
{
    public class CustomerManager
    {
        /// <summary>
        /// Lazy Instance of CustomerManager singelton
        /// </summary>
        private static readonly Lazy<CustomerManager> instance = new Lazy<CustomerManager>(() => new CustomerManager());

        /// <summary>
        /// CustomerRepository object for handling data
        /// </summary>
        CustomerRepository objCustomerRepository = new CustomerRepository();
        
        /// <summary>
        /// The instance property
        /// </summary>
        public static CustomerManager Instance
        {
            get { return instance.Value; }
        }

        public IEnumerable<Customer> getCustomers()
        {
            return from c in objCustomerRepository.getCustomers() select c;
        }
        public IEnumerable<Customer> getCustomer(int id)
        {
            return from c in objCustomerRepository.getCustomers() where c.CustomerID == id select c;
        }
    }
}
