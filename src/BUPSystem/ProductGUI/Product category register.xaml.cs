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
            btnSelect.Visibility = Visibility.Collapsed;
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
                    lvProductCategories.Items.Refresh();
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
                    // Check that the product category is not associated to any product groups before removing
                    bool empty = ProductCategoryManagement.Instance.IsProductCategoryEmpty(CategoriesList[lvProductCategories.SelectedIndex]);

                    if (empty)
                    {
                        ProductCategoryManagement.Instance.DeleteProductCategory(CategoriesList[lvProductCategories.SelectedIndex]);
                        //lblInfo.Content = "Användaren togs bort";
                    }
                    else
                        MessageBox.Show("Det finns produktgrupper som är kopplade till den här produktkategorin." +
                                        "Ta bort dessa produktgrupper först", "Kan inte ta bort");
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
                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Markera en kategori i listan först.", "Välj en kategori");
        }
    }
}
