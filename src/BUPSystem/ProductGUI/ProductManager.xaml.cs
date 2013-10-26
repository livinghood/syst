using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Logic_Layer;


namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductManager.xaml
    /// </summary>
    public partial class ProductManager : Window
    {
        public string m_partProductID;

        public string m_ProductCategoryID;

        public Product Product { get; set; }

        public string PartProductID 
        {
            get { return m_partProductID; }
            set
            {
                m_partProductID = value;
                // Update the main product ID
                updateProductID();
            }
        }

        public string ProductCategoryID
        {
            get { return m_ProductCategoryID; }
            set
            {
                m_ProductCategoryID = value;
            }
        }

        public string ProductionDepartmentID { get; set; }

        public ObservableCollection<string> ProductionDepartments
        {
            get { return new ObservableCollection<string>(ProductManagement.Instance.GetProductDepartments()); }
        }

        /// <summary>
        /// Constructor called when creating a new product
        /// </summary>
        public ProductManager()
        {
            InitializeComponent();
            cbDepartment.ItemsSource = ProductionDepartments;

            Logic_Layer.Product product = new Logic_Layer.Product();
            DataContext = product;
            Product = product;
            PartProductIDGrid.DataContext = this;
        }

        /// <summary>
        /// Constructor called when changing data of an existing product
        /// </summary>
        /// <param name="product"></param>
        public ProductManager(Product product)
        {
            InitializeComponent();
            // We can't change the product group
            btnProductGroup.Visibility = Visibility.Collapsed;
            // We can't change the ID
            tbProductID.IsEnabled = false;
            // Hide the partial ID
            PartProductIDGrid.Visibility = Visibility.Collapsed;
            // Show the productgroup
            lblProductGroup.Content = product.ProductGroupID;
            cbDepartment.ItemsSource = ProductionDepartments;
            DataContext = product;
            Product = product;

            // Disable the textbox for product id as its a primary key
            tbProductID.IsEnabled = false;
            // Disable validation for product id and part-id
            Binding binding = BindingOperations.GetBinding(tbProductID, TextBox.TextProperty);
            Binding partIDbinding = BindingOperations.GetBinding(tbPartProductID, TextBox.TextProperty);
            binding.ValidationRules.Clear();
            partIDbinding.ValidationRules.Clear();
            
        }

        /// <summary>
        /// Opens product group register for selection of a product group which
        /// the product is to be associated to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProductGroup_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupRegister pgr = new ProductGroupRegister(true);

            // Reset the group (fixes problem if user deletes current selected group)
            Product.ProductGroupID = null;

            if (pgr.ShowDialog() == true)
            {
                Product.ProductGroupID = pgr.SelectedProductGroup.ProductGroupID;
            }
            // Update the main product ID
            updateProductID();
        }


        private void btnSelectCategory_Click(object sender, RoutedEventArgs e)
        {
            ProductCategoryRegister pcat = new ProductCategoryRegister(true);

            // Reset the category (fixes problem if user deletes current selected category)
            
            // Product.ProductCategoryID = null;

            if (pcat.ShowDialog() == true)
            {
                // Product.ProductCategoryID = pcat.SelectedProductCategory.ProductCategoryID;
            }

            // Update the main product ID
            updateProductID();
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            tbProductID.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            if (Validation.GetHasError(tbProductID) == true)
                return;

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Update the product ID
        /// </summary>
        private void updateProductID()
        {
            // If no part-id or productgroup is set, set to null
            if (!String.IsNullOrEmpty(m_partProductID) && !String.IsNullOrEmpty(Product.ProductGroupID))
            {
                Product.ProductID = m_partProductID.ToUpper() + Product.ProductGroupID.Substring(0, 2).ToUpper();
            }
            else
            {
                Product.ProductID = null;
            }
            
        }

    }
}
