using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductManager.xaml
    /// </summary>
    public partial class ProductManager : Window
    {
        public Product Product { get; set; }

        private bool changeExisting;

        // Hold a selected product group from product group register
        public ProductGroup ProductGroup  { get; set; }

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
        }

        /// <summary>
        /// Constructor called when changing data of an existing product
        /// </summary>
        /// <param name="product"></param>
        public ProductManager(Product product)
        {
            InitializeComponent();
            tbProductID.IsEnabled = false;
            btnProductGroup.IsEnabled = false;
            cbDepartment.ItemsSource = ProductionDepartments;
            DataContext = product;
            Product = product;
            ProductGroup = product.ProductGroup;

            changeExisting = true;
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
                ProductGroup = pgr.ProductGroup;
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
            if (!changeExisting)
            {
                ProductManagement.Instance.CreateProduct(tbProductID.Text, tbProductName.Text,
                    ProductionDepartments[cbDepartment.SelectedIndex], ProductGroup);
            }

            // An existing product was edited
            else
            {
                Product.DepartmentID = ProductionDepartments[cbDepartment.SelectedIndex];
                Product.ProductGroup = ProductGroup;
                Product.ProductName = tbProductName.Text;
                Product.ProductID = tbProductID.Text;
            }

            DialogResult = true;
            Close();
        }
    }
}
