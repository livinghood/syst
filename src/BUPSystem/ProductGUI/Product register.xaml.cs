using System.Collections.ObjectModel;
using System.Windows;
using Logic_Layer;

namespace BUPSystem.ProductGUI
{
    /// <summary>
    /// Interaction logic for ProductRegister.xaml
    /// </summary>
    public partial class ProductRegister : Window
    {
        public ObservableCollection<Product> Products { get { return ProductManagement.Instance.Products; } }
        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProductRegister()
        {
            InitializeComponent();
            DataContext = this;
        }
        /// <summary>
        /// Adds a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductManager pm = new ProductManager();
            pm.ShowDialog();
            if (pm.DialogResult == true)
            {
                //ProductManagement.Instance.AddProduct(pm.Product);
            }
        }
        /// <summary>
        /// Edit a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (lvProducts.SelectedItem != null)
            {
                ProductManager pm = new ProductManager(Products[lvProducts.SelectedIndex]);
                pm.ShowDialog();
                if (pm.DialogResult == true)
                {
                    ProductManagement.Instance.UpdateProduct();
                    lvProducts.Items.Refresh();
                }
            }
            else
                MessageBox.Show("Markera en produkt att redigera först", "Ingen vald produkt");
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Make sure the sure the user has selected an item in the listview
            if (lvProducts.SelectedItem != null)
            {
                // Confirm that the user wishes to delete
                MessageBoxResult mbr = MessageBox.Show("Vill du verkligen ta bort den här produkten?",
                    "Ta bort", MessageBoxButton.YesNo);

                if (mbr == MessageBoxResult.Yes)
                {
                    ProductManagement.Instance.DeleteProduct(Products[lvProducts.SelectedIndex]);
                    //lblInfo.Content = "Användaren togs bort";
                }
            }
            else
                MessageBox.Show("Markera en produkt att ta bort", "Ingen vald produkt");
        }
    }
}
