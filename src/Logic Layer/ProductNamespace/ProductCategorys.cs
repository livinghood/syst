using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic_Layer.ProductNamespace
{
    /// <summary>
    /// Defines a Product category
    /// </summary>
    public class ProductCategorys
    {
        public string ProductCategoryName { get; set; }
        public int ProductCategoryID { get; set; }

        /// <summary>
        /// Constructor with two parameters
        /// </summary>
        /// <param name="productCategoryName"></param>
        /// <param name="productCategoryId"></param>
        public ProductCategorys(string productCategoryName, int productCategoryId)
        {
            ProductCategoryName = productCategoryName;
            ProductCategoryID = productCategoryId;
        }
    }
}
