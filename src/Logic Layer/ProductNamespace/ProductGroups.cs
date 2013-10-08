using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic_Layer.ProductNamespace
{
    /// <summary>
    /// Defines a Product group
    /// </summary>
    public class ProductGroups
    {
        public string ProductGroupName { get; set; }
        public string ProductGroupID { get; set; }
        public ProductCategorys ProductCategory { get; set; }

        /// <summary>
        /// Constructor with two parameters
        /// </summary>
        /// <param name="productGroupName"></param>
        /// <param name="productGroupId"></param>
        /// <param name="productCategory"></param>
        public ProductGroups(string productGroupName, string productGroupId, ProductCategorys productCategory)
        {
            ProductGroupName = productGroupName;
            ProductGroupID = productGroupId;
            ProductCategory = productCategory;
        }
    }
}
