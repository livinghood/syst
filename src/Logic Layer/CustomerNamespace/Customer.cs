using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace Logic_Layer.CustomerNamespace
{
    /// <summary>
    /// Defines a customer
    /// </summary>
    [Table(Name = "Customer")]
    public class Customer
    {
        // Primary key must be specified
        [Column(IsPrimaryKey=true)]
        public string CustomerID { get; set; }

        [Column]
        public string CustomerName { get; set; }

        // Enum types needs to be saved as a nvarchar
        [Column(DbType="NVarChar(10)")]
        public CustomerCategories CustomerCategory { get; set; }

    }
}
