using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    public class ProductManagement
    {
        public ProductGroup ProductGroup { get; set; }
        /// <summary>
        /// Lazy Instance of ProductManagement singelton
        /// </summary>
        private static readonly Lazy<ProductManagement> instance = new Lazy<ProductManagement>(() => new ProductManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static ProductManagement Instance
        {
            get { return instance.Value; }
        }


        /// <summary>
        /// Get a list of all products
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProducts()
        {
            IEnumerable<Product> products = from p in db.Product
                                              orderby p.ProductName
                                              select p;

            return products;
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        public void CreateProduct(string id, string name, ProductGroup productGroup , Department department)
        {
            Product newProduct = new Product {ProductID = id, ProductName = name, ProductGroupID = productGroup.ProductGroupID, DepartmentID = department.DepartmentID };
            db.Product.Add(newProduct);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        public void DeleteProduct(Product product)
        {
            db.Product.Remove(product);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a product
        /// </summary>
        public void UpdateProduct()
        {
            db.SaveChanges();
        }
    }
}
