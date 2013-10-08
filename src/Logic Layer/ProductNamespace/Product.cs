using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.ProductNamespace
{
    /// <summary>
    /// There are two departments in ProductDepartment, "Utveckling" and "Drift".
    /// </summary>
    public enum ProductDepartments
    {
        Utveckling,
        Drift
    }
    /// <summary>
    /// Class that defines a product.
    /// </summary>
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public ProductGroups ProductGroup { get; set; }
        public ProductCategorys ProductCategory { get; set; }
        public ProductDepartments ProductDeparment { get; set; }
        /// <summary>
        /// Constructor with five parameters
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="productName"></param>
        /// <param name="productGroup"></param>
        /// <param name="productCategory"></param>
        /// <param name="productDepartment"></param>
        public Product(int productID, string productName, ProductGroups productGroup, ProductCategorys productCategory,
            ProductDepartments productDepartment)
        {
            ProductID = productID;
            ProductName = productName;
            ProductGroup = productGroup;
            ProductCategory = productCategory;
            ProductDeparment = productDepartment;
        }
    }
}
