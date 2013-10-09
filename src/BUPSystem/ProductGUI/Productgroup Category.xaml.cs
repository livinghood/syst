using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductGroupCategory.xaml
    /// </summary>
    public partial class ProductGroupCategory : Window
    {
        private bool changeExisting;

        public ProductGroupCategory()
        {
            InitializeComponent();

            changeExisting = false;
        }

        public ProductGroupCategory(ProductCategory category)
        {
            InitializeComponent();

            changeExisting = true;

            btnSelect.Visibility = Visibility.Collapsed;
            rbProductGroup.IsEnabled = false;

            tbID.Text = category.ProductCategoryID;
            tbName.Text = category.ProductCategoryName;
        }

        public ProductGroupCategory(ProductGroup group)
        {
            InitializeComponent();

            changeExisting = true;

            tbID.Text = group.ProductGroupID;
            tbName.Text = group.ProductGroupName;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!changeExisting)
            {                
                if (rbProductGroup.IsChecked.HasValue && rbProductGroup.IsChecked.Value)
                {
                    ProductGroupManagement.Instance.CreateProductGroup(tbID.Text, tbName.Text);
                }
                
                else if (rbProductCategory.IsChecked.HasValue && rbProductCategory.IsChecked.Value)
                {
                    ProductCategoryManagement.Instance.CreateProductCategory(tbID.Text, tbName.Text);
                }
            }

            else
            {
                if (rbProductGroup.IsChecked.HasValue && rbProductGroup.IsChecked.Value)
                {
                    ProductGroupManagement.Instance.UpdateProductGroup();
                }

                else if (rbProductCategory.IsChecked.HasValue && rbProductCategory.IsChecked.Value)
                {
                    ProductCategoryManagement.Instance.UpdateProductCategory();
                }
            }
        }

        private void rbProductCategory_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.Visibility = Visibility.Collapsed;
        }

        private void rbProductGroup_Click(object sender, RoutedEventArgs e)
        {
            btnSelect.Visibility = Visibility.Visible;
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ProductCategoryRegister pgr = new ProductCategoryRegister();
            pgr.ShowDialog();
        }
    }
}
