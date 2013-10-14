using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    /// <summary>
    /// Class for management of products
    /// </summary>
    public class ProductManagement
    {
        public ObservableCollection<Product> Products { get; set; }

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
        /// Constructor with initialization of products list
        /// </summary>
        public ProductManagement()
        {
            Products = new ObservableCollection<Product>(GetProducts());
        }

        /// <summary>
        /// Get a list of all products
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetProducts()
        {
            var products = from p in db.Product
                           orderby p.ProductName
                           select p;
            return products;
        }

        public IEnumerable<string> GetProductDepartments()
        {
            var departments = from d in db.Department
                           orderby d.DepartmentID 
                              where d.DepartmentID == "DA" 
                              || d.DepartmentID == "UF"

                           select d.DepartmentID;
            return departments;
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        public void CreateProduct(string id, string name, string departmentID, ProductGroup group)
        {
            Product newProduct = new Product { ProductID = id, ProductName = name, ProductGroupID = group.ProductGroupID, DepartmentID = departmentID };
            db.Product.Add(newProduct);
            db.SaveChanges();
            Products.Add(newProduct);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        public void DeleteProduct(Product product)
        {
            db.Product.Remove(product);
            db.SaveChanges();
            Products.Remove(product);
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
