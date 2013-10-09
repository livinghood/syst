using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductCategoryRegister.xaml
    /// </summary>
    public partial class ProductCategoryRegister : Window
    {
        public ObservableCollection<ProductCategory> CategoriesList
        {
            get
            {
                return new ObservableCollection<ProductCategory>(ProductCategoryManagement.Instance.GetProductCategories());
            }
        }

        public ProductCategoryRegister()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupCategory pgc = new ProductGroupCategory();
            pgc.ShowDialog();
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupCategory pgc = new ProductGroupCategory(CategoriesList[lvProductCategories.SelectedIndex]);
            pgc.ShowDialog();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ProductCategoryManagement.Instance.DeleteProductCategory(CategoriesList[lvProductCategories.SelectedIndex]);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lvProductCategories.SelectedItem != null)
            {
                ProductGroupManagement.Instance.ProductCategory = CategoriesList[lvProductCategories.SelectedIndex];
            }
            else
            {
                MessageBox.Show("Markera en kategori i listan först.", "Välj en kategori");
            }
        }
    }
}
