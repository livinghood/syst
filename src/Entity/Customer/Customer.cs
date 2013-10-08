﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace Common
{
    [Table(Name = "Customer")]
    public class Customer
    {
        [Column]
        public int CustomerID { get; set; }

        [Column]
        public string CustomerName { get; set; }

        public CustomerCategories CustomerCategory { get; set; }

    }

}