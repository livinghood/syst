using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;
using Logic_Layer.UserNamespace;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductCategoryRegister.xaml
    /// </summary>
    public partial class ProductCategoryRegister : Window
    {
        public ObservableCollection<ProductCategory> CategoriesList
        {
            get { return ProductCategoryManagement.Instance.ProductCategories; }
            set { ProductCategoryManagement.Instance.ProductCategories = value; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProductCategoryRegister()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Add a new product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductGroupCategory pgc = new ProductGroupCategory();
            pgc.ShowDialog();
            if (pgc.DialogResult == true)
            {
                ProductCategoryManagement.Instance.AddProductCategory(pgc.ProductCategory);
            }
        }

        /// <summary>
        /// Change a product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (lvProductCategories.SelectedItem != null)
            {
                ProductGroupCategory pgc = new ProductGroupCategory(CategoriesList[lvProductCategories.SelectedIndex]);
                pgc.ShowDialog();
                if (pgc.DialogResult == true)
                {
                    ProductCategoryManagement.Instance.UpdateProductCategory();
                }
            }
            else
                MessageBox.Show("Markera en produktkategori att redigera först", "Ingen vald produktkategori");

        }

        /// <summary>
        /// Delete a product category
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProductCategories.SelectedItem != null)
            {
                // Confirm that the user wishes to delete
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här produktkategorin?",
                    "Ta bort", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    ProductCategoryManagement.Instance.DeleteProductCategory(CategoriesList[lvProductCategories.SelectedIndex]);
                    //lblInfo.Content = "Användaren togs bort";
                }
            }
            else
                MessageBox.Show("Markera en produktkategori att ta bort", "Ingen vald produktkategori"); 
        }

        /// <summary>
        /// Selects a product category when creating a product group which the product group
        /// will be associated to
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lvProductCategories.SelectedItem != null)
            {
                ProductGroupManagement.Instance.ProductCategory = CategoriesList[lvProductCategories.SelectedIndex];
            }
            else
                MessageBox.Show("Markera en kategori i listan först.", "Välj en kategori");
        }
    }
}
