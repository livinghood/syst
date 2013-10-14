using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductManager.xaml
    /// </summary>
    public partial class ProductManager : Window
    {
        public Product Product { get; set; }

        // Hold a selected product group from product group register
        private ProductGroup productGroup;

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
            DataContext = this;
        }

        /// <summary>
        /// Constructor called when changing data of an existing product
        /// </summary>
        /// <param name="product"></param>
        public ProductManager(Product product)
        {
            InitializeComponent();
            tbProductID.IsEnabled = false;
            cbDepartment.ItemsSource = ProductionDepartments;
            DataContext = product;
            Product = product;
        }

        /// <summary>
        /// Opens product gropup register for selection of a product group which
        /// the product is to be associated to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProductGroup_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupRegister pgr = new ProductGroupRegister();
           
            if (pgr.ShowDialog() == true)
            {
                productGroup = pgr.ProductGroup;
            }                  
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // A new product is to be created
            if (Product == null)
            {
                ProductManagement.Instance.CreateProduct(tbProductID.Text, tbProductName.Text,
                    ProductionDepartments[cbDepartment.SelectedIndex], productGroup);
            }

            // An existing product was edited
            else
            {
                ProductManagement.Instance.UpdateProduct();
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Closes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
