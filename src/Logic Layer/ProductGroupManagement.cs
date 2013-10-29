using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Logic_Layer
{
    public class ProductGroupManagement
    {

        public ObservableCollection<ProductGroup> ProductGroups { get; set; }

        /// <summary>
        /// Lazy Instance of CustomerManager singelton
        /// </summary>
        private static readonly Lazy<ProductGroupManagement> instance = new Lazy<ProductGroupManagement>(() => new ProductGroupManagement());

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        /// <summary>
        /// The instance property
        /// </summary>
        public static ProductGroupManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Constructor with initialization of ProductGroups list
        /// </summary>
        public ProductGroupManagement()
        {
            ProductGroups = new ObservableCollection<ProductGroup>(GetProductGroups());
        }

        /// <summary>
        /// Get a list of all product groups
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductGroup> GetProductGroups()
        {
            return db.ProductGroup.OrderBy(g => g.ProductGroupID);
        }


        /// <summary>
        /// Add created product group to database
        /// </summary>
        /// <param name="productGroup"></param>
        public void AddProductGroup(ProductGroup productGroup)
        {
            productGroup.ProductGroupID.ToUpper();
            db.ProductGroup.Add(productGroup);
            db.SaveChanges();
            ProductGroups.Add(productGroup);
        }


        /// <summary>
        /// Delete a customer
        /// </summary>
        public void DeleteProductGroup(ProductGroup group)
        {
            db.ProductGroup.Remove(group);
            db.SaveChanges();
            ProductGroups.Remove(group);
        }

        /// <summary>
        /// Update a product group
        /// </summary>
        public void UpdateProductGroup()
        {
            db.SaveChanges();
        }

        /// <summary>
        /// Check if a product group is connected to any products
        /// </summary>
        /// <param name="productGroup"></param>
        /// <returns></returns>
        public bool IsProductGroupEmpty(ProductGroup productGroup)
        {
            var query = db.Product.Where(p => p.ProductGroupID.Equals(productGroup.ProductGroupID));
            return !query.Any();
        }
        public void ResetProductGroup(ProductGroup productGroup)
        {
            db.Entry(productGroup).State = EntityState.Unchanged;
        }

        /// <summary>
        /// Check if a specific group exists
        /// </summary>
        public bool ProductGroupIDExist(string id)
        {
            return db.ProductGroup.Any(pg => pg.ProductGroupID == id);
        }

        /// <summary>
        /// Check if a specific group exists
        /// </summary>
        public bool ProductGroupNameExist(string name)
        {
            return db.ProductGroup.Any(pg => pg.ProductGroupName == name);
        }
        
    }
}
