using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    public class ProductCategoryManagement
    {
        /// <summary>
        /// Lazy Instance of ProductCategoryManagement singelton
        /// </summary>
        private static readonly Lazy<ProductCategoryManagement> instance = new Lazy<ProductCategoryManagement>(() => new ProductCategoryManagement());

        public ObservableCollection<ProductCategory> ProductCategories { get; set; } 

        /// <summary>
        /// The instance property
        /// </summary>
        public static ProductCategoryManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        public ProductCategoryManagement()
        {
            ProductCategories = new ObservableCollection<ProductCategory>(GetProductCategories());
        }

        /// <summary>
        /// Get a list of all product categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductCategory> GetProductCategories()
        {
            return db.ProductCategory.OrderBy(c => c.ProductCategoryID);
        }

        /// <summary>
        /// Add created product category to database
        /// </summary>
        /// <param name="pc"></param>
        public void AddProductCategory(ProductCategory pc)
        {
            ProductCategories.Add(pc);
            db.ProductCategory.Add(pc);
            db.SaveChanges();
        }

        /// <summary>
        /// Delete a product category
        /// </summary>
        public void DeleteProductCategory(ProductCategory category)
        {
            ProductCategories.Remove(category);
            db.ProductCategory.Remove(category);
            db.SaveChanges();
        }

        /// <summary>
        /// Update a product category
        /// </summary>
        public void UpdateProductCategory()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Check if a product category is connected to any product groups
        /// </summary>
        /// <param name="productCategory"></param>
        /// <returns></returns>
        public bool IsProductCategoryEmpty(ProductCategory productCategory)
        {
            var query = db.ProductGroup.Where(p => p.ProductCategoryID.Equals(productCategory.ProductCategoryID));
            return !query.Any();
        }
    }
}
