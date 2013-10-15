using System.Windows;
using System.Windows.Controls;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductGroupCategory.xaml
    /// </summary>
    public partial class ProductGroupCategory : Window
    {
        public ProductCategory ProductCategory { get; set; }
        public ProductGroup ProductGroup { get; set; }

        /// <summary>
        /// Constructor called when creating a new product category or 
        /// product group
        /// </summary>
        public ProductGroupCategory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor called when editing a product category
        /// </summary>
        /// <param name="category"></param>
        public ProductGroupCategory(ProductCategory category)
        {
            InitializeComponent();

            btnSelect.Visibility = Visibility.Collapsed;
            rbProductGroup.IsEnabled = false;

            tbID.Text = category.ProductCategoryID;
            tbName.Text = category.ProductCategoryName;
        }

        /// <summary>
        /// Constructor called when editing a product group
        /// </summary>
        /// <param name="group"></param>
        public ProductGroupCategory(ProductGroup group)
        {
            InitializeComponent();

            tbID.Text = group.ProductGroupID;
            tbName.Text = group.ProductGroupName;
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // TheProduct category
            if (rbProductGroup.IsChecked.HasValue && rbProductGroup.IsChecked.Value)
            {
                Logic_Layer.ProductGroup pg = new Logic_Layer.ProductGroup();
                ProductGroup = pg;

                tbID.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }

            // TheProduct group
            else if (rbProductCategory.IsChecked.HasValue && rbProductCategory.IsChecked.Value)
            {
                Logic_Layer.ProductCategory pc = new Logic_Layer.ProductCategory();
                ProductCategory = pc;

                tbID.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                tbName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Hides the select category button since we're 
        /// creating a category and it can't be connected to another category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbProductCategory_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Makes sure the select category button is visible if we're creating a product group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbProductGroup_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Opens the product category register window for selection of a product category to
        /// associate a product group to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ProductCategoryRegister pgr = new ProductCategoryRegister();
            pgr.ShowDialog();
        }
    }
}
