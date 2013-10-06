using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Configuration;
using Common;

namespace Database_Layer
{
    public class CustomerRepository
    {

        public Table<Customer> getCustomers()
        {
            DataContext db = new DataContext(ConfigurationManager.ConnectionStrings["DatabaseConnectionString"].ConnectionString);
            Table<Customer> Customers = db.GetTable<Customer>();
            return Customers;
        }
    }
}
