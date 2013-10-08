using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Collections;
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
        IRepository<CustomerNamespace.Customer> objCustomerRepository = new Repository<CustomerNamespace.Customer>();
        
        /// <summary>
        /// The instance property
        /// </summary>
        public static CustomerManager Instance
        {
            get { return instance.Value; }
        }

        public IEnumerable<CustomerNamespace.Customer> getCustomers()
        {
            return from c in objCustomerRepository.FindAll() select c;
        }
    }
}
