﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;


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
            return db.Product.OrderBy(p => p.ProductName);
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
        /// Add created product to databse
        /// </summary>
        /// <param name="product"></param>
        public void AddProduct(Product product)
        {
            Products.Add(product);
            db.Product.Add(product);
            db.SaveChanges();
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

        public void ResetProduct(Product productObj)
        {
            db.Entry(productObj).State = EntityState.Unchanged;
        }

        /// <summary>
        /// Check if a specific customer exists
        /// </summary>
        public bool ProductExist(string id)
        {
            return db.Product.Any(p => p.ProductID == id);
        }

        /// <summary>
        /// Method that returns a list of all non budgeted products
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetNonBudgetedProducts(string department)
        {
            var budgetedProducts = from p in db.FinancialIncome
                                   where p.Product.DepartmentID == department
                                   select p.ProductID;

            var allProducts = from p in db.Product
                              where p.DepartmentID == department
                              select p;

            return allProducts.Where(product => !budgetedProducts.Contains(product.ProductID));
        }
    }
}